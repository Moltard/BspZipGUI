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
    internal abstract class Bspzip
    {

        #region Attributes

        /// <summary>
        /// The current settings of the tool 
        /// </summary>
        protected readonly ToolSettings toolSettings;

        /// <summary>
        /// The game used for packing (bspzip.exe)
        /// </summary>
        protected readonly GameConfig game;

        /// <summary>
        /// The path to the BSP file
        /// </summary>
        protected readonly string bspPath;

        /// <summary>
        /// The output logs from the bspzip process
        /// </summary>
        protected readonly LogText logsOutput;

        /// <summary>
        /// Should the logs be written asynchronously or synchronously
        /// </summary>
        protected readonly bool isSyncLogsOutput;

        #endregion

        #region Constructor

        protected Bspzip(ToolSettings toolSettings, GameConfig game, string bspPath, LogText logsOutput)
        {
            this.toolSettings = toolSettings;
            this.game = game;
            this.bspPath = bspPath;
            this.logsOutput = logsOutput;
            isSyncLogsOutput = toolSettings.IsSyncLogs;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Function to start bspzip.exe<br/>
        /// Implemented by the classes that inherit from <see cref="Bspzip"/>
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Launch a process of bspzip.exe with the given arguments
        /// </summary>
        protected void StartProcess()
        {
            string arguments = GetProcessArguments();
            logsOutput.AppendLine($"Executing \"{game.BspZip}\" with the following arguments:\n   {arguments}\n");

            logsOutput.AppendLine("==========================================================================\n");
            ProcessStartInfo startInfo = new ProcessStartInfo(game.BspZip, arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,

            };
            startInfo.EnvironmentVariables["VPROJECT"] = game.GameInfoFolder;

            Process p = new Process { StartInfo = startInfo };

            if (isSyncLogsOutput)
            {
                // Sync output
                p.Start();
                string output = p.StandardOutput.ReadToEnd();
                logsOutput.AppendLine(output);
                p.WaitForExit();
            }
            else
            {
                // Async output
                p.OutputDataReceived += new DataReceivedEventHandler(ProcessOutputHandler);
                p.ErrorDataReceived += new DataReceivedEventHandler(ProcessErrorHandler);
                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();
                p.Close();
            }

            // To stop the output reading
            // p.CancelOutputRead();
            // p.Close()

            logsOutput.AppendLine("==========================================================================\n");

        }

        /// <summary>
        /// Create a backup of the BSP
        /// </summary>
        /// <exception cref="Exception">Exception of any type thrown if an error happened during the file deletion/creation</exception>
        protected void CreateBackupBsp()
        {
            string backupBsp = bspPath + "_old";
            if (System.IO.File.Exists(backupBsp))
            {
                System.IO.File.Delete(backupBsp);
            }
            System.IO.File.Copy(bspPath, backupBsp);
            logsOutput.AppendLine($"Created a copy of \"{bspPath}\" \n=> \"{backupBsp}\"\n");
        }

        /// <summary>
        /// Get the arguments used to launch bspzip
        /// </summary>
        /// <returns>The arguments to use for the bspzip process</returns>
        protected abstract string GetProcessArguments();

        /// <summary>
        /// Save the current settings in settings.xml<br/>
        /// Log any error that may happen while saving
        /// </summary>
        protected void SaveSettings()
        {
            try
            {
                XmlUtils.SerializeSettings(toolSettings);
            }
            catch (SettingsSerializationException ex)
            {
                logsOutput.AppendLine(ex.GetMessageAndInner());
                logsOutput.AppendLine();
            }
        }

        /// <summary>
        /// Update the values of <see cref="toolSettings"/> based on last parameters used<br/>
        /// And save them in settings.xml
        /// </summary>
        protected abstract void UpdateSettings();

        /// <summary>
        /// Delegate function used to log the process outputs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessOutputHandler(object sender, DataReceivedEventArgs e)
        {
            logsOutput.AppendLine(e.Data);
        }

        /// <summary>
        /// Delegate function used to log the process errors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProcessErrorHandler(object sender, DataReceivedEventArgs e)
        {
            logsOutput.AppendLine(e.Data);
        }

        #endregion

    }

}
