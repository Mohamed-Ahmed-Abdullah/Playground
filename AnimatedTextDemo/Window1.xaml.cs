using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        }

        private void tb_ContextMenuOpening(object sender, RoutedEventArgs e)
        {
            // Clear the context menu from its previous suggestions.
            spellCheckContextMenu.Items.Clear();

            // Get the spelling error and add its suggestions to the context menu.
            var caretIndex = mATextBox.CaretIndex;
            var cmdIndex = 0;
            var spellingError = mATextBox.GetSpellingError(caretIndex);
            var wrongWordSatrts = mATextBox.GetSpellingErrorStart(caretIndex);
            var wrongWordLength = mATextBox.GetSpellingErrorLength(caretIndex);
            var wrongWord = new string(mATextBox.Text.Skip(wrongWordSatrts).Take(wrongWordLength).ToArray());

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
                        CommandTarget = mATextBox
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

        public async Task DoIt()
        {
            //await Task.Delay(1100);
            Down(0);
            await Task.Delay(1100);
            Up(1);
            await Task.Delay(1100);
            Right(1,2);
            await Task.Delay(1100);
            Left(2,3);
        }

        void mohamedAhmed_Loaded(object sender, RoutedEventArgs e)
        {
            DoIt();
            //Right(3,4);
            //Left(4,5);
        }

        public void Down(int index)
        {
            mohamedAhmed.TextEffects = new TextEffectCollection();
            var storyBoardWave = new Storyboard();
            var storyBoardScale = new Storyboard();
            for (var i = 0; i <= index; ++i)
            {
                //move down
                var transGrp = new TransformGroup();
                transGrp.Children.Add(new TranslateTransform());
                transGrp.Children.Add(new ScaleTransform());

                mohamedAhmed.TextEffects.Add(new TextEffect
                {
                    PositionStart = i,
                    PositionCount = 1,
                    Transform = transGrp
                });
                var anim = FindResource("CharacterWaveAnimation2") as DoubleAnimation;
                
                storyBoardWave.BeginTime = TimeSpan.FromSeconds(.5);

                Storyboard.SetTargetProperty(anim, new PropertyPath(
                    String.Format("TextEffects[{0}].Transform.Children[0].Y",index)));
                storyBoardWave.Children.Add(anim);

                //disapear
                var animation = FindResource("AnimationScale") as DoubleAnimation;
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath(String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", index)));
                storyBoardScale.Children.Add(animation);
                storyBoardScale.BeginTime = TimeSpan.FromSeconds(1);

            }
            storyBoardWave.Begin(this);
            storyBoardScale.Begin(this);
        }
        public void Up(int index)
        {
            mohamedAhmed.TextEffects = new TextEffectCollection();
            var storyBoardWave = new Storyboard();
            var storyBoardScale = new Storyboard();
            for (var i = 0; i <= index; ++i)
            {
                //move down
                var transGrp = new TransformGroup();
                transGrp.Children.Add(new TranslateTransform());
                transGrp.Children.Add(new ScaleTransform());

                mohamedAhmed.TextEffects.Add(new TextEffect
                {
                    PositionStart = i,
                    PositionCount = 1,
                    Transform = transGrp
                });
                var anim = FindResource("CharacterUpAnimation") as DoubleAnimation;
                
                storyBoardWave.BeginTime = TimeSpan.FromSeconds(.5);

                Storyboard.SetTargetProperty(anim, new PropertyPath(
                    String.Format("TextEffects[{0}].Transform.Children[0].Y",index)));
                storyBoardWave.Children.Add(anim);

                //disapear
                var animation = FindResource("AnimationScale") as DoubleAnimation;
                Storyboard.SetTargetProperty(animation,
                    new PropertyPath(String.Format("TextEffects[{0}].Transform.Children[1].ScaleX", index)));
                storyBoardScale.Children.Add(animation);
                storyBoardScale.BeginTime = TimeSpan.FromSeconds(1);

            }
            storyBoardWave.Begin(this);
            storyBoardScale.Begin(this);
        }
        public void Left(int from, int to)
        {
            Move(from, to,true);
        }
        public void Right(int from, int to)
        {
            Move(from, to,false);
        }

        private void Move(int from, int to, bool isLeft)
        {
            mohamedAhmed.TextEffects = new TextEffectCollection();
            var storyBoardWave = new Storyboard();
            for (var i = 0; i <= to; ++i)
            {
                var transGrp = new TransformGroup();
                transGrp.Children.Add(new TranslateTransform());
                transGrp.Children.Add(new ScaleTransform());

                mohamedAhmed.TextEffects.Add(new TextEffect
                {
                    PositionStart = i,
                    PositionCount = 1,
                    Transform = transGrp
                });
                var anim = FindResource("CharacterLeftAnimation") as DoubleAnimation;
                anim.To = (mohamedAhmed.FontSize / 2) * (isLeft?-1:1);
                storyBoardWave.BeginTime = TimeSpan.FromSeconds(.5);

                if (i < from || i > to) continue;

                Storyboard.SetTargetProperty(anim, new PropertyPath(
                    String.Format("TextEffects[{0}].Transform.Children[0].X", i)));
                storyBoardWave.Children.Add(anim);
            }
            storyBoardWave.Begin(this);
        }
    }
}