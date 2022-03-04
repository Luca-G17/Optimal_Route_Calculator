using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;

namespace Optimal_Route_Calculator
{
    class TidalFlowModel
    {
        // Low tide to High tide in 6hrs 12.5mins
        // 22,350 Seconds
        // Frequency = 1 / Period
        const double FREQ = 0.161030596;
        public PlotModel TideModel { get; private set; }
        public double[] max_min = new double[2] { 0, 0 };
        private string HighWaterStr { get; set; } = "01/01/0001 00:00:00";
        private double HighWaterNum { get; set; }
        public TidalFlowModel()
        {
            // Creates the OxyPlot model and sets the axis
            TideModel = new PlotModel { Title = "Tidal Flow Model", TitleFontSize = 10 };
            TideModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, AbsoluteMaximum = 5, MajorStep = 2, AbsoluteMinimum = 0, Title = "Velocity (Kts)" });
            TideModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, AbsoluteMaximum = 10, MajorStep = 1, Title = "Time (Hrs)" });
            TideModel.Series.Add(GetFunction());
        }

        public void SetMaxMin(double maximum, double minimum, string highWater)
        {
            // Re-Calculates the graph with new min, max values and resest axis
            HighWaterStr = highWater;
            CalculateTimeOffset();
            max_min[0] = maximum;
            max_min[1] = minimum;
            TideModel.Axes[0].AbsoluteMaximum = maximum + 3;
            TideModel.Series.Add(GetFunction());
        }

        private FunctionSeries GetFunction()
        {
            // Places a new point at specified intervals forming the graph
            FunctionSeries fs = new FunctionSeries();
            for (double i = 0; i < 10; i += 0.1)
            {
                DataPoint dp = new DataPoint(i, GetFlow(i));
                fs.Points.Add(dp);
            }
            return fs;
        }
        private void CalculateTimeOffset()
        {
            DateTime high_water_time = Convert.ToDateTime(HighWaterStr);
            TimeSpan time_diff = high_water_time - DateTime.Now;
            HighWaterNum = time_diff.TotalHours;
        }
        private double GetFlow(double i)
        {
            // Y = Acos2(X * pi * Frequency) + min
            double Amplitude = max_min[0] - max_min[1];
            return Amplitude * Math.Pow(Math.Cos((i - HighWaterNum) * Math.PI * FREQ), 2) + max_min[1];
        }
    }
}
