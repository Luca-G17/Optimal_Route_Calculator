using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Optimal_Route_Calculator
{
    class Arrow : MainObject
    {
        protected ImageBrush Skin = new ImageBrush();
        protected BitmapImage bitmapImage;
        protected string uri;

        protected RotateTransform rotate = new RotateTransform();
        protected double rotateAngle;

        public virtual double Rotation
        {
            set
            {
                rotate.Angle = AngleAddition(value, -90) - rotateAngle;
                rotateAngle += rotate.Angle;
                shape.RenderTransform = rotate;
            }
            get { return rotateAngle; }
        }
    }
}
