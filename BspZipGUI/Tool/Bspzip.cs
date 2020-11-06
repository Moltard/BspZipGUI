using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool
{
    abstract class Bspzip
    {

        #region Attributes

        /// <summary>
        /// The current settings of the tool 
        /// </summary>
        protected readonly Settings toolSettings;

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
        protected readonly StringBuilder logsOutput;

        #endregion

        #region Constructor

        protected Bspzip(Settings toolSettings, GameConfig game, string bspPath)
        {
            this.toolSettings = toolSettings;
            this.game = game;
            this.bspPath = bspPath;
            logsOutput = new StringBuilder();
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Make a backup of the BSP
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        protected bool BackupBsp()
        {
            string backupBsp = bspPath + "_old";
            try
            {
                if (System.IO.File.Exists(backupBsp))
                {
                    System.IO.File.Delete(backupBsp);
                }
                System.IO.File.Copy(bspPath, backupBsp);
            }
            catch
            {
                return false;
            }
            logsOutput.AppendLine(string.Format("Created a copy of {0} to {1}\n", bspPath, backupBsp));
            return true;
        }

        /// <summary>
        /// Update settings.xml with the current settings
        /// </summary>
        protected abstract void UpdateSettings();

        /// <summary>
        /// Returns the LogsOutput as a String
        /// </summary>
        /// <returns></returns>
        public string GetLogsOutput()
        {
            return logsOutput.ToString();
        }

        /// <summary>
        /// Launch a process of bspzip.exe with the given arguments
        /// </summary>
        protected void StartProcess(string arguments)
        {
            var startInfo = new ProcessStartInfo(game.BspZip, arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,

            };
            startInfo.EnvironmentVariables["VPROJECT"] = game.GameInfoFolder;

            var p = new Process { StartInfo = startInfo };

            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            logsOutput.Append(output);
            p.WaitForExit();
        }

        #endregion

    }
}
