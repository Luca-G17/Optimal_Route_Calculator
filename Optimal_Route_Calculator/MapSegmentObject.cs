using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Optimal_Route_Calculator
{

    class MapSegmentObject : MainObject
    {
        private BmpBitmapEncoder encoder = new BmpBitmapEncoder();
        private MemoryStream memoryStream = new MemoryStream();
        private BitmapImage bitmapImage;

        private byte[,] bitmapByteArr = new byte[750,1600];
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
            byte[] result = null;
            int result_index = 0;

            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(memoryStream);

            result = memoryStream.ToArray();
            for (int row = 0; row < bitmapImage.PixelHeight - 1; row++)
            {
                for (int col = 0; col < bitmapImage.PixelWidth - 1; col++)
                {
                    bitmapByteArr[row,col] = result[result_index];
                    result_index += 1;
                }    
            } 

        }
    }
}
