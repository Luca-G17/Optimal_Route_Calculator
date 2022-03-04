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

namespace Optimal_Route_Calculator
{
    /// <summary>
    /// Interaction logic for NewTidalPointWindow.xaml
    /// </summary>
    public partial class NewTidalPointWindow : Window
    {
        private string high_water { get; set; }
        Point tidal_point_loc;
        private bool point_placed = false;
        public NewTidalPointWindow(Point mouse_pos)
        {
            InitializeComponent();
            APIManager.ResetClient();
            TideAPICall();
            tidal_point_loc = mouse_pos;
        }
        private void btnNewTidePointClick(object sender, RoutedEventArgs e)
        {
            if (InputValidator.TextInputCheck(txtBoxMaxFlow, 0) && (InputValidator.TextInputCheck(txtBoxMinFlow, 0)) && 
                InputValidator.TextInputCheck(txtBoxTideBearing, 0) && !point_placed)
            {
                string min_str = txtBoxMinFlow.Text;
                string max_str = txtBoxMaxFlow.Text;
                string bearing_str = txtBoxTideBearing.Text;
                // Check all inputs are numbers 
                if (InputValidator.NumValidate(min_str) && InputValidator.NumValidate(max_str) && InputValidator.NumValidate(bearing_str))
                {

                    double min = Convert.ToDouble(min_str);
                    double max = Convert.ToDouble(max_str);
                    double bearing = Convert.ToDouble(bearing_str);

                    // Create the tideFlowModel and draw the graph
                    TidalFlowModel tidalFlowModel = new TidalFlowModel();
                    tidalFlowModel.SetMaxMin(max, min, high_water);
                    TidalFlowPlotModel.Model = tidalFlowModel.tideModel;

                    
                    MainWindow main_window = (MainWindow)Application.Current.MainWindow;
                    main_window.GetFullMap.VisibleSegment().AddTidePoint(tidal_point_loc, main_window.MyCanvas, bearing, max, min);
                    point_placed = true;
                }
            }
        }
        private async void TideAPICall()
        {
            MainWindow main_window = (MainWindow)Application.Current.MainWindow;
            string existing_high_water = main_window.GetFullMap.VisibleSegment().high_water;
            if(existing_high_water == "")
            {
                // Calls the Tide Height API with the station ID: 0065 - Portsmouth
                var tide_data = await TideAPIProcessor.LoadTideData("0065");
                high_water = tide_data.HighWater;
                main_window.GetFullMap.VisibleSegment().high_water = high_water;
            }
            else
            {
                high_water = existing_high_water;
            }
        }

        private void btnCloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
