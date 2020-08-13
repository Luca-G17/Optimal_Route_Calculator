using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Security.Policy;
using System.Windows.Documents;

namespace Optimal_Route_Calculator
{

    class MapSegmentObject : MainObject
    {
        private BmpBitmapEncoder encoder = new BmpBitmapEncoder();
        private MemoryStream memoryStream = new MemoryStream();
        private Rectangle rect = new Rectangle();
        
        private ImageBrush Skin = new ImageBrush();
        private BitmapImage bitmapImage;

        //private byte[,] bitmapByteArr = new byte[750, 4557];
        private byte[] bitmapByteArr = new byte[120000];

        private string uri;
        private int mapNum;


        public MapSegmentObject(Canvas MyCanvas, int Row, int Col, FullMapObject fullMap)
        {
            map_segment_index[0] = Row;
            map_segment_index[1] = Col;
             
            uri = ($"pack://application:,,,/Images/Map {Col},{Row}.JPG");
            bitmapImage = new BitmapImage(new Uri(uri));
            rect.Width = bitmapImage.Width;
            rect.Height = bitmapImage.Height;
            Skin.ImageSource = bitmapImage;
            rect.Fill = Skin;

            Canvas.SetLeft(rect, GetLeft);
            Canvas.SetTop(rect, GetTop);

            fullMap.SetMapSegmentArr(Row, Col, this);
        }
        public BitmapImage GetBitmap
        {
            get { return bitmapImage; }
        }
        public int[] GetSegmentIndex
        {
            get { return map_segment_index; }
            set { map_segment_index = value; }
        }
        public override bool GetVisible()
        {
            return visible;
        }
        public override void SetVisible(bool Visable, Canvas MyCanvas)
        {
            visible = Visable;
            if (visible)
            {
                MyCanvas.Children.Add(rect);
            }
            else
            {
                MyCanvas.Children.Remove(rect);
            }
        }
        public byte[] GetBitmapByteArr()
        {
            return bitmapByteArr;
        }
        public string GetUri
        {
            get { return uri; }
        }
        public int MapNum
        {
            get { return mapNum; }
            set { mapNum = value; }
        }
        public void ImageToByte()
        {
            byte[] data;
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            bitmapByteArr = data;
            /*
            for (int i = 0; i < bitmapImage.Height - 1; i++)
            {
                for (int f = 0; f < bitmapImage.Width * 3 - 1; f++)
                {
                    byte item = data[data_index];
                    bitmapByteArr[i, f] = item;
                    data_index += 1;
                }
            }
            Console.WriteLine("hi");
            */
        }
    }
}
