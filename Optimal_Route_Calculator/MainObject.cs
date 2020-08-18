using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace Optimal_Route_Calculator
{
    abstract class MainObject
    {
        protected UIElement shape;
        protected bool visible = true;
        protected double getLeft;
        protected double getTop;
        protected int[] map_segment_index = new int[2];
        public virtual double GetLeft
        {
            get { return getLeft; }
            set { getLeft = value; }
        }
        public virtual double GetTop
        {
            get { return getTop; }
            set { getTop = value; }
        }
        public virtual bool GetVisible()
        {
            return visible;
        }
        public virtual void SetVisible(bool Visable, Canvas MyCanvas)
        {
            visible = Visable;
            if (visible)
            {
                MyCanvas.Children.Add(shape);
            }
            else
            {
                MyCanvas.Children.Remove(shape);
            }
        }

        public virtual int[] GetMapSegmentIndex
        {
            get { return map_segment_index; }
            set { map_segment_index = value; }
        }

        public virtual double AngleAddition(double angle1, double angle2)
        {
            if (angle1 + angle2 > 360)
            {
                return (angle1 + angle2 - 360);
            }
            else if (angle1 + angle2 < 0)
            {
                return 360 - (angle1 + angle2 - 360);
            }
            else
            {
                return angle1 + angle2;
            }
        }

        public virtual void ConeSideSwap()
        {
        }

    }
    
}
