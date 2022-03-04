using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;

namespace Optimal_Route_Calculator
{
    class TidalFlowModel
    {
        // Low tide to High tide in 6hrs 12.5mins
        // 22,350 Seconds
        // Frequency = 1 / Period
        const double Freq = 0.161030596;
        public PlotModel tideModel { get; private set; }
        public double[] max_min = new double[2] { 0, 0 };
        private string high_water_str { get; set; } = "01/01/0001 00:00:00";
        private double high_water { get; set; }
        public TidalFlowModel()
        {
            // Creates the OxyPlot model and sets the axis
            tideModel = new PlotModel { Title = "Tidal Flow Model", TitleFontSize = 10 };
            tideModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, AbsoluteMaximum = 5, MajorStep = 2, AbsoluteMinimum = 0, Title = "Velocity (Kts)" });
            tideModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, AbsoluteMaximum = 10, MajorStep = 1, Title = "Time (Hrs)" });
            tideModel.Series.Add(GetFunction());
        }
        
        public void SetMaxMin(double maximum, double minimum, string highWater)
        {
            // Re-Calculates the graph with new min, max values and resest axis
            high_water_str = highWater;
            CalculateTimeOffset();
            max_min[0] = maximum;
            max_min[1] = minimum;
            tideModel.Axes[0].AbsoluteMaximum = maximum + 3;
            tideModel.Series.Add(GetFunction());
        }
        
        private FunctionSeries GetFunction()
        {
            // Places a new point at specified intervals forming the graph
            FunctionSeries fs = new FunctionSeries();
            for (double i = 0; i < 10; i += 0.1)
            {
                DataPoint dp = new DataPoint(i, GetValue(i));
                fs.Points.Add(dp);
            }
            return fs;
        }
        private void CalculateTimeOffset()
        {
            DateTime high_water_time = Convert.ToDateTime(high_water_str);
            TimeSpan time_diff = high_water_time - DateTime.Now;
            high_water = time_diff.TotalHours;
        }
        private double GetValue(double i)
        {
            // Y = Acos2(X * pi * Frequency) + min
            double Amplitude = max_min[0] - max_min[1];
            return Amplitude * Math.Pow(Math.Cos((i - high_water) * Math.PI * Freq), 2) + max_min[1];
        }
    }
}
