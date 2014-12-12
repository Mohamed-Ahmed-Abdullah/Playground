using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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

            mohamedAhmed.Loaded += mohamedAhmed_Loaded;
            mATextBox.ContextMenuOpening += tb_ContextMenuOpening;
            mATextBox.AddHandler(CommandManager.ExecutedEvent, new RoutedEventHandler(CommandExecuted), true);
        }

        private void CommandExecuted(object sender, RoutedEventArgs e)
        {
            var executedRoutedEventArgs = e as ExecutedRoutedEventArgs;
            if (executedRoutedEventArgs == null)
                return;

            if (executedRoutedEventArgs.Command == EditingCommands.CorrectSpellingError)
            {
                _rightWord = executedRoutedEventArgs.Parameter as string;
                var spaces = GetEmptyString(MeasureString(_wrongWord).Width).ToArray();
                var first = mATextBox.Text.Take(_wrongWordStarts).ToArray();
                var second = mATextBox.Text.Skip(_wrongWordStarts + _wrongWordLength).ToArray();
                mATextBox.Text = new string(first.Concat(spaces).Concat(second).ToArray());
                 
                mohamedAhmed.Text = mATextBox.Text;

                Debug.WriteLine("And Let The Animation begins");
                Debug.WriteLine("_rightWord=" + _rightWord);
                Debug.WriteLine("_wrongWord=" + _wrongWord);
                Debug.WriteLine("_wrongWordStarts=" + _wrongWordStarts);
                Debug.WriteLine("_wrongWordLength=" + _wrongWordLength);
                Debug.WriteLine("width=" + MeasureString(_wrongWord).Width);
            }
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

        public async Task DoIt()
        {
            PrepareTextEffect();
            Down(0);
            await Task.Delay(1100);
            Up(1);
            await Task.Delay(1100);
            Right(1, 2);
            await Task.Delay(1100);
            Left(3, 4);
            await Task.Delay(1100);
            Swap(5, 8);
        }

        void mohamedAhmed_Loaded(object sender, RoutedEventArgs e)
        {
            //DoIt();
        }

        public void PrepareTextEffect()
        {
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

        public void Down(int index)
        {
            var storyBoardWave = new Storyboard();
            var storyBoardScale = new Storyboard();


            //color animation 
            var solidColorBrush = new SolidColorBrush();
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                FindResource("ColorAnimationRed") as ColorAnimation);

            var textEffect = mohamedAhmed.TextEffects[index];
            textEffect.Foreground = solidColorBrush;

            //move down
            var anim = FindResource("CharacterWaveAnimation2") as DoubleAnimation;

            storyBoardWave.BeginTime = TimeSpan.FromSeconds(.5);

            Storyboard.SetTargetProperty(anim, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", index)));
            storyBoardWave.Children.Add(anim);

            //disapear
            var animation = FindResource("AnimationScale") as DoubleAnimation;
            Storyboard.SetTargetProperty(animation,
                new PropertyPath(String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", index)));
            storyBoardScale.Children.Add(animation);
            storyBoardScale.BeginTime = TimeSpan.FromSeconds(1);


            storyBoardWave.Begin(this);
            storyBoardScale.Begin(this);
        }

        public void Up(int index)
        {
            var storyBoardWave = new Storyboard();
            var storyBoardScale = new Storyboard();

            //color animation 
            var solidColorBrush = new SolidColorBrush();
            solidColorBrush.BeginAnimation(SolidColorBrush.ColorProperty,
                FindResource("ColorAnimationGreen") as ColorAnimation);

            var textEffect = mohamedAhmed.TextEffects[index];
            textEffect.Foreground = solidColorBrush;

            //move down
            var anim = FindResource("CharacterUpAnimation") as DoubleAnimation;

            storyBoardWave.BeginTime = TimeSpan.FromSeconds(.5);

            Storyboard.SetTargetProperty(anim, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", index)));
            storyBoardWave.Children.Add(anim);

            //disapear
            var animation = FindResource("AnimationScale") as DoubleAnimation;
            Storyboard.SetTargetProperty(animation,
                new PropertyPath(String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", index)));
            storyBoardScale.Children.Add(animation);
            storyBoardScale.BeginTime = TimeSpan.FromSeconds(1);


            storyBoardWave.Begin(this);
            storyBoardScale.Begin(this);
        }

        public void Swap(int index1,int index2)
        {
            var storyBoardWave = new Storyboard();
            //move down
            var up = FindResource("CharacterUpAnimation") as DoubleAnimation;
            Storyboard.SetTargetProperty(up, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", index1)));
            up.AutoReverse = true;

            var left = FindResource("CharacterLeftAnimation") as DoubleAnimation;
            var distance = Math.Abs(index1 - index2);
            //fontsize /2 => in assumption that any distance between two chars is fontsize/2
            left.To = (mohamedAhmed.FontSize / 2) * distance;
            left.Duration = TimeSpan.FromSeconds(1);
            Storyboard.SetTargetProperty(left, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].X", index1)));

            storyBoardWave.Children.Add(left);
            storyBoardWave.Children.Add(up);

            var up2 = FindResource("CharacterUpAnimation") as DoubleAnimation;
            Storyboard.SetTargetProperty(up2, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].Y", index2)));
            up2.AutoReverse = true;

            var left2 = FindResource("CharacterLeftAnimation") as DoubleAnimation;
            left2.To = (mohamedAhmed.FontSize / 2) * -distance;
            left2.Duration = TimeSpan.FromSeconds(1);
            Storyboard.SetTargetProperty(left2, new PropertyPath(
                String.Format("TextEffects[{0}].Transform.Children[0].X", index2)));

            storyBoardWave.Children.Add(left2);
            storyBoardWave.Children.Add(up2);


            storyBoardWave.BeginTime = TimeSpan.FromSeconds(.5);
            storyBoardWave.Begin(this);
        }

        public void Left(int from, int to)
        {
            Move(from, to, true);
        }

        public void Right(int from, int to)
        {
            Move(from, to, false);
        }

        private void Move(int from, int to, bool isLeft)
        {
            var storyBoardWave = new Storyboard();
            for (var i = 0; i <= to; ++i)
            {
                if (i < from || i > to) continue;

                var anim = FindResource("CharacterLeftAnimation") as DoubleAnimation;
                anim.To = (mohamedAhmed.FontSize / 2) * (isLeft ? -1 : 1);
                storyBoardWave.BeginTime = TimeSpan.FromSeconds(.5);


                Storyboard.SetTargetProperty(anim, new PropertyPath(
                    String.Format("TextEffects[{0}].Transform.Children[0].X", i)));
                storyBoardWave.Children.Add(anim);
            }
            storyBoardWave.Begin(this);
        }
    }
}