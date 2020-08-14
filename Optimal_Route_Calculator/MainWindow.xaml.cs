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
        private List<Waypoint> waypoints = new List<Waypoint>();
        private List<LineObject> lines = new List<LineObject>();

        private double HEIGHT = 700 / (166 / 96);
        private double WIDTH = 1200 / (166 / 96);

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
        }

        private void GameLoop(object sender, EventArgs e)
        {

        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                fullMap.SetVisiblePos(MyCanvas, 0, 1, waypoints, lines);
            }
            if (e.Key == Key.A)
            {
                fullMap.SetVisiblePos(MyCanvas, 0, -1, waypoints, lines);
            }
            if (e.Key == Key.W)
            {
                fullMap.SetVisiblePos(MyCanvas, -1, 0, waypoints, lines);
            }
            if (e.Key == Key.S)
            {
                fullMap.SetVisiblePos(MyCanvas, 1, 0, waypoints, lines);
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {

        }

        private void LeftMouseIsUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(MyCanvas);
            int[] visible_segment = { fullMap.GetVisibleSegment[0], fullMap.GetVisibleSegment[1] };
            if (IsPixelLand(visible_segment ,(int)Math.Round(point.Y), (int)Math.Round(point.X)))
            {
                Waypoint new_waypoint = new Waypoint(MyCanvas, point.X - 25, point.Y - 25, visible_segment[0], visible_segment[1]);
                waypoints.Add(new_waypoint);
                if (waypoints.Count > 1)
                {
                    Waypoint old_waypoint = waypoints[waypoints.Count - 2];
                    double[] LinePos = { old_waypoint.GetLeft + 25, old_waypoint.GetTop + 25, new_waypoint.GetLeft + 25, new_waypoint.GetTop + 25 };
                    lines.Add(new LineObject(MyCanvas, visible_segment[0], visible_segment[1], LinePos));
                }
            }
        }

        private bool IsPixelLand(int[] visible_segment,int pixel_row, int pixel_col)
        {
            MapSegmentObject Segment = fullMap.GetMapSegmentArr()[visible_segment[0], visible_segment[1]];
            Color color = GetPixelColor(Segment.GetRectangle, pixel_row, pixel_col);
            return true;
        }
        public Color GetPixelColor(Visual visual, int row, int col)
        {
            Point Dpi = getScreenDPI(visual);

            // Viewbox uses values between 0 & 1 so normalize the Rect with respect to the canvas's Height & Width
            Rect percentSrceenRec = new Rect(col / WIDTH, row / HEIGHT,
                                          1 / WIDTH, 1 / HEIGHT);

            // var bmpOut = new RenderTargetBitmap(1, 1, 96, 96, PixelFormats.Pbgra32); //assumes 96 dpi
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

