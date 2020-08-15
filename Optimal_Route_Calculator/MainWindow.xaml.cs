using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
 

namespace Optimal_Route_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        private FullMapObject fullMap = new FullMapObject();
        private WindArrow WindArrow;

        private double HEIGHT = 700 / (144 / 96);
        private double WIDTH = 1200 / (144 / 96);

        public MainWindow()
        {
            InitializeComponent();
            GenerateMap();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            MyCanvas.Focus();
            Height = HEIGHT;
            Width = WIDTH;
        }
        private void GenerateMap()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int f = 0; f < 5; f++)
                {
                    //Find a better method to check if image exists
                    try
                    {
                        new BitmapImage(new Uri($"pack://application:,,,/Images/Map {i},{f}.JPG"));
                        new MapSegmentObject(MyCanvas, f, i, fullMap);
                    }
                    catch (IOException e)
                    {
                    }
                }

            }
            WindArrow = new WindArrow(MyCanvas);
        }

        private void GameLoop(object sender, EventArgs e)
        {

        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                fullMap.SetVisiblePos(MyCanvas, 0, 1);
            }
            if (e.Key == Key.A)
            {
                fullMap.SetVisiblePos(MyCanvas, 0, -1);
            }
            if (e.Key == Key.W)
            {
                fullMap.SetVisiblePos(MyCanvas, -1, 0);
            }
            if (e.Key == Key.S)
            {
                fullMap.SetVisiblePos(MyCanvas, 1, 0);
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {

        }
        private void RightMouseIsUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(MyCanvas);
            for (int i = 0; i < fullMap.VisibleSegment().GetWaypointsAndLines().Count(); i += 2)
            {
                MainObject waypoint = fullMap.VisibleSegment().GetWaypointsAndLines()[i];
                if (Hypotenuse(waypoint.GetLeft + 25 - point.X, waypoint.GetTop + 25 - point.Y) < Hypotenuse(25, 25))
                {
                    RemoveWaypoint(i);
                }
            }
            
        }
        private void RemoveWaypoint(int index)
        {
            //Find a better way of doing this 
            MapSegmentObject segment = fullMap.VisibleSegment();
            segment.DelWaypointOrLine(index, MyCanvas);

            if (index != 0 && segment.GetWaypointsAndLines().Count() != index)
            {
                segment.DelWaypointOrLine(index - 1, MyCanvas);
                segment.DelWaypointOrLine(index - 1, MyCanvas);
                PlaceLine(index - 1, segment.GetWaypointsAndLines()[index - 1], segment.GetWaypointsAndLines()[index - 2]);
            }
            else if (index != 0)
            {
                segment.DelWaypointOrLine(index - 1, MyCanvas);
            }
            else if (segment.GetWaypointsAndLines().Count != index)
            {
                segment.DelWaypointOrLine(index + 1, MyCanvas);
            }


        }
        private double Hypotenuse(double num1, double num2)
        {
            return Math.Sqrt(num1 * num1 + num2 * num2);
        }
        private void LeftMouseIsUp(object sender, MouseButtonEventArgs e)
        {
            PlaceWaypoint(e);
        }

        private void PlaceWaypoint(MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(MyCanvas);
            int[] visible_segment_index = { fullMap.GetVisibleSegmentIndex[0], fullMap.GetVisibleSegmentIndex[1] };
            if (!IsPixelLand(visible_segment_index, (int)Math.Round(point.Y), (int)Math.Round(point.X)))
            {
                Waypoint new_waypoint = new Waypoint(MyCanvas, point.X - 25, point.Y - 25);
                List<MainObject> waypoints = fullMap.VisibleSegment().GetWaypointsAndLines();
                if (waypoints.Count > 0)
                {
                    MainObject old_waypoint = waypoints[waypoints.Count - 1];
                    PlaceLine(fullMap.VisibleSegment().GetWaypointsAndLines().Count(),old_waypoint, new_waypoint);
                }
                fullMap.VisibleSegment().AddWaypointOrLine(fullMap.VisibleSegment().GetWaypointsAndLines().Count(), new_waypoint);
            }
        }
        private void PlaceLine(int index, MainObject waypoint1, MainObject waypoint2)
        {
            double[] LinePos = { waypoint1.GetLeft + 25, waypoint1.GetTop + 25, waypoint2.GetLeft + 25, waypoint2.GetTop + 25 };
            fullMap.VisibleSegment().AddWaypointOrLine(index, new LineObject(MyCanvas, LinePos));
        }
        private bool IsPixelLand(int[] visible_segment,int pixel_row, int pixel_col)
        {
            MapSegmentObject Segment = fullMap.VisibleSegment();
            double[] segment_dimentions = { Segment.GetHeight, Segment.GetWidth };
            Color color = GetPixelColor(Segment.GetRectangle, pixel_row, pixel_col, segment_dimentions);
            if (!(color.R == 218 && color.G == 170 && color.B == 255) && !(color.R == 218 && color.G == 173 && color.B == 255))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Color GetPixelColor(Visual visual, int row, int col, double[] segmentDimentions)
        {
            Point Dpi = getScreenDPI(visual);

            // Viewbox uses values between 0 & 1 so normalize the Rect with respect to the canvas's Height & Width
            Rect percentSrceenRec = new Rect(col / segmentDimentions[1], row / segmentDimentions[0],
                                          1 / segmentDimentions[1], 1 / segmentDimentions[0]);

            var bmpOut = new RenderTargetBitmap((int)(Dpi.X / 96.0),
                                                (int)(Dpi.Y / 96.0),
                                                Dpi.X, Dpi.Y, PixelFormats.Default); // generalized for monitors with different dpi

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                // Creates the rectangle without drawing it
                dc.DrawRectangle(new VisualBrush { Visual = visual, Viewbox = percentSrceenRec }, null, new Rect(0, 0, 1.0, 1.0));
            }
            bmpOut.Render(drawingVisual);

            var bytes = new byte[4];
            int iStride = 4; // = 4 * bmpOut.Width (for 32 bit graphics with 4 bytes per pixel -- 4 * 8 bits per byte = 32)
            bmpOut.CopyPixels(bytes, iStride, 0);
            return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
        }
        public Point getScreenDPI(Visual visual)
        {
            PresentationSource source = PresentationSource.FromVisual(visual);
            Point Dpi;
            Dpi = new Point(96.0 * source.CompositionTarget.TransformToDevice.M11, 96.0 * source.CompositionTarget.TransformToDevice.M22);
            return Dpi;
        }

        
    }
}

