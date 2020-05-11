using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool.Utils
{

    enum FileFilters
    {
        None,
        Bsp,
        Text,
        BspZipExe,
        GameinfoTxt
    }

    static class FilesUtils
    {
        private const string FilterText = "Text Files (*.txt) | *.txt";
        private const string FilterGameinfoTxt = "Gameinfo (gameinfo.txt)|gameinfo.txt|Text files (*.txt)|*.txt|All files (*.*)|*.*";
        private const string FilterBsp = "BSP Files (*.bsp)| *.bsp";
        private const string FilterBspZipExe = "BspZip (bspzip.exe)| bspzip.exe|Executable files (*.exe)|*.exe";
        private const string FolderSelection = "[Folder Selection]";

        public const string ExtensionBsp = ".bsp";
        public const string ExtensionExe = ".exe";
        public const string ExtensionTxt = ".txt";
        public const string BspZipFile = "bspzip.exe";
        public const string GameinfoFile = "gameinfo.txt";
        private const string searchPattern = "*.*";
        public const char slash = '/';
        public const char antislash = '\\';

        /// <summary>
        /// Read contents of an embedded resource file
        /// </summary>
        public static string ReadResourceFile(string filename)
        {
            var thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var stream = thisAssembly.GetManifestResourceStream(filename))
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Create a file at the given path with the given text
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="text">Content of the file</param>
        public static void WriteAllText(string path, string text)
        {
            try
            {
                System.IO.File.WriteAllText(path, text);
            }
            catch { }
        }


        /// <summary>
        /// Remove any extra '/' in the directory path and '/' becomes '\'
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string directory)
        {
            try
            {
                return System.IO.Path.GetDirectoryName(directory);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Return the list of all files in the given directory and its subdirectories
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>Return the path of all files in the directories. null if error</returns>
        public static List<string> GetFilesList(string directory)
        {
            if (System.IO.Directory.Exists(directory))
            {
                try
                {
                    var files = System.IO.Directory.GetFiles(directory, searchPattern, System.IO.SearchOption.AllDirectories);
                    return new List<string>(files);
                } 
                catch { }
            }
            return null;
        }

        /// <summary>
        /// Return the list of all files matching the list of extensions
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="allowedExtension"></param>
        /// <returns></returns>
        public static List<string> GetFilesList(string directory, string[] allowedExtension)
        {
            List<string> matchingFiles = new List<string>();
            var allFiles = GetFilesList(directory);
            if (allFiles != null)
            {
                foreach (var file in allFiles)
                {
                    string fileExtension = System.IO.Path.GetExtension(file);
                    if (IsExtension(fileExtension, allowedExtension))
                    {
                        matchingFiles.Add(file);
                    }
                }
            }
            return matchingFiles;
        }

        /// <summary>
        /// Check if the file name is the same as an expected one
        /// </summary>
        /// <param name="path"></param>
        /// <param name="expectedFile"></param>
        /// <returns></returns>
        public static bool IsFileName(string path, string expectedFile)
        {
            string file = System.IO.Path.GetFileName(path).ToLower();
            return file.Equals(expectedFile.ToLower());
        }

        /// <summary>
        /// Check if the file extension of the file is matching a list of extensions
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="allowedExtension"></param>
        /// <returns></returns>
        public static bool IsExtension(string path, string allowedExtension)
        {
            string fileExtension = System.IO.Path.GetExtension(path);
            return fileExtension.Equals(allowedExtension);
        }

        /// <summary>
        /// Check if the file extension of the file is matching a list of extensions
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <param name="allowedExtension"></param>
        /// <returns></returns>
        public static bool IsExtension(string path, string[] allowedExtensions)
        {
            string fileExtension = System.IO.Path.GetExtension(path);
            foreach (string ext in allowedExtensions)
            {
                if (fileExtension.Equals(ext))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Return the file filter depending of the given value
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private static string GetFileFilter(FileFilters filter)
        {
            switch (filter)
            {
                case FileFilters.Bsp:
                    return FilterBsp;
                case FileFilters.Text:
                    return FilterText;
                case FileFilters.BspZipExe:
                    return FilterBspZipExe;
                case FileFilters.GameinfoTxt:
                    return FilterGameinfoTxt;
                case FileFilters.None:
                default:
                    break;
            }
            return string.Empty;
        }

        /// <summary>
        /// Open the File Browser Dialog and return the path of the selected file, null if none
        /// </summary>
        /// <param name="filter">Filter for the file selection</param>
        /// <returns></returns>
        public static string OpenFileDialog(FileFilters filter)
        {
            return OpenFileDialog(GetFileFilter(filter));
        }
        
        private static string OpenFileDialog(string filter)
        {
            Microsoft.Win32.FileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter
            };
            bool? result = fileDialog.ShowDialog();
            if (result == true)
                return fileDialog.FileName;
            return null;
        }

        /// <summary>
        /// Open the File Browser Dialog and return the path of the selected folder, null if none
        /// </summary>
        /// <returns>Returns the selected folder</returns>
        public static string OpenFolderDialog()
        {
            Microsoft.Win32.FileDialog folderDialog = new Microsoft.Win32.OpenFileDialog
            {
                CheckFileExists = false,
                FileName = FolderSelection // Default name
            };
            bool? result = folderDialog.ShowDialog();
            if (result == true)
                return System.IO.Path.GetDirectoryName(folderDialog.FileName);

            return null;
        }


    }
}
