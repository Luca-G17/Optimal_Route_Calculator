using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using System.Windows.Media.Imaging;

namespace Optimal_Route_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        FullMapObject fullMap = new FullMapObject();
        List<Waypoint> waypoints = new List<Waypoint>();

        public MainWindow()
        {
            InitializeComponent();
            GenerateMap();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            MyCanvas.Focus();
            Height = 700 / (166 / 96);
            Width = 1200 / (166 / 96);
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
                        fullMap.GetMapSegmentArr()[f, i].ImageToByte();
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
                fullMap.SetVisiblePos(MyCanvas, 0, 1, waypoints);
            }
            if (e.Key == Key.A)
            {
                fullMap.SetVisiblePos(MyCanvas, 0, -1, waypoints);
            }
            if (e.Key == Key.W)
            {
                fullMap.SetVisiblePos(MyCanvas, -1, 0, waypoints);
            }
            if (e.Key == Key.S)
            {
                fullMap.SetVisiblePos(MyCanvas, 1, 0, waypoints);
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {

        }

        private void LeftMouseIsUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(MyCanvas);
            waypoints.Add(new Waypoint(MyCanvas, point.X, point.Y, fullMap.GetVisibleSegment[0], fullMap.GetVisibleSegment[1]));
        }
    }
}

