   M S B A M L   `   `   `  ����>  :OWA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:OWAOWA  5   	       OWA.AddAccount   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:OWA     5   	   -   5          � Title$  Add Account��5   	   $	��234��6   $	��230��6*    � 
ResizeMode$ NoResize=�66    � WindowStartupLocation$ CenterOwner=�6L   .��5	      �  � RowDefinitions 5
   
   �� 5      $��10 �6   �� 5      �� 5      �� 5      �� 5      �� 5      $��10 �6    � ColumnDefinitions 5   
   �� 5      $��10 �6   �� 5      $��7* �6   �� 5      $	��10* �6   �� 5      $��10 �6   .+�5   
   �� $��Account name:��6   $��1��6)   $��1��66   $
��Left=�6F   $��Center=�6a   �� 5   
   -   5         ��Name$ uiAccountName��5      $��1��6)   $��2��66   $��Right=�6F   $��Center=�6b   $	��110��6}   �� 5   
   $��	Password:��6   $��2��6%   $��1��62   $
��Left=�6B   $��Center=�6]   G� 5   
   -   5         G�Name$ 
uiPassword��5      $��2��6*   $��2��67   $��Right=�6G   $��Center=�6c   $	��110��6~   �� 5"   
   $��Region:��6   $��3��6#   $��1��60   $
��Left=�6@   $��Center=�6[   �� 5#   
   -   5         ��Name$ uiRegion��5#      $��3��6%   $��2��62   $��Right=�6B   $��Center=�6^   $��56��6y   .@�5$      �� $��US��6    �� 
IsSelected . 6(   �� 5%      $��EU��6   5&      �� 5)   
   -   5        	 ��Name$	 bSave��5)      $
��Save��6    $��4��6/   $��1��6<   $��2��6L   $��Right=�6`   $��Bottom=�6|   $��85��6�   $��28��6�   $��0, 0, 90, 10q�6�   
 �� 	IsDefault
 . 6�   �� 5*   
   -   5        $	 bCancel��5*      $��Cancel��6"   $��4��63   $��1��6@   $��2��6P   $��Right=�6d   $��Bottom=�6�   $��85��6�   $��28��6�   $��0, 0, 0, 10q�6�    �� IsCancel . 6�   5+      5,      