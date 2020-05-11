using BspZipGUI.Tool.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool
{
    class FilePack
    {

        /// <summary>
        /// The list of files to pack: &lt;internalPath, externalPath&gt;
        /// </summary>
        private IDictionary<string, string> FilesList { get; }

        /// <summary>
        /// The path to the custom directory (no ending '/')
        /// </summary>
        private string CustomDirectory { get; }

        /// <summary>
        /// The length of custom directory path + 1
        /// </summary>
        private int CustomDirectoryLength { get; }


        public FilePack(string customDirectory)
        {
            CustomDirectory = customDirectory;
            CustomDirectoryLength = customDirectory.Length + 1;
            FilesList = new Dictionary<string, string>();
        }

        /// <summary>
        /// Find all the files to pack and store them in the <see cref="FilesList"/>
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="hasRestrictions"></param>
        /// <param name="directoriesRestrictions"></param>
        public void FindAllFiles(bool hasRestrictions, ICollection<DirectoryRestrictions> directoriesRestrictions)
        {
            if (hasRestrictions)
            {
                // Add only the files matching specific extensions
                foreach (var directoryRestriction in directoriesRestrictions)
                {
                    GetAllFilesSubDirectory(directoryRestriction);
                }
            }
            else
            {
                // Add every single file in the subfolders
                var filesList = FilesUtils.GetFilesList(CustomDirectory);
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
        /// Find all the files matching the extensions restrictions and store them in the <see cref="FilesList"/>
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="directoryRestrictions"></param>
        private void GetAllFilesSubDirectory(DirectoryRestrictions directoryRestrictions)
        {
            string subDirectory = System.IO.Path.Combine(CustomDirectory, directoryRestrictions.DirectoryName);
            string[] extensions = directoryRestrictions.AllowedExtension.ToArray();
            if (System.IO.Directory.Exists(subDirectory))
            {
                var filesList = FilesUtils.GetFilesList(subDirectory);
                if (filesList != null)
                {
                    foreach (string path in filesList)
                    {
                        if (FilesUtils.IsExtension(path, extensions))
                        {
                            AppendPath(path);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Append the given path to <see cref="FilesList"/> 
        /// </summary>
        /// <param name="externalPath"></param>
        private void AppendPath(string externalPath)
        {
            string internalPath = GetInternalPath(externalPath);
            FilesList.Add(new KeyValuePair<string, string>(internalPath, externalPath));
        }

        /// <summary>
        /// Returns the local path of a given file (based on the custom directory)
        /// </summary>
        /// <param name="externalPath"></param>
        /// <returns></returns>
        private string GetInternalPath(string externalPath)
        {
            return externalPath.Substring(CustomDirectoryLength).
                Replace(FilesUtils.antislash, FilesUtils.slash);
        }

        /// <summary>
        /// Create filesList.txt with the paths of each files to pack
        /// </summary>
        public bool OutputToFile()
        {
            var outputLines = new List<string>();
            foreach (KeyValuePair<string, string> entry in FilesList)
            {
                outputLines.Add(entry.Key);
                outputLines.Add(entry.Value);
            }

            try
            {
                if (System.IO.File.Exists(Pack.filesListText))
                {
                    System.IO.File.Delete(Pack.filesListText);
                }
                System.IO.File.WriteAllLines(Pack.filesListText, outputLines);

            }
            catch
            {
                return false;
            }
            return true;
        }
        
    }
}
