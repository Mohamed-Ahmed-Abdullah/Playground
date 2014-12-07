using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
	        int caretIndex = mATextBox.CaretIndex;
	        int cmdIndex = 0;
	        SpellingError spellingError = mATextBox.GetSpellingError(caretIndex);
	        var wrongWordSatrts = mATextBox.GetSpellingErrorStart(caretIndex);
	        var wrongWordLength = mATextBox.GetSpellingErrorLength(caretIndex);
	        var wrongWord = new string(mATextBox.Text.Skip(wrongWordSatrts).Take(wrongWordLength).ToArray());

	        if (spellingError != null) 
	        {
	            foreach (string str in spellingError.Suggestions)
	            {
	                MenuItem mi = new MenuItem();
	                mi.Header = str;
	                mi.FontWeight = FontWeights.Bold;
	                mi.Command = EditingCommands.CorrectSpellingError;
	                mi.CommandParameter = str;
	                mi.CommandTarget = mATextBox;
	                mATextBox.ContextMenu.Items.Insert(cmdIndex, mi);
	                cmdIndex++;
	            }
	            // Add separator lines and IgnoreAll command.
	            Separator separatorMenuItem1 = new Separator();
	            mATextBox.ContextMenu.Items.Insert(cmdIndex, separatorMenuItem1);
	            cmdIndex++;
	            MenuItem ignoreAllMI = new MenuItem();
	            ignoreAllMI.Header = "Ignore All";
	            ignoreAllMI.Command = EditingCommands.IgnoreSpellingError;
	            ignoreAllMI.CommandTarget = mATextBox;
	            mATextBox.ContextMenu.Items.Insert(cmdIndex, ignoreAllMI);
	            cmdIndex++;
	            Separator separatorMenuItem2 = new Separator();
	            mATextBox.ContextMenu.Items.Insert(cmdIndex, separatorMenuItem2);
	        }
	    }

	    void mohamedAhmed_Loaded(object sender, RoutedEventArgs e)
        {
            mohamedAhmed.TextEffects = new TextEffectCollection();

            Storyboard storyBoardWave = new Storyboard();

            Storyboard storyBoardRotation = new Storyboard();
            storyBoardRotation.RepeatBehavior = RepeatBehavior.Forever;
            storyBoardRotation.AutoReverse = true;

            for (int i = 0; i < mohamedAhmed.Text.Length; ++i)
            {
                TextEffect effect = new TextEffect();

                // Tell the effect which character 
                // it applies to in the text.
                effect.PositionStart = i;
                effect.PositionCount = 1;

                TransformGroup transGrp = new TransformGroup();
                transGrp.Children.Add(new TranslateTransform());
                transGrp.Children.Add(new RotateTransform());
                effect.Transform = transGrp;

                mohamedAhmed.TextEffects.Add(effect);

                //this.AddWaveAnimation(storyBoardWave, i);
                DoubleAnimation anim =
                this.FindResource("CharacterWaveAnimation2")
                as DoubleAnimation;

                this.SetBeginTime(anim, i);

                string path = String.Format(
                    "TextEffects[{0}].Transform.Children[0].Y",
                    i);

                PropertyPath propPath = new PropertyPath(path);
                Storyboard.SetTargetProperty(anim, propPath);

                storyBoardWave.Children.Add(anim);
            }
            Timeline pause =
                this.FindResource("CharacterRotationPauseAnimation")
                as Timeline;

            storyBoardRotation.Children.Add(pause);
            storyBoardWave.Begin(this);
            storyBoardRotation.Begin(this);
        }

		void SetBeginTime(Timeline anim, int charIndex)
		{
			double totalMs = anim.Duration.TimeSpan.TotalMilliseconds;
			double offset = totalMs / 10;
			double resolvedOffset = offset * charIndex;
			anim.BeginTime = TimeSpan.FromMilliseconds(resolvedOffset);
		}
	}
}