<<<<<<< HEAD
﻿using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Optimal_Route_Calculator
{
    class LineObject : MainObject
    {
        private double[] line_pos = new double[4];
        private double Length { get; }

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
            Length = MainWindow.Hypotenuse(line_pos[0] - line_pos[2], line_pos[1] - line_pos[3]);

            MyCanvas.Children.Add(shape);
        }

        public double RouteLineLength()
        {
            double total = 0;
            foreach (LineObject line in GetRouteLines)
            {
                // Sums its own route lines if it has any
                total += line.Length;
            }
            return total;
        }
        public double[] LinePos
        {
            get { return line_pos; }
            set { line_pos = value; }
        }
        public List<LineObject> GetRouteLines { get; } = new List<LineObject>();
        public void AddRouteLine(LineObject line)
        {
            GetRouteLines.Add(line);
        }
        /// <summary>
        /// Changes its own route line visibility if it has any
        /// </summary>
        /// <param name="MyCanvas"></param>
        public void ChangeRouteLineVisibility(Canvas MyCanvas)
        {
            foreach (LineObject line in GetRouteLines)
            {
                line.SetVisible(!line.GetVisible(), MyCanvas);
            }
        }
        /// <summary>
        /// removes its own route lines if it has any
        /// </summary>
        public void RemoveRouteLines()
        {
            GetRouteLines.Clear();
        }

        public Line GetShape
        {
            get { return (Line)shape; }
        }

    }
}
=======
﻿using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Optimal_Route_Calculator
{
    class LineObject : MainObject
    {
        private double[] line_pos = new double[4];
        private double length { get; }
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
            length = MainWindow.Hypotenuse(line_pos[0] - line_pos[2], line_pos[1] - line_pos[3]);

            MyCanvas.Children.Add(shape);
        }

        public double RouteLineLength()
        {
            double total = 0;
            foreach (LineObject line in routeLines)
            {
                total += line.length;
            }
            return total;
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
        public void KillRouteLines()
        {
            routeLines.Clear();
        }

        public Line GetShape
        {
            get { return (Line)shape; }
        }

    }
}
>>>>>>> 3da34f7792296f9183bb3aefb50d77191f829b09
