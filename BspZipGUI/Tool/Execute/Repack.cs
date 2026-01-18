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
    internal class Repack : Bspzip
    {

        #region Attributes

        /// <summary>
        /// If the goal is to Compress or Uncompress the BSP
        /// </summary>
        private readonly bool isCompress;

        #endregion

        #region Constructor

        public Repack(ToolSettings toolSettings, GameConfig game, string bspPath, LogText logsOutput, bool isCompress) :
            base(toolSettings, game, bspPath, logsOutput)
        {
            this.isCompress = isCompress;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start the repacking for the current options
        /// </summary>
        /// <exception cref="BspBackupCreationException">Error when creating the bsp backup</exception>
        /// <exception cref="BspZipExecutionException">Error during bspzip execution</exception>
        public override void Start()
        {
            UpdateSettings();
            logsOutput.AppendLine();
            try
            {
                CreateBackupBsp();
            }
            catch (Exception ex)
            {
                throw new BspBackupCreationException(MessageConstants.MessageCopyBspFail, ex);
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
        /// Return the arguments to launch bspzip.exe, to repack or un-repack a BSP
        /// </summary>
        protected override string GetProcessArguments()
        {
            // bspzip -repack [ -compress ] "<bspfile>"
            // bspzipplusplus [ -verbose ] -repack [ -compress ] "<bspfile>" [ -threads N ]
            StringBuilder sb = new StringBuilder("");

            // Verbose mode possible with bspzipplusplus (doesnt exist on default bspzip)
            if (toolSettings.UseBspZipPlusPlusArguments)
            {
                if (toolSettings.UseVerboseForRepack)
                    sb.Append("-verbose ");
            }

            sb.Append("-repack ");
            if (isCompress)
            {
                sb.Append("-compress ");
            }
            sb.Append($"\"{bspPath}\"");

            // Extra parameter like "-threads N" exist for bspzipplusplus, so the user may have added them in the settings
            if (toolSettings.UseBspZipPlusPlusArguments)
            {
                sb.Append(" " + toolSettings.ExtraParametersRepack);
            }

            return sb.ToString();
        }

        /// <summary>
        /// <inheritdoc cref="Bspzip.UpdateSettings"/>
        /// </summary>
        protected override void UpdateSettings()
        {
            toolSettings.LastBsp = bspPath;
            toolSettings.LastGame = game.Name;
            SaveSettings();
        }

        #endregion

    }
}
