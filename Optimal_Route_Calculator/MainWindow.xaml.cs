using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace Optimal_Route_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly DispatcherTimer gameTimer = new DispatcherTimer();
        WindArrow windArrow;
        TextBlockObject waypointTimeIndicator;
        TextBlockObject windSpeedIndicator;
        TextBlockObject routeDistanceIndicator;
        TextBlockObject routeTimeIndicator;
        TextBlockObject clockDisplay;
        

        private const double WAYPOINT_RADIUS = 25;

        private readonly double HEIGHT = 620;
        private readonly double WIDTH = 1250;

        private DateTime plot_time;
        private double node_seperation = 5;
        private double wind_angle = 40;
        private double max_speed = 6;

        public MainWindow()
        {
            InitializeComponent();
            APIManager.InitialiseClient();
            WindAPICall();
            GenerateMap();
            gameTimer.Tick += MainLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            MyCanvas.Focus();

            Height = HEIGHT;
            Width = WIDTH;
        }
        private async void WindAPICall()
        {
            double[] loc = { 50.800314, -1.289956 };
            var wind_data = await WindAPIProcessor.LoadWindData(loc);
            windArrow.GetRotation = wind_data.wind_bearing;
            windSpeedIndicator.SetMessage = $"Wind Speed: {wind_data.wind_speed}Kts";
        }
        public void GenerateMap()
        {
            for (int i = 0; i < 5; i++)
            {
                //Find a better method to check if image exists
                try
                {
                    double scalar = i == 1 ? 0.0375956 : 0.0190481; // TODO: Replace this line with a better solution
                    new BitmapImage(new Uri($"pack://application:,,,/Images/Map{i}.JPG"));
                    new MapSegmentObject(i, GetFullMap, scalar);
                }
                catch (IOException)
                {
                }
            }
            GetFullMap.SetVisiblePos(MyCanvas, -1);
            windArrow = new WindArrow(MyCanvas);
            windSpeedIndicator = new TextBlockObject(0, 0, "Wind Speed: 0 Kts", MyCanvas, 22, 1);
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
            if (TextInputCheck(StepInput, node_seperation))
            {
                StepChanged();
            }
            if (TextInputCheck(WindAngleInput, wind_angle))
            {
                ChangeWindAngle();
            }
            if (TextInputCheck(MaxHullSpeedInput, max_speed))
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
        private bool TextInputCheck(TextBox textBox, double existing_value)
        {
            if (textBox.Text != "" && textBox.Text != existing_value.ToString())
            {
                return true;
            }
            return false;
        }
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
        private void DisplayWaypointData(int index)
        {
            // Display the time when you should reach this waypoint
            MainObject waypoint = GetFullMap.VisibleSegment().GetWaypointsAndLines()[index];
            double route_time_mins = 60 * TotalRouteLineLength(index - 1) / max_speed;

            DateTime waypoint_arrival_time = plot_time.AddMinutes(route_time_mins);
            string arrival_time_str = waypoint_arrival_time.ToString("t");
            waypointTimeIndicator = new TextBlockObject((int)waypoint.GetLeft - 10, (int)waypoint.GetTop - 10, $"Arrival Time = {arrival_time_str}", MyCanvas, 12, 1);
            waypointTimeIndicator.SetBackground(Brushes.White);
        }
        private bool NumValidate(string inp)
        {
            foreach (char character in inp)
            {
                // ASCII: 48 = '0', 57 = '9', 46 = '.'
                if ((character < 48 || character > 57) && character != 46)
                {
                    return false;
                }
            }
            return true;
        }
        private void ChangeMaxSpeed()
        {
            string input = MaxHullSpeedInput.Text;
            if (NumValidate(input))
            {
                max_speed = Convert.ToDouble(input);
            }
        }

        private void ChangeWindAngle()
        {
            // WindAngle = How close the boat can point to the wind (Default = 40 degrees either side)
            string input = WindAngleInput.Text;
            if (NumValidate(input))
            {
                double new_angle = Convert.ToDouble(input);        
                if (new_angle < 45 && new_angle >= 0)
                {
                    GetFullMap.VisibleSegment().GetShip.GetBoatToWind = wind_angle;
                    wind_angle = new_angle;
                }
            }
        }
        private void StepChanged()
        {
            string input = StepInput.Text;
            if (NumValidate(input))
            {
                node_seperation = Convert.ToDouble(input);
            }
        }
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D)
            {
                GetFullMap.SetVisiblePos(MyCanvas, 1);
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


        }
        private void OnPlot(object sender, RoutedEventArgs e)
        {
            plot_time = DateTime.Now;
            MapSegmentObject visible_segment = GetFullMap.VisibleSegment();
            if (visible_segment.GetWaypointsAndLines().Count >= 3)
            {
                // Removes existing route lines
                visible_segment.DelRouteLines(MyCanvas);
                
                GenerateRoute(visible_segment);
            }
        }
        private void OnOptimise(object sender, RoutedEventArgs e)
        {
            MapSegmentObject segment = GetFullMap.VisibleSegment();
            // Fix this to allow for more than 2 user placed waypoints
            if (segment.GetWaypointsAndLines().Count <= 3 && segment.GetWaypointsAndLines().Count > 0)
            {
                LineObject line = (LineObject)segment.GetWaypointsAndLines()[1];
                ShortestRouteObject short_route = new ShortestRouteObject(line.LinePos, node_seperation);
                RemoveWaypoint(2);

                for (int f = 0; f <= short_route.GetRouteCoords.Count - 1; f++)
                {
                    List<double> node = short_route.GetRouteCoords[f];
                    double[] coords = { node[0], node[1] };
                    PlaceWaypoint(coords);
                }
            }
        }
        private void OnReset(object sender, RoutedEventArgs e)
        {
            Reset();
        }
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
            PlaceWaypoint(coords);
        }
        private void RightMouseIsUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(MyCanvas);
            for (int i = 0; i < GetFullMap.VisibleSegment().GetWaypointsAndLines().Count(); i += 2)
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

        #region RoutePlotting
        private void GenerateRoute(MapSegmentObject visible_segment)
        {
            int end_point_index = visible_segment.GetWaypointsAndLines().Count() - 1;
            MainObject firstPoint = visible_segment.GetWaypointsAndLines()[0];

            visible_segment.GetShip.GetTop = firstPoint.GetTop + WAYPOINT_RADIUS;
            visible_segment.GetShip.GetLeft = firstPoint.GetLeft + WAYPOINT_RADIUS;

            for (int i = 2; i <= end_point_index; i += 2)
            {
                MainObject nextPoint = visible_segment.GetWaypointsAndLines()[i];
                ((Waypoint)nextPoint).GenerateMaxTackCone(visible_segment.GetShip.GetLeft, visible_segment.GetShip.GetTop);
                visible_segment.GetShip.GenerateWindConeAngles(windArrow.GetRotation);
                CalcNextRouteLine(nextPoint, visible_segment);
            }

            double route_length = TotalRouteLineLength(visible_segment.GetWaypointsAndLines().Count() - 2);

            routeDistanceIndicator.SetMessage = $" - Route Distance = {Math.Round(route_length, 2)}Nm";
            string time = TimeConversion(route_length);
            routeTimeIndicator.SetMessage = $" - Route Time = {time}";
        }
        private string TimeConversion(double routeLength)
        {
            string time = "";
            double route_time_hrs = routeLength / max_speed;
            time += Math.Floor(route_time_hrs).ToString() + "hrs ";

            double mins = (route_time_hrs - Math.Floor(route_time_hrs)) * 60;

            time += mins < 10 ? "0" : "";
            time += Math.Round(mins) + "mins";

            return time;
        }
        private void CalcNextRouteLine(MainObject next_point, MapSegmentObject visible_segment)
        {
            int next_point_index = visible_segment.GetWaypointsAndLines().IndexOf(next_point);
            double[] shipPos = { visible_segment.GetShip.GetLeft, visible_segment.GetShip.GetTop };
            double[] nextLoc = { 0, 0 };
            double active_wind_cone = visible_segment.GetShip.GetWindConeAngles(1);
            double[] tack_cone = ((Waypoint)next_point).GetMaxTackCone;
            double angle_to_waypoint = (180 / Math.PI) * Math.Atan2(-visible_segment.GetShip.GetTop + next_point.GetTop + WAYPOINT_RADIUS, -visible_segment.GetShip.GetLeft + next_point.GetLeft + WAYPOINT_RADIUS);
            angle_to_waypoint = (angle_to_waypoint + 360) % 360;

            // If the ship can sail towards the waypoint then it will
            if (visible_segment.GetShip.CanSailTowards(angle_to_waypoint))
            {
                nextLoc[0] += next_point.GetLeft + WAYPOINT_RADIUS;
                nextLoc[1] += next_point.GetTop + WAYPOINT_RADIUS;
                double[] linePos = { shipPos[0], shipPos[1], nextLoc[0], nextLoc[1] };
                PlaceRouteLine(linePos, next_point_index - 1);
                visible_segment.GetShip.GetLeft = nextLoc[0];
                visible_segment.GetShip.GetTop = nextLoc[1];
            }
            else
            {
                double[] waypointPos = { next_point.GetLeft + WAYPOINT_RADIUS, next_point.GetTop + WAYPOINT_RADIUS };
                double inactive_wind_cone = visible_segment.AngleAddition(visible_segment.GetShip.GetWindConeAngles(2), 180);

                if (Hypotenuse(shipPos[0] - waypointPos[0], shipPos[1] - waypointPos[1]) > 1)
                {
                    // Finds a potential end point of the next route line - this is where the edge of the wind cone intersects the edge of the tacking cone
                    nextLoc = CalcLineIntersection(tack_cone[(int)tack_cone[2]], active_wind_cone, shipPos, waypointPos);
                    if (Hypotenuse(nextLoc[0] - shipPos[0], nextLoc[1] - shipPos[1]) * visible_segment.GetScalar / max_speed < 0.1666) // if route line is less than 10 mins 
                    {
                        // Finds a potential end point of the next route line - this is where the edge of the wind cone intersects the other edge of the wind cone
                        nextLoc = CalcLineIntersection(inactive_wind_cone, active_wind_cone, shipPos, waypointPos);
                    }

                    double[] linePos = { shipPos[0], shipPos[1], nextLoc[0], nextLoc[1] };

                    // Places the route line, then adds the route line length to the total length
                    PlaceRouteLine(linePos, next_point_index - 1);

                    // Moves the ship to the end of the new Route Line
                    visible_segment.GetShip.GetLeft = nextLoc[0];
                    visible_segment.GetShip.GetTop = nextLoc[1];

                    // Allows the program to switch between each edge of the tack cone/wind cone
                    visible_segment.GetShip.ConeSwapSide();
                    ((Waypoint)next_point).ConeSwapSide();

                    // Uses recursion to keep placing Lines until it can go straight to the waypoint
                    CalcNextRouteLine(next_point, visible_segment);
                }
            }
        }

        public double[] CalcLineIntersection(double tack_cone_angle, double wind_cone_angle, double[] ship_pos, double[] waypoint_pos)
        {
            // M = tan(theta)
            double tack_cone_gradient = Math.Tan(tack_cone_angle * (Math.PI / 180));
            double wind_cone_gradient = Math.Tan(wind_cone_angle * (Math.PI / 180));

            // C = Gradient * -Xcoord + Ycoord
            double tack_cone_Yintercept = tack_cone_gradient * -waypoint_pos[0] + waypoint_pos[1];
            double wind_cone_Yintercept = wind_cone_gradient * -ship_pos[0] + ship_pos[1];

            // X intersection = Cone Y-intercept - Wind Y-Intercept / Wind Gradient - Cone Gradient
            // Y intersection = Wind Gradient * X intersection + Wind Y-intercept
            double[] final_coords = { 0, 0 };
            final_coords[0] += (tack_cone_Yintercept - wind_cone_Yintercept) / (wind_cone_gradient - tack_cone_gradient);
            final_coords[1] += wind_cone_gradient * final_coords[0] + wind_cone_Yintercept;

            return final_coords;
        }
        private void PlaceRouteLine(double[] line_pos, int waypoint_line_index)
        {
            // adds a new Route Line between the ship and the next location, new line is stored in a List, the list of route lines is a property of each waypoint line
            ((LineObject)GetFullMap.VisibleSegment().GetWaypointsAndLines()[waypoint_line_index]).AddRouteLine(new LineObject(MyCanvas, line_pos, Brushes.Red));
        }
        #endregion

        #region Waypoints
        public void RemoveWaypoint(int index)
        {
            //Find a better way of doing this 
            MapSegmentObject segment = GetFullMap.VisibleSegment();
            segment.DelWaypointOrLine(index, MyCanvas);

            int Count = segment.GetWaypointsAndLines().Count();

            if (index != 0 && Count != index)
            {
                // If its not the first or last waypoint then delete the line either side of the waypoint and place a line between the two waypoints either side
                segment.DelWaypointOrLine(index - 1, MyCanvas);
                segment.DelWaypointOrLine(index - 1, MyCanvas);
                PlaceWaypointLine(index - 1, segment.GetWaypointsAndLines()[index - 1], segment.GetWaypointsAndLines()[index - 2]);
            }
            else if (Count == 0)
            {
                return;
            }
            else if (index == 0)
            {
                // If its the first waypoint then only delete the line after
                segment.DelWaypointOrLine(index, MyCanvas);
            }
            else if (Count == index)
            {
                // If its the lastt waypoint then only delete the line before
                segment.DelWaypointOrLine(index - 1, MyCanvas);
            }
            else
            {
                throw new Exception("Waypoint deletion problem");
            }

        }
        public static double Hypotenuse(double num1, double num2)
        {
            return Math.Sqrt(num1 * num1 + num2 * num2);
        }


        public void PlaceWaypoint(double[] coords)
        {
            if (PixelIsLand((int)Math.Round(coords[1]), (int)Math.Round(coords[0])) == false)
            {
                Waypoint new_waypoint = new Waypoint(MyCanvas, coords[0] - WAYPOINT_RADIUS, coords[1] - WAYPOINT_RADIUS);
                List<MainObject> waypoints = GetFullMap.VisibleSegment().GetWaypointsAndLines();
                if (waypoints.Count > 0)
                {
                    MainObject old_waypoint = waypoints[waypoints.Count - 1];
                    PlaceWaypointLine(GetFullMap.VisibleSegment().GetWaypointsAndLines().Count(), old_waypoint, new_waypoint);
                }
                GetFullMap.VisibleSegment().AddWaypointOrLine(GetFullMap.VisibleSegment().GetWaypointsAndLines().Count(), new_waypoint);
            }
        }
        private void PlaceWaypointLine(int index, MainObject object1, MainObject object2)
        {
            double[] LinePos = { object1.GetLeft + WAYPOINT_RADIUS, object1.GetTop + WAYPOINT_RADIUS, object2.GetLeft + WAYPOINT_RADIUS, object2.GetTop + WAYPOINT_RADIUS };

            GetFullMap.VisibleSegment().AddWaypointOrLine(index, new LineObject(MyCanvas, LinePos, Brushes.Black));
        }
        #endregion

        #region LandDetection
        public bool LineIntersectsLand(double[] line_pos)
        {
            double[] lineData = WhereLineIntersectsLand(line_pos);
            if (lineData[2] > 10)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static double[] CalculateStep(double[] line_pos)
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

            // At Grad_to_node = 0, step = 2 | As Grad_to_node tends towards infinity, step = 0.01
            double step = 0.01 + Math.Pow(2, -Math.Abs(grad_to_node));

            if (X_dist_to_node > 0)
            {
                step *= -1;
            }

            return new double[] { step, grad_to_node, Y_intercept };
        }
        public double[] WhereLineIntersectsLand(double[] line_pos)
        {
            // 0 = Step, 1 = Gradient to node, 2 = Y-intercept
            double[] stepData = CalculateStep(line_pos);

            int LandPixelCount = 0;
            double Y;
            double[] land_data = new double[3];
            for (double X = line_pos[0]; X > line_pos[2] + 1 || X < line_pos[2] - 1; X += stepData[0])
            {
                // Y = mX + c
                Y = stepData[1] * X + stepData[2];
                if (PixelIsLand((int)Y, (int)X))
                {
                    LandPixelCount++;
                    // Defines how many pixels of "land" can be between two nodes before they can't see eachother
                    if (LandPixelCount == 1)
                    {
                        // Returns the position of the first point where the line intersects the land
                        land_data[0] = X;
                        land_data[1] = Y;
                    }
                }

            }
            if (land_data[0] == 0 && land_data[1] == 0)
            {
                land_data[0] = line_pos[2];
                land_data[1] = line_pos[3];
            }
            land_data[2] = LandPixelCount;
            return land_data;
        }
        public bool PixelIsLand(int pixel_row, int pixel_col)
        {
            MapSegmentObject Segment = GetFullMap.VisibleSegment();
            double[] segment_dimentions = { Segment.GetHeight, Segment.GetWidth };
            Color colour = GetPixelColor(Segment.GetRectangle, pixel_row, pixel_col, segment_dimentions);
            if ((colour.R <= 225 && colour.R >= 185 && colour.G <= 180 && colour.G >= 145 && colour.B <= 255 && colour.B >= 235))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public Color GetPixelColor(Visual visual, int row, int col, double[] segmentDimentions)
        {
            GC.WaitForPendingFinalizers();

            Point Dpi = GetScreenDPI(visual);

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
            int iStride = 4; // = 4 * bmpOut.Width (for 32 bit graphics with 4 bytes per pixel - 4 * 8 bits per byte = 32)
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

