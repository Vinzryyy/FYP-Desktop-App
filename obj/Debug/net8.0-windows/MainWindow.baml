   M S B A M L   `   `   `  ����>  :FYP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:FYPFYP  5   	       FYP.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:FYP     5   	     � Title$  
MainWindow��5   	   $	��600��6   $
��1000��6)   .��5	      � .+�5
   
   �� $��Image/Background FYP.jpg��6   $
��Fill=�62    K� ZIndex$ 0��6A   ]� 5   
   $ 1��6   .+�5   
   � .+�5      �� -   5        .@�5      �� $��
Controller��6   $��-3,0,0,0q�6.   .��5      � $��10q�6   .+�5      �� $$��Image/controllerbackground.jpg��6$   $
��Fill=�6L   $ 0��6[    J� Opacity$	 0.3Q�6l   � 5      $ 1��6#    � RowDefinitions 5   "   �� 5   &   $
��Auto �64   �� 5   &   $��* �64   .+�5   "   � $��0��6'   $��0,0,0,5q�64    � ColumnDefinitions 5   &   �� 5    *   $��30 �6;   �� 5!   *   $	��180 �6;   �� 5"   *   $��80 �6;   �� 5#   *   $	��200 �6;   �� 5$   *   $	��200 �6;   �� 5%   *   $	��120 �6;   �� 5&   *   $��70 �6;   �� 5'   *   $��70 �6;   .+�5*   &   �� $��ID��60   $
��Bold"�6:   �� 5+   &   $
��Name��60   $��1��6<   $
��Bold"�6L   �� 5,   &   $��Status��60   $��2��6>   $
��Bold"�6N   �� 5-   &   $��Selected Input Profile��60   $��3��6N   $
��Bold"�6^   $��Center=�6p   �� 5.   &   $��Selected Profile Number��60   $��4��6O   $
��Bold"�6_   $��Center=�6q   �� 5/   &   $��Remove Device��60   $��5��6E   $
��Bold"�6U   $��Center=�6g   �� 50   &   $��Linked��60   $��6��6>   $
��Bold"�6N   $��Center=�6`   �� 51   &   $��Updates��60   $��7��6?   $
��Bold"�6O   $��Center=�6a   52   #   �� 56   "   $��1��6/   $
��Auto=�6<   .��57   &   �� ��63   �� *Controllers+��58   *   �� 59   .   .R�5:   2   � $	��0,2q�67    5;   6   �� 5<   :   $��30 �6K   �� 5=   :   $	��180 �6K   �� 5>   :   $��80 �6K   �� 5?   :   $	��200 �6K   �� 5@   :   $	��200 �6K   �� 5A   :   $	��120 �6K   �� 5B   :   $��70 �6K   �� 5C   :   $��70 �6K   .+�5F   6   �� $��0��6T   ��6@   �� *Id+6T   �� 5G   6   $��1��6\   ��6@   �� *
DeviceName+6\   �� 5H   6   $��2��6X   ��6@   �� *Status+6X   �� 5J   6   $��3��6?    �� DisplayMemberPath$ ProfileName��6�   $	��170��6  $��Center=�6  ��6P   �� *DataContext.InputProfiles+ �� RelativeSource ��  �� AncestorType
 Window �� SelectedItem 6�   �� *SelectedProfile+6  �� 5L   6   $��4��6?   $	��170��6{   $��Center=�6�    6O   �� *SelectedProfileNum+.@�5M   :   �� $��	Profile 1��6G   �� 5N   :   $��	Profile 2��6G   �� 5O   :   $��	Profile 3��6G   5P   7   �� 5R   6   -   5        $��5��5R   =   $��Delete��6M   $��60��6^   $	��5,0q�6i   6v   �� 5T   6   $��6��6@   $��30��6m   $��15��6x   $
��15,4q�6�   ��6P   �� *LinkedBrush+6�   �� 5V   6   -   5        $��7��5V   =   $��Update��6M   $��60��6^   $	��5,0q�6i   6v   5X   3   5Z   /   5\   '   5]   #   5^      5_      5`      �� 5c      $��Input Profile��6   .��5d      � $��10q�6   .+�5e      �� $"��/Image/Profilebackground.png��6$   $
��Fill=�6J   $ 0��6Y   $	 0.3Q�6j   � 5f      $ 1��6#    5g   "   �� 5h   &   $
��Auto �64   �� 5i   &   $��* �64   �� 5j   &   $
��Auto �64   .+�5n   "   � $��0��6'   $��0,0,0,5q�64    5o   &   �� 5p   *   $��40 �6;   �� 5q   *   $	��250 �6;   �� 5r   *   $	��300 �6;   �� 5s   *   $	��300 �6;   .+�5v   &   �� $��ID��60   $
��Bold"�6:   �� 5w   &   $��Profile Name��60   $��1��6D   $
��Bold"�6T   �� 5x   &   $��Date Created��60   $��2��6D   $
��Bold"�6T   �� 5y   &   $��Last Updated��60   $��3��6D   $
��Bold"�6T   5z   #   �� 5}   "   $��1��6/   ��6<   �� *InputProfiles+��5~   &   �� 5   *   .R�5�   .   � $	��0,2q�63    5�   2   �� 5�   6   $��40 �6G   �� 5�   6   $	��250 �6G   �� 5�   6   $	��300 �6G   �� 5�   6   $	��300 �6G   .+�5�   2   �� $��0��6P   ��6<   �� *Id+6P   �� 5�   2   $��1��6Y   ��6<   �� *ProfileName+6Y   �� 5�   2   $��2��6Y   ��6<   �� *DateCreated+6Y   �� 5�   2   $��3��6Y   ��6<   �� *LastUpdated+6Y   5�   /   5�   +   5�   #   � 5�   "   $��0��6'   $��0,0,0,5q�64    5�   &   �� 5�   *   $��40 �6;   �� 5�   *   $	��250 �6;   �� 5�   *   $	��300 �6;   �� 5�   *   $	��300 �6;   .+�5�   &   �� $��ID��60   $
��Bold"�6:   �� 5�   &   $��Profile Name��60   $��1��6D   $
��Bold"�6T   �� 5�   &   $��Date Created��60   $��2��6D   $
��Bold"�6T   �� 5�   &   $��Last Updated��60   $��3��6D   $
��Bold"�6T   5�   #   �� 5�   "   $��1��6/   ��6<   �� *InputProfiles+��5�   &   �� 5�   *   .R�5�   .   � $	��0,2q�63    5�   2   �� 5�   6   $��40 �6G   �� 5�   6   $	��250 �6G   �� 5�   6   $	��300 �6G   �� 5�   6   $	��300 �6G   .+�5�   2   �� $��0��6P   ��6<   �� *Id+6P   �� 5�   2   $��1��6Y   ��6<   �� *ProfileName+6Y   �� 5�   2   $��2��6Y   ��6<   �� *DateCreated+6Y   �� 5�   2   $��3��6Y   ��6<   �� *LastUpdated+6Y   5�   /   5�   +   5�   #   �� 5�   "   $��2��6-   	 �� Orientation$	 
Horizontal=�6:   $��0,10,0,0q�6S   .+�5�   &   �� -   5        $��Create Input Profile��5�   -   $	��150��6L   $��0,0,10,0q�6X   6j   �� 5�   &   -   5        $��Edit Input Profile��5�   -   $	��150��6J   6V   5�   #   5�      5�      5�      �� 5�      $
��Scan��6   .��5�      � $��10q�6   .+�5�      �� $ ��/Image/scan_background.jpg��6$   $
��Fill=�6H   $ 0��6W   $	 0.3Q�6h   � 5�      $ 1��6#    5�   "   �� 5�   &   $��* �64   �� 5�   &   $
��Auto �64   �� 5�   &   $	��100 �64   .+�5�   "   �� 
�������$��1q�6<   
 �� CornerRadius$
 8��6P    �� Padding$ 10q�6a   $��Center=�6n   $��Center=�6�   ._�5�   &   �� -   5         ��Name$ QrDisplayImage��5�   ,   $
��None=�6D   5�   #   �� 5�   "   $	 
Horizontal=�6-   $��1��6F   $��0,10,0,0q�6S   $��Center=�6e   .+�5�   &   �� -   5        $��Generate New QR Code��5�   -   $��5q�6i   �� 5�   &   -   5        $��Manual Login��5�   -   $��5q�6^   �� 5�   &   -	   5        $��Save Profile��5�   -   $��5q�6d   5�   #   �� 5�   "   $��2��6/   $
��Auto=�6<   $��0,10,0,0q�6_   .��5�   &   �� -
   5         ��Name$ QrStatusText��5�   0   $ ��Ready to generate QR code.��6F   $
��Wrap=�6h   5�   #   5�      5�      5�      5�      5�      5�      5�      5�      