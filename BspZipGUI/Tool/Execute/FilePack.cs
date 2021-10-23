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
        private readonly IDictionary<string, string> filesList;

        /// <summary>
        /// The path to the custom directory (no ending '/')
        /// </summary>
        private readonly string customDirectory;

        /// <summary>
        /// The length of custom directory path + 1 (used for a substring)
        /// </summary>
        private readonly int customDirectoryLength;

        /// <summary>
        /// The list of base directory and extension allowed to be packed
        /// </summary>
        private readonly ICollection<DirectoryRestrictions> directoriesRestrictions;

        /// <summary>
        /// Use the list of directory when packing
        /// </summary>
        private readonly bool useRestrictions;

        #endregion

        #region Constructor

        public FilePack(string customDirectory, bool useRestrictions, ICollection<DirectoryRestrictions> directoriesRestrictions)
        {
            this.customDirectory = customDirectory;
            customDirectoryLength = customDirectory.Length + 1;
            filesList = new Dictionary<string, string>();
            this.directoriesRestrictions = directoriesRestrictions;
            this.useRestrictions = useRestrictions;
        }

        #endregion

        #region Methods - Find Files

        /// <summary>
        /// Find all the files to pack and store them in the <see cref="filesList"/>
        /// </summary>
        public void FindAllFiles()
        {
            if (useRestrictions)
            {
                // Add only the files matching specific extensions (defined in the Settings)
                if (directoriesRestrictions != null)
                {
                    foreach (DirectoryRestrictions directoryRestriction in directoriesRestrictions)
                    {
                        GetAllFilesSubDirectory(directoryRestriction);
                    }
                }
                
            }
            else
            {
                // Add every single file in the folder and its subfolders
                List<string> filesList = FileUtils.GetFilesListRecursive(customDirectory);
                if (filesList != null)
                {
                    foreach (string path in filesList)
                    {
                        AppendPath(path);
                    }
                }
            }
        }
        
        /// <summary>
        /// Find all the files matching the extensions restrictions and store them in the <see cref="filesList"/>
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="directoryRestrictions"></param>
        private void GetAllFilesSubDirectory(DirectoryRestrictions directoryRestrictions)
        {
            string subDirectory = System.IO.Path.Combine(customDirectory, directoryRestrictions.DirectoryName);
            string[] extensions = directoryRestrictions.AllowedExtension.ToArray();
            if (System.IO.Directory.Exists(subDirectory))
            {
                List<string> filesList = FileUtils.GetFilesListRecursive(subDirectory);
                if (filesList != null)
                {
                    foreach (string path in filesList)
                    {
                        if (FileUtils.IsExtension(path, extensions))
                        {
                            AppendPath(path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Append the given path to <see cref="filesList"/> 
        /// </summary>
        /// <param name="externalPath"></param>
        private void AppendPath(string externalPath)
        {
            string internalPath = GetInternalPath(externalPath);
            filesList.Add(new KeyValuePair<string, string>(internalPath, externalPath));
        }

        /// <summary>
        /// Returns the local path of a given file (based on the custom directory)
        /// </summary>
        /// <param name="externalPath"></param>
        /// <returns></returns>
        private string GetInternalPath(string externalPath)
        {
            // e.g. c:\programfiles\....\materials\myFolder\texture.vtf
            // => materials/myFolder/texture.vtf
            return externalPath.Substring(customDirectoryLength).
                Replace(Constants.Backslash, Constants.Slash);
        }

        #endregion

        #region Methods - Write List

        /// <summary>
        /// Create filesList.txt with the paths of each files to pack
        /// </summary>
        /// <exception cref="Exception">In case there is an error during the file creation</exception>
        public void OutputToFile()
        {
            List<string> outputLines = new List<string>();
            foreach (KeyValuePair<string, string> entry in filesList)
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
