using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Optimal_Route_Calculator
{
    class ShipObject : MainObject
    {
        private double[] windConeAngles = { 0, 0, 0 };
        private double boat_to_wind = 40;
        public ShipObject()
        {

        }
    
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
            wind_angle = AngleAddition(wind_angle, 180);
            windConeAngles[0] = AngleAddition(wind_angle, boat_to_wind);
            windConeAngles[1] = AngleAddition(wind_angle, -boat_to_wind);
            windConeAngles[2] = 0;
        }

        public bool CanSailTowards(double waypoint_angle)
        {
            if (!(waypoint_angle > 90 && waypoint_angle < 180))
            {
                waypoint_angle = Math.Abs(waypoint_angle);
                waypoint_angle = AngleAddition(-waypoint_angle, 360);
            }
         
            
            if (waypoint_angle >= windConeAngles[0] || waypoint_angle <= windConeAngles[1])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override void ConeSideSwap()
        {
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
