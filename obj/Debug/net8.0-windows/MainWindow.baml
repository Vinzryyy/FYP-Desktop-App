   M S B A M L   `   `   `  ����>  :FYP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:FYPFYP  5   	       FYP.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:FYP     5   	     � Title$  
MainWindow��5   	   $	��600��6   $
��1000��6)   .��5	      � .+�5
   
   �� $��Image/Background FYP.jpg��6   $��UniformToFill=�62    K� ZIndex$ 0��6J   ]� 5   
   $ 1��6   .+�5   
   � .+�5      �� -   5        .@�5      �� $��
Controller��6   $��-3,0,0,0q�6.   .��5      � $��10q�6    � RowDefinitions 5      �� 5   "   $
��Auto �60   �� 5   "   $��* �60   .+�5      � $��0��6#   $��0,0,0,5q�60    � ColumnDefinitions 5   "   �� 5   &   $��30 �67   �� 5   &   $	��180 �67   �� 5   &   $��80 �67   �� 5   &   $	��200 �67   �� 5   &   $	��200 �67   �� 5   &   $	��120 �67   �� 5    &   $��70 �67   �� 5!   &   $��70 �67   .+�5$   "   �� $��ID��6,   $
��Bold"�66   �� 5%   "   $
��Name��6,   $��1��68   $
��Bold"�6H   �� 5&   "   $��Status��6,   $��2��6:   $
��Bold"�6J   �� 5'   "   $��Selected Input Profile��6,   $��3��6J   $
��Bold"�6Z   $��Center=�6l   �� 5(   "   $��Selected Profile Number��6,   $��4��6K   $
��Bold"�6[   $��Center=�6m   �� 5)   "   $��Remove Device��6,   $��5��6A   $
��Bold"�6Q   $��Center=�6c   �� 5*   "   $��Linked��6,   $��6��6:   $
��Bold"�6J   $��Center=�6\   �� 5+   "   $��Updates��6,   $��7��6;   $
��Bold"�6K   $��Center=�6]   5,      �� 5/      $��1��6+   $
��Auto=�68   .��50   "   �� ��6/   �� *Controllers+��51   &   �� 52   *   .R�53   .   � $	��0,2q�63    54   2   �� 55   6   $��30 �6G   �� 56   6   $	��180 �6G   �� 57   6   $��80 �6G   �� 58   6   $	��200 �6G   �� 59   6   $	��200 �6G   �� 5:   6   $	��120 �6G   �� 5;   6   $��70 �6G   �� 5<   6   $��70 �6G   .+�5?   2   �� $��0��6P   ��6<   �� *Id+6P   �� 5@   2   $��1��6X   ��6<   �� *
DeviceName+6X   �� 5A   2   $��2��6T   ��6<   �� *Status+6T   �� 5C   2   $��3��6;    �� DisplayMemberPath$ ProfileName��6�   $	��170��6�   $��Center=�6	  ��6L   �� *DataContext.InputProfiles+ �� RelativeSource ��  �� AncestorType
 Window �� SelectedItem 6�   �� *SelectedProfile+6	  �� 5E   2   $��4��6;   $	��170��6w   $��Center=�6�    6K   �� *SelectedProfileNum+.@�5F   6   �� $��Keyboard n Mouse��6C   �� 5G   6   $��Xbox emulation��6C   �� 5H   6   $��DS4 emulation��6C   5I   3   �� 5K   2   $��5��69   $��Delete��6I   $��60��6Z   $	��5,0q�6e   �� 5M   2   $��6��6<   $��30��6i   $��15��6t   $
��15,4q�6�   ��6L   �� *LinkedBrush+6�   �� 5O   2   $��7��69   $��Update��6I   $��60��6Z   $	��5,0q�6e   5P   /   5Q   +   5S   #   5T      5U      5V      �� 5Y      $��Input Profile��6   .��5Z      � $��10q�6    5[   
   �� 5\      $
��Auto �6   �� 5]      $��* �6   �� 5^      $
��Auto �6   .+�5b   
   � $��0��6   $��0,0,0,5q�6    5c      �� 5d      $��40 �6#   �� 5e      $	��250 �6#   �� 5f      $	��300 �6#   �� 5g      $	��300 �6#   .+�5j      �� $��ID��6   $
��Bold"�6"   �� 5k      $��Profile Name��6   $��1��6,   $
��Bold"�6<   �� 5l      $��Date Created��6   $��2��6,   $
��Bold"�6<   �� 5m      $��Last Updated��6   $��3��6,   $
��Bold"�6<   5n      �� 5q      $��1��6+   ��68   �� *InputProfiles+��5r   "   �� 5s   &   .R�5t   *   � $	��0,2q�6/    5u   .   �� 5v   2   $��40 �6C   �� 5w   2   $	��250 �6C   �� 5x   2   $	��300 �6C   �� 5y   2   $	��300 �6C   .+�5|   .   �� $��0��6L   ��68   �� *Id+6L   �� 5}   .   $��1��6U   ��68   �� *ProfileName+6U   �� 5~   .   $��2��6U   ��68   �� *DateCreated+6U   �� 5   .   $��3��6U   ��68   �� *LastUpdated+6U   5�   +   5�   '   5�      �� 5�   
   $��2��6    �� Orientation$ 
Horizontal=�6"   $��0,10,0,0q�6;   .+�5�      �� -   5        $��Create Input Profile��5�      $	��150��64   $��0,0,10,0q�6@   6R   �� 5�      -   5        $��Edit Input Profile��5�      $	��150��62   6>   5�      5�      5�      �� 5�      $
��Scan��6   .��5�      � $��10q�6    5�      �� 5�   "   $��* �60   �� 5�   "   $
��Auto �60   �� 5�   "   $	��100 �60   .+�5�      �� 
�������$��1q�68   	 �� CornerRadius$	 8��6L   
 �� Padding$
 10q�6]   $��Center=�6j   $��Center=�6�   ._�5�   "   �� -   5         ��Name$ QrDisplayImage��5�   (   $
��None=�6@   5�      �� 5�      $ 
Horizontal=�6)   $��1��6B   $��0,10,0,0q�6O   $��Center=�6a   .+�5�   "   �� -   5        $��Generate New QR Code��5�   )   $��5q�6e   �� 5�   "   -   5        $��Manual Login��5�   )   $��5q�6Z   �� 5�   "   -   5        $��Save Profile��5�   )   $��5q�6`   5�      �� 5�      $��2��6+   $
��Auto=�68   $��0,10,0,0q�6[   .��5�   "   �� -   5         ��Name$ QrStatusText��5�   ,   $ ��Ready to generate QR code.��6B   $
��Wrap=�6d   5�      5�      5�      5�      5�      5�      5�      5�      