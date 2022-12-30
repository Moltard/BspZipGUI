using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool.Utils
{
    /// <summary>
    /// Enum to handle File Browser Dialog and Save Browser Dialog
    /// </summary>
    internal enum FileFilters
    {
        None,
        Bsp,
        Zip,
        BspZipExe,
        GameinfoTxt
    }

    /// <summary>
    /// Class containing useful file related functions
    /// </summary>
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
 
        private const string titleDirectory = "Select a directory";
        private const string directorySelection = "[Folder Selection]";
        private const string searchPatternAny = "*.*";

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
        /// Tries to get the directory name of a given path<br/>
        /// Remove any extra '/' in the path and '/' becomes '\'
        /// </summary>
        /// <param name="path">The path of a file or directory</param>
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
        /// Get the list of all files in a given directory and its subdirectories
        /// </summary>
        /// <param name="directory">The path of the directory</param>
        /// <exception cref="Exception">An exception of any type, if there is an error getting the list of files</exception>
        /// <returns>The path of all files in the directories, null if the directory doesn't exist</returns>
        public static List<string> GetFilesListRecursive(string directory)
        {
            if (System.IO.Directory.Exists(directory))
            {
                string[] files = System.IO.Directory.GetFiles(directory, searchPatternAny, System.IO.SearchOption.AllDirectories);
                return new List<string>(files);
            }
            return null;
        }

        /// <summary>
        /// Check if the name of a file is the same as an expected one
        /// </summary>
        /// <param name="path">The path of the file</param>
        /// <param name="expectedFile">Expected file name</param>
        /// <returns>true if it's the same file name, false otherwise</returns>
        public static bool IsFileName(string path, string expectedFile)
        {
            string file = System.IO.Path.GetFileName(path).ToLower();
            return file.Equals(expectedFile.ToLower());
        }

        /// <summary>
        /// Check if the extension of a given file is matching a specific extension
        /// </summary>
        /// <param name="fileName">Path of the file</param>
        /// <param name="allowedExtension">An extension</param>
        /// <returns>True if the file has the good extension, false otherwise</returns>
        public static bool IsExtension(string fileName, string allowedExtension)
        {
            return allowedExtension.Equals(System.IO.Path.GetExtension(fileName));
        }

        /// <summary>
        /// Check if the extension of a given file is matching a list of extensions
        /// </summary>
        /// <param name="fileName">Path of the file</param>
        /// <param name="allowedExtensions">List of extensions</param>
        /// <returns>True if the file has the good extension, false otherwise</returns>
        public static bool IsExtension(string fileName, HashSet<string> allowedExtensions)
        {
            return allowedExtensions.Contains(System.IO.Path.GetExtension(fileName));
        }

        /// <summary>
        /// Clean the path of a directory by removing extra '/' within the path
        /// </summary>
        /// <param name="path">Path of a directory</param>
        /// <returns>The cleaned directory path</returns>
        public static string CleanDirectoryPath(string path)
        {
            return TryGetDirectoryName(path + Constants.Slash);
        }

        #endregion

        #region Methods - Dialog File / Folder 

        /// <summary>
        /// Open the File Browser Dialog with the given preset (<see cref="FileFilters"/>)
        /// </summary>
        /// <param name="filter">Filter for the preset parameters</param>
        /// <returns>The path of the selected file, null if none</returns>
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

        /// <summary>
        /// Open the File Browser Dialog with given parameters
        /// </summary>
        /// <param name="filter">Type of files to filter in the File Dialog</param>
        /// <param name="title">Title of the File Dialog</param>
        /// <returns></returns>
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
        /// Open the File Browser Dialog as a Folder Browser Dialog<br/>
        /// <b>Way better than the default "System.Windows.Forms.FolderBrowserDialog"</b>
        /// </summary>
        /// <returns>Returns the selected folder, null if none</returns>
        public static string OpenFolderDialog()
        {
            Microsoft.Win32.FileDialog folderDialog = new Microsoft.Win32.OpenFileDialog
            {
                CheckFileExists = false, // Allow for the selection of a directory
                Title = titleDirectory,
                FileName = directorySelection // Default name
            };
            bool? result = folderDialog.ShowDialog();
            if (result == true)
            {
                return System.IO.Path.GetDirectoryName(folderDialog.FileName);
            }
            return null;
        }

        /// <summary>
        /// Open the Save File Browser Dialog with the given preset (<see cref="FileFilters"/>) and parameters
        /// </summary>
        /// <param name="filter">Filter for the preset parameters</param>
        /// <param name="defaultFileName">Default filename</param>
        /// <param name="defaultExt">Default extension</param>
        /// <param name="initialDirectory">Intitial directory to open the File Dialog in</param>
        /// <returns>The path of the file to save, null if none</returns>
        public static string SaveFileDialog(FileFilters filter, string defaultFileName, string defaultExt, string initialDirectory = "")
        {
            switch (filter)
            {
                case FileFilters.Bsp:
                    return SaveFileDialog(filterBsp, titleBspOutput, defaultFileName, defaultExt, initialDirectory);
                case FileFilters.Zip:
                    return SaveFileDialog(filterZip, titleZipOutput, defaultFileName, defaultExt, initialDirectory);
                case FileFilters.None:
                default:
                    break;
            }
            return SaveFileDialog(string.Empty, string.Empty, defaultFileName, defaultExt, initialDirectory);
        }

        /// <summary>
        /// Open the Save File Browser Dialog with the given parameters
        /// </summary>
        /// <param name="filter">Type of files to filter in the File Dialog</param>
        /// <param name="title">Title of the File Dialog</param>
        /// <param name="defaultFileName">Default filename</param>
        /// <param name="defaultExt">Default extension</param>
        /// <param name="initialDirectory">Intitial directory to open the File Dialog in</param>
        /// <returns>The path of the file to save, null if none</returns>
        private static string SaveFileDialog(string filter, string title, string defaultFileName, string defaultExt, string initialDirectory)
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
