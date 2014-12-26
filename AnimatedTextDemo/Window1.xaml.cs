using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AnimatedTextDemo
{
    public partial class Window1
    {
        private ContextMenu spellCheckContextMenu;

        public Window1()
        {
            InitializeComponent();
            spellCheckContextMenu = new ContextMenu();
            mATextBox.ContextMenu = spellCheckContextMenu;
            mATextBox.ContextMenuOpening += tb_ContextMenuOpening;
            //mohamedAhmed.Loaded += mohamedAhmed_Loaded;
            mATextBox.AddHandler(CommandManager.ExecutedEvent, new RoutedEventHandler(CommandExecuted), true);
        }

        #region Animation Core
        public DoubleAnimation GetDoubleAnimation()
        {
            var doubleAnimation = new DoubleAnimation();
            doubleAnimation.SetValue(Storyboard.TargetNameProperty,mohamedAhmed.Name);
            return doubleAnimation;
        }

        public DoubleAnimation MoveVertically(double to, Duration duration, TimeSpan? timeSpan, int index, bool autoReverse =false)
        {
            var animation = GetDoubleAnimation();//FindResource("CharacterMoveAnimation") as DoubleAnimation;
            animation.To = to; //it comes from top to center
            animation.Duration = duration;
            if (timeSpan != null)
                animation.BeginTime = timeSpan;
            animation.AutoReverse = autoReverse;
            Storyboard.SetTargetProperty(animation, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", index)));
            return animation;
        }

        public DoubleAnimation MoveHorizontally(double to, Duration duration, TimeSpan? timeSpan, int index, bool autoReverse = false)
        {
            var animation = GetDoubleAnimation();//FindResource("CharacterMoveAnimation") as DoubleAnimation;
            animation.To = to; //it comes from top to center
            animation.Duration = duration;
            if (timeSpan != null)
                animation.BeginTime = timeSpan;
            animation.AutoReverse = autoReverse;
            Storyboard.SetTargetProperty(animation, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].X", index)));
            return animation;
        }

        public DoubleAnimation Scale(double to, Duration duration, TimeSpan? beginTime, int index, bool autoReverse = false)
        {
            var animation = GetDoubleAnimation();
            animation.To = to;
            animation.Duration = duration;
            if (beginTime != null)
                animation.BeginTime = beginTime;
            animation.AutoReverse = autoReverse;
            Storyboard.SetTargetProperty(animation,
                new PropertyPath(String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", index)));
            return animation;
        }

        public void SetColor(int index,bool isGreen=true)
        {
            var solidColorBrush = new SolidColorBrush();
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                FindResource((isGreen ? "ColorAnimationGreen" : "ColorAnimationRed")) as ColorAnimation);
            var textEffect = mohamedAhmed.TextEffects[index];
            textEffect.Foreground = solidColorBrush;
        }
        #endregion

        void mohamedAhmed_Loaded(object sender, RoutedEventArgs e)
        {
            PrepareJustTextEffect();

            //insert init Storyboard
            var initStoryboard = new Storyboard();
            var index = 0;
            var noTime = TimeSpan.FromSeconds(0);
            var timeLine = TimeSpan.FromSeconds(0);
            var duration = TimeSpan.FromSeconds(.5);

            //init
            initStoryboard.Children.Add(MoveVertically(-40, noTime, null, index));
            initStoryboard.Children.Add(Scale(0, noTime, null, index));
            SetColor(index);
            initStoryboard.Children.Add(Scale(1, noTime, null, index));
            timeLine += duration;
            initStoryboard.Children.Add(MoveVertically(0, duration, null, index));
            //initStoryboard.BeginTime = TimeSpan.FromSeconds(.5);

            index = 1;
            timeLine += duration;
            initStoryboard.Children.Add(MoveVertically(-40, noTime, null, index));
            initStoryboard.Children.Add(Scale(0, noTime, null, index));
            SetColor(index);
            initStoryboard.Children.Add(Scale(1, noTime, timeLine, index));
            initStoryboard.Children.Add(MoveVertically(0, duration, timeLine, index));

            initStoryboard.BeginTime = TimeSpan.FromSeconds(1);

            initStoryboard.Begin(this);
        }

        private void CommandExecuted(object sender, RoutedEventArgs e)
        {
            var executedRoutedEventArgs = e as ExecutedRoutedEventArgs;
            if (executedRoutedEventArgs == null)
                return;

            if (executedRoutedEventArgs.Command == EditingCommands.CorrectSpellingError)
            {
                mohamedAhmed.Visibility = Visibility.Visible;
                mATextBox.Visibility = Visibility.Collapsed;

                _rightWord = executedRoutedEventArgs.Parameter as string;
                var spaces = GetEmptyString(MeasureString(_wrongWord).Width).ToArray();
                var first = mATextBox.Text.Take(_wrongWordStarts).ToArray();
                var second = mATextBox.Text.Skip(
                    _wrongWordStarts + 
                    (_wrongWordLength > _rightWord.Length ? _wrongWordLength : _rightWord.Length)
                    ).ToArray();
                mATextBox.Text = new string(first.Concat(spaces).Concat(second).ToArray());

                var changesFider = new ChangesFinder();
                var changes = changesFider.Find(_wrongWord, _rightWord);
                var animationString = CreateAnimatedString(_wrongWord, _rightWord, changes);
                mohamedAhmed.Text = new string(first.Concat(animationString).Concat(second).ToArray());
                AnimateIt(changes, () =>
                {
                    mohamedAhmed.Visibility = Visibility.Collapsed;
                    mATextBox.Visibility = Visibility.Visible;
                    if (changes.Any(a => a.ChangeType == ChangeType.Remove))  _rightWord = _rightWord + " ";//hacky solution to solve a bug
                    mATextBox.Text = new string(first.Concat(_rightWord).Concat(second).ToArray());
                });
            }
        }

        public string CreateAnimatedString(string wrong, string right, List<Change> changes)
        {
            var word = wrong;
            foreach (var change in changes)
            {
                switch (change.ChangeType)
                {
                    case ChangeType.Insert:
                    {
                        word = word.Insert(change.Index, change.Character + "");
                        //when you insert a charahter the indexes will be effected so you need to push them
                        changes.Skip(changes.IndexOf(change) + 1).ToList().ForEach(f =>
                        {
                            f.Index++;
                            if (f.Index2.HasValue)
                                f.Index2++;
                        });
                    }
                    break;
                    case ChangeType.Remove:
                    //leav the char in there cuz the animation will take it out
                    break;
                    case ChangeType.Swap:
                    //leav them unswaped cuz the animation will do it 
                    break;
                    case ChangeType.Replace:
                    {
                        //put the new char next to the old one 
                        word = word.Insert(change.Index, change.Character2 + "");
                        //when you insert a charahter the indexes will be effected so you need to push them
                        changes.Skip(changes.IndexOf(change) + 1).ToList().ForEach(f =>
                        {
                            f.Index++;
                            if (f.Index2.HasValue)
                                f.Index2++;
                        });

                    }
                    break;
                }
            }
            return word;
        }

        public void PrepareTextEffect(List<Change> changes)
        {
            //text effects 
            mohamedAhmed.TextEffects = new TextEffectCollection();
            for (var i = 0; i < mohamedAhmed.Text.Count(); i++)
            {
                var transGrp = new TransformGroup();
                transGrp.Children.Add(new TranslateTransform());
                transGrp.Children.Add(new ScaleTransform());

                mohamedAhmed.TextEffects.Add(new TextEffect
                {
                    PositionStart = i,
                    PositionCount = 1,
                    Transform = transGrp,
                });
            }
        }

        public void PrepareJustTextEffect()
        {
            //text effects 
            mohamedAhmed.TextEffects = new TextEffectCollection();
            for (var i = 0; i < mohamedAhmed.Text.Count(); i++)
            {
                var transGrp = new TransformGroup();
                transGrp.Children.Add(new TranslateTransform());
                transGrp.Children.Add(new ScaleTransform());

                mohamedAhmed.TextEffects.Add(new TextEffect
                {
                    PositionStart = i,
                    PositionCount = 1,
                    Transform = transGrp,
                });
            }
        }

        public async Task AnimateIt(List<Change> changes,Action done)
        {
            PrepareTextEffect(changes);

            var storyboard = new Storyboard();
            var timeLine = TimeSpan.FromSeconds(0);
            var duration = TimeSpan.FromSeconds(0.5);

            foreach (var change in changes)
            {
                switch (change.ChangeType)
                {
                    case ChangeType.Insert:
                    {
                        timeLine += Insert(_wrongWordStarts + change.Index, storyboard, timeLine, duration);
                    }
                        break;
                    case ChangeType.Remove:
                    {
                        timeLine += Remove(_wrongWordStarts + change.Index, storyboard, timeLine, duration);
                    }
                    break;
                    case ChangeType.Swap:
                    {
                        timeLine += Swap(_wrongWordStarts + change.Index, _wrongWordStarts + change.Index2.Value, storyboard, timeLine, duration);
                    }
                    break;
                    case ChangeType.Replace:
                    {
                        var newIndex = _wrongWordStarts + change.Index + 1;
                        var oldIndex = _wrongWordStarts + change.Index;
                        timeLine += Replace(newIndex, oldIndex, storyboard, timeLine, duration);
                    }
                    break;
                }
                timeLine += duration;
            }
            storyboard.Begin(this);
            await Task.Delay(timeLine + (duration+duration) );
            done();
        }

        private string _wrongWord = "";
        private string _rightWord = "";
        private int _wrongWordStarts = -1;
        private int _wrongWordLength = -1;
        readonly TimeSpan _noTime = TimeSpan.FromSeconds(0);
        private void tb_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            spellCheckContextMenu.Items.Clear();
            var caretIndex = mATextBox.CaretIndex;
            var cmdIndex = 0;
            var spellingError = mATextBox.GetSpellingError(caretIndex);
            _wrongWordStarts = mATextBox.GetSpellingErrorStart(caretIndex);
            _wrongWordLength = mATextBox.GetSpellingErrorLength(caretIndex);
            var wrongWord = new string(mATextBox.Text.Skip(_wrongWordStarts).Take(_wrongWordLength).ToArray());

            _wrongWord = wrongWord;

            if (spellingError != null)
            {
                foreach (var str in spellingError.Suggestions)
                {
                    var mi = new MenuItem
                    {
                        Header = str,
                        FontWeight = FontWeights.Bold,
                        Command = EditingCommands.CorrectSpellingError,
                        CommandParameter = str,
                        CommandTarget = mATextBox,
                    };
                    mATextBox.ContextMenu.Items.Insert(cmdIndex, mi);
                    cmdIndex++;
                }
                // Add separator lines and IgnoreAll command.
                var separatorMenuItem1 = new Separator();
                mATextBox.ContextMenu.Items.Insert(cmdIndex, separatorMenuItem1);
                cmdIndex++;
                var ignoreAllMI = new MenuItem
                {
                    Header = "Ignore All",
                    Command = EditingCommands.IgnoreSpellingError,
                    CommandTarget = mATextBox
                };
                mATextBox.ContextMenu.Items.Insert(cmdIndex, ignoreAllMI);
                cmdIndex++;
                var separatorMenuItem2 = new Separator();
                mATextBox.ContextMenu.Items.Insert(cmdIndex, separatorMenuItem2);
            }
        }

        public string GetEmptyString(double stringWidth)
        {
            var spaces = "";

            while (true)
            {
                if (MeasureString(spaces).Width >= stringWidth)
                    break;
                spaces += "_";
            }

            var withoutLastCharString = new string(spaces.Skip(1).ToArray());
            var withLastChar = MeasureString(spaces).Width - stringWidth;
            var withoutLastChar = stringWidth - MeasureString(withoutLastCharString).Width;
            if (withLastChar > withoutLastChar)
                spaces = withoutLastCharString;

            return spaces;
        }

        private Size MeasureString(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(mohamedAhmed.FontFamily, mohamedAhmed.FontStyle, mohamedAhmed.FontWeight, mohamedAhmed.FontStretch),
                mohamedAhmed.FontSize,
                Brushes.Black);
            return new Size(formattedText.Width, formattedText.Height);
        }

        private double AverageCharWidth()
        {
            var widths = mohamedAhmed.Text
                .Select(charachter => MeasureString(charachter + "").Width)
                .Where(w => w > 0)
                .ToList();

            return widths.Sum() / widths.Count;
            //return widths.Max();
        }

        private TimeSpan Remove(int index, Storyboard storyboard, TimeSpan timeLine, TimeSpan duration)
        {
            SetColor(index,false);
            storyboard.Children.Add(MoveVertically(40,duration,timeLine,index));
            timeLine += duration;
            storyboard.Children.Add(Scale(0,_noTime,timeLine,index));
            return timeLine;
        }

        private TimeSpan Insert(int index, Storyboard storyboard,TimeSpan timeLine,TimeSpan duration)
        {
            timeLine += duration;
            storyboard.Children.Add(MoveVertically(-40, _noTime, null, index));
            storyboard.Children.Add(Scale(0, _noTime, null, index));
            SetColor(index);
            storyboard.Children.Add(Scale(1, _noTime, timeLine, index));
            storyboard.Children.Add(MoveVertically(0, duration, timeLine, index));
            return timeLine;
        }

        private TimeSpan Replace(int newIndex, int oldIndex, Storyboard storyboard, TimeSpan timeLine, TimeSpan duration)
        {
            SetColor(newIndex);
            SetColor(oldIndex,false);

            storyboard.Children.Add(MoveVertically(-AverageCharWidth(), _noTime, _noTime, newIndex));
            storyboard.Children.Add(Scale(0, _noTime, _noTime, newIndex));
            storyboard.Children.Add(MoveHorizontally(-AverageCharWidth(), _noTime, _noTime, newIndex));

            timeLine += duration;

            storyboard.Children.Add(Scale(1, _noTime, timeLine, newIndex));
            storyboard.Children.Add(MoveVertically(0, duration, timeLine, newIndex));
            storyboard.Children.Add(MoveVertically(AverageCharWidth(), duration, timeLine, oldIndex));

            return timeLine;
        }

        private TimeSpan Swap(int index1, int index2, Storyboard storyboard, TimeSpan timeLine, TimeSpan duration)
        {
            timeLine += duration;

            storyboard.Children.Add(MoveVertically(-AverageCharWidth(),duration,timeLine,index1,true));
            storyboard.Children.Add(MoveVertically(-AverageCharWidth(),duration,timeLine,index2,true));
            var distance = Math.Abs(index1 - index2);
            var to = AverageCharWidth() * distance;
            storyboard.Children.Add(MoveHorizontally(to, duration, timeLine, index1));
            storyboard.Children.Add(MoveHorizontally(-to, duration, timeLine, index2));

            return timeLine;
        }
    }
}