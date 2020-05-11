using BspZipGUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool
{
    class Pack
    {

        /// <summary>
        /// The text file with the paths of all files to pack
        /// </summary>
        public const string filesListText = "filesList.txt";

        
        /// <summary>
        /// The current settings of the tool 
        /// </summary>
        public Settings GlobalSettings { get; }

        /// <summary>
        /// The game used for packing (bspzip.exe)
        /// </summary>
        public GameConfig Game { get; }

        /// <summary>
        /// The custom directory with the files to pack
        /// </summary>
        public MapConfig MapContent { get; }

        /// <summary>
        /// The path to the BSP file
        /// </summary>
        public string BspPath { get; }

        /// <summary>
        /// Only pack specific files types in each specific subfolders
        /// </summary>
        public bool HasRestrictions { get; }

        
        /// <summary>
        /// The output logs from the bspzip process
        /// </summary>
        public StringBuilder LogsOutput { get; }

        /// <summary>
        /// The error logs from the bspzip process
        /// </summary>
        public StringBuilder LogsError { get; }

        public Pack(Settings globalSettings, GameConfig game, MapConfig mapContent, string bspPath, bool hasRestrictions)
        {
            GlobalSettings = globalSettings;
            Game = game;
            MapContent = mapContent;
            BspPath = bspPath;
            HasRestrictions = hasRestrictions;
            LogsOutput = new StringBuilder();
            LogsError = new StringBuilder();
        }

        /// <summary>
        /// Update settings.xml with the bsp loaded, the custom folder loaded, and the game used
        /// </summary>
        private void UpdateSettings()
        {
            GlobalSettings.LastBsp = BspPath;
            GlobalSettings.LastCustomDirectory = MapContent.Name;
            GlobalSettings.LastGame = Game.Name;
            GlobalSettings.SaveSettings();
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
                MapContent.CleanPath(); // Remove any extra '/' 
                FilePack filePack = new FilePack(MapContent.Path); // The base path doesn't have a final '\'
                filePack.FindAllFiles(HasRestrictions, GlobalSettings.DirectoriesRestrictions);
                if (filePack.OutputToFile())
                {
                    LogsOutput.AppendLine("Created " + filesListText);
                    if (System.IO.File.Exists(filesListText))
                    {
                        bool backupSuccess = BackupBsp();
                        if (backupSuccess)
                        {
                            try
                            {
                                PackBSP();
                                success = true;
                            }
                            catch
                            {
                                LogsError.AppendLine("Error: bspzip.exe process ended unexpectedly");
                            }
                        }
                        else
                        {
                            LogsError.AppendLine("Error: Couldn't make a copy of the BSP");
                        }

                    }
                    else
                    {
                        LogsError.AppendLine("Error: Couldn't find the list of files to pack");
                    }
                }
                else
                {
                    LogsError.AppendLine("Error: Couldn't create the list of files to pack");
                }
            });
            return success;
        }

        /// <summary>
        /// Make a backup of the unpacked BSP
        /// </summary>
        /// <returns>Returns true if successful, false otherwise</returns>
        private bool BackupBsp()
        {
            string backupBsp = BspPath + "_old";
            try
            {
                if (System.IO.File.Exists(backupBsp))
                {
                    System.IO.File.Delete(backupBsp);
                }
                System.IO.File.Copy(BspPath, backupBsp);
            }
            catch
            {
                return false;
            }
            LogsOutput.AppendLine("Created a copy of " + BspPath + " to " + backupBsp + "\n");
            return true;
        }

        /// <summary>
        /// Launch a process of bspzip.exe to pack the files in the map
        /// </summary>
        private void PackBSP()
        {
            string arguments = "-addlist \"$bspinput\"  \"$list\" \"$bspoutput\"";
            arguments = arguments.Replace("$bspinput", BspPath);
            arguments = arguments.Replace("$bspoutput", BspPath);
            arguments = arguments.Replace("$list", filesListText);

            var startInfo = new ProcessStartInfo(Game.BspZip, arguments)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                
            };
            startInfo.EnvironmentVariables["VPROJECT"] = Game.GameInfoFolder;

            var p = new Process { StartInfo = startInfo };

            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            LogsOutput.Append(output);
            p.WaitForExit();
        }
    }
}
