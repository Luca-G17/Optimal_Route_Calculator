using System;
using System.IO;
using System.Windows;

namespace Optimal_Route_Calculator
{
    abstract class FileHandler
    {
        public static string PathToAppDirectory(string localPath, int fileCode)
        {
            string currentDir = Environment.CurrentDirectory;
            DirectoryInfo directory = new DirectoryInfo(Path.GetFullPath(Path.Combine(currentDir, @"..\..\" + localPath)));
            if (!(currentDir = directory.ToString()).EndsWith(".txt") && fileCode == 0)
            {
                return currentDir += ".txt";
            }
            return currentDir;

        }
    }
}
