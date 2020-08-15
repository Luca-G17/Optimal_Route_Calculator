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
        
    }
    
}
