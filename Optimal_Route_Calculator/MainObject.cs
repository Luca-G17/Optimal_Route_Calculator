using System;
using System.Windows;
using System.Windows.Controls;

namespace Optimal_Route_Calculator
{
    abstract class MainObject
    {
        protected UIElement shape;
        protected bool visible = true;
        protected double getLeft;
        protected double getTop;
        protected int map_segment_index;
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

        public virtual int GetMapSegmentIndex
        {
            get { return map_segment_index; }
            set { map_segment_index = value; }
        }

        public virtual double AngleAddition(double angle1, double angle2)
        {
            if (angle1 + angle2 >= 360)
            {
                return (angle1 + angle2 - 360);
            }
            else if (angle1 + angle2 < 0)
            {
                return 360 - Math.Abs(angle1 + angle2);
            }
            else
            {
                return angle1 + angle2;
            }
        }

    }

}
