using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace Optimal_Route_Calculator
{
    class LineObject : MainObject
    {
        private double[] line_pos = new double[4];
        private List<LineObject> routeLines = new List<LineObject>();
        public LineObject(Canvas MyCanvas, double[] LinePos, SolidColorBrush colour)
        {
            shape = new Line
            {
                X1 = line_pos[0] = LinePos[0],
                Y1 = line_pos[1] = LinePos[1],
                X2 = line_pos[2] = LinePos[2],
                Y2 = line_pos[3] = LinePos[3],

                Stroke = colour,
                StrokeThickness = 2
            };

            MyCanvas.Children.Add(shape);
        }
        public double[] LinePos
        {
            get { return line_pos; }
            set { line_pos = value; }
        }
        public List<LineObject> GetRouteLines
        {
            get { return routeLines; }
        }
        public void AddRouteLine(LineObject line)
        {
            routeLines.Add(line);
        }
        public void ChangeRouteLineVisibility(Canvas MyCanvas)
        {
            foreach (LineObject line in routeLines)
            {
                line.SetVisible(!line.GetVisible(), MyCanvas);
            }
        }
        public Line GetShape
        {
            get { return (Line)shape; }
        }

    }
}
