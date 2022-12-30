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
    internal class Extract : Bspzip
    {

        #region Attributes

        /// <summary>
        /// True if extract to a zip file, false if extract to a folder
        /// </summary>
        private readonly bool isExtractToZip;

        /// <summary>
        /// Name of the zip file or folder path
        /// </summary>
        private readonly string extractPath;

        #endregion

        #region Constructor

        public Extract(ToolSettings toolSettings, GameConfig game, string bspPath, LogText logsOutput, bool isExtractToZip, string extractPath) :
            base(toolSettings, game, bspPath, logsOutput)
        {
            this.isExtractToZip = isExtractToZip;
            this.extractPath = extractPath;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Extract the map content into a zip file or folder
        /// </summary>
        /// <exception cref="BspZipExecutionException">Error during bspzip execution</exception>
        public override void Start()
        {
            UpdateSettings();
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
        /// Return the arguments to launch bspzip.exe, to extract or delete cubemaps from a BSP
        /// </summary>
        /// <returns><inheritdoc/></returns>
        protected override string GetProcessArguments()
        {
            // bspzip -extract "<bspfile>" "<targetPathZipFile>"
            // bspzip -extractfiles "<bspfile>" "<targetPathDirectory>"
            StringBuilder sb;
            if (isExtractToZip)
            {
                sb = new StringBuilder("-extract ")
                    .Append($"\"{bspPath}\" ")
                    .Append($"\"{extractPath}\"");
            }
            else
            {
                sb = new StringBuilder("-extractfiles ")
                    .Append($"\"{bspPath}\" ")
                    .Append($"\"{extractPath}\"");
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
            if (!isExtractToZip)
            {
                toolSettings.LastExtractDirectory = extractPath;
            }
            SaveSettings();
        }


        #endregion

    }




}
