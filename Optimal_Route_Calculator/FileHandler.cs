using System;
using System.IO;

namespace Optimal_Route_Calculator
{
    abstract class FileHandler
    {
        /// <summary>
        /// Gets the full path of the file specified from the localPath parameter
        /// The file code parameter determines wether the file is a text file or not so it knows whether to check for .txt or not
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="fileCode">If its a text file then this should be 0</param>
        /// <returns></returns>
        public static string PathToAppDirectory(string localPath, int fileCode)
        {
            // Gets the current directory that the program is stored in
            string currentDir = Environment.CurrentDirectory;

            // Gets the stored infomation about that directory
            DirectoryInfo directory = new DirectoryInfo(Path.GetFullPath(Path.Combine(currentDir, @"..\..\" + localPath)));

            // If the localPath needs to lead to a text file and the suggested local path doesnt have a .txt extension then its added
            if (!(currentDir = directory.ToString()).EndsWith(".txt") && fileCode == 0)
            {
                return currentDir += ".txt";
            }
            return currentDir;
        }
    }
}
