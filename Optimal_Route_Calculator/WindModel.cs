using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Design;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Optimal_Route_Calculator
{
    public class WindModel
    {
        private const double conversion_to_knots = 1.943844;
        public Dictionary<string, string> Currently { get; set; }

        public string wind_speed;
        public double wind_bearing;

        public void setWindData()
        {
            double windSpeed = Math.Round(Convert.ToDouble(Currently["windSpeed"]) * conversion_to_knots, 2);
            wind_speed = Convert.ToString(windSpeed);
            wind_bearing = Convert.ToDouble(Currently["windBearing"]);
        }

    }
}
