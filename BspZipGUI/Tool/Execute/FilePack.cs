using BspZipGUI.Tool.Utils;
using BspZipGUI.Tool.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool.Execute
{
    internal class FilePack
    {

        #region Attributes

        /// <summary>
        /// The list of files to pack: &lt;internalPath, externalPath&gt;
        /// </summary>
        private readonly IDictionary<string, string> filesPathsList;

        /// <summary>
        /// List of pairs of &lt;customDirectoryPath, length&gt;
        /// </summary>
        private readonly ICollection<KeyValuePair<string, int>> customDirectoriesPairs;

        /// <summary>
        /// The list of base directory and extension allowed to be packed
        /// </summary>
        private readonly ICollection<DirectoryRestrictions> directoriesRestrictions;

        /// <summary>
        /// Use the list of directory whitelist when packing
        /// </summary>
        private readonly bool useWhitelist;

        /// <summary>
        /// Store the list of paths that are longer than <see cref="Constants.MAX_PATH"/>
        /// </summary>
        public readonly HashSet<string> MaxPathSizeList;

        #endregion

        #region Constructor

        public FilePack(ICollection<string> customDirectories, bool useWhitelist, ICollection<DirectoryRestrictions> directoriesRestrictions)
        {
            this.customDirectoriesPairs = new List<KeyValuePair<string, int>>();
            foreach (string customDirectory in customDirectories)
            {
                // Each directory path will be cleaned from any ending '/', so + 1 for the length
                this.customDirectoriesPairs.Add(new KeyValuePair<string, int>(customDirectory, customDirectory.Length + 1));
            }
            this.filesPathsList = new Dictionary<string, string>();
            this.directoriesRestrictions = directoriesRestrictions;
            this.useWhitelist = useWhitelist;
            this.MaxPathSizeList = new HashSet<string>();
        }

        #endregion

        #region Methods - Find Files

        /// <summary>
        /// Find all the files to pack and store them in the <see cref="filesPathsList"/> list
        /// </summary>
        /// <returns><c>true</c> if one or multiple paths are longer than <see cref="Constants.MAX_PATH"/>, <c>false</c> otherwise</returns>
        public bool FindAllFilesToPack()
        {
            if (useWhitelist)
            {
                // Add only the files matching specific extensions (defined in the Settings)
                return AddAllFilesFromWhitelistDirectories();
            }
            else
            {
                // Add every single file from the directories and subdirectories, regardless of extension
                return AddAllFilesFromAnyDirectories();
            }
        }

        /// <summary>
        /// Add every single files from the directories, to <see cref="filesPathsList"/>, regardless of their extensions
        /// </summary>
        /// <returns><c>true</c> if one or multiple paths are longer than <see cref="Constants.MAX_PATH"/>, <c>false</c> otherwise</returns>
        private bool AddAllFilesFromAnyDirectories()
        {
            bool hasMaxPathSize = false;
            foreach (KeyValuePair<string, int> customDirectoryPair in customDirectoriesPairs)
            {
                List<string> filesList = FileUtils.GetFilesListRecursive(customDirectoryPair.Key);
                if (filesList != null)
                {
                    foreach (string path in filesList)
                    {
                        AppendPath(path, customDirectoryPair.Value);
                        // bspzip.exe will not behave correctly if a very long path is in the file
                        if (path.Length >= Constants.MAX_PATH)
                        {
                            MaxPathSizeList.Add(path);
                            hasMaxPathSize = true;
                        }
                    }
                }
            }
            return hasMaxPathSize;
        }

        /// <summary>
        /// Add files matching specific extensions, located in specific subdirectories, to <see cref="filesPathsList"/>
        /// </summary>
        /// <returns><c>true</c> if one or multiple paths are longer than <see cref="Constants.MAX_PATH"/>, <c>false</c> otherwise</returns>
        private bool AddAllFilesFromWhitelistDirectories()
        {
            bool hasMaxPathSize = false;
            if (directoriesRestrictions != null)
            {
                // Go through each whitelisted subdirectory (materials, models,...)
                foreach (DirectoryRestrictions directoryRestrictions in directoriesRestrictions)
                {
                    HashSet<string> allowedExtensions = new HashSet<string>(directoryRestrictions.AllowedExtension);

                    // Go through each custom directory
                    foreach (KeyValuePair<string, int> customDirectoryPair in customDirectoriesPairs)
                    {
                        // Combine the custom directory path + the whitelisted subdirectory
                        string subDirectory = System.IO.Path.Combine(customDirectoryPair.Key, directoryRestrictions.DirectoryName);
                        if (System.IO.Directory.Exists(subDirectory))
                        {
                            List<string> filesList = FileUtils.GetFilesListRecursive(subDirectory);
                            if (filesList != null)
                            {
                                foreach (string path in filesList)
                                {
                                    // Verify the file extension
                                    if (FileUtils.IsExtension(path, allowedExtensions))
                                    {
                                        AppendPath(path, customDirectoryPair.Value);
                                        // bspzip.exe will not behave correctly if a very long path is in the file
                                        if (path.Length >= Constants.MAX_PATH)
                                        {
                                            MaxPathSizeList.Add(path);
                                            hasMaxPathSize = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return hasMaxPathSize;
        }

        /// <summary>
        /// Append a given file path to <see cref="filesPathsList"/> 
        /// </summary>
        /// <param name="externalPath">Absolute path of a file</param>
        private void AppendPath(string externalPath, int customDirectoryLength)
        {
            string internalPath = GetInternalPath(externalPath, customDirectoryLength);
            if (!filesPathsList.ContainsKey(internalPath))
                filesPathsList.Add(new KeyValuePair<string, string>(internalPath, externalPath));
        }

        /// <summary>
        /// Get the local path of a given file (based on the custom directory)
        /// </summary>
        /// <param name="externalPath">Absolute path of a file</param>
        /// <returns>The relative path to save in the txt file used by bspzip.exe</returns>
        private string GetInternalPath(string externalPath, int customDirectoryLength)
        {
            // e.g. c:\programfiles\....\materials\myFolder\texture.vtf
            // => materials/myFolder/texture.vtf
            return externalPath.Substring(customDirectoryLength).
                Replace(Constants.Backslash, Constants.Slash);
        }

        #endregion

        #region Methods - Write List

        /// <summary>
        /// Return the amount of files previously stored in <see cref="filesPathsList"/>
        /// </summary>
        public int CountListOfFiles()
        {
            return filesPathsList.Count;
        }

        /// <summary>
        /// Create filesList.txt with the paths of each files, located in <see cref="filesPathsList"/>
        /// </summary>
        /// <exception cref="Exception">In case there is an error during the file creation</exception>
        public void OutputToFile()
        {
            List<string> outputLines = new List<string>();
            foreach (KeyValuePair<string, string> entry in filesPathsList)
            {
                outputLines.Add(entry.Key);
                outputLines.Add(entry.Value);
            }

            if (System.IO.File.Exists(Constants.FilesListText))
            {
                System.IO.File.Delete(Constants.FilesListText);
            }
            System.IO.File.WriteAllLines(Constants.FilesListText, outputLines);
        }

        #endregion

    }


}
