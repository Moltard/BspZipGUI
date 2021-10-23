using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool.Utils
{
    internal enum FileFilters
    {
        None,
        Bsp,
        Zip,
        BspZipExe,
        GameinfoTxt
    }

    internal static class FileUtils
    {
        #region Constants

        private const string filterGameinfoTxt = "Gameinfo (gameinfo.txt)|gameinfo.txt|Text files (*.txt)|*.txt|All files (*.*)|*.*";
        private const string filterBsp = "BSP Files (*.bsp)| *.bsp";
        private const string filterZip = "ZIP Files (*.zip)| *.zip";
        private const string filterBspZipExe = "BspZip (bspzip.exe)| bspzip.exe|Executable files (*.exe)|*.exe";


        private const string titleBspInput = "Select a .bsp";
        private const string titleBspZipExe = "Select bspzip.exe file";
        private const string titleGameinfoTxt = "Select gameinfo.txt file";
        private const string titleBspOutput = "Save .bsp as";
        private const string titleZipOutput = "Save .zip as";
 
        private const string titleFolder = "Select a folder";

        private const string titleSaveAs = "Save as";

        private const string folderSelection = "[Folder Selection]";
        private const string searchPattern = "*.*";

        #endregion

        #region Methods - Files / Folders

        /// <summary>
        /// Read contents of an embedded resource file
        /// </summary>
        /// <exception cref="Exception">An exception of any type, if there is an error when reading the stream</exception>
        public static string ReadResourceFile(string filename)
        {
            System.Reflection.Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (System.IO.Stream stream = thisAssembly.GetManifestResourceStream(filename))
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Get the directory name of a given path<br/>
        /// Remove any extra '/' in the path and '/' becomes '\'
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The directory path if successful, null otherwise</returns>
        public static string TryGetDirectoryName(string path)
        {
            try
            {
                return System.IO.Path.GetDirectoryName(path);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Return the list of all files in the given directory and its subdirectories
        /// </summary>
        /// <param name="directory"></param>
        /// <exception cref="Exception">An exception of any type, if there is an error getting the list of files</exception>
        /// <returns>Return the path of all files in the directories, null if the directory doesn't exist</returns>
        public static List<string> GetFilesListRecursive(string directory)
        {
            if (System.IO.Directory.Exists(directory))
            {
                string[] files = System.IO.Directory.GetFiles(directory, searchPattern, System.IO.SearchOption.AllDirectories);
                return new List<string>(files);
            }
            return null;
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
        /// Check if the file extension of the file is matching a specific extension
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="allowedExtension"></param>
        /// <returns></returns>
        public static bool IsExtension(string fileName, string allowedExtension)
        {
            string fileExtension = System.IO.Path.GetExtension(fileName);
            return fileExtension.Equals(allowedExtension);
        }

        /// <summary>
        /// Check if the file extension of the file is matching a list of extensions
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="allowedExtensions"></param>
        /// <returns></returns>
        public static bool IsExtension(string fileName, string[] allowedExtensions)
        {
            string fileExtension = System.IO.Path.GetExtension(fileName);
            foreach (string ext in allowedExtensions)
            {
                if (fileExtension.Equals(ext))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Methods - Dialog File / Folder 

        /// <summary>
        /// Open the File Browser Dialog and return the path of the selected file, null if none
        /// </summary>
        /// <param name="filter">Filter for the file selection</param>
        /// <returns></returns>
        public static string OpenFileDialog(FileFilters filter)
        {
            switch (filter)
            {
                case FileFilters.Bsp:
                    return OpenFileDialog(filterBsp, titleBspInput);
                case FileFilters.BspZipExe:
                    return OpenFileDialog(filterBspZipExe, titleBspZipExe);
                case FileFilters.GameinfoTxt:
                    return OpenFileDialog(filterGameinfoTxt, titleGameinfoTxt);
                case FileFilters.None:
                default:
                    break;
            }
            return OpenFileDialog(string.Empty, string.Empty);
        }
        
        private static string OpenFileDialog(string filter, string title)
        {
            Microsoft.Win32.FileDialog fileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter,
                Title = title
            };
            bool? result = fileDialog.ShowDialog();
            if (result == true)
            {
                return fileDialog.FileName;
            }
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
                Title = titleFolder,
                FileName = folderSelection // Default name
            };
            bool? result = folderDialog.ShowDialog();
            if (result == true)
            {
                return System.IO.Path.GetDirectoryName(folderDialog.FileName);
            }
            return null;
        }

        /// <summary>
        /// Open the Save File Browser Dialog with the given settings
        /// </summary>
        /// <param name="filter">Type of files to filter</param>
        /// <param name="defaultFileName">Default filename</param>
        /// <param name="defaultExt">Default extension</param>
        /// <param name="initialDirectory">Intitial directory</param>
        /// <returns>Return the path of the file to save, null if none</returns>
        public static string SaveFileDialog(FileFilters filter, string defaultFileName, string defaultExt, string initialDirectory = "")
        {
            switch (filter)
            {
                case FileFilters.Bsp:
                    return SaveFileDialog(filterBsp, defaultFileName, defaultExt, titleBspOutput, initialDirectory);
                case FileFilters.Zip:
                    return SaveFileDialog(filterZip, defaultFileName, defaultExt, titleBspOutput, initialDirectory);
                case FileFilters.None:
                default:
                    break;
            }
            return SaveFileDialog(string.Empty, defaultFileName, defaultExt, string.Empty, initialDirectory);
        }

        private static string SaveFileDialog(string filter, string defaultFileName, string defaultExt, string title, string initialDirectory)
        {
            Microsoft.Win32.FileDialog saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                InitialDirectory = initialDirectory,
                Filter = filter,
                FileName = defaultFileName,
                DefaultExt = defaultExt,
                Title = title,
            };

            bool? result = saveDialog.ShowDialog();
            if (result == true)
            {
                return saveDialog.FileName;
            }
            return null;
        }

        #endregion

    }
}
