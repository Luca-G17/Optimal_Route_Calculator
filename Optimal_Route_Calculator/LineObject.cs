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
        private double[] line_pos = new double[4];

        public LineObject(Canvas MyCanvas, double[] LinePos)
        {
            shape = new Line
            {
                X1 = line_pos[0] = LinePos[0],
                Y1 = line_pos[1] = LinePos[1],
                X2 = line_pos[2] = LinePos[2],
                Y2 = line_pos[3] = LinePos[3],

                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            MyCanvas.Children.Add(shape);
        }
        public double[] LinePos
        {
            get { return line_pos; }
            set { line_pos = value; }
        }

    }
}
