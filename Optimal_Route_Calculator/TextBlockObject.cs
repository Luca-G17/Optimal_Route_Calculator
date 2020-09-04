using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Optimal_Route_Calculator
{
    class TextBlockObject : MainObject
    {
        private string message;
        public TextBlockObject(int x, int y, string msg, Canvas MyCanvas, int font_size)
        {
            shape = new TextBlock { Text = msg, FontSize = font_size, Style = (Style)Application.Current.Resources["CustomFont"], Margin = new Thickness(5, 5, 5, 5), FontWeight = FontWeights.Thin };
        
            message = msg;
            Canvas.SetLeft(shape, x);
            Canvas.SetTop(shape, y);

            MyCanvas.Children.Add(shape);
        }

        public string SetMessage
        {
            get { return message; }
            set { ((TextBlock)shape).Text = value; }
        }
        public void SetBackground(SolidColorBrush colour)
        {
            ((TextBlock)shape).Background = colour;
        }
    }
}
