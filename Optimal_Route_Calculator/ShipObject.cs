namespace Optimal_Route_Calculator
{
    class ShipObject : MainObject, IShipsandWaypoints
    {
        private readonly double[] windConeAngles = { 0, 0, 0 };
        private double boat_to_wind = 40;
        public ShipObject()
        {

        }
        public double GetMaxSpeed { get; set; } = 6;
        public double GetWindConeAngles(int getCode)
        {
            // getCode 1 == Get active cone angle
            // getCode 2 == Get inactive cone angle
            if (getCode == 1)
            {
                return windConeAngles[(int)windConeAngles[2]];
            }
            else
            {
                if (windConeAngles[2] == 0)
                {
                    return windConeAngles[1];
                }
                else
                {
                    return windConeAngles[0];
                }
            }
        }
        public void GenerateWindConeAngles(double wind_angle)
        {
            // Gets the wind cone angles by adding 180 to the wind bearing and then adding and substracting the angle to wind
            // This value can be set by the user but the default is 40 degrees
            wind_angle = AngleAddition(wind_angle, 180);
            windConeAngles[0] = AngleAddition(wind_angle, boat_to_wind);
            windConeAngles[1] = AngleAddition(wind_angle, -boat_to_wind);
            windConeAngles[2] = 0;
        }

        public bool CanSailTowards(double waypoint_angle)
        {
            // Gets the difference in angle between the angle to waypoint and the wind cone angles
            double angle_diff = (AngleAddition(windConeAngles[1], boat_to_wind) - waypoint_angle + 180 + 360) % 360 - 180;

            // If the waypoint bearing is inside the wind cone the boat cannot sail towards the waypoint
            if (angle_diff <= boat_to_wind && angle_diff >= -boat_to_wind)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void ConeSwapSide()
        {
            // Change the active wind cone
            if (windConeAngles[2] == 0)
            {
                windConeAngles[2] += 1;
            }
            else
            {
                windConeAngles[2] -= 1;
            }
        }
        public double GetBoatToWind { set { boat_to_wind = value; } }

    }
}
