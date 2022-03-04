using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Optimal_Route_Calculator
{
    /// <summary>
    /// Both the Wind arrow and the Tidal Points require an arrow
    /// They should inherit from this class
    /// </summary>
    public class Arrow : MainObject
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
                // Rotations are zeroed at the +ve x-axis so add 90
                rotate.Angle = AngleAddition(value, -90) - rotateAngle;
                rotateAngle += rotate.Angle;
                shape.RenderTransform = rotate;
            }
            get { return rotateAngle; }
        }
    }
}
