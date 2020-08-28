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
        public Dictionary<string, string> Currently { get; set; }

        public double wind_speed;
        public double wind_bearing;

        public void setWindData()
        {
            wind_speed = Convert.ToDouble(Currently["windSpeed"]);
            wind_bearing = Convert.ToDouble(Currently["windBearing"]);
        }

    }
}
