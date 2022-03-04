using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace Optimal_Route_Calculator
{

    class MapSegmentObject : MainObject
    {
        private readonly ImageBrush Skin = new ImageBrush();
        private readonly BitmapImage bitmapImage;

        private bool[,] LandPixelMap;
        private readonly List<MainObject> waypointsLines = new List<MainObject>(); // Waypoint -- Line -- Waypoint -- Line -- Waypoint

        private readonly string uri;

        private int node_seperation = 5;

        public string high_water { get; set; } = "";
        private List<TidalFlowPoint> tidePoints = new List<TidalFlowPoint>();
     
        public MapSegmentObject(int index, FullMapObject fullMap)
        {
            uri = ($"pack://application:,,,/Images/Map{index}.JPG");
            bitmapImage = new BitmapImage(new Uri(uri));
            Skin.ImageSource = bitmapImage;

            GetHeight = bitmapImage.Height;
            GetWidth = bitmapImage.Width;

            shape = new Rectangle { Width = GetWidth, Height = GetHeight, Fill = Skin };

            map_segment_index = index;
            GetScalar = fullMap.GetScalar[index - 1];

            Canvas.SetLeft(shape, GetLeft);
            Canvas.SetTop(shape, GetTop);
            Panel.SetZIndex(shape, -1);

            fullMap.SetMapSegmentArr(index, this);
        }
        public double GetTidalFlowAt(Point pos, double inbound_bearing)
        {
            double dist = 100000;
            double hypot;
            TidalFlowPoint closest_point = tidePoints[0];
            foreach(TidalFlowPoint tide_point in tidePoints)
            {
                hypot = MainWindow.Hypotenuse(tide_point.GetLeft - pos.X, tide_point.GetTop - pos.Y);
                if (hypot < dist)
                {
                    dist = hypot;
                    closest_point = tide_point;
                }
            }

            if(AngleAddition(closest_point.Bearing, 45) > inbound_bearing && AngleAddition(closest_point.Bearing, -45) < inbound_bearing)
            {

            }
            return 0;
        }
        public List<TidalFlowPoint> GetTidalFlowPoints
        {
            get { return tidePoints; }
        }
        public void AddTidePoint(Point pos, Canvas MyCanvas, double Bearing, double Max, double Min)
        {
            tidePoints.Add(new TidalFlowPoint(MyCanvas, pos.X, pos.Y, Bearing, Max, Min));
        }
        public void DelTidePoint(int index, Canvas MyCanvas)
        {
            tidePoints[index].SetVisible(false, MyCanvas);
            tidePoints.RemoveAt(index);
        }
        public bool[,] GetLandPixelMap
        {
            get { return LandPixelMap; }
        }
        public void GenerateLandMap(MainWindow main_window)
        {
            double[] segment_dimentions = { GetHeight, GetWidth };
            bool[,] land_map = new bool[(int)segment_dimentions[0] / node_seperation + 1, (int)segment_dimentions[1] / node_seperation + 1];
            for (int i = 0; i < segment_dimentions[0] / node_seperation; i++)
            {
                for (int f = 0; f < segment_dimentions[1] / node_seperation; f++)
                {
                    land_map[i, f] = main_window.PixelIsLandFromVisual(i * node_seperation, f * node_seperation);
                }
            }
            LandPixelMap = land_map;
        }
        public bool landMapFileExists { get; set; } = false;
        public void CheckForMapFile()
        {
            MainWindow main_window = (MainWindow)Application.Current.MainWindow;
            string path = "LandMapFiles\\" + $"Map{map_segment_index}";
            path = FileHandler.PathToAppDirectory(path, 0);

            if (!File.Exists(path))
            {
                GenerateLandMap(main_window);
                GenerateMapFile(path);
                landMapFileExists = true;
            }
            else
            {
                LoadFromMapFile(path);
            }
        }
        private void GenerateMapFile(string path)
        {
            string line;
            using (StreamWriter file = new StreamWriter(path))
            {
                for (int i = 0; i < LandPixelMap.GetLength(0); i++)
                {
                    line = "";
                    for (int f = 0; f < LandPixelMap.GetLength(1); f++)
                    {
                        // If its land then place 1, if its sea then place a zero
                        line += LandPixelMap[i, f] == true ? "1" : "0";
                    }
                    file.WriteLine(line);
                }
            }
        }
        private void LoadFromMapFile(string path)
        {
            double[] segment_dimentions = { GetHeight, GetWidth };
            bool[,] land_map = new bool[(int)segment_dimentions[0] / node_seperation + 1, (int)segment_dimentions[1] / node_seperation + 1];
            string line;
            int line_num = 0;
            int char_num;
            using (StreamReader file = new StreamReader(path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    char_num = 0;
                    foreach (char character in line)
                    {
                        land_map[line_num, char_num] = character == '1';
                        char_num++;
                    }
                    line_num++;
                }
            }
            LandPixelMap = land_map;
        }
        public List<MainObject> GetWaypointsAndLines()
        {
            return waypointsLines;
        }
        public void AddWaypointOrLine(int index, MainObject mainObject)
        {
            waypointsLines.Insert(index, mainObject);
        }
        public void DelWaypointOrLine(int index, Canvas MyCanvas)
        {
            if (index % 2 != 0)
            {
                ((LineObject)waypointsLines[index]).ChangeRouteLineVisibility(MyCanvas);
            }
            waypointsLines[index].SetVisible(false, MyCanvas);
            waypointsLines.RemoveAt(index);
        }
        public void DelRouteLines(Canvas MyCanvas)
        {
            // If its a LineObject it will remove its RouteLines
            for (int i = 0; i < waypointsLines.Count - 1; i++)
            {
                if (i % 2 != 0)
                {
                    ((LineObject)waypointsLines[i]).ChangeRouteLineVisibility(MyCanvas);
                    ((LineObject)waypointsLines[i]).KillRouteLines();
                }
            }

        }
        public override void SetVisible(bool Visable, Canvas MyCanvas)
        {
            visible = Visable;
            if (visible)
            {
                MyCanvas.Children.Add(shape);
            }
            else
            {
                MyCanvas.Children.Remove(shape);
            }
            ChangeObjectVisibility(MyCanvas);
        }
        public void ChangeObjectVisibility(Canvas MyCanvas)
        {
            for (int i = 0; i < waypointsLines.Count(); i++)
            {
                MainObject mainObject = waypointsLines[i];
                mainObject.SetVisible(!mainObject.GetVisible(), MyCanvas);

                // If its a LineObject it will reverse the lines' route lines' visibility
                if (i % 2 != 0)
                {
                    ((LineObject)mainObject).ChangeRouteLineVisibility(MyCanvas);
                }
            }
            foreach(TidalFlowPoint tidePoint in tidePoints)
            {
                tidePoint.SetVisible(!tidePoint.GetVisible(), MyCanvas);
            }
        }
        public UIElement GetRectangle
        {
            get { return shape; }
        }
        public double GetHeight { get; }
        public double GetWidth { get; }
        public ShipObject GetShip { get; } = new ShipObject();
        public double GetScalar { get; }
    }
}
