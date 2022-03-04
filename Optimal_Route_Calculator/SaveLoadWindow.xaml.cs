using System;
using System.IO;
using System.Windows;


namespace Optimal_Route_Calculator
{
    /// <summary>
    /// Interaction logic for SaveLoadWindow.xaml
    /// </summary>
    public partial class SaveLoadWindow : Window
    {
        public SaveLoadWindow()
        {
            InitializeComponent();
            string directoryPath = FileHandler.PathToAppDirectory("SaveFiles", 1);
            string[] existingSaveFiles = Directory.GetFiles(directoryPath);
            foreach (string SaveFile in existingSaveFiles)
            {
                lstBoxExistingFiles.Items.Add(Path.GetFileName(SaveFile));
            }
        }
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            string path = "SaveFiles\\" + txtBoxFileName.Text;
            // TODO: This is yucky remove when you figure out how to do it better
            
            path = FileHandler.PathToAppDirectory(path, 0);

            if (File.Exists(path))
            {
                MainWindow main_window = (MainWindow)Application.Current.MainWindow;
                main_window.Reset();
               
                using (StreamReader file = new StreamReader(path))
                {
                    int line_num = 0;
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        line_num++;
                        // Splits the line into a (x, y) position of the waypoint
                        int comma_index = line.IndexOf(',');
                        double[] coords = { Convert.ToDouble(line.Substring(0, comma_index)) + 25, Convert.ToDouble(line.Substring(comma_index + 1, line.Length - comma_index - 1)) + 25 };
                        main_window.PlaceWaypoint(coords);
                    }
                }
                StatusMsg.Text = "Waypoints Loaded";
            }
            else
            {
                StatusMsg.Text = "File doesn't exist, try a different name";
            }
        }
        private void OnSave(object sender, RoutedEventArgs e)
        {
            MainWindow main_window = (MainWindow)Application.Current.MainWindow;
            string path = "SaveFiles\\" + txtBoxFileName.Text;
            path = FileHandler.PathToAppDirectory(path, 0);

            if (!File.Exists(path))
            {
                using (StreamWriter file = new StreamWriter(path))
                {
                    for (int i = 0; i < main_window.GetFullMap.VisibleSegment().GetWaypointsAndLines().Count; i += 2)
                    {
                        MainObject waypoint = main_window.GetFullMap.VisibleSegment().GetWaypointsAndLines()[i];
                        file.WriteLine($"{waypoint.GetLeft},{waypoint.GetTop}");
                    }
                }
                StatusMsg.Text = "Waypoints Saved";
            }
            else
            {
                StatusMsg.Text = "File already exists, try a new name";
            }
        }
    }
}
