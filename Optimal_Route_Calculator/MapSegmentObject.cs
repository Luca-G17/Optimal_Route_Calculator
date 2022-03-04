using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Optimal_Route_Calculator
{
    /// <summary>
    /// The class that defines the areas of map
    /// It contains all of the objects on that specific map
    /// When the visible map is changed this class will handle changing the visibility of its objects
    /// </summary>
    class MapSegmentObject : MainObject
    {
        private readonly ImageBrush Skin = new ImageBrush();
        private readonly BitmapImage bitmapImage;

        private bool[,] TerrainPixelMap;
        private readonly List<MainObject> waypointsLines = new List<MainObject>(); // Waypoint -- Line -- Waypoint -- Line -- Waypoint

        private readonly string uri;

        private const int NODE_SEPERATION = 4;

        public string HighWater { get; set; } = "";

        public MapSegmentObject(int index, FullMapObject fullMap)
        {
            // Get the map image URI
            uri = ($"pack://application:,,,/Images/Map{index}.JPG");

            // Set the bitmapImage of the map with the source URI
            bitmapImage = new BitmapImage(new Uri(uri));

            // Add the bitmap as the fill of the rectangle for the map
            Skin.ImageSource = bitmapImage;

            GetHeight = bitmapImage.Height;
            GetWidth = bitmapImage.Width;

            shape = new Rectangle { Width = GetWidth, Height = GetHeight, Fill = Skin };

            map_segment_index = index;
            GetScalar = fullMap.GetScalar[index - 1];

            Canvas.SetLeft(shape, GetLeft);
            Canvas.SetTop(shape, GetTop);

            // Makes sure that the map will always be behind any objects placed onto it
            Panel.SetZIndex(shape, -1);

            fullMap.SetMapSegmentArr(index, this);
        }
        /// <summary>
        /// Iterates through the tide points and returns the tide point thats nearest to the specified position and less than 400 DIPs away
        /// </summary>
        public TidalFlowPoint NearestTidePoint(Point pos)
        {
            double dist = 400;
            double hypot;
            TidalFlowPoint closest_point = null;
            foreach (TidalFlowPoint tide_point in GetTidalFlowPoints)
            {
                hypot = MainWindow.Hypotenuse(tide_point.GetLeft - pos.X, tide_point.GetTop - pos.Y);
                if (hypot < dist)
                {
                    dist = hypot;
                    closest_point = tide_point;
                }
            }
            return closest_point;
        }
        public double GetBoatSpeedAt(Point pos, double inbound_bearing)
        {
            TidalFlowPoint closest_point = NearestTidePoint(pos);
            if (closest_point == null)
            {
                return GetShip.GetMaxSpeed;
            }

            //Calculate time until the next high water which will be used to offset the cos graph in order to calculate the tidal flow now
            closest_point.CalculateTimeOffset(HighWater);

            // u^2 = v^2 + r^2 - 2vr cos(theta) -- Consine rule, rearrange to equal 0;
            // v^2 + 2r sin(theta)v + r^2 - u^2 = 0 -- Sub coefficients into quadratic formula
            // rcos(theta) +- sqrt((-2rcos(theta))^2 - 4r^2 + 4u^2) = v
            // Where r = tidal velocity, 
            // u = initial velocity, 
            // v = final velocity, 
            // theta = angle between initial velocity vector and tidal velocity vector

            // Calculate Flow at time = 0, Now
            double r = closest_point.CalculateFlow(0);
            double u = GetShip.GetMaxSpeed;
            double tide_bearing = BearingNormaliser(closest_point.Bearing);
            double ship_bearing = BearingNormaliser(GetShip.AngleAddition(inbound_bearing * 180 / Math.PI, 90)) * Math.PI / 180;

            double theta = tide_bearing - ship_bearing;

            // Math.Abs ensures the equation always returns the positive root
            double v = Math.Abs(r * Math.Sin(theta) - Math.Sqrt(Math.Pow(2 * r * Math.Sin(theta), 2) - 4 * (r * r - u * u)) / 2);

            return v;
        }
        /// <summary>
        /// Converts a North-0 based bearing to a angle from the X-axis
        /// </summary>
        /// <param name="bearing"></param>
        /// <returns></returns>
        public double BearingNormaliser(double bearing)
        {
            // TODO: This process can be optmised most likely using MOD
            if (bearing > 3 * Math.PI / 2)
            {
                bearing = 2 * Math.PI - bearing;
            }
            else if (bearing > Math.PI)
            {
                bearing = -(bearing - Math.PI);
            }
            else if (bearing > Math.PI / 2)
            {
                bearing -= Math.PI / 2;
            }
            else
            {
                bearing = -bearing;
            }
            return bearing;
        }
        public List<TidalFlowPoint> GetTidalFlowPoints { get; } = new List<TidalFlowPoint>();
        public void AddTidePoint(Point pos, Canvas MyCanvas, double Bearing, double Max, double Min)
        {
            GetTidalFlowPoints.Add(new TidalFlowPoint(MyCanvas, pos.X, pos.Y, Bearing, Max, Min, HighWater));
        }
        public void DelTidePoint(int index, Canvas MyCanvas)
        {
            // Removes the specified tide point
            GetTidalFlowPoints[index].SetVisible(false, MyCanvas);
            GetTidalFlowPoints.RemoveAt(index);
        }
        public bool[,] GetTerrainPixelMap
        {
            get { return TerrainPixelMap; }
        }
        /// <summary>
        /// This method will only be called on the first launch of the program, it generates the terrain map 2D array
        /// It will iterate through the bitmap every 5 pixels getting the colour and returing true to the array if land is detected
        /// This process can be slow so this tarrain map will be saved to file
        /// </summary>
        /// <param name="main_window"></param>
        public void GenerateTerrainMap(MainWindow main_window)
        {
            double[] segment_dimentions = { GetHeight, GetWidth };
            bool[,] terrain_map = new bool[(int)segment_dimentions[0] / NODE_SEPERATION + 1, (int)segment_dimentions[1] / NODE_SEPERATION + 1];
            for (int i = 0; i < segment_dimentions[0] / NODE_SEPERATION; i++)
            {
                for (int f = 0; f < segment_dimentions[1] / NODE_SEPERATION; f++)
                {
                    terrain_map[i, f] = main_window.PixelIsLandFromVisual(i * NODE_SEPERATION, f * NODE_SEPERATION);
                }
            }
            TerrainPixelMap = terrain_map;
        }
        public bool TerrainMapFileExists { get; set; } = false;

        /// <summary>
        /// Checks the if TerrainMapFile exists, if not a new one is generated
        /// </summary>
        public void CheckForMapFile()
        {
            // Checks if the terrain map already exists
            MainWindow main_window = (MainWindow)Application.Current.MainWindow;
            string path = "TerrainMapFiles\\" + $"Map{map_segment_index}";
            path = FileHandler.PathToAppDirectory(path, 0);

            if (!File.Exists(path))
            {
                GenerateTerrainMap(main_window);
                GenerateMapFile(path);
                TerrainMapFileExists = true;
            }
            else
            {
                LoadFromMapFile(path);
            }
        }
        /// <summary>
        /// Creates the terrain map file and writes the terrain map as 1s and 0s to that file
        /// 1 = Land, 0 = Water
        /// </summary>
        /// <param name="path"></param>
        private void GenerateMapFile(string path)
        {
            string line;
            using (StreamWriter file = new StreamWriter(path))
            {
                for (int i = 0; i < TerrainPixelMap.GetLength(0); i++)
                {
                    line = "";
                    for (int f = 0; f < TerrainPixelMap.GetLength(1); f++)
                    {
                        // If its land then place 1, if its sea then place a zero
                        line += TerrainPixelMap[i, f] == true ? "1" : "0";
                    }
                    file.WriteLine(line);
                }
            }
        }
        /// <summary>
        /// Loads the terrain map from the terrain map file
        /// </summary>
        /// <param name="path"></param>
        private void LoadFromMapFile(string path)
        {
            double[] segment_dimentions = { GetHeight, GetWidth };
            bool[,] terrain_map = new bool[(int)segment_dimentions[0] / NODE_SEPERATION + 1, (int)segment_dimentions[1] / NODE_SEPERATION + 1];
            string line;
            int line_num = 0;
            int char_num;
            using (StreamReader file = new StreamReader(path))
            {
                // Iterates through the text file line by line
                while ((line = file.ReadLine()) != null)
                {
                    char_num = 0;

                    // Iterates through each line
                    foreach (char character in line)
                    {
                        // If character == '1' return true
                        // Else return false
                        terrain_map[line_num, char_num] = character == '1';
                        char_num++;
                    }
                    line_num++;
                }
            }
            TerrainPixelMap = terrain_map;
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
            if (index % 2 != 0 || (index == 0 && waypointsLines.Count % 2 == 0))
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
                    ((LineObject)waypointsLines[i]).RemoveRouteLines();
                }
            }

        }
        public override void SetVisible(bool Visable, Canvas MyCanvas)
        {
            // Map segments needs to override this method because it needs to also change its objects visibility
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
            // Iterate through waypoints and lines and change visibilty
            for (int i = 0; i < waypointsLines.Count(); i++)
            {
                MainObject mainObject = waypointsLines[i];
                mainObject.SetVisible(!mainObject.GetVisible(), MyCanvas);

                // If its a LineObject it will reverse the line's 'route lines' visibility
                if (i % 2 != 0)
                {
                    ((LineObject)mainObject).ChangeRouteLineVisibility(MyCanvas);
                }
            }
            foreach (TidalFlowPoint tidePoint in GetTidalFlowPoints)
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
