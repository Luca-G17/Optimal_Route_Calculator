﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Net;

namespace Optimal_Route_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer mainTimer = new DispatcherTimer();
        public WindArrow windArrow;
        private TextBlockObject waypointTimeIndicator;
        private TextBlockObject windSpeedIndicator;
        private TextBlockObject routeDistanceIndicator;
        private TextBlockObject routeTimeIndicator;
        private TextBlockObject clockDisplay;

        private delegate void NoArgDelegate();

        private const double WAYPOINT_RADIUS = 25;
        private const double NODE_SEPERATION = 4;
        private const double LINE_STEP_DIST = 1;

        private readonly double HEIGHT = 620;
        private readonly double WIDTH = 1250;

        private DateTime plot_time;
        private double wind_angle = 40;
        private double max_speed = 6;
        private Point DPI;
        private bool first_run = true;
        private readonly double[] map_coords = { 50.800314, -1.289956 };

        public MainWindow()
        {
            InitializeComponent();

            // Initialises API
            APIManager.InitialiseClient();
            WindAPICall();

            GenerateMap();

            // Attaches Main loop as an event handler to the Timer
            mainTimer.Tick += MainLoop;
            mainTimer.Interval = TimeSpan.FromMilliseconds(20);
            mainTimer.Start();

            MyCanvas.Focus();

            Height = HEIGHT;
            Width = WIDTH;

        }

        private async void WindAPICall()
        {
            windArrow = new WindArrow(MyCanvas);
            windSpeedIndicator = new TextBlockObject(0, 0, "Wind Speed: 0 Kts", MyCanvas, 22, 1);
            WindModel wind_data;
            if (IsConnectedToInternet())
            {
                wind_data = await WindAPIProcessor.LoadWindData(map_coords);
            }
            else
            {
                // No network connection:
                wind_data = new WindModel
                {
                    wind_bearing = 90,
                    wind_speed = "0"
                };
            }
            windArrow.Rotation = wind_data.wind_bearing;
            windSpeedIndicator.SetMessage = $"Wind Speed: {wind_data.wind_speed}Kts";
        }
        public static bool IsConnectedToInternet()
        {
            try
            {
                using (var client = new WebClient())
                    using (client.OpenRead("http://google.com/generate_204"))
                        return true;
            }
            catch
            {
                return false;
            }
        }
        public void GenerateMap()
        {
            GetFullMap.SetScalar = 0.0375956;
            GetFullMap.SetScalar = 0.0190481;
            for (int i = 1; i <= 2; i++)
            {
                //Find a better method to check if image exists
                try
                {
                    new BitmapImage(new Uri($"pack://application:,,,/Images/Map{i}.JPG"));
                    new MapSegmentObject(i, GetFullMap);
                }
                catch (IOException)
                {
                    throw new Exception("Map Image file not found");
                }
            }

            GetFullMap.SetVisiblePos(MyCanvas, -1);
            routeDistanceIndicator = new TextBlockObject(1070, 425, " - Route Distance = 0Nm", InfoPanel, 12, 0);
            routeTimeIndicator = new TextBlockObject(1070, 450, " - Route Time = 0", InfoPanel, 12, 0);
            clockDisplay = new TextBlockObject(990, 0, "", MyCanvas, 22, 1);
        }

        internal FullMapObject GetFullMap { get; } = new FullMapObject();
        private void UpdateClock()
        {
            string time = DateTime.Now.ToString("T");
            clockDisplay.SetMessage = time;
        }
        private void MainLoop(object sender, EventArgs e)
        {

            if (first_run)
            {
                // Add anything here that can't be run in the constructor but has to be run on launch
                first_run = !first_run;
                DPI = GetScreenDPI(MyCanvas);
                GetFullMap.VisibleSegment().CheckForMapFile();

                LoadingCompass.Visibility = Visibility.Collapsed;
                LoadingRectangle.Visibility = Visibility.Collapsed;
                LoadingText.Visibility = Visibility.Collapsed;
            }
            if (InputValidator.TextInputCheck(WindAngleInput, wind_angle))
            {
                ChangeWindAngle();
            }
            if (InputValidator.TextInputCheck(MaxHullSpeedInput, max_speed))
            {
                ChangeMaxSpeed();
            }
            if (waypointTimeIndicator != null)
            {
                if (MouseOverWaypoint() == default)
                {
                    waypointTimeIndicator.SetVisible(false, MyCanvas);
                    waypointTimeIndicator = null;
                }
            }
            UpdateClock();

        }

        #region UserInputs
        /// <summary>
        /// Returns the index of the waypoint that the mouse is over if any
        /// </summary>
        /// <returns></returns>
        private int MouseOverWaypoint()
        {
            Point point = Mouse.GetPosition(MyCanvas);
            MapSegmentObject segment = GetFullMap.VisibleSegment();
            for (int i = 0; i < segment.GetWaypointsAndLines().Count; i += 2)
            {
                MainObject waypoint = segment.GetWaypointsAndLines()[i];
                if (Hypotenuse(point.X - waypoint.GetLeft - WAYPOINT_RADIUS, point.Y - waypoint.GetTop - WAYPOINT_RADIUS) < WAYPOINT_RADIUS)
                {
                    return i;
                }
            }
            return default;
        }
        /// <summary>
        /// Sums the total length of all route lines
        /// </summary>
        /// <param name="final_index"></param>
        /// <returns></returns>
        private double TotalRouteLineLength(int final_index)
        {
            double route_line_length = 0;
            for (int i = final_index; i > 0; i -= 2)
            {
                route_line_length += ((LineObject)GetFullMap.VisibleSegment().GetWaypointsAndLines()[i]).RouteLineLength();
            }
            // Converts from DIPs to Nm
            route_line_length *= GetFullMap.VisibleSegment().GetScalar;
            return route_line_length;
        }

        /// <summary>
        /// Display the time when you should reach this waypoint
        /// </summary>
        /// <param name="index"></param>
        private void DisplayWaypointData(int index)
        {
            MainObject waypoint = GetFullMap.VisibleSegment().GetWaypointsAndLines()[index];
            double route_time_mins = 60 * TotalRouteLineLength(index - 1) / max_speed;

            DateTime waypoint_arrival_time = plot_time.AddMinutes(route_time_mins);
            string arrival_time_str = waypoint_arrival_time.ToString("t");
            waypointTimeIndicator = new TextBlockObject((int)waypoint.GetLeft - 10, (int)waypoint.GetTop - 10, $"Arrival Time = {arrival_time_str}", MyCanvas, 12, 1);
            waypointTimeIndicator.SetBackground(Brushes.White);
        }

        private void ChangeMaxSpeed()
        {
            string input = MaxHullSpeedInput.Text;
            if (InputValidator.NumValidate(input))
            {
                max_speed = Convert.ToDouble(input);
                InputStatusMessageTextBlock.Foreground = Brushes.Black;
                InputStatusMessageTextBlock.Text = "Hull Speed Accepted";
                return;
            }
            InputStatusMessageTextBlock.Foreground = Brushes.Red;
            InputStatusMessageTextBlock.Text = "Hull Speed Invalid";
        }

        private void ChangeWindAngle()
        {
            // WindAngle = How close the boat can point to the wind (Default = 40 degrees either side)
            string input = WindAngleInput.Text;
            if (InputValidator.NumValidate(input))
            {
                double new_angle = Convert.ToDouble(input);
                if (new_angle < 90 && new_angle >= 0)
                {
                    GetFullMap.VisibleSegment().GetShip.GetBoatToWind = new_angle;
                    wind_angle = new_angle;
                    InputStatusMessageTextBlock.Foreground = Brushes.Black;
                    InputStatusMessageTextBlock.Text = "Wind Angle Accepted";
                    return;
                }
            }
            InputStatusMessageTextBlock.Foreground = Brushes.Red;
            InputStatusMessageTextBlock.Text = "Invalid Wind Angle";
        }
        private void KeyIsUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.D)
            {
                GetFullMap.SetVisiblePos(MyCanvas, 1);
                if (!GetFullMap.VisibleSegment().TerrainMapFileExists)
                {
                    // Forces a UI refresh which is necesarry to build a new terrain map file if required
                    MyCanvas.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, (NoArgDelegate)delegate { });
                    GetFullMap.VisibleSegment().CheckForMapFile();
                }
            }
            if (e.Key == Key.A)
            {
                GetFullMap.SetVisiblePos(MyCanvas, -1);
            }
            if (e.Key == Key.F)
            {
                int waypoint_index = MouseOverWaypoint();
                if (waypoint_index != default)
                {
                    DisplayWaypointData(MouseOverWaypoint());
                }
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed) // Add Tide Point
                {
                    NewTidalPointWindow newTidalPointWindow = new NewTidalPointWindow(Mouse.GetPosition(MyCanvas));
                    newTidalPointWindow.Show();
                }
                if (Mouse.RightButton == MouseButtonState.Pressed) // Delete Tide Point
                {
                    Point mousePos = Mouse.GetPosition(MyCanvas);
                    List<TidalFlowPoint> tidePoints = GetFullMap.VisibleSegment().GetTidalFlowPoints;
                    foreach (TidalFlowPoint tidePoint in tidePoints)
                    {
                        if (Hypotenuse(tidePoint.GetLeft - mousePos.X, tidePoint.GetTop - mousePos.Y) <= 25)
                        {
                            GetFullMap.VisibleSegment().DelTidePoint(tidePoints.IndexOf(tidePoint), MyCanvas);
                            return;
                        }
                    }
                }
            }

        }
        private void OnPlot(object sender, RoutedEventArgs e)
        {
            plot_time = DateTime.Now;
            MapSegmentObject visible_segment = GetFullMap.VisibleSegment();
            if (visible_segment.GetWaypointsAndLines().Count >= 3)
            {
                // Removes existing route lines
                visible_segment.DelRouteLines(MyCanvas);

                // Plot the new route
                TackingRoutePlotter tacking_route = new TackingRoutePlotter(visible_segment, this, max_speed);

                routeDistanceIndicator.SetMessage = $" - Route Distance = {Math.Round(tacking_route.RouteDistance, 2)}Nm";
                routeTimeIndicator.SetMessage = $" - Route Time = {tacking_route.RouteTime}";
            }
        }
        private async void OnOptimise(object sender, RoutedEventArgs e)
        {
            await OptimiseStart(this); // Asynchronous
        }
        private void OnReset(object sender, RoutedEventArgs e)
        {
            Reset();
        }
        /// <summary>
        /// Deletes all waypoints and tide points on all map segements
        /// </summary>
        public void Reset()
        {
            foreach (MapSegmentObject segment in GetFullMap.GetMapSegmentArr())
            {
                if (segment != null)
                {
                    segment.DelRouteLines(MyCanvas);
                    for (int i = 0; i < segment.GetWaypointsAndLines().Count;)
                    {
                        segment.DelWaypointOrLine(i, MyCanvas);
                    }

                    for (int i = 0; i < segment.GetTidalFlowPoints.Count; i++)
                    {
                        segment.DelTidePoint(i, MyCanvas);
                    }
                }
            }
        }
        private void OnHelp(object sender, RoutedEventArgs e)
        {
            HelpWindow help_window = new HelpWindow();
            help_window.Show();
        }
        private void LeftMouseIsUp(object sender, MouseButtonEventArgs e)
        {
            double[] coords = { e.GetPosition(MyCanvas).X, e.GetPosition(MyCanvas).Y };
            if (coords[0] < 1070 && coords[1] < HEIGHT && !Keyboard.IsKeyDown(Key.LeftShift)) // Add waypoint
            {
                PlaceWaypoint(coords);
            }
        }
        private void RightMouseIsUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(MyCanvas);
            for (int i = 0; i < GetFullMap.VisibleSegment().GetWaypointsAndLines().Count(); i += 2) // Delete Waypoint
            {
                MainObject waypoint = GetFullMap.VisibleSegment().GetWaypointsAndLines()[i];
                if (Hypotenuse(waypoint.GetLeft + WAYPOINT_RADIUS - point.X, waypoint.GetTop + WAYPOINT_RADIUS - point.Y) <= WAYPOINT_RADIUS)
                {
                    RemoveWaypoint(i);
                }
            }
        }
        private void OnSaveLoad(object sender, RoutedEventArgs e)
        {
            SaveLoadWindow saveLoad = new SaveLoadWindow();
            saveLoad.Show();
        }
        #endregion

        #region Waypoints
        public void RemoveWaypoint(int index)
        {
            //Find a better way of doing this 
            MapSegmentObject segment = GetFullMap.VisibleSegment();
            segment.DelWaypointOrLine(index, MyCanvas);

            int Count = segment.GetWaypointsAndLines().Count();
            if (Count != 0)
            {
                if (index == Count)
                {
                    // If its the lastt waypoint then only delete the line before
                    segment.DelWaypointOrLine(index - 1, MyCanvas);
                }
                else if (index == 0)
                {
                    // If its the first waypoint then only delete the line after
                    segment.DelWaypointOrLine(index, MyCanvas);
                }
                else
                {
                    // If its not the first or last waypoint then delete the line either side of the waypoint and place a line between the two waypoints either side
                    segment.DelWaypointOrLine(index - 1, MyCanvas);
                    segment.DelWaypointOrLine(index - 1, MyCanvas);
                    PlaceWaypointLine(index - 1, segment.GetWaypointsAndLines()[index - 1], segment.GetWaypointsAndLines()[index - 2]);
                }
            }
        }
        public static double Hypotenuse(double num1, double num2)
        {
            return Math.Sqrt(num1 * num1 + num2 * num2);
        }
        public void PlaceWaypoint(double[] coords)
        {
            if (PixelIsLand((int)Math.Round(coords[1]), (int)Math.Round(coords[0])) == false) // If not land
            {
                Waypoint new_waypoint = new Waypoint(MyCanvas, coords[0] - WAYPOINT_RADIUS, coords[1] - WAYPOINT_RADIUS);
                List<MainObject> waypoints = GetFullMap.VisibleSegment().GetWaypointsAndLines();
                if (waypoints.Count > 0)
                {
                    MainObject old_waypoint = waypoints[waypoints.Count - 1];
                    PlaceWaypointLine(GetFullMap.VisibleSegment().GetWaypointsAndLines().Count(), old_waypoint, new_waypoint); // Add connector line
                }
                GetFullMap.VisibleSegment().AddWaypointOrLine(GetFullMap.VisibleSegment().GetWaypointsAndLines().Count(), new_waypoint);
            }
        }
        private void PlaceWaypointLine(int index, MainObject object1, MainObject object2)
        {
            double[] LinePos = { object1.GetLeft + WAYPOINT_RADIUS, object1.GetTop + WAYPOINT_RADIUS, object2.GetLeft + WAYPOINT_RADIUS, object2.GetTop + WAYPOINT_RADIUS };

            GetFullMap.VisibleSegment().AddWaypointOrLine(index, new LineObject(MyCanvas, LinePos, Brushes.Black));
        }

        private async Task OptimiseStart(MainWindow mainWindow)
        {
            // Generating the route on a seperate thread so the program doesnt freeze up

            ShortestRouteQueue shortestRoutes = new ShortestRouteQueue();
            List<MainObject> lines = GetFullMap.VisibleSegment().GetWaypointsAndLines();

            for (int i = 1; i < lines.Count; i += 2)
            {
                shortestRoutes.Enqueue(await Task.Run(() => Optimise(mainWindow, ((LineObject)lines[i]).LinePos)));
            }

            // The new waypoints have to be placed on the main thread because the UI can only be accessed on one thread
            PlaceGeneratedWaypoints(shortestRoutes, mainWindow);
        }
        private ShortestRouteObject Optimise(MainWindow main_window, double[] line_pos)
        {
            ShortestRouteObject short_route = new ShortestRouteObject(line_pos, NODE_SEPERATION, main_window);
            return short_route;
        }
        private static void PlaceGeneratedWaypoints(ShortestRouteQueue shortestRoutes, MainWindow main_window)
        {
            for (int i = 0; i < 2 * shortestRoutes.Count(); i += 2) // Remove user placed waypoints
            {
                main_window.RemoveWaypoint(2);
            }
            while (shortestRoutes.Count() != 0) // Add Generated waypoints in order from the queue
            {
                ShortestRouteObject short_route = shortestRoutes.Dequeue();

                foreach (GridNode node in short_route.GetRouteCoords)
                {
                    main_window.PlaceWaypoint(new double[] { node.X, node.Y });
                }
            }
        }
        #endregion

        #region LandDetection
        public static double[] CalculateLineEquation(double[] line_pos)
        {
            // Gradient = Change in Y / Change in X
            double grad_to_node = (line_pos[1] - line_pos[3]) / (line_pos[0] - line_pos[2]);
            double X_dist_to_node = line_pos[0] - line_pos[2];
            double Y_intercept = grad_to_node * -line_pos[0] + line_pos[1];

            // If line is near-vertical Y_intercept will default to infinity
            // This ensures that it can never be infinity
            if (double.IsPositiveInfinity(Y_intercept))
            {
                Y_intercept = 99999;
            }
            if (double.IsNegativeInfinity(Y_intercept))
            {
                Y_intercept = -99999;
            }

            double step_in_X = LINE_STEP_DIST / Math.Sqrt(1 + (grad_to_node * grad_to_node));

            if (X_dist_to_node > 0)
            {
                step_in_X *= -1;
            }

            return new double[] { step_in_X, grad_to_node, Y_intercept };
        }
        public bool LineIntersectsLand(double[] line_pos)
        {
            // 0 = Step, 1 = Gradient to node, 2 = Y-intercept
            double[] line_info = CalculateLineEquation(line_pos);

            int LandPixelCount = 0;
            double Y;
            for (double X = line_pos[0]; X > line_pos[2] + 1 || X < line_pos[2] - 1; X += line_info[0])
            {
                // Y = mX + c
                Y = line_info[1] * X + line_info[2];
                if (PixelIsLand((int)Y, (int)X))
                {
                    LandPixelCount++;
                    if (LandPixelCount > 10)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Queries the Terrain map array to see if a specified location contains land or sea
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool PixelIsLand(int row, int col)
        {
            bool[,] terrain_pixel_map = GetFullMap.VisibleSegment().GetTerrainPixelMap;
            row /= (int)NODE_SEPERATION;
            col /= (int)NODE_SEPERATION;
            if (row > terrain_pixel_map.GetLength(0) - 1 || col > terrain_pixel_map.GetLength(1) - 1 || row < 0 + 1 || col < 0 + 1)
            {
                return true;
            }
            else
            {
                try
                {
                    return terrain_pixel_map[row, col];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new Exception($"Land Detection out of range, coords: ({col * NODE_SEPERATION}, {row * NODE_SEPERATION})");
                }
            }
        }
        /// <summary>
        /// Gets whether a pixel is land from the image itself
        /// </summary>
        /// <param name="pixel_row"></param>
        /// <param name="pixel_col"></param>
        /// <returns></returns>
        public bool PixelIsLandFromVisual(int pixel_row, int pixel_col)
        {
            MapSegmentObject Segment = GetFullMap.VisibleSegment();
            double[] segment_dimentions = { Segment.GetHeight, Segment.GetWidth };
            Color colour = GetPixelColor(Segment.GetRectangle, pixel_row, pixel_col, segment_dimentions);
            if (colour.R <= 225 && colour.R >= 185 && colour.G <= 180 && colour.G >= 145 && colour.B <= 255 && colour.B >= 235)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Gets the pixel colour at a specified place on the screen
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="segmentDimentions"></param>
        /// <returns></returns>
        public Color GetPixelColor(Visual visual, int row, int col, double[] segmentDimentions)
        {
            GC.WaitForPendingFinalizers();

            // Viewbox uses values between 0 & 1 so normalize the Rect with respect to the canvas Height & Width
            Rect percentSrceenRec = new Rect(col / segmentDimentions[1], row / segmentDimentions[0],
                                          1 / segmentDimentions[1], 1 / segmentDimentions[0]);
            var bmpOut = new RenderTargetBitmap((int)(DPI.X / 96.0),
                                                (int)(DPI.Y / 96.0),
                                                DPI.X, DPI.Y, PixelFormats.Default); // generalized for monitors with different dpi

            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                // Creates the rectangle without drawing it
                dc.DrawRectangle(new VisualBrush { Visual = visual, Viewbox = percentSrceenRec }, null, new Rect(0, 0, 1.0, 1.0));
            }
            bmpOut.Render(drawingVisual);

            var bytes = new byte[4];
            int iStride = 4; // = 4 * bmpOut.Width (for 32 bit graphics with 4 bytes per pixel - 4 * 8 bits = 32)
            bmpOut.CopyPixels(bytes, iStride, 0);
            return Color.FromArgb(bytes[0], bytes[1], bytes[2], bytes[3]);
        }
        public static Point GetScreenDPI(Visual visual)
        {
            PresentationSource source = PresentationSource.FromVisual(visual);
            Point Dpi;
            Dpi = new Point(96.0 * source.CompositionTarget.TransformToDevice.M11, 96.0 * source.CompositionTarget.TransformToDevice.M22);
            return Dpi;
        }

        #endregion


    }
}

