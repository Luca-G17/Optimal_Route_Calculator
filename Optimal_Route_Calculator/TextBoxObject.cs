using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Optimal_Route_Calculator
{
    class TextBoxObject : MainObject
    {
        private string message;
        public TextBoxObject(int x, int y, string msg, Canvas MyCanvas)
        {
            shape = new TextBlock { Text = msg, FontSize = 20, FontWeight = FontWeights.Bold };

            message = msg;

            Canvas.SetLeft(shape, x);
            Canvas.SetTop(shape, y);

            MyCanvas.Children.Add(shape);
        }

        public string SetMessage
        {
            get { return message; }
            set { message = value; }
        }
    }
}
