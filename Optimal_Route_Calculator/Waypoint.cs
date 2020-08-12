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
        private Ellipse ellipse = new Ellipse();
        private double width = 50;
        private double height = 50;

        private int map_segment_row;
        private int map_segment_col;

        public Waypoint(Canvas MyCanvas, double SetLeft, double SetTop, int MapSegmentRow, int MagSegmentCol)
        {
            ellipse.Width = width;
            ellipse.Height = height;
            ellipse.Stroke = Brushes.Red;
            ellipse.Fill = Brushes.Transparent;

            map_segment_row = MapSegmentRow;
            map_segment_col = MagSegmentCol;

            getLeft = SetLeft;
            getTop = SetTop;

            Canvas.SetLeft(ellipse, getLeft);
            Canvas.SetTop(ellipse, getTop);
            MyCanvas.Children.Add(ellipse);
        }
        public override bool GetVisible()
        {
            return visible;
        }
        public override void SetVisible(bool Visable, Canvas MyCanvas)
        {
            visible = Visable;
            if (visible)
            {
                MyCanvas.Children.Add(ellipse);
            }
            else
            {
                MyCanvas.Children.Remove(ellipse);
            }
        }

    }
}
