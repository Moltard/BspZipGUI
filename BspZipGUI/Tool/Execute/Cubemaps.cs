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
    internal class Cubemaps : Bspzip
    {

        #region Attributes

        /// <summary>
        /// True if extract cubemaps, false if delete cubemaps
        /// </summary>
        private readonly bool isExtractCubemap;

        /// <summary>
        /// Path for the cubemap extraction
        /// </summary>
        private readonly string extractPath;

        #endregion

        #region Constructor

        public Cubemaps(ToolSettings toolSettings, GameConfig game, string bspPath, LogText logsOutput, bool isExtractCubemap, string extractPath) :
            base(toolSettings, game, bspPath, logsOutput)
        {
            this.isExtractCubemap = isExtractCubemap;
            this.extractPath = extractPath;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Extract or Delete cubemaps
        /// </summary>
        /// <exception cref="BspZipExecutionException">Error during bspzip execution</exception>
        public override void Start()
        {
            UpdateSettings();
            if (!isExtractCubemap)
            {
                // If delete cubemaps, we make a backup of the bsp
                try
                {
                    CreateBackupBsp();
                }
                catch (Exception ex)
                {
                    logsOutput.AppendLine();
                    throw new BspBackupCreationException(MessageConstants.MessageCopyBspFail, ex);
                }
            }
            try
            {
                StartProcess();
            }
            catch (Exception ex)
            {
                logsOutput.AppendLine();
                throw new BspZipExecutionException(MessageConstants.MessageBspzipFail, ex);
            }
        }

        /// <summary>
        /// Return the arguments to launch bspzip.exe, to extract or delete cubemaps from a BSP
        /// </summary>
        /// <returns><inheritdoc/></returns>
        protected override string GetProcessArguments()
        {
            // bspzip -extractcubemaps "<bspfile>" "<targetPath>"
            // bspzip -deletecubemaps "<bspfile>"
            // bspzipplusplus [ -verbose ] -extract "<bspfile>" "<targetPathZipFile>"
            // bspzipplusplus [ -verbose ] -extractfiles "<bspfile>" "<targetPathDirectory>"
            StringBuilder sb = new StringBuilder("");

            // Verbose mode possible with bspzipplusplus (doesnt exist on default bspzip)
            if (toolSettings.UseBspZipPlusPlusArguments)
            {
                if (toolSettings.UseVerboseForCubemaps)
                    sb.Append("-verbose ");
            }

            if (isExtractCubemap)
            {
                sb.Append("-extractcubemaps ")
                    .Append($"\"{bspPath}\" ")
                    .Append($"\"{extractPath}\"");
            }
            else
            {
                sb.Append("-deletecubemaps ")
                    .Append($"\"{bspPath}\"");
            }

            // No extra parameter exist for this one with bspzipplusplus, but i let the user add parameters if they want
            if (toolSettings.UseBspZipPlusPlusArguments)
            {
                sb.Append(" " + toolSettings.ExtraParametersCubemaps);
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
            if (isExtractCubemap)
            {
                toolSettings.LastExtractDirectory = extractPath;
            }
            SaveSettings();
        }

        #endregion

    }




}
