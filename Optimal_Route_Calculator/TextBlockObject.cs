using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Optimal_Route_Calculator
{
    class TextBlockObject : MainObject
    {
        private string message;
        public TextBlockObject(int x, int y, string msg, Canvas MyCanvas)
        {
            shape = new TextBlock { Text = msg, FontSize = 20, Style = (Style)Application.Current.Resources["CustomFont"] };
        

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
    }
}
