   M S B A M L   `   `   `  ����>  :OWA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:OWAOWA  5   	       OWA.Authentication   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   #localclr-namespace:OWA     5   	   -   5          � Title$  Authentication��5   	   $	��163��6    $	��216��6-    � 
ResizeMode$ NoResize=�69    � WindowStartupLocation$ CenterOwner=�6O   .��5	      �  � RowDefinitions 5
   
   �� 5      $��10 �6   �� 5      �� 5      �� 5      �� 5      $��10 �6    � ColumnDefinitions 5   
   �� 5      $��10 �6   �� 5      $	��61* �6   �� 5      $
��154* �6   �� 5      $��10 �6   .+�5   
   �� $��Account:��6   $��1��6$   $��1��64   $
��Left=�6A   $��Center=�6\   �� 5   
   -   5         ��Name$ 	UIAccount��5      $��account@account.com��6'   $��2��6B   $��1��6R   $��Right=�6_   $��Center=�6{   �� 5   
   $��Code:��6   $��1��6!   $��2��61   $
��Left=�6>   $��Center=�6Y   �� 5   
   -   5         ��Name$ UICode��5      $��2��6"   $��2��62   $��Right=�6?   $��Center=�6[   $	��120��6v   �� 5   
   -   5         ��Name$ UISubmit��5      $��Submit��6#   $��1��64   $��2��6D   $��3��6X   $��Right=�6e   $��Bottom=�6�   $��28��6�   $��70��6�   $��0, 0, 75, 0q�6�    �� 	IsDefault . 6�   �� 5   
   -   5        $ UICancel��5      $��Cancel��6#   $��1��64   $��2��6D   $��3��6X   $��Right=�6e   $��Bottom=�6�   $��28��6�   $��70��6�   	 �� IsCancel	 . 6�   5      5       