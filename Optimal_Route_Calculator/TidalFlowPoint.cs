using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Optimal_Route_Calculator
{
    class TidalFlowPoint : Arrow
    {
        private const double ARROW_WIDTH = 20;
        private const double ARROW_HEIGHT = 20;
        private readonly double[] max_min = new double[2];
        private const double FREQ = 0.161030596;
        private double high_water_diff;
        private double bearing;
        private readonly string high_water;
        private readonly Ellipse circle;
        public TidalFlowPoint(Canvas MyCanvas, double SetLeft, double SetTop, double _bearing, double max, double min, string highWaterTime)
        {
            circle = new Ellipse { Width = ARROW_HEIGHT + 5, Height = ARROW_HEIGHT + 5, Stroke = Brushes.Blue, Fill = Brushes.Transparent };

            uri = ($"pack://application:,,,/Images/Arrow.png");
            bitmapImage = new BitmapImage(new Uri(uri));
            Skin.ImageSource = bitmapImage;

            shape = new Rectangle { Width = ARROW_WIDTH, Height = ARROW_HEIGHT, Fill = Skin };

            rotate.CenterX = GetLeft + 10;
            rotate.CenterY = GetTop + 10;

            // Sets high water time, bearing, min-max tide
            high_water = highWaterTime;
            Bearing = _bearing;
            max_min[0] = max;
            max_min[1] = min;
            getLeft = SetLeft;
            getTop = SetTop;

            // Adds the arrow
            Canvas.SetLeft(shape, GetLeft + 2.5);
            Canvas.SetTop(shape, GetTop + 2.5);
            MyCanvas.Children.Add(shape);

            // Adds the circle
            Canvas.SetLeft(circle, getLeft);
            Canvas.SetTop(circle, getTop);
            MyCanvas.Children.Add(circle);

            Rotation = bearing;
        }

        public double Bearing
        {
            get
            {
                // 6hrs 12.5mins
                // Gets the current tidal bearing which will flip 180 degrees every 6hrs 12.5mins
                double time_diff = Math.Abs((Convert.ToDateTime(high_water) - DateTime.Now).TotalHours);
                double cycle_region = time_diff % 12.4;
                double deg_bearing;

                if (cycle_region > 6.2)
                {
                    deg_bearing = AngleAddition(bearing, 180);
                }
                else
                {
                    deg_bearing = bearing;
                }

                // Converts to radians
                return Math.PI / 180 * deg_bearing;
            }
            set { bearing = value; }
        }
        public void CalculateTimeOffset(string high_water_str)
        {
            // Calculates the time until next high water
            DateTime high_water_time = Convert.ToDateTime(high_water_str);
            TimeSpan time_diff = high_water_time - DateTime.Now;
            high_water_diff = time_diff.TotalHours;
        }
        public double CalculateFlow(int t)
        {
            // Calculates the flow at a specified time
            double Amplitude = max_min[0] - max_min[1];
            return Amplitude * Math.Pow(Math.Cos((t - high_water_diff) * Math.PI * FREQ), 2) + max_min[1];
        }
        public override void SetVisible(bool Visable, Canvas MyCanvas)
        {
            visible = Visable;
            if (visible)
            {
                MyCanvas.Children.Add(shape);
                MyCanvas.Children.Add(circle);
            }
            else
            {
                MyCanvas.Children.Remove(shape);
                MyCanvas.Children.Remove(circle);
            }
        }
    }
}
