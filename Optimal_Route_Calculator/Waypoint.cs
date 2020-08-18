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

        private double[] maxTackingCone = { 0, 0, 0 };

        public Waypoint(Canvas MyCanvas, double SetLeft, double SetTop)
        {
            shape = new Ellipse { Width = width, Height = height, Stroke = Brushes.Red, Fill = Brushes.Transparent};

            getLeft = SetLeft;
            getTop = SetTop;

            Canvas.SetLeft(shape, getLeft);
            Canvas.SetTop(shape, getTop);
            MyCanvas.Children.Add(shape);
        }

        public void GenerateMaxTackCone(double ship_x, double ship_y)
        {
            double tack_cone_centre = (180 / Math.PI) * Math.Atan2(-getTop + ship_y, -getLeft + ship_x);
            tack_cone_centre = Math.Abs(tack_cone_centre);
            maxTackingCone[0] = AngleAddition(tack_cone_centre, -10);
            maxTackingCone[1] = AngleAddition(tack_cone_centre, 10);
            maxTackingCone[2] = 0;
        }
        public double[] getMaxTackCone
        {
            get { return maxTackingCone; }
        }
        public override void ConeSideSwap()
        {
            if (maxTackingCone[2] == 0)
            {
                maxTackingCone[2] += 1;
            }
            else
            {
                maxTackingCone[2] -= 1;
            }
        }



    }
}
