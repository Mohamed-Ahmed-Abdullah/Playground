   M S B A M L   `   `   `  ����  AnimatedTextDemo    AnimatedTextDemo.Window1   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        � Title$  Animated Text Demo��5      $	��500��6    $	��800��6-   $��40(�5       � WindowStartupLocation$ CenterScreen=�5       � WindowState$ 	Maximized=�5       � 	Resources 5         CharacterWaveAnimation	 W� To g� AccelerationRatio g� AutoReverse g� DecelerationRatio g� Duration	 g� RepeatBehavior
 �� 
TargetName  CharacterWaveAnimation2  CharacterMoveAnimation  CharacterLeftAnimation  AnimationScale  AnimationScale2  CharacterRotationAnimation  CharacterRotationAnimation2 # CharacterRotationPauseAnimation �� TargetProperty 	 ColorAnimationRed	 �� To 
 ColorAnimationGreen  
FontFamily  TextFont �� Property �� Value��%�  &	       &	 �    &	 �    &	    &	 X    &	 �    &	 �   &	 ]   &	 �    &		   &	
 U  &	 �    &	 �    W� 5	   
   $ 20X�5
      $	 0.5Q�6"    . $	 0.5Q�5      $ 0:0:1D�62   $	 Forever��5      $
 textBlk��63   W� 5   
   $ 40X�5      $ 0:0:0.2D�6"   $
 mohamedAhmed��65   W� 5   
   $	 -40X�5      $ 0:0:0.2D�6#   $
 mohamedAhmed��66   W� 5   
   $ 0:0:0.2D�5      $
 mohamedAhmed��6-   W� 5   
   $
 mohamedAhmed��61   $	 0.0X�5      $ 0:0:.009D�6#   W� 5   
   $
 mohamedAhmed��62   $	 0.0X�5      $ 0:0:.009D�6#   W� 5   
   $	 360X�5      $	 0.5Q�6#   $	 0.5Q�5      $ 0:0:2D�62   $
 textBlk��5       W� 5"   
   $	 360X�5#      $	 0.5Q�6#   $	 0.5Q�5$      $ 0:0:2D�62   $
 mohamedAhmed��5%      W� 5'   
   $ 0:0:8D�6B   $ Opacity�5(      �� 5*   
   $ 0:0:0D�5+      $
 mohamedAhmed��5,      $	 RedX�5-      �� 5.   
   $ 0:0:0D�5/      $
 mohamedAhmed��50      $ GreenX�51      *�53   
   /Font/#texgyreadventor)�6A   �� 55   
   .�56      ��  � ��$ /Font/#texgyreadventor)�67   57      59      .��5;      �  � ColumnDefinitions 5<   
   �� 5=      $��1* �6   �� 5>      $��2.33* �6   �� 5?      $��1* �6    � RowDefinitions 5A   
   �� 5B      $��* �6   6)   �� 5C      $
��1.5* �6   6,   �� 5D      $��40 �6   6*   .+�5F   
   �� $��/Images/word.png��6   $��Uniform=�6*   $��3��6<   $��3��6P   6c   �� 5H   
   -   5         ��Name$ mohamedAhmed��5H      $��1��6*   $��1��6:   $9��3/AnimatedTextDemo;component/Font/#TeX Gyre Adventor)�5J      
���   �$
��Wrap=�5K   '   $��Stretch=�6;   $��50(�5L      $��3,1,0,0q�6"   ��5I      ��  �� ElementName$ 	mATextBox�� �� Path$
 Text� �� UpdateSourceTrigger$ PropertyChanged=�5L   "   �� 5P   
   -   5         ��Name$ 	mATextBox��5P      $��1��6%   $��1��65   $'��!rquirez  requriez rekurie rekuir ��5Q      $��Stretch=�5R      $9��3/AnimatedTextDemo;component/Font/#TeX Gyre Adventor)�5S      
������ $��0q�5T   ,    J� Opacity$ 1Q�6@    �� AcceptsReturn .  �� 
AcceptsTab .  �� TextWrapping$
 Wrap=�5U   9   $��50(�6M   
���   �
������  �� 	IsEnabled . 5V   >   5W      5X      