using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Optimal_Route_Calculator
{
    class LineObject : MainObject
    {
        private Line line = new Line();
        private double[] line_pos = new double[4];

        public LineObject(Canvas MyCanvas, int MapSegmentRow, int MagSegmentCol, double[] LinePos)
        {
            map_segment_index[0] = MapSegmentRow;
            map_segment_index[1] = MagSegmentCol;

            line.Stroke = Brushes.Black;
            line.StrokeThickness = 2;

            line.X1 = line_pos[0] = LinePos[0];
            line.Y1 = line_pos[1] = LinePos[1];
            line.X2 = line_pos[2] = LinePos[2];
            line.Y2 = line_pos[3] = LinePos[3];

            MyCanvas.Children.Add(line);
        }
        public double[] LinePos
        {
            get { return line_pos; }
            set { line_pos = value; }
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
                MyCanvas.Children.Add(line);
            }
            else
            {
                MyCanvas.Children.Remove(line);
            }
        }
    }
}
