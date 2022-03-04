using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OxyPlot;

namespace Optimal_Route_Calculator
{
    /// <summary>
    /// Interaction logic for NewTidalPointWindow.xaml
    /// </summary>
    public partial class NewTidalPointWindow : Window
    {
        private string HighWater { get; set; }
        Point tidal_point_loc;
        private bool point_placed = false;
        public NewTidalPointWindow(Point mouse_pos)
        {
            InitializeComponent();
            APIManager.ResetClient();
            TideAPICall();
            tidal_point_loc = mouse_pos;
        }
        private bool LocalInputValidation()
        {
            if (InputValidator.TextInputCheck(txtBoxMaxFlow, -1) && (InputValidator.TextInputCheck(txtBoxMinFlow, -1)) &&
                InputValidator.TextInputCheck(txtBoxTideBearing, -1) && !point_placed)
            {
                string min_str = txtBoxMinFlow.Text;
                string max_str = txtBoxMaxFlow.Text;
                string bearing_str = txtBoxTideBearing.Text;
                // Check all inputs are numbers 
                if (InputValidator.NumValidate(min_str) && InputValidator.NumValidate(max_str) && InputValidator.NumValidate(bearing_str))
                {
                    return true;
                }
            }
            return false;
        }
        private void BtnNewTidePointClick(object sender, RoutedEventArgs e)
        {
            if (LocalInputValidation())
            {

                double min = Convert.ToDouble(txtBoxMinFlow.Text);
                double max = Convert.ToDouble(txtBoxMaxFlow.Text);
                double bearing = Convert.ToDouble(txtBoxTideBearing.Text);

                if (min >= 0 && max >= 0 && bearing <= 360 && bearing >= 0 && max > min)
                {
                    InputStatusMessageTextBlock.Foreground = Brushes.Black;
                    InputStatusMessageTextBlock.Text = "New Tide Point Created";

                    // Create the tideFlowModel and draw the graph
                    TidalFlowModel tidalFlowModel = new TidalFlowModel();
                    tidalFlowModel.SetMaxMin(max, min, HighWater);
                    TidalFlowPlotModel.Model = tidalFlowModel.TideModel;

                    MainWindow main_window = (MainWindow)Application.Current.MainWindow;
                    main_window.GetFullMap.VisibleSegment().AddTidePoint(tidal_point_loc, main_window.MyCanvas, bearing, max, min);
                    point_placed = true;
                    return;
                }
            }
            InputStatusMessageTextBlock.Foreground = Brushes.Red;
            InputStatusMessageTextBlock.Text = "Invalid Input";
        }
        private async void TideAPICall()
        {
            MainWindow main_window = (MainWindow)Application.Current.MainWindow;
            string existing_high_water = main_window.GetFullMap.VisibleSegment().HighWater;

            if (existing_high_water != "")
            {
                HighWater = existing_high_water;   
            }
            else if (MainWindow.IsConnectedToInternet())
            {
                // Calls the Tide Height API with the station ID: 0065 - Portsmouth
                var tide_data = await TideAPIProcessor.LoadTideData("0065");
                HighWater = tide_data.HighWater;
                main_window.GetFullMap.VisibleSegment().HighWater = HighWater;
            }
            else
            {
                main_window.GetFullMap.VisibleSegment().HighWater = DateTime.Now.ToString("T");
            }
        }

        private void BtnCloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
