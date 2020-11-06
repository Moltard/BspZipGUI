using BspZipGUI.Tool.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool
{
    class Repack : Bspzip
    {

        #region Attributes

        /// <summary>
        /// If the goal is to Compress or Uncompress the BSP
        /// </summary>
        private readonly bool isCompress;

        #endregion

        #region Constructor

        public Repack(Settings toolSettings, GameConfig game, string bspPath, bool isCompress) : base(toolSettings, game, bspPath)
        {
            this.isCompress = isCompress;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Launch a process of bspzip.exe to repack or un-repack a BSP
        /// </summary>
        private void RepackBSP()
        {
            // bspzip -repack [ -compress ] "<bspfile>"
            StringBuilder sb = new StringBuilder("-repack ");
            if (isCompress)
            {
                sb.Append("-compress ");
            }
            sb.Append("\"" + bspPath + "\"");
            string arguments = sb.ToString();

            StartProcess(arguments);
        }

        /// <summary>
        /// Start the repacking for the current options
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Start()
        {
            bool success = false;
            await Task.Run(() =>
            {
                UpdateSettings();
                if (BackupBsp())
                {
                    try
                    {
                        RepackBSP();
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
            toolSettings.SaveSettings();
        }

        #endregion

    }
}
