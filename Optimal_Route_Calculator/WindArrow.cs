using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Optimal_Route_Calculator
{
    public class WindArrow : Arrow
    {

        private readonly int height = 40;
        private readonly int width = 60;
        private readonly int top = 40;
        private readonly int left = 40;

        public WindArrow(Canvas MyCanvas)
        {
            uri = ($"pack://application:,,,/Images/Arrow.png");
            bitmapImage = new BitmapImage(new Uri(uri));
            Skin.ImageSource = bitmapImage;

            shape = new Rectangle { Width = width, Height = height, Fill = Skin };

            // Sets the centre of rotation
            rotate.CenterX = 30;
            rotate.CenterY = 20;

            GetTop = top;
            GetLeft = left;

            Canvas.SetLeft(shape, GetLeft);
            Canvas.SetTop(shape, GetTop);
            MyCanvas.Children.Add(shape);
        }
    }
}
