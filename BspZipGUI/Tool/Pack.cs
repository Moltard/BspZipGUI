using BspZipGUI.Models;
using BspZipGUI.Tool.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool
{
    class Pack : Bspzip
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

        #endregion

        #region Constructor

        public Pack(Settings toolSettings, GameConfig game, string bspPath, MapConfig mapContent, bool hasRestrictions) : base(toolSettings, game, bspPath)
        {
            this.mapContent = mapContent;
            this.hasRestrictions = hasRestrictions;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Launch a process of bspzip.exe to pack the files in the map
        /// </summary>
        private void PackBSP()
        {
            // bspzip -addlist "<bspfile>" "<listfile>" "<newbspfile>"
            StringBuilder sb = new StringBuilder("-addlist ")
                .Append("\"" + bspPath + "\" ")
                .Append("\"" + Constants.FilesListText + "\" ")
                .Append("\"" + bspPath + "\"");
            string arguments = sb.ToString();

            StartProcess(arguments);
        }

        /// <summary>
        /// Start the packing for the current options
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Start()
        {
            bool success = false;
            await Task.Run(() =>
            {
                UpdateSettings();
                mapContent.CleanPath(); // Remove any extra '/' 
                FilePack filePack = new FilePack(mapContent.Path); // The base path doesn't have a final '\'
                filePack.FindAllFiles(hasRestrictions, toolSettings.DirectoriesRestrictions);
                if (filePack.OutputToFile())
                {
                    logsOutput.AppendLine("Created " + Constants.FilesListText);
                    if (System.IO.File.Exists(Constants.FilesListText))
                    {
                        if (BackupBsp())
                        {
                            try
                            {
                                PackBSP();
                                success = true;
                            }
                            catch
                            {
                                logsOutput.AppendLine(MessageConstants.MessageBspzipFail);
                            }
                        }
                        else
                        {
                            logsOutput.AppendLine(MessageConstants.MessageCopyBspFail);
                        }

                    }
                    else
                    {
                        logsOutput.AppendLine(MessageConstants.MessageListFilesNotFound);
                    }
                }
                else
                {
                    logsOutput.AppendLine(MessageConstants.MessageListFilesFail);
                }
            });
            return success;
        }

        /// <summary>
        /// <inheritdoc cref="Bspzip.UpdateSettings"/>
        /// </summary>
        protected override void UpdateSettings()
        {
            toolSettings.LastBsp = bspPath;
            toolSettings.LastGame = game.Name;
            toolSettings.LastCustomDirectory = mapContent.Name;
            toolSettings.SaveSettings();
        }

        #endregion

    }
}
