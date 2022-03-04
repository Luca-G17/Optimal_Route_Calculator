using System;
using System.Linq;
using System.Windows.Media;

namespace Optimal_Route_Calculator
{
    class TackingRoutePlotter
    {
        const int WAYPOINT_RADIUS = 25;
        public double RouteDistance { get; set; }
        public string RouteTime { get; set; }
        public TackingRoutePlotter(MapSegmentObject visible_segement, MainWindow mainWindow, double max_speed)
        {
            GenerateRoute(visible_segement, mainWindow, max_speed);
        }
        private void GenerateRoute(MapSegmentObject visible_segment, MainWindow mainWindow, double max_speed)
        {
            // Get the first waypoint in the list
            int end_point_index = visible_segment.GetWaypointsAndLines().Count() - 1;
            MainObject firstPoint = visible_segment.GetWaypointsAndLines()[0];

            // Get the starting coordinates of the ship, the first waypoint location + waypoint radius
            // Placing it at the centre of the waypoint
            visible_segment.GetShip.GetTop = firstPoint.GetTop + WAYPOINT_RADIUS;
            visible_segment.GetShip.GetLeft = firstPoint.GetLeft + WAYPOINT_RADIUS;

            // Only selects waypoints not lines
            for (int i = 2; i <= end_point_index; i += 2)
            {
                // Get the next point in the list
                MainObject nextPoint = visible_segment.GetWaypointsAndLines()[i];

                // Generate the tacking cone based off the bearing from the ship to the waypoint
                ((Waypoint)nextPoint).GenerateMaxTackCone(visible_segment.GetShip.GetLeft, visible_segment.GetShip.GetTop);

                // Generate the ships wind cone bassed off the wind direction
                visible_segment.GetShip.GenerateWindConeAngles(mainWindow.windArrow.Rotation);

                CalcNextRouteLine(nextPoint, visible_segment, mainWindow, max_speed);
            }

            RouteDistance = TotalRouteLineLength(visible_segment.GetWaypointsAndLines().Count() - 2, mainWindow);

            CalculateRouteTime(max_speed);
        }
        private void CalculateRouteTime(double maxSpeed)
        {
            string time = "";

            // Time = Distance / Speed
            double route_time_hrs = RouteDistance / maxSpeed;

            // Seperate hours
            time += Math.Floor(route_time_hrs).ToString() + "hrs ";

            // Seperate Minutes
            double mins = (route_time_hrs - Math.Floor(route_time_hrs)) * 60;

            // Add the placeholder zero if mins is less than 10
            time += mins < 10 ? "0" : "";
            time += Math.Round(mins) + "mins";

            RouteTime = time;
        }
        private void CalcNextRouteLine(MainObject next_point, MapSegmentObject visible_segment, MainWindow mainWindow, double max_speed)
        {
            // Get the index of the next point in the list
            int next_point_index = visible_segment.GetWaypointsAndLines().IndexOf(next_point);

            // Get the ships position
            double[] shipPos = { visible_segment.GetShip.GetLeft, visible_segment.GetShip.GetTop };

            // Define a new array for the end of the next route line - This will become the boats new position
            double[] nextLoc = { 0, 0 };

            // Get the edge of the wind cone that the boat is currently travelling along
            double active_wind_cone = visible_segment.GetShip.GetWindConeAngles(1);
            double[] tack_cone = ((Waypoint)next_point).GetMaxTackCone;

            // Get the bearing to the waypoint using Atan2 which converts cartesian coords into a polar angle
            double angle_to_waypoint = (180 / Math.PI) * Math.Atan2(-visible_segment.GetShip.GetTop + next_point.GetTop + WAYPOINT_RADIUS, -visible_segment.GetShip.GetLeft + next_point.GetLeft + WAYPOINT_RADIUS);

            // Convert polar angle into a bearing
            angle_to_waypoint = (angle_to_waypoint + 360) % 360;

            // If the ship can sail towards the waypoint then it will
            if (visible_segment.GetShip.CanSailTowards(angle_to_waypoint))
            {
                // Next location is the next waypoint + waypoint radius to set it in the middle of the waypoint
                nextLoc[0] += next_point.GetLeft + WAYPOINT_RADIUS;
                nextLoc[1] += next_point.GetTop + WAYPOINT_RADIUS;

                // Creates a new route line with the next waypoints coordinates as the end location
                double[] linePos = { shipPos[0], shipPos[1], nextLoc[0], nextLoc[1] };
                PlaceRouteLine(linePos, next_point_index - 1, mainWindow);

                // Set the ships location as the waypoints location
                visible_segment.GetShip.GetLeft = nextLoc[0];
                visible_segment.GetShip.GetTop = nextLoc[1];
            }
            else
            {
                // Gets the next waypoints coords
                double[] waypointPos = { next_point.GetLeft + WAYPOINT_RADIUS, next_point.GetTop + WAYPOINT_RADIUS };

                // Get the wind cone angle that the boat is not travelling along
                double inactive_wind_cone = visible_segment.AngleAddition(visible_segment.GetShip.GetWindConeAngles(2), 180);

                if (MainWindow.Hypotenuse(shipPos[0] - waypointPos[0], shipPos[1] - waypointPos[1]) > 1)
                {
                    // Finds a potential end point of the next route line - this is where the edge of the wind cone intersects the edge of the tacking cone
                    nextLoc = CalcLineIntersection(tack_cone[(int)tack_cone[2]], active_wind_cone, shipPos, waypointPos);
                    if (MainWindow.Hypotenuse(nextLoc[0] - shipPos[0], nextLoc[1] - shipPos[1]) * visible_segment.GetScalar / max_speed < 0.1666) // if route line is less than 10 mins 
                    {
                        // Finds a potential end point of the next route line - this is where the edge of the wind cone intersects the other edge of the wind cone
                        nextLoc = CalcLineIntersection(inactive_wind_cone, active_wind_cone, shipPos, waypointPos);
                    }

                    double[] linePos = { shipPos[0], shipPos[1], nextLoc[0], nextLoc[1] };

                    // Places the route line, then adds the route line length to the total length
                    PlaceRouteLine(linePos, next_point_index - 1, mainWindow);

                    // Moves the ship to the end of the new Route Line
                    visible_segment.GetShip.GetLeft = nextLoc[0];
                    visible_segment.GetShip.GetTop = nextLoc[1];

                    // Allows the program to switch between each edge of the tack cone/wind cone
                    visible_segment.GetShip.ConeSwapSide();
                    ((Waypoint)next_point).ConeSwapSide();

                    // Uses recursion to keep placing Lines until it can go straight to the waypoint
                    CalcNextRouteLine(next_point, visible_segment, mainWindow, max_speed);
                }
            }
        }
        public double[] CalcLineIntersection(double tack_cone_angle, double wind_cone_angle, double[] ship_pos, double[] waypoint_pos)
        {
            // M = tan(theta (In radians))
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
        private double TotalRouteLineLength(int final_index, MainWindow mainWindow)
        {
            FullMapObject fullMap = mainWindow.GetFullMap;
            double route_line_length = 0;
            for (int i = final_index; i > 0; i -= 2)
            {
                // Sums the route lines length
                route_line_length += ((LineObject)fullMap.VisibleSegment().GetWaypointsAndLines()[i]).RouteLineLength();
            }
            // Converts from DIPs to Nm
            route_line_length *= fullMap.VisibleSegment().GetScalar;
            return route_line_length;
        }
        private void PlaceRouteLine(double[] line_pos, int waypoint_line_index, MainWindow mainWindow)
        {
            // adds a new Route Line between the ship and the next location, new line is stored in a List, the list of route lines is a property of each waypoint line
            ((LineObject)mainWindow.GetFullMap.VisibleSegment().
                GetWaypointsAndLines()[waypoint_line_index]).
                AddRouteLine(new LineObject(mainWindow.MyCanvas, line_pos, Brushes.Red));
        }
    }
}
