   M S B A M L   `   `   `  ����S  OOptimal_Route_Calculator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullC&clr-namespace:Optimal_Route_CalculatorOptimal_Route_Calculator  5   	   -    'Optimal_Route_Calculator.SaveLoadWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   8local&clr-namespace:Optimal_Route_Calculator     5   	     � Title$  SaveLoadWindow��5   	   $	��300��6    $	��450��6-   .��5	      �  � RowDefinitions 5
   
   �� 5      $��* �6   �� 5      $��2* �6   .+�5   
   �� $��0��6   ���#2f5cb66   �� 5   
   $��1��6   ���#1f3d7a6   �� 5   
   $��0��6   .+�5      �� $��SAVE/LOAD WAYPOINTS:��6   $��
5, 5, 5, 5q�64   $��20(�6H      
CustomFont#��[  6V   $
��Thin"�6z   �� 5      $��y  - Save or load from a text file (.txt)
  - If there are existing waypoints, they will be deleted on loading a new route��6   $��12(�6�   #��[  6�   $
��Thin"�6�   �� 5      -   5         ��Name$ 	StatusMsg��5      $�� ��6+   $��
5, 5, 5, 5q�63   $��12(�6G   #��[  6U   $
��Thin"�6y   
��� E��6�   5      �� 5   
   $��1��6   .+�5      �� $#��Enter the save/load filename:��6   $��
5, 5, 5, 5q�6=   $��20(�6Q   #��[  6_   $
��Thin"�6�   �� 5      -   5         ��Name$ txtBoxFileName��5      $��
5, 5, 5, 5q�6.   �� 5       �� Orientation$ 
Horizontal=�6   .+�5      �� -   5        $
��SAVE��5      $	��150��6(   $��
5, 5, 5, 5q�64    �� 	ClickMode$ Release=�6W   �� 5      -   5        $
��LOAD��5      $	��150��6(   $��
5, 5, 5 ,5q�64   $ Release=�6W   5      �� 5      -   5        $ txtBlockExistingFiles��5      $��Previous Save Files:��67   $��
5, 5, 5, 5q�6S   $��12(�6g   #��[  6u   $
��Thin"�6�   �� 5       -   5         ��Name$ lstBoxExistingFiles��5       $��
5, 5, 5, 5q�63   5!      5"      5$      