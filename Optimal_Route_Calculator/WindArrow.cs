using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Media;


namespace Optimal_Route_Calculator
{
    class WindArrow : MainObject
    {
        private ImageBrush Skin = new ImageBrush();
        private BitmapImage bitmapImage;
        private string uri;

        private readonly int height = 40;
        private readonly int width = 60;
        private readonly int top = 40;
        private readonly int left = 40;
        

        private RotateTransform rotate = new RotateTransform();
        private double rotateAngle;
        public WindArrow(Canvas MyCanvas)
        {
            uri = ($"pack://application:,,,/Images/Arrow.png");
            bitmapImage = new BitmapImage(new Uri(uri));
            Skin.ImageSource = bitmapImage;

            shape = new Rectangle { Width = width, Height = height, Fill = Skin };

            GetTop = top;
            GetLeft = left;
            
            //Delete this later:
            #region 
            rotate.Angle = 45;
            rotateAngle = 45;
            shape.RenderTransform = rotate;
            #endregion

            Canvas.SetLeft(shape, GetLeft);
            Canvas.SetTop(shape, GetTop);
            MyCanvas.Children.Add(shape);
        }
        public double GetRotation
        {
            set 
            {
                rotateAngle = AngleAddition(rotateAngle, value);
                rotate.Angle = value;
                shape.RenderTransform = rotate;
            }
            get { return rotateAngle; }
        }

    }
}
