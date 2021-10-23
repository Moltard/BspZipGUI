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
        /// Only pack specific files types in each specific subfolders
        /// </summary>
        private readonly bool hasRestrictions;

        /// <summary>
        /// Path of the BSP to create
        /// </summary>
        private readonly string outputBspPath;

        #endregion

        #region Constructor

        public Pack(ToolSettings toolSettings, GameConfig game, string bspPath, LogText logsOutput, MapConfig mapContent, string outputBspPath, bool hasRestrictions) :
            base(toolSettings, game, bspPath, logsOutput)
        {
            this.mapContent = mapContent;
            this.outputBspPath = outputBspPath;
            this.hasRestrictions = hasRestrictions;
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
            // Remove any extra '/'
            mapContent.CleanPath();

            // The base path doesn't have a final '\'
            FilePack filePack = new FilePack(mapContent.Path, hasRestrictions, toolSettings.DirectoriesRestrictions);
            try
            {
                filePack.FindAllFiles();
                filePack.OutputToFile();
            }
            catch (Exception ex)
            {
                throw new FilePackCreationException(MessageConstants.MessageListFilesFail, ex);
            }
            logsOutput.AppendLine("Created " + Constants.FilesListText);
            if (!System.IO.File.Exists(Constants.FilesListText))
            {
                // Just for safety, but the file is supposed to exist
                throw new FilePackCreationException(MessageConstants.MessageListFilesNotFound);
            }
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

        }

        /// <summary>
        /// Return the arguments to launch bspzip.exe, to pack the files in a BSP
        /// </summary>
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
            toolSettings.LastCustomDirectory = mapContent.Name;
            SaveSettings();
        }


        #endregion

    }




}
