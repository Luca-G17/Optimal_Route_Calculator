using System;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Optimal_Route_Calculator
{
    class ScaleObject : MainObject
    {
        private ImageBrush Skin = new ImageBrush();
        private BitmapImage bitmapImage;
        private string uri;

        private readonly int height = 10;
        private readonly int width = 114;
        private readonly int top = 572;
        private readonly int left = 0;

        public ScaleObject(Canvas MyCanvas)
        {
            uri = ($"pack://application:,,,/Images/Scale.png");
            bitmapImage = new BitmapImage(new Uri(uri));
            Skin.ImageSource = bitmapImage;

            shape = new Rectangle { Width = width, Height = height, Fill = Skin };

            GetTop = top;
            GetLeft = left;

            Canvas.SetLeft(shape, GetLeft);
            Canvas.SetTop(shape, GetTop);
            MyCanvas.Children.Add(shape);
        }
    }
}
