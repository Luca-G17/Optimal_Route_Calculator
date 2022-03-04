﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace Optimal_Route_Calculator
{
    /// <summary>
    /// Interaction logic for SaveLoadWindow.xaml
    /// </summary>
    public partial class SaveLoadWindow : Window
    {
        private readonly DispatcherTimer saveLoadTimer = new DispatcherTimer();
        private readonly string saveFilesPath;
        private readonly string[] invalid_fileNames = new string[] { "CON", "PRN", "AUX", "NUL", "COM", "LPT",
                                                            "<", "?", ">", "/", "|", "*" };
        public SaveLoadWindow()
        {
            InitializeComponent();

            saveLoadTimer.Tick += mainLoop;
            saveLoadTimer.Interval = TimeSpan.FromMilliseconds(20);
            saveLoadTimer.Start();

            saveFilesPath = FileHandler.PathToAppDirectory("SaveFiles", 1);

            UpdateListBox();
        }
        private void mainLoop(object sender, EventArgs e)
        {

        }
        private void UpdateListBox()
        {
            string saveFileName;
            string[] existingSaveFiles = Directory.GetFiles(saveFilesPath);
            foreach (string SaveFilePath in existingSaveFiles)
            {
                saveFileName = Path.GetFileName(SaveFilePath);
                if (!lstBoxExistingFiles.Items.Contains(saveFileName))
                {
                    lstBoxExistingFiles.Items.Add(saveFileName);
                }
            }
        }
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            string path;

            object saveFile = lstBoxExistingFiles.SelectedItem;

            if (saveFile != null)
            {
                path = "SaveFiles\\" + saveFile.ToString();
                path = FileHandler.PathToAppDirectory(path, 0);


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
                StatusMsg.Text = "No File Selected";
            }

        }
        private bool ValidFileName(string path)
        {
            foreach (string fileName in invalid_fileNames)
            {
                if (path.Contains(fileName))
                {
                    return false;
                }
            }

            if (File.Exists(path))
            {
                return false;
            }

            return true;
        }
        private void OnSave(object sender, RoutedEventArgs e)
        {
            MainWindow main_window = (MainWindow)Application.Current.MainWindow;
            string path = "SaveFiles\\" + txtBoxFileName.Text;
            path = FileHandler.PathToAppDirectory(path, 0);

            if (ValidFileName(path))
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
                UpdateListBox();
            }
            else
            {
                StatusMsg.Text = "File already exists or filename is invalid, try a new name";
            }
        }
    }
}
