using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media.Animation;

namespace Optimal_Route_Calculator
{
    class Waypoint : MainObject
    {
        private double width = 50;
        private double height = 50;

        public Waypoint(Canvas MyCanvas, double SetLeft, double SetTop)
        {
            shape = new Ellipse { Width = width, Height = height, Stroke = Brushes.Red, Fill = Brushes.Transparent};

            getLeft = SetLeft;
            getTop = SetTop;

            Canvas.SetLeft(shape, getLeft);
            Canvas.SetTop(shape, getTop);
            MyCanvas.Children.Add(shape);
        }
 

    }
}
