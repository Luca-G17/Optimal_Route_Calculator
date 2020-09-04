using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;


namespace Optimal_Route_Calculator
{
    class Waypoint : MainObject, IShipsandWaypoints
    {
        private readonly double Width = 50;
        private readonly double Height = 50;

        public Waypoint(Canvas MyCanvas, double SetLeft, double SetTop)
        {
            shape = new Ellipse { Width = Width, Height = Height, Stroke = Brushes.Red, Fill = Brushes.Transparent};

            getLeft = SetLeft;
            getTop = SetTop;

            Canvas.SetLeft(shape, getLeft);
            Canvas.SetTop(shape, getTop);
            MyCanvas.Children.Add(shape);
        }

        public void GenerateMaxTackCone(double ship_x, double ship_y)
        {
            double tack_cone_centre = (180 / Math.PI) * Math.Atan2(getTop - ship_y + 25, getLeft - ship_x + 25);
            tack_cone_centre = AngleAddition(tack_cone_centre, 180);
            GetMaxTackCone[0] = AngleAddition(tack_cone_centre, -10);
            GetMaxTackCone[1] = AngleAddition(tack_cone_centre, 10);
            GetMaxTackCone[2] = 0;
        }
        public double[] GetMaxTackCone { get; } = { 0, 0, 0 };
        public void ConeSwapSide()
        {
            if (GetMaxTackCone[2] == 0)
            {
                GetMaxTackCone[2] += 1;
            }
            else
            {
                GetMaxTackCone[2] -= 1;
            }
        }



    }
}
