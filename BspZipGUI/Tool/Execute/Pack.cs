using BspZipGUI.Models;
using BspZipGUI.Tool.Utils;
using BspZipGUI.Tool.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool.Execute
{
    internal class Pack : Bspzip
    {

        #region Attributes

        /// <summary>
        /// The custom directory with the files to pack
        /// </summary>
        private readonly MapConfig mapContent;

        /// <summary>
        /// The multiple custom directory with the files to pack
        /// </summary>
        private readonly MultiMapConfig multiMapContent;

        /// <summary>
        /// Only pack specific files types in each specific subfolders
        /// </summary>
        private readonly bool useWhitelist;

        /// <summary>
        /// Path of the BSP to create
        /// </summary>
        private readonly string outputBspPath;

        #endregion

        #region Constructor

        public Pack(ToolSettings toolSettings, GameConfig game, string bspPath, LogText logsOutput, MapConfig mapContent, string outputBspPath, bool useWhitelist) :
            base(toolSettings, game, bspPath, logsOutput)
        {
            this.mapContent = mapContent;
            this.outputBspPath = outputBspPath;
            this.useWhitelist = useWhitelist;
        }

        public Pack(ToolSettings toolSettings, GameConfig game, string bspPath, LogText logsOutput, MultiMapConfig multiMapContent, string outputBspPath, bool useWhitelist) :
            base(toolSettings, game, bspPath, logsOutput)
        {
            this.multiMapContent = multiMapContent;
            this.outputBspPath = outputBspPath;
            this.useWhitelist = useWhitelist;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start the packing for the current options
        /// </summary>
        /// <exception cref="FilePackCreationException">Error when creating the list of file to pack</exception>
        /// <exception cref="BspBackupCreationException">Error when creating the bsp backup</exception>
        /// <exception cref="BspZipExecutionException">Error during bspzip execution</exception>
        public override void Start()
        {
            UpdateSettings();
            logsOutput.AppendLine();
            bool hasMaxPathSize;
            FilePack filePack = new FilePack(GetCustomDirectories(), useWhitelist, toolSettings.DirectoriesRestrictions);
            try
            {
                hasMaxPathSize = filePack.FindAllFilesToPack();
                filePack.OutputToFile();
            }
            catch (Exception ex)
            {
                throw new FilePackCreationException(MessageConstants.MessageListFilesFail, ex);
            }
            if (!System.IO.File.Exists(Constants.FilesListText))
            {
                // Just for safety, but the file is supposed to exist
                throw new FilePackCreationException(MessageConstants.MessageListFilesNotFound);
            }
            logsOutput.AppendLine("Created " + Constants.FilesListText);
            if (bspPath.Equals(outputBspPath))
            {
                // If we override the original BSP, we make a backup
                try
                {
                    CreateBackupBsp();
                }
                catch (Exception ex)
                {
                    throw new BspBackupCreationException(MessageConstants.MessageCopyBspFail, ex);
                }
            }
            try
            {
                StartProcess();
            }
            catch (Exception ex)
            {
                throw new BspZipExecutionException(MessageConstants.MessageBspzipFail, ex);
            }
            if (hasMaxPathSize)
            {
                // One or multiple path longer than MAX_PATH were encountered
                // bspzip.exe likely didn't pack correctly the files
                // We add the list to the logs and throw an exception
                logsOutput.AppendLine($"/!\\ {MessageConstants.MessageMaxPathSizeWarning} :");
                foreach (string path in filePack.MaxPathSizeList)
                {
                    logsOutput.AppendLine($"- {path.Length} : \"{path}\"");
                }
                throw new MaxPathSizeLimitException(MessageConstants.MessageMaxPathSizeSuggestion);
            }
        }

        /// <summary>
        /// Return the arguments to launch bspzip.exe, to pack the files in a BSP
        /// </summary>
        /// <returns><inheritdoc/></returns>
        protected override string GetProcessArguments()
        {
            // bspzip -addlist "<bspfile>" "<listfile>" "<newbspfile>"
            StringBuilder sb = new StringBuilder("-addlist ")
                .Append($"\"{bspPath}\" ")
                .Append($"\"{Constants.FilesListText}\" ")
                .Append($"\"{outputBspPath}\"");
            return sb.ToString();
        }

        /// <summary>
        /// <inheritdoc cref="Bspzip.UpdateSettings"/>
        /// </summary>
        protected override void UpdateSettings()
        {
            toolSettings.LastBsp = bspPath;
            toolSettings.LastGame = game.Name;
            if (mapContent != null)
                toolSettings.LastCustomDirectory = mapContent.Name;
            if (multiMapContent != null)
                toolSettings.LastMultiCustomDirectory = multiMapContent.Name;
            SaveSettings();
        }

        /// <summary>
        /// Get the list of custom directories to use for packing
        /// </summary>
        /// <returns>A list of directories paths cleaned from any extra character</returns>
        private ICollection<string> GetCustomDirectories()
        {
            // Clean the directories path first
            CleanCustomDirectoriesPath();
            if (mapContent != null)
                return new List<string> { mapContent.CleanedPath };
            if (multiMapContent != null)
                return multiMapContent.HashSetCleanedPath;
            return new List<string>();
        }

        /// <summary>
        /// Remove any extra <c>/</c> from the custom directories
        /// </summary>
        private void CleanCustomDirectoriesPath()
        {
            if (mapContent != null)
                mapContent.CleanPath();
            if (multiMapContent != null)
                multiMapContent.CleanPaths();
        }

        #endregion

    }

}
