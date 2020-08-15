using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public WindArrow(Canvas MyCanvas)
        {
            uri = ($"pack://application:,,,/Images/Arrow.png");
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
