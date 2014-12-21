using System;
using System.Collections.Generic;
using System.Globalization;
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
            mohamedAhmed.Loaded += mohamedAhmed_Loaded;
            mATextBox.AddHandler(CommandManager.ExecutedEvent, new RoutedEventHandler(CommandExecuted), true);
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

        public DoubleAnimation MoveVertically(double to, Duration duration, TimeSpan? timeSpan, int index)
        {
            var animation = FindResource("CharacterMoveAnimation") as DoubleAnimation;
            animation.To = to; //it comes from top to center
            animation.Duration = duration;
            if (timeSpan != null)
                animation.BeginTime = timeSpan;
            Storyboard.SetTargetProperty(animation, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", index)));
            return animation;
        }

        public DoubleAnimation Scale(double to, Duration duration, TimeSpan? timeSpan, int index)
        {
            var animation = FindResource("AnimationScale") as DoubleAnimation;
            animation.To = to;
            animation.Duration = duration;
            if (timeSpan != null)
                animation.BeginTime = timeSpan;
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

        void mohamedAhmed_Loaded(object sender, RoutedEventArgs e)
        {
            PrepareJustTextEffect();

            //insert init Storyboard
            var initStoryboard = new Storyboard();
            var index = 0;
            var noTime = TimeSpan.FromSeconds(0);
            var timeLine = TimeSpan.FromSeconds(0);
            var duration = TimeSpan.FromSeconds(.5);

            initStoryboard.Children.Add(MoveVertically(-40, noTime, null, index));
            initStoryboard.Children.Add(Scale(0, noTime, null, index));
            SetColor(index);
            initStoryboard.Children.Add(Scale(1, noTime, null, index));
            timeLine += duration;
            initStoryboard.Children.Add(MoveVertically(0, duration, null, index));
            //initStoryboard.BeginTime = TimeSpan.FromSeconds(.5);

            index = 1;
            initStoryboard.Children.Add(MoveVertically(-40, noTime, null, index));
            initStoryboard.Children.Add(Scale(0, noTime, null, index));
            SetColor(index);
            initStoryboard.Children.Add(Scale(1, noTime, null, index));
            timeLine += duration;
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
            if (changes.Any(a => a.ChangeType == ChangeType.Remove))
                return wrong;

            if (changes.Any(a => a.ChangeType == ChangeType.Insert))
                return right;

            if (changes.Any(a => a.ChangeType == ChangeType.Swap))
                return wrong;

            return changes
                .Where(change => change.ChangeType == ChangeType.Replace)
                .Aggregate(right, (current, change) => current.Insert(change.Index, change.Character + ""));
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

            //folding for the replace 
            foreach (var change in changes)
            {
                switch (change.ChangeType)
                {
                    case ChangeType.Insert:
                    {
                        //insert init Storyboard
                        var initStoryboard = new Storyboard();
                        var index = _wrongWordStarts + change.Index;
                        //move to far place on top 
                        var anim = FindResource("CharacterMoveAnimation") as DoubleAnimation;
                        anim.To = -40; //it comes from top to center
                        anim.Duration = TimeSpan.FromSeconds(0);
                        Storyboard.SetTargetProperty(anim, new PropertyPath(
                            String.Format("TextEffects[{0}].Transform.Children[0].Y", index)));
                        initStoryboard.Children.Add(anim);
                        //disapear
                        var animation = FindResource("AnimationScale") as DoubleAnimation;
                        animation.To = 0;
                        animation.Duration = TimeSpan.FromSeconds(0);
                        Storyboard.SetTargetProperty(animation, new PropertyPath(String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", index)));
                        initStoryboard.Children.Add(animation);
                        initStoryboard.Begin(this);
                    }break;
                    case ChangeType.Remove:
                    {
                    }break;
                    case ChangeType.Swap:
                    {
                    }break;
                    case ChangeType.Replace:
                    {
                        var initStoryboard = new Storyboard();
                        var index = _wrongWordStarts + change.Index+1;
                        //move to far place on top 
                        var anim = FindResource("CharacterMoveAnimation") as DoubleAnimation;
                        anim.To = -40; //it comes from top to center
                        anim.Duration = TimeSpan.FromSeconds(0);
                        Storyboard.SetTargetProperty(anim, new PropertyPath(
                            String.Format("TextEffects[{0}].Transform.Children[0].Y", index)));
                        initStoryboard.Children.Add(anim);

                        //to left 
                        //TODO move all chars to left 
                        var animationToLeft = FindResource("CharacterLeftAnimation") as DoubleAnimation;
                        animationToLeft.To = (mohamedAhmed.FontSize / 2) * -1;
                        animationToLeft.Duration = TimeSpan.FromSeconds(0);
                        Storyboard.SetTargetProperty(animationToLeft, new PropertyPath(
                            String.Format("TextEffects[{0}].Transform.Children[0].X", index)));
                        initStoryboard.Children.Add(animationToLeft);

                        //disapear
                        var animation = FindResource("AnimationScale") as DoubleAnimation;
                        animation.To = 0;
                        animation.Duration = TimeSpan.FromSeconds(0);
                        Storyboard.SetTargetProperty(animation, new PropertyPath(String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", index)));
                        initStoryboard.Children.Add(animation);
                        initStoryboard.Begin(this);
                    }break;
                }
            }
        }

        public async Task AnimateIt(List<Change> changes,Action done)
        {
            PrepareTextEffect(changes);            
            
            foreach (var change in changes)
            {
                switch (change.ChangeType)
                {
                    case ChangeType.Insert:
                    {
                        var index = _wrongWordStarts + change.Index;
                        await Task.Delay(200);  
                        Insert(index);
                    }
                        break;
                    case ChangeType.Remove:
                    {
                        await Task.Delay(200);
                        Remove(_wrongWordStarts + change.Index);
                    }
                    break;
                    case ChangeType.Swap:
                    {
                        await Task.Delay(200);
                        Swap(_wrongWordStarts + change.Index, _wrongWordStarts + change.Index2.Value);
                    }
                    break;
                    case ChangeType.Replace:
                    {
                        await Task.Delay(500);
                        var newIndex = _wrongWordStarts + change.Index + 1;
                        var oldIndex = _wrongWordStarts + change.Index;
                        Replace(newIndex, oldIndex);
                        //await Task.Delay(500);
                    }
                    break;
                }
                await Task.Delay(1000);
            }
            done();
        }

        private string _wrongWord = "";
        private string _rightWord = "";
        private int _wrongWordStarts = -1;
        private int _wrongWordLength = -1;
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

            //return widths.Sum() / widths.Count;
            return widths.Max();
        }

        private void Remove(int index)
        {
            var storyBoard = new Storyboard();

            //color animation 
            var solidColorBrush = new SolidColorBrush();
            var colorAnimation = FindResource("ColorAnimationRed") as ColorAnimation;
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty,colorAnimation);
            var textEffect = mohamedAhmed.TextEffects[index];
            textEffect.Foreground = solidColorBrush;

            //move down
            var anim = FindResource("CharacterWaveAnimation2") as DoubleAnimation;
            Storyboard.SetTargetProperty(anim, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", index)));
            storyBoard.Children.Add(anim);

            //disapear
            var animation = FindResource("AnimationScale") as DoubleAnimation;
            animation.BeginTime = TimeSpan.FromSeconds(.5);
            Storyboard.SetTargetProperty(animation,
                new PropertyPath(String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", index)));
            storyBoard.Children.Add(animation);

            storyBoard.Begin(this);
        }

        private void Insert(int index)
        {
            var storyboard = new Storyboard();

            //appear
            var animation = FindResource("AnimationScale") as DoubleAnimation;
            animation.To = 1;
            animation.Duration = TimeSpan.FromSeconds(0);
            Storyboard.SetTargetProperty(animation,
                new PropertyPath(String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", index)));
            storyboard.Children.Add(animation);

            //color animation 
            var solidColorBrush = new SolidColorBrush();
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                FindResource("ColorAnimationGreen") as ColorAnimation);
            var textEffect = mohamedAhmed.TextEffects[index];
            textEffect.Foreground = solidColorBrush;

            //move down
            var anim = FindResource("CharacterMoveAnimation") as DoubleAnimation;
            anim.To = 0;//it comes from top to center
            Storyboard.SetTargetProperty(anim, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", index)));
            storyboard.Children.Add(anim);

            storyboard.BeginTime = TimeSpan.FromSeconds(.5);
            storyboard.Begin(this);
        }

        private void Replace(int newIndex, int oldIndex)
        {
            var storyboard = new Storyboard();
            //appear
            var animation = FindResource("AnimationScale") as DoubleAnimation;
            animation.To = 1;
            animation.Duration = TimeSpan.FromSeconds(0);
            Storyboard.SetTargetProperty(animation,
                new PropertyPath(String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", newIndex)));
            storyboard.Children.Add(animation);

            //color animation new green
            var solidColorBrush = new SolidColorBrush();
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                FindResource("ColorAnimationGreen") as ColorAnimation);
            var textEffect = mohamedAhmed.TextEffects[newIndex];
            textEffect.Foreground = solidColorBrush;

            //color animation old red
            var solidColorBrushRed = new SolidColorBrush();
            solidColorBrushRed.BeginAnimation(SolidColorBrush.ColorProperty,
                FindResource("ColorAnimationRed") as ColorAnimation);
            var textEffectOld = mohamedAhmed.TextEffects[oldIndex];
            textEffectOld.Foreground = solidColorBrushRed;

            //move down old char
            var animationOld = FindResource("CharacterMoveAnimation") as DoubleAnimation;
            animationOld.To = 0; //it comes from top to center
            Storyboard.SetTargetProperty(animationOld, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", newIndex)));
            storyboard.Children.Add(animationOld);

            //move down new char
            var animationNew = FindResource("CharacterMoveAnimation") as DoubleAnimation;
            animationNew.To = 40; //it comes from top to center
            Storyboard.SetTargetProperty(animationNew, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", oldIndex)));
            storyboard.Children.Add(animationNew);

            //Old Char disappear
            var scaleAnimation = FindResource("AnimationScale2") as DoubleAnimation;
            scaleAnimation.To = 0;
            scaleAnimation.Duration = TimeSpan.FromSeconds(0);
            scaleAnimation.BeginTime = TimeSpan.FromSeconds(.6);
            Storyboard.SetTargetProperty(scaleAnimation,
                new PropertyPath(String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", oldIndex)));
            storyboard.Children.Add(scaleAnimation);

            storyboard.Begin(this);
        }

        private void Swap(int index1, int index2)
        {
            var storyBoard = new Storyboard();
            //move down
            var up = FindResource("CharacterMoveAnimation") as DoubleAnimation;
            Storyboard.SetTargetProperty(up, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", index1)));
            up.AutoReverse = true;
            storyBoard.Children.Add(up);

            //move left
            var left = FindResource("CharacterLeftAnimation") as DoubleAnimation;
            var distance = Math.Abs(index1 - index2);
            left.To = AverageCharWidth()*distance;
            left.Duration = TimeSpan.FromSeconds(0.2);
            Storyboard.SetTargetProperty(left, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].X", index1)));
            storyBoard.Children.Add(left);

            //move up then down
            var up2 = FindResource("CharacterMoveAnimation") as DoubleAnimation;
            Storyboard.SetTargetProperty(up2, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", index2)));
            up2.AutoReverse = true;
            storyBoard.Children.Add(up2);

            //move right
            var toRight = FindResource("CharacterLeftAnimation") as DoubleAnimation;
            toRight.To = AverageCharWidth()*-distance;
            toRight.Duration = TimeSpan.FromSeconds(0.2);
            Storyboard.SetTargetProperty(toRight, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].X", index2)));
            storyBoard.Children.Add(toRight);

            storyBoard.Begin(this);
        }
    }
}