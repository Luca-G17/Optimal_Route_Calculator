using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System;
using System.Windows;

namespace Optimal_Route_Calculator
{
    class TidalFlowPoint : Arrow
    {
        private readonly double arrow_width = 20;
        private readonly double arrow_height = 20;
        private double[] max_min = new double[2];
        public double Bearing = 0;
        Ellipse circle;
        public TidalFlowPoint(Canvas MyCanvas, double SetLeft, double SetTop, double bearing, double max, double min)
        {
            circle = new Ellipse { Width = arrow_height + 5, Height = arrow_height + 5, Stroke = Brushes.Blue, Fill = Brushes.Transparent };

            uri = ($"pack://application:,,,/Images/Arrow.png");
            bitmapImage = new BitmapImage(new Uri(uri));
            Skin.ImageSource = bitmapImage;

            shape = new Rectangle { Width = arrow_width, Height = arrow_height, Fill = Skin };

            rotate.CenterX = GetLeft + 10;
            rotate.CenterY = GetTop + 10;

            Bearing = bearing;
            max_min[0] = max;
            max_min[1] = min;
            getLeft = SetLeft;
            getTop = SetTop;

            Canvas.SetLeft(shape, GetLeft + 2.5);
            Canvas.SetTop(shape, GetTop + 2.5);
            MyCanvas.Children.Add(shape);
           

            Canvas.SetLeft(circle, getLeft);
            Canvas.SetTop(circle, getTop);
            MyCanvas.Children.Add(circle);

            Rotation = Bearing;
        }
        /*
        public double Bearing
        {
            get 
            { 
                // 6hrs 12.5mins
                string highWater = ((MainWindow)Application.Current.MainWindow).GetFullMap.VisibleSegment().high_water;
                double time_diff = Math.Abs((Convert.ToDateTime(highWater) - DateTime.Now).TotalHours);
                double cycle_region = time_diff % 12.4;

                if ( ==   ) 
            }
            set { Bearing = value; }
        } 
        */
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
