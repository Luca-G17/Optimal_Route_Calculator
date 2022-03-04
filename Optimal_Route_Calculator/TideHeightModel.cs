using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Optimal_Route_Calculator
{
    class TideHeightModel
    {
        public JArray JObjects { get; set; }
        public string HighWater { get; private set; }
        public TideHeightModel(string json)
        {
            JObjects = JArray.Parse(json);
            GetNextHighWaterTime();
        }

        private void GetNextHighWaterTime()
        {
            // Searches through JSON objects and get the nearest High water time
            foreach (JObject tide_event in JObjects)
            {   
                if (tide_event.GetValue("EventType").ToString() == "HighWater")
                {
                    string tmp_high_water = tide_event.GetValue("DateTime").ToString();
                    if (Convert.ToDateTime(tmp_high_water) > DateTime.Now)
                    {
                        HighWater = tmp_high_water;
                        return;
                    }
                }
            }
        }
    }
}
