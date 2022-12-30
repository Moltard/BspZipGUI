using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BspZipGUI.Models;
using BspZipGUI.Tool;
using BspZipGUI.Tool.Utils;
using BspZipGUI.Tool.Xml;
using BspZipGUI.Tool.Execute;

namespace BspZipGUI
{

    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Attributes

        private const string MessageTitle = "BspZipGUI";
        private readonly LogText logText = new LogText();
        private ToolSettings toolSettings;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
           
            InitLogs();

            InitSettings();
        }

        #endregion

        #region Initialize GUI

        private void InitLogs()
        {
            LogsTextBox.DataContext = logText;
        }

        private void InitSettings()
        {
            try
            {
                toolSettings = XmlUtils.GetSettingsFromFile();
            }
            catch (SettingsSerializationException ex)
            {
                string messageText = ex.GetMessageAndInner() + "\n\t" + MessageConstants.MessageSettingsUseDefault;
                MessageBox.Show(messageText, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (toolSettings == null)
            {
                try
                {
                    toolSettings = XmlUtils.GetSettingsFromResource();
                }
                catch (SettingsSerializationException ex)
                {
                    // There should never be an error when reading from the resources, this is for safety
                    string messageText = ex.GetMessageAndInner() + "\n\t" + MessageConstants.MessageSettingsUseEmpty;
                    MessageBox.Show(messageText, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    toolSettings = new ToolSettings();
                }
            }
            if (toolSettings != null)
            {
                // Init all attributes that were null, and delete the game and mapconfigs that are invalid
                toolSettings.InitAllAttributes();

                // Init the Games combobox and set the last game used (if possible)
                InitGamesComboBox();

                // Init the Custom Folders combobox and set the last folder used (if possible)
                InitCustomFoldersComboBox();

                // Init the Multi Custom Folders combobox and set the last folder used (if possible)
                InitMultiCustomFoldersComboBox();

                // Set the last BSP loaded (if possible)
                PackBspFileTextBox.Text = toolSettings.LastBsp;
                MultiPackBspFileTextBox.Text = toolSettings.LastBsp;
                RepackBspFileTextBox.Text = toolSettings.LastBsp;
                ExtractBspFileTextBox.Text = toolSettings.LastBsp;
                CubemapsBspFileTextBox.Text = toolSettings.LastBsp;

                // Set the extraction folder (if possible)
                ExtractBspFolderTextBox.Text = toolSettings.LastExtractDirectory;
                CubemapsBspFolderTextBox.Text = toolSettings.LastExtractDirectory;

                // Init the whitelist combobox
                InitDirectoryRestrictionsComboBox();

            }
        }

        #endregion

        #region Handlers Games GUI

        /// <summary>
        /// Init the datacontext of all game combobox and set the selection to a default value
        /// </summary>
        private void InitGamesComboBox()
        {
            PackBspGameComboBox.DataContext = toolSettings.GamesConfigs;
            MultiPackBspGameComboBox.DataContext = toolSettings.GamesConfigs;
            RepackBspGameComboBox.DataContext = toolSettings.GamesConfigs;
            ExtractBspGameComboBox.DataContext = toolSettings.GamesConfigs;
            CubemapsBspGameComboBox.DataContext = toolSettings.GamesConfigs;
            SettingsGameComboBox.DataContext = toolSettings.GamesConfigs;

            GameConfig game = toolSettings.FindGameConfig(toolSettings.LastGame);
            if (game != null)
            {
                GameSetSelected(game);
            }
            else
            {
                GamesForceSelect();
            }
        }

        /// <summary>
        /// Set the selected game config of all Game combobox to the given game
        /// </summary>
        /// <param name="game">Game config to use</param>
        private void GameSetSelected(GameConfig game)
        {
            PackBspGameComboBox.SelectedItem = game;
            MultiPackBspGameComboBox.SelectedItem = game;
            RepackBspGameComboBox.SelectedItem = game;
            ExtractBspGameComboBox.SelectedItem = game;
            CubemapsBspGameComboBox.SelectedItem = game;
            SettingsGameComboBox.SelectedItem = game;
        }

        /// <summary>
        /// Force all game combobox to the first element if possible
        /// </summary>
        private void GamesForceSelect()
        {
            if (toolSettings.GamesConfigs.Count > 0)
            {
                GameSetSelected(toolSettings.GamesConfigs[0]);
            }
            else
            {
                GamesListUpdateGUI();
            }
        }

        /// <summary>
        /// Update the visibility of the Game combobox and settings based on how many game are available
        /// </summary>
        private void GamesListUpdateGUI()
        {
            if (toolSettings.GamesConfigs.Count > 0)
            {
                PackBspGameComboBox.IsEnabled = true;
                RepackBspGameComboBox.IsEnabled = true;
                ExtractBspGameComboBox.IsEnabled = true;
                CubemapsBspGameComboBox.IsEnabled = true;
                SettingsGameComboBox.IsEnabled = true;
                SettingsGamesContainer.Visibility = Visibility.Visible;
            }
            else
            {
                PackBspGameComboBox.IsEnabled = false;
                RepackBspGameComboBox.IsEnabled = false;
                ExtractBspGameComboBox.IsEnabled = false;
                CubemapsBspGameComboBox.IsEnabled = false;
                SettingsGameComboBox.IsEnabled = false;
                SettingsGamesContainer.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region Handlers Custom Folder GUI

        /// <summary>
        /// Init the datacontext of the custom folder combobox and set the selection to a default value
        /// </summary>
        private void InitCustomFoldersComboBox()
        {
            PackBspCustomFolderComboBox.DataContext = toolSettings.MapsConfigs;
            SettingsCustomFolderComboBox.DataContext = toolSettings.MapsConfigs;

            MapConfig map = toolSettings.FindMapConfig(toolSettings.LastCustomDirectory);
            if (map != null)
            {
                CustomFolderSetSelected(map);
            }
            else
            {
                CustomFoldersForceSelect();
            }
        }

        /// <summary>
        /// Set the selected map config of the combobox to the given map
        /// </summary>
        /// <param name="map"></param>
        private void CustomFolderSetSelected(MapConfig map)
        {
            PackBspCustomFolderComboBox.SelectedItem = map;
            SettingsCustomFolderComboBox.SelectedItem = map;
        }

        /// <summary>
        /// Force the custom folder combobox to the first element if possible
        /// </summary>
        private void CustomFoldersForceSelect()
        {
            if (toolSettings.MapsConfigs.Count > 0)
            {
                CustomFolderSetSelected(toolSettings.MapsConfigs[0]);
            }
            else
            {
                CustomFoldersListUpdateGUI();
            }
        }

        /// <summary>
        /// Update the visibility of the Custom folder combobox and settings based on how many 'mapconfigs' are available
        /// </summary>
        private void CustomFoldersListUpdateGUI()
        {
            if (toolSettings.MapsConfigs.Count > 0)
            {
                PackBspCustomFolderComboBox.IsEnabled = true;
                SettingsCustomFolderComboBox.IsEnabled = true;
                SettingsCustomFolderContainer.Visibility = Visibility.Visible;
            }
            else
            {
                PackBspCustomFolderComboBox.IsEnabled = false;
                SettingsCustomFolderComboBox.IsEnabled = false;
                SettingsCustomFolderContainer.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region Handlers MultiCustom Folder GUI

        /// <summary>
        /// Init the datacontext of the multi custom folder combobox and set the selection to a default value
        /// </summary>
        private void InitMultiCustomFoldersComboBox()
        {
            MultiPackBspCustomFolderComboBox.DataContext = toolSettings.MultiMapsConfigs;
            SettingsMultiCustomFolderComboBox.DataContext = toolSettings.MultiMapsConfigs;

            MultiMapConfig map = toolSettings.FindMultiMapConfig(toolSettings.LastMultiCustomDirectory);
            if (map != null)
            {
                MultiCustomFolderSetSelected(map);
            }
            else
            {
                MultiCustomFoldersForceSelect();
            }
        }

        /// <summary>
        /// Set the selected map config of the combobox to the given map
        /// </summary>
        /// <param name="map"></param>
        private void MultiCustomFolderSetSelected(MultiMapConfig map)
        {
            MultiPackBspCustomFolderComboBox.SelectedItem = map;
            SettingsMultiCustomFolderComboBox.SelectedItem = map;
        }

        /// <summary>
        /// Force the multi custom folder combobox to the first element if possible
        /// </summary>
        private void MultiCustomFoldersForceSelect()
        {
            if (toolSettings.MultiMapsConfigs.Count > 0)
            {
                MultiCustomFolderSetSelected(toolSettings.MultiMapsConfigs[0]);
            }
            else
            {
                MultiCustomFoldersListUpdateGUI();
            }
        }

        /// <summary>
        /// Update the visibility of the MultiCustom folder combobox and settings based on how many 'mapconfigs' are available
        /// </summary>
        private void MultiCustomFoldersListUpdateGUI()
        {
            if (toolSettings.MultiMapsConfigs.Count > 0)
            {
                MultiPackBspCustomFolderComboBox.IsEnabled = true;
                SettingsMultiCustomFolderComboBox.IsEnabled = true;
                SettingsMultiCustomFolderContainer.Visibility = Visibility.Visible;
            }
            else
            {
                MultiPackBspCustomFolderComboBox.IsEnabled = false;
                SettingsMultiCustomFolderComboBox.IsEnabled = false;
                SettingsMultiCustomFolderContainer.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region Handlers Whitelist GUI

        /// <summary>
        /// Init the datacontext of the whitelist directory combobox and set the selection to a default value
        /// </summary>
        private void InitDirectoryRestrictionsComboBox()
        {
            SettingsWhitelistComboBox.DataContext = toolSettings.DirectoriesRestrictions;
            DirectoryRestrictionsForceSelect();
        }

        /// <summary>
        /// Set the selected whitelisted directory of the combobox
        /// </summary>
        /// <param name="map"></param>
        private void DirectoryRestrictionsSetSelected(DirectoryRestrictions restrictions)
        {
            SettingsWhitelistComboBox.SelectedItem = restrictions;
        }

        /// <summary>
        /// Force the whitelist folder combobox to the first element if possible
        /// </summary>
        private void DirectoryRestrictionsForceSelect()
        {
            if (toolSettings.DirectoriesRestrictions.Count > 0)
            {
                DirectoryRestrictionsSetSelected(toolSettings.DirectoriesRestrictions[0]);
            }
            else
            {
                DirectoryRestrictionsListUpdateGUI();
            }
        }

        /// <summary>
        /// Update the visibility of the whitelist combobox and settings based on how many folders are defined
        /// </summary>
        private void DirectoryRestrictionsListUpdateGUI()
        {
            if (toolSettings.DirectoriesRestrictions.Count > 0)
            {
                SettingsWhitelistComboBox.IsEnabled = true;
                SettingsWhitelistContainer.Visibility = Visibility.Visible;
            }
            else
            {
                SettingsWhitelistComboBox.IsEnabled = false;
                SettingsWhitelistContainer.Visibility = Visibility.Hidden;
            }
        }

        #endregion

        #region Events Drag and Drop

        /// <summary>
        /// Extract the list of paths that were drag and dropped
        /// </summary>
        /// <param name="e">Event data</param>
        /// <returns>The list of paths, null if impossible to get the list</returns>
        private string[] DragDropGetPaths(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                return e.Data.GetData(DataFormats.FileDrop, true) as string[];
            }
            return null;
        }

        /// <summary>
        /// Extract the path of the first dragged file matching the mode
        /// </summary>
        /// <param name="e">Event data</param>
        /// <param name="mode">Mode</param>
        /// <returns>The path of a matching file/directory found, null otherwise</returns>
        private string DragDropGetPath(DragEventArgs e, DragDropMode mode)
        {
            string filePath = null;
            string[] droppedFilePaths = DragDropGetPaths(e);
            if (droppedFilePaths != null)
            {
                switch (mode)
                {
                    case DragDropMode.Any:
                        // Get the first dragged file
                        if (droppedFilePaths.Length > 0)
                        {
                            filePath = droppedFilePaths[0];
                        }
                        break;
                    case DragDropMode.Bsp:
                        foreach (string path in droppedFilePaths)
                        {
                            // Get the first BSP dragged if multiple files are dragged
                            if (FileUtils.IsExtension(path, Constants.ExtensionBsp))
                            {
                                filePath = path;
                                break;
                            }
                        }
                        break;
                    case DragDropMode.BspZipExe:
                        foreach (string path in droppedFilePaths)
                        {
                            // Get the first bspzip.exe dragged if multiple files are dragged
                            if (FileUtils.IsFileName(path, Constants.BspZipFile))
                            {
                                filePath = path;
                                break;
                            }
                            // Otherwise we get any exe file that was dragged
                            if (FileUtils.IsExtension(path, Constants.ExtensionExe))
                            {
                                filePath = path;
                            }
                        }
                        break;
                    case DragDropMode.GameinfoTxt:
                        foreach (string path in droppedFilePaths)
                        {
                            // Get the first gameinfo.txt dragged if multiple files are dragged
                            if (FileUtils.IsFileName(path, Constants.GameinfoFile))
                            {
                                filePath = path;
                                break;
                            }
                            // Otherwise we get any txt file that was dragged
                            if (FileUtils.IsExtension(path, Constants.ExtensionTxt))
                            {
                                filePath = path;
                            }
                        }
                        break;
                    case DragDropMode.Directory:
                        foreach (string path in droppedFilePaths)
                        {
                            // Get the directory path of the dragged files
                            if (System.IO.Directory.Exists(path))
                            {
                                filePath = path;
                                break;
                            }
                            else if (System.IO.File.Exists(path))
                            {
                                filePath = FileUtils.TryGetDirectoryName(path);
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return filePath;
        }

        /// <summary>
        /// Allow dragging on TextBox
        /// </summary>
        /// <param name="e"></param>
        private void DragOverTextBoxFix(object sender, DragEventArgs e)
        {
            e.Handled = true;
            e.Effects = DragDropEffects.Move;
        }

        #endregion

        #region Events Generic GUI

        /// <summary>
        /// Lock all tabs but the 'Logs' one
        /// </summary>
        private void LockNonLogsTabs()
        {
            BspPackerTab.IsEnabled = false;
            MultiPackerBspTab.IsEnabled = false;
            RepackBspTab.IsEnabled = false;
            SettingsTab.IsEnabled = false;
            ExtractBspTab.IsEnabled = false;
            CubemapsBspTab.IsEnabled = false;
        }

        /// <summary>
        /// Unlock all non 'Logs' tabs
        /// </summary>
        private void UnlockNonLogsTabs()
        {
            BspPackerTab.IsEnabled = true;
            MultiPackerBspTab.IsEnabled = true;
            RepackBspTab.IsEnabled = true;
            SettingsTab.IsEnabled = true;
            ExtractBspTab.IsEnabled = true;
            CubemapsBspTab.IsEnabled = true;
        }
        
        private void GameLinkButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = SettingsTab;
            SettingsTabControl.SelectedItem = SettingsGamesTab;
        }
        
        private void CustomFolderLinkButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = SettingsTab;
            SettingsTabControl.SelectedItem = SettingsCustomFoldersTab;
        }

        private void MultiCustomFolderLinkButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = SettingsTab;
            SettingsTabControl.SelectedItem = SettingsMultiCustomFoldersTab;
        }

        #endregion

        #region Events Generic Bspzip

        /// <summary>
        /// Execute Bspzip for a given mode (Pack / Repack / Extract / Cubemaps)
        /// </summary>
        /// <param name="bspzip">A variable that inherit from Bspzip class</param>
        private async void ExecuteBspzip(Bspzip bspzip)
        {
            // Change the current tab and lock all others
            MainTabControl.SelectedItem = LogsTab;
            LockNonLogsTabs();

            MessageBoxImage messageIcon = MessageBoxImage.None;
            string messageText = string.Empty;
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            try
            {
                await ExecuteTaskBspzip(bspzip);
                sw.Stop();
                messageIcon = MessageBoxImage.Information;
                messageText = MessageConstants.MessageBspzipSuccess + MessageConstants.MessageSeeLogs;
            }
            catch (FilePackCreationException ex)
            {
                // List of files to pack Creation Error
                sw.Stop();
                logText.AppendLine(ex.GetMessageAndInner());
                logText.AppendLine();
                messageIcon = MessageBoxImage.Error;
                messageText = MessageConstants.MessageListFilesFail + MessageConstants.MessageSeeLogs;
            }
            catch (BspBackupCreationException ex)
            {
                // BSP Backup Creation Error
                sw.Stop();
                logText.AppendLine(ex.GetMessageAndInner());
                logText.AppendLine();
                messageIcon = MessageBoxImage.Error;
                messageText = MessageConstants.MessageCopyBspFail + MessageConstants.MessageSeeLogs;
            }
            catch (BspZipExecutionException ex)
            {
                // bspzip.exe Execution Error
                sw.Stop();
                logText.AppendLine(ex.GetMessageAndInner());
                logText.AppendLine();
                messageIcon = MessageBoxImage.Error;
                messageText = MessageConstants.MessageBspzipFail + MessageConstants.MessageSeeLogs;
            }
            catch (MaxPathSizeLimitException ex)
            {
                // MAX_PATH limit encountered
                sw.Stop();
                logText.AppendLine(ex.GetMessageAndInner());
                logText.AppendLine();
                messageIcon = MessageBoxImage.Warning;
                messageText = MessageConstants.MessageBspzipPackingWarning + MessageConstants.MessageSeeLogs;
            }
            finally
            {
                logText.AppendLine($"Time elapsed: {sw.ElapsedMilliseconds} ms");
                MessageBox.Show(messageText, MessageTitle, MessageBoxButton.OK, messageIcon);
                LogsTextBox.ScrollToEnd();
            }

            // Unlock the tabs
            UnlockNonLogsTabs();
        }

        /// <summary>
        /// Execute an asynchronous task of Bspzip for a given mode (Pack / Repack / Extract / Cubemaps)
        /// </summary>
        /// <param name="bspzip">A variable that inherit from Bspzip class</param>
        /// <returns></returns>
        /// <exception cref="FilePackCreationException">Error creating the list of files to pack</exception>
        /// <exception cref="BspBackupCreationException">Error creating a backup of the bsp file</exception>
        /// <exception cref="BspZipExecutionException">Error during bspzip.exe process</exception>
        /// <exception cref="MaxPathSizeLimitException">Warning when one or multiples paths are longer than <see cref="Constants.MAX_PATH"/></exception>
        private async Task ExecuteTaskBspzip(Bspzip bspzip)
        {
            try
            {
                await Task.Run(() =>
                {
                    bspzip.Start();
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Events Shared PackBsp & MultiPackBsp

        /// <summary>
        /// Shared function for both single folder pack and multi folder pack.<br/>
        /// Open files dialog and user prompt to confirm the packing.
        /// </summary>
        /// <param name="game">Game config to use</param>
        /// <param name="mapContent">Custom folder to pack, null if multi pack</param>
        /// <param name="multiMapContent">Custom folders to pack, null if single pack</param>
        /// <param name="bspPath">Path of the bsp to pack</param>
        /// <param name="useWhitelist">Use the whitelisted subdirectories</param>
        /// <param name="isOutputToNewBsp">Override the bsp to pack</param>
        private void PackBspMapConfigMultiMapConfig(GameConfig game, MapConfig mapContent,
            MultiMapConfig multiMapContent, string bspPath, bool useWhitelist, bool isOutputToNewBsp)
        {
            // Verify if it's a single pack or multi pack
            bool useMultiFolder;
            if (mapContent != null)
            {
                useMultiFolder = false;
            }
            else if (multiMapContent != null)
            {
                useMultiFolder = true;
            }
            else
            {
                // Logically impossible to go in here if we call this function properly
                MessageBox.Show(MessageConstants.MessageSimpleMultiCustomFolderNotSelected, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Default is to override the bsp with the packed bsp
            string outputBsp = bspPath;
            if (isOutputToNewBsp)
            {
                // Open Save Dialog to save the bsp to a new location
                outputBsp = FileUtils.SaveFileDialog(FileFilters.Bsp,
                    System.IO.Path.GetFileNameWithoutExtension(bspPath),
                    Constants.ExtensionBsp,
                    System.IO.Path.GetDirectoryName(bspPath));
            }

            if (outputBsp == null)
            {
                // If the Save Dialog was closed, we dont continue the packing
                return;
            }
            
            if (!useWhitelist)
            {
                // If not using whitelist, need the user to confirm their choice
                MessageBoxResult resultWhitelist = MessageBox.Show(MessageConstants.MessageWhitelistWarning,
                        MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                switch (resultWhitelist)
                {
                    case MessageBoxResult.Yes:
                        break;
                    case MessageBoxResult.No:
                        return;
                }
            }

            string mapName = System.IO.Path.GetFileName(bspPath);
            string message;
            if (isOutputToNewBsp)
            {
                string outputMapName = System.IO.Path.GetFileName(outputBsp);
                if (useMultiFolder)
                    message = $"Pack \"{mapName}\" into \"{outputMapName}\" using :\n{multiMapContent.PathAsDashedList}";
                else
                    message = $"Pack \"{mapName}\" into \"{outputMapName}\" using \"{mapContent.Path}\" ?";
            }
            else
            {
                if (useMultiFolder)
                    message = $"Pack \"{mapName}\" using :\n{multiMapContent.PathAsDashedList}";
                else
                    message = $"Pack \"{mapName}\" using \"{mapContent.Path}\" ?";
            }
            MessageBoxResult result = MessageBox.Show(message, MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    if (useMultiFolder)
                        ExecuteMultiPackBsp(game, bspPath, multiMapContent, outputBsp, useWhitelist);
                    else
                        ExecutePackBsp(game, bspPath, mapContent, outputBsp, useWhitelist);
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        #endregion

        #region Events PackBsp

        /// <summary>
        /// Enable or disable the Pack Bsp button based on which fields are currently valid
        /// </summary>
        private void UpdatePackBspGUI()
        {
            bool isGameSelected = PackBspGameComboBox.SelectedValue != null;
            bool isFolderSelected = PackBspCustomFolderComboBox.SelectedValue != null;
            bool isBspSelected = !string.IsNullOrEmpty(PackBspFileTextBox.Text);
            if (isGameSelected && isFolderSelected && isBspSelected)
            {
                PackBspButton.IsEnabled = true;
            }
            else
            {
                PackBspButton.IsEnabled = false;
            }
        }

        private void PackBspGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePackBspGUI();
        }

        private void PackBspCustomFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePackBspGUI();
            // Set the datacontext of the folder path textbox to the current selection (or null)    
            PackBspCustomFolderPath.DataContext = PackBspCustomFolderComboBox.SelectedItem;
        }

        private void PackBspFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePackBspGUI();
        }

        private void PackBspFileTextBox_Drop(object sender, DragEventArgs e)
        {
            string fileName = DragDropGetPath(e, DragDropMode.Bsp);
            if (fileName != null)
            {
                PackBspFileTextBox.Text = fileName;
            }
        }

        private void PackBspFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FileUtils.OpenFileDialog(FileFilters.Bsp);
            if (fileName != null)
            {
                PackBspFileTextBox.Text = fileName;
            }
        }

        private void PackBspButton_Click(object sender, RoutedEventArgs e)
        {
            GameConfig game = PackBspGameComboBox.SelectedItem as GameConfig;
            MapConfig mapContent = PackBspCustomFolderComboBox.SelectedItem as MapConfig;
            string bspPath = PackBspFileTextBox.Text;
            bool useWhitelist = (bool)PackBspWhitelistCheckbox.IsChecked;
            bool isOutputToNewBsp = (bool)PackBspOutputToNewBspCheckbox.IsChecked;

            if (game == null)
            {
                MessageBox.Show(MessageConstants.MessageGameInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!game.FilesExist())
            {
                MessageBox.Show(MessageConstants.MessageGameNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (mapContent == null)
            {
                MessageBox.Show(MessageConstants.MessageCustomFolderInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!mapContent.DirectoryExists())
            {
                MessageBox.Show(MessageConstants.MessageCustomFolderNotFound + " :\n- " + mapContent.Path, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!System.IO.File.Exists(bspPath))
            {
                MessageBox.Show(MessageConstants.MessageBspFileNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!FileUtils.IsExtension(bspPath, Constants.ExtensionBsp))
            {
                MessageBox.Show(MessageConstants.MessageFileNotBsp, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            PackBspMapConfigMultiMapConfig(game, mapContent, null, bspPath, useWhitelist, isOutputToNewBsp);
        }

        /// <summary>
        /// Execute BspZip to pack a bsp with the given parameters
        /// </summary>
        /// <param name="game">Game config to use</param>
        /// <param name="bspPath">Path of the bsp to pack</param>
        /// <param name="mapContent">Map config to pack</param>
        /// <param name="outputBsp">Path of the bsp to create after packing</param>
        /// <param name="useWhitelist"><c>true</c> to use the whitelist of directories, <c>false</c> to pack everything</param>
        private void ExecutePackBsp(GameConfig game, string bspPath, MapConfig mapContent, string outputBsp, bool useWhitelist)
        {
            Pack pack = new Pack(toolSettings, game, bspPath, logText, mapContent, outputBsp, useWhitelist);
            PackBspButton.IsEnabled = false;

            logText.Clear();
            logText.AppendLine($"Game: \"{game.Name}\"\nBSP: \"{bspPath}\"");
            logText.AppendLine($"Custom Folder: \"{mapContent.Path}\"\nOutput BSP: \"{outputBsp}\"");
            logText.AppendLine($"Use Directory Whitelist: {useWhitelist}\n");
            logText.AppendLine(MessageConstants.MessageBspPacking);

            ExecuteBspzip(pack);

            UpdatePackBspGUI();
        }

        #endregion

        #region Events MultiPackBsp

        /// <summary>
        /// Enable or disable the Multi Pack Bsp button based on which fields are currently valid
        /// </summary>
        private void UpdateMultiPackBspGUI()
        {
            bool isGameSelected = MultiPackBspGameComboBox.SelectedValue != null;
            bool isFolderSelected = MultiPackBspCustomFolderComboBox.SelectedValue != null;
            bool isBspSelected = !string.IsNullOrEmpty(MultiPackBspFileTextBox.Text);
            if (isGameSelected && isFolderSelected && isBspSelected)
            {
                MultiPackBspButton.IsEnabled = true;
            }
            else
            {
                MultiPackBspButton.IsEnabled = false;
            }
        }

        private void MultiPackGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMultiPackBspGUI();
        }

        private void MultiPackCustomFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMultiPackBspGUI();
            // Set the datacontext of the folder path textbox to the current selection (or null)    
            MultiPackBspCustomFolderPath.DataContext = MultiPackBspCustomFolderComboBox.SelectedItem;
        }

        private void MultiPackBspFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateMultiPackBspGUI();
        }

        private void MultiPackBspFileTextBox_Drop(object sender, DragEventArgs e)
        {
            string fileName = DragDropGetPath(e, DragDropMode.Bsp);
            if (fileName != null)
            {
                MultiPackBspFileTextBox.Text = fileName;
            }
        }

        private void MultiPackBspFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FileUtils.OpenFileDialog(FileFilters.Bsp);
            if (fileName != null)
            {
                MultiPackBspFileTextBox.Text = fileName;
            }
        }

        private void MultiPackBspButton_Click(object sender, RoutedEventArgs e)
        {
            GameConfig game = MultiPackBspGameComboBox.SelectedItem as GameConfig;
            MultiMapConfig multiMapContent = MultiPackBspCustomFolderComboBox.SelectedItem as MultiMapConfig;
            string bspPath = MultiPackBspFileTextBox.Text;
            bool useWhitelist = (bool)MultiPackBspWhitelistCheckbox.IsChecked;
            bool isOutputToNewBsp = (bool)MultiPackBspOutputToNewBspCheckbox.IsChecked;

            if (game == null)
            {
                MessageBox.Show(MessageConstants.MessageGameInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!game.FilesExist())
            {
                MessageBox.Show(MessageConstants.MessageGameNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (multiMapContent == null)
            {
                MessageBox.Show(MessageConstants.MessageCustomFolderInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!multiMapContent.IsNotEmpty())
            {
                MessageBox.Show(MessageConstants.MessageMultiCustomFolderEmpty, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!multiMapContent.DirectoriesExists())
            {
                string subMessage = string.Join("\n-\u00A0", multiMapContent.GetNonExistingDirectories());
                MessageBox.Show(MessageConstants.MessageMultiCustomFolderNotFound + ": \n-\u00A0" + subMessage, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!System.IO.File.Exists(bspPath))
            {
                MessageBox.Show(MessageConstants.MessageBspFileNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!FileUtils.IsExtension(bspPath, Constants.ExtensionBsp))
            {
                MessageBox.Show(MessageConstants.MessageFileNotBsp, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            PackBspMapConfigMultiMapConfig(game, null, multiMapContent, bspPath, useWhitelist, isOutputToNewBsp);
        }

        /// <summary>
        /// Execute BspZip to pack a bsp with the given parameters
        /// </summary>
        /// <param name="game">Game config to use</param>
        /// <param name="bspPath">Path of the bsp to pack</param>
        /// <param name="multiMapContent">Multi Map config to pack</param>
        /// <param name="outputBsp">Path of the bsp to create after packing</param>
        /// <param name="useWhitelist"><c>true</c> to use the whitelist of directories, <c>false</c> to pack everything</param>
        private void ExecuteMultiPackBsp(GameConfig game, string bspPath, MultiMapConfig multiMapContent, string outputBsp, bool useWhitelist)
        {
            Pack multiPack = new Pack(toolSettings, game, bspPath, logText, multiMapContent, outputBsp, useWhitelist);
            PackBspButton.IsEnabled = false;

            logText.Clear();
            logText.AppendLine($"Game: \"{game.Name}\"\nBSP: \"{bspPath}\"");
            logText.AppendLine($"Custom Folders:\n{multiMapContent.PathAsDashedList}\nOutput BSP: \"{outputBsp}\"");
            logText.AppendLine($"Use Directory Whitelist: {useWhitelist}\n");
            logText.AppendLine(MessageConstants.MessageBspPacking);

            ExecuteBspzip(multiPack);

            UpdatePackBspGUI();
        }

        #endregion

        #region Events RepackBsp

        /// <summary>
        /// Enable or disable the Repack Bsp button based on which fields are currently valid
        /// </summary>
        private void UpdateRepackBspGUI()
        {
            bool isGameSelected = RepackBspGameComboBox.SelectedValue != null;
            bool isBspSelected = !string.IsNullOrEmpty(RepackBspFileTextBox.Text);
            if (isGameSelected && isBspSelected)
            {
                RepackBspCompressButton.IsEnabled = true;
                RepackBspDecompressButton.IsEnabled = true;
            }
            else
            {
                RepackBspCompressButton.IsEnabled = false;
                RepackBspDecompressButton.IsEnabled = false;
            }
        }

        private void RepackGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateRepackBspGUI();
        }

        private void RepackBspFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateRepackBspGUI();
        }

        private void RepackBspFileTextBox_Drop(object sender, DragEventArgs e)
        {
            string fileName = DragDropGetPath(e, DragDropMode.Bsp);
            if (fileName != null)
            {
                RepackBspFileTextBox.Text = fileName;
            }
        }

        private void RepackBspFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FileUtils.OpenFileDialog(FileFilters.Bsp);
            if (fileName != null)
            {
                RepackBspFileTextBox.Text = fileName;
            }
        }

        private void RepackBspCompressButton_Click(object sender, RoutedEventArgs e)
        {
            RepackBspCompressDecompress(true);
        }

        private void RepackBspDecompressButton_Click(object sender, RoutedEventArgs e)
        {
            RepackBspCompressDecompress(false);
        }

        /// <summary>
        /// Shared function for Repack, to handle both compress and decompress
        /// </summary>
        /// <param name="isCompress"><c>true</c> if compress the map, <c>false</c> if decompress</param>
        private void RepackBspCompressDecompress(bool isCompress)
        {
            GameConfig game = RepackBspGameComboBox.SelectedItem as GameConfig;
            string bspPath = RepackBspFileTextBox.Text;
            if (game == null)
            {
                MessageBox.Show(MessageConstants.MessageGameInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!game.FilesExist())
            {
                MessageBox.Show(MessageConstants.MessageGameNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!System.IO.File.Exists(bspPath))
            {
                MessageBox.Show(MessageConstants.MessageBspFileNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!FileUtils.IsExtension(bspPath, Constants.ExtensionBsp))
            {
                MessageBox.Show(MessageConstants.MessageFileNotBsp, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string mapName = System.IO.Path.GetFileName(bspPath);
            string message = isCompress ? $"Compress \"{mapName}\" ?" : $"Decompress \"{mapName}\" ?";
            MessageBoxResult result = MessageBox.Show(message, MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    ExecuteRepackBsp(game, bspPath, isCompress);
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        /// Execute BspZip to repack a bsp with the given parameters
        /// </summary>
        /// <param name="game">Game config to use</param>
        /// <param name="bspPath">Path of the bsp to repack</param>
        /// <param name="isCompress"><c>true</c> to compress the map, <c>false</c> to decompress</param>
        private void ExecuteRepackBsp(GameConfig game, string bspPath, bool isCompress)
        {
            Repack repack = new Repack(toolSettings, game, bspPath, logText, isCompress);
            RepackBspCompressButton.IsEnabled = false;
            RepackBspDecompressButton.IsEnabled = false;

            logText.Clear();
            logText.AppendLine($"Game: \"{game.Name}\"\nBSP: \"{bspPath}\"");
            if (isCompress)
            {
                logText.AppendLine("Compress BSP: True\n");
                logText.AppendLine(MessageConstants.MessageBspRepackCompress);
            }
            else
            {
                logText.AppendLine("Decompress BSP: True\n");
                logText.AppendLine(MessageConstants.MessageBspRepackDecompress);
            }
            
            ExecuteBspzip(repack);

            UpdateRepackBspGUI();
        }

        #endregion

        #region Events Extract Files

        /// <summary>
        /// Enable or disable the Extract Bsp buttons based on which fields are currently valid
        /// </summary>
        private void UpdateExtractBspGUI()
        {
            bool isGameSelected = ExtractBspGameComboBox.SelectedValue != null;
            bool isBspSelected = !string.IsNullOrEmpty(ExtractBspFileTextBox.Text);
            if (isGameSelected && isBspSelected)
            {
                ExtractBspZipButton.IsEnabled = true;
                ExtractBspDirectoryButton.IsEnabled = !string.IsNullOrEmpty(ExtractBspFolderTextBox.Text);
            }
            else
            {
                ExtractBspZipButton.IsEnabled = false;
                ExtractBspDirectoryButton.IsEnabled = false;
            }
        }

        private void ExtractBspGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateExtractBspGUI();
        }

        private void ExtractBspFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateExtractBspGUI();
        }

        private void ExtractBspFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FileUtils.OpenFileDialog(FileFilters.Bsp);
            if (fileName != null)
            {
                ExtractBspFileTextBox.Text = fileName;
            }
        }

        private void ExtractBspFileTextBox_Drop(object sender, DragEventArgs e)
        {
            string fileName = DragDropGetPath(e, DragDropMode.Bsp);
            if (fileName != null)
            {
                ExtractBspFileTextBox.Text = fileName;
            }
        }

        private void ExtractBspFolderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateExtractBspGUI();
        }

        private void ExtractBspFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = FileUtils.OpenFolderDialog();
            if (folderName != null)
            {
                ExtractBspFolderTextBox.Text = folderName;
            }
        }

        private void ExtractBspFolderTextBox_Drop(object sender, DragEventArgs e)
        {
            string fileName = DragDropGetPath(e, DragDropMode.Directory);
            if (fileName != null)
            {
                ExtractBspFolderTextBox.Text = fileName;
            }
        }

        private void ExtractBspZipButton_Click(object sender, RoutedEventArgs e)
        {
            ExtractBspZipDirectory(true);
        }

        private void ExtractBspDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            ExtractBspZipDirectory(false);
        }

        /// <summary>
        /// Shared function for Extract, to handle both extract to directory and to zip file
        /// </summary>
        /// <param name="isExtractToZip"><c>true</c> if extract to zip, <c>false</c> to extract to directory</param>
        private void ExtractBspZipDirectory(bool isExtractToZip)
        {
            GameConfig game = ExtractBspGameComboBox.SelectedItem as GameConfig;
            string bspPath = ExtractBspFileTextBox.Text;
            string extractPath = ExtractBspFolderTextBox.Text;
            if (game == null)
            {
                MessageBox.Show(MessageConstants.MessageGameInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!game.FilesExist())
            {
                MessageBox.Show(MessageConstants.MessageGameNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!System.IO.File.Exists(bspPath))
            {
                MessageBox.Show(MessageConstants.MessageBspFileNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!FileUtils.IsExtension(bspPath, Constants.ExtensionBsp))
            {
                MessageBox.Show(MessageConstants.MessageFileNotBsp, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (isExtractToZip)
            {
                // Extracting packed content to a zip file

                // Open Save Menu
                extractPath = FileUtils.SaveFileDialog(FileFilters.Zip,
                            System.IO.Path.GetFileNameWithoutExtension(bspPath),
                            Constants.ExtensionZip,
                            System.IO.Path.GetDirectoryName(bspPath));
                if (extractPath == null)
                {
                    return;
                }
            }
            else
            {
                // Extracting packed content to a directory
                if (!System.IO.Directory.Exists(extractPath))
                {
                    MessageBox.Show(MessageConstants.MessageDirectoryNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            string mapName = System.IO.Path.GetFileName(bspPath);
            string message = $"Extract \"{mapName}\" content into \"{extractPath}\" ?";
            MessageBoxResult result = MessageBox.Show(message, MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    ExecuteExtractFiles(game, bspPath, isExtractToZip, extractPath);
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        /// Execute BspZip to extract content from a bsp, with the given parameters
        /// </summary>
        /// <param name="game">Game config to use</param>
        /// <param name="bspPath">Path of the bsp to use</param>
        /// <param name="isExtractToZip"><c>true</c> to extract to a zip file, <c>false</c> to extract to a directory</param>
        /// <param name="extractPath">If <see cref="isExtractToZip"/> is <c>true</c>, path to the zip file to create, <br/>
        /// otherwise path to the directory to extract to</param>
        private void ExecuteExtractFiles(GameConfig game, string bspPath, bool isExtractToZip, string extractPath)
        {
            Extract extract = new Extract(toolSettings, game, bspPath, logText, isExtractToZip, extractPath);
            ExtractBspZipButton.IsEnabled = false;
            ExtractBspDirectoryButton.IsEnabled = false;

            logText.Clear();
            logText.AppendLine($"Game: \"{game.Name}\"\nBSP: \"{bspPath}\"");
            if (isExtractToZip)
            {
                logText.AppendLine("Extract to .zip file: True");
            }
            else
            {
                logText.AppendLine("Extract to directory: True");
            }
            logText.AppendLine($"Extract Path: \"{extractPath}\"\n");
            logText.AppendLine(MessageConstants.MessageBspExtractFile);

            ExecuteBspzip(extract);

            UpdateExtractBspGUI();
        }

        #endregion

        #region Events Cubemaps

        /// <summary>
        /// Enable or disable the Cubemaps Bsp buttons based on which fields are currently valid
        /// </summary>
        private void UpdateCubemapsBspGUI()
        {
            bool isGameSelected = CubemapsBspGameComboBox.SelectedValue != null;
            bool isBspSelected = !string.IsNullOrEmpty(CubemapsBspFileTextBox.Text);
            if (isGameSelected && isBspSelected)
            {
                CubemapsBspDeleteButton.IsEnabled = true;
                CubemapsBspExtractButton.IsEnabled = !string.IsNullOrEmpty(CubemapsBspFolderTextBox.Text);
            }
            else
            {
                CubemapsBspDeleteButton.IsEnabled = false;
                CubemapsBspExtractButton.IsEnabled = false;
            }
        }

        private void CubemapsBspGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCubemapsBspGUI();
        }

        private void CubemapsBspFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCubemapsBspGUI();
        }

        private void CubemapsBspFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FileUtils.OpenFileDialog(FileFilters.Bsp);
            if (fileName != null)
            {
                CubemapsBspFileTextBox.Text = fileName;
            }
        }

        private void CubemapsBspFileTextBox_Drop(object sender, DragEventArgs e)
        {
            string fileName = DragDropGetPath(e, DragDropMode.Bsp);
            if (fileName != null)
            {
                CubemapsBspFileTextBox.Text = fileName;
            }
        }

        private void CubemapsBspFolderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateCubemapsBspGUI();
        }

        private void CubemapsBspFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = FileUtils.OpenFolderDialog();
            if (folderName != null)
            {
                CubemapsBspFolderTextBox.Text = folderName;
            }
        }

        private void CubemapsBspFolderTextBox_Drop(object sender, DragEventArgs e)
        {
            string fileName = DragDropGetPath(e, DragDropMode.Directory);
            if (fileName != null)
            {
                CubemapsBspFolderTextBox.Text = fileName;
            }
        }

        private void CubemapsBspExtractButton_Click(object sender, RoutedEventArgs e)
        {
            CubemapsBspExtractDelete(true);
        }

        private void CubemapsBspDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            CubemapsBspExtractDelete(false);
        }

        /// <summary>
        /// Shared function for Cubemaps, to handle both extract cubemaps and delete cubemaps
        /// </summary>
        /// <param name="isExtractCubemaps"><c>true</c> if extract cubemaps, <c>false</c> if delete cubemaps</param>
        private void CubemapsBspExtractDelete(bool isExtractCubemaps)
        {
            GameConfig game = CubemapsBspGameComboBox.SelectedItem as GameConfig;
            string bspPath = CubemapsBspFileTextBox.Text;
            string extractPath = CubemapsBspFolderTextBox.Text;
            if (game == null)
            {
                MessageBox.Show(MessageConstants.MessageGameInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!game.FilesExist())
            {
                MessageBox.Show(MessageConstants.MessageGameNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!System.IO.File.Exists(bspPath))
            {
                MessageBox.Show(MessageConstants.MessageBspFileNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!FileUtils.IsExtension(bspPath, Constants.ExtensionBsp))
            {
                MessageBox.Show(MessageConstants.MessageFileNotBsp, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string mapName = System.IO.Path.GetFileName(bspPath);
            string message;
            if (isExtractCubemaps)
            {
                // Extracting cubemaps to a directory
                if (!System.IO.Directory.Exists(extractPath))
                {
                    MessageBox.Show(MessageConstants.MessageDirectoryNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                message = $"Extract \"{mapName}\" cubemaps to \"{extractPath}\" ?";
            }
            else
            {
                // Delete cubemaps
                message = $"Delete \"{mapName}\" cubemaps ?\n/!\\ This will delete all VTFs of the map";
            }
            MessageBoxResult result = MessageBox.Show(message, MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    ExecuteCubemaps(game, bspPath, isExtractCubemaps, extractPath);
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }

        /// <summary>
        /// Execute BspZip to extract or delete cubemaps from a bsp with the given parameters
        /// </summary>
        /// <param name="game">Game config to use</param>
        /// <param name="bspPath">Path of the bsp to use</param>
        /// <param name="isExtractCubemaps"><c>true</c> if extract cubemaps, <c>false</c> if delete cubemaps</param>
        /// <param name="extractPath">If <see cref="isExtractCubemaps"/> is <c>true</c>, path to the directory to extract to, null otherwise</param>
        private void ExecuteCubemaps(GameConfig game, string bspPath, bool isExtractCubemaps, string extractPath = null)
        {
            Cubemaps cubemaps = new Cubemaps(toolSettings, game, bspPath, logText, isExtractCubemaps, extractPath);
            CubemapsBspDeleteButton.IsEnabled = false;
            CubemapsBspExtractButton.IsEnabled = false;

            logText.Clear();
            logText.AppendLine($"Game: \"{game.Name}\"\nBSP: \"{bspPath}\"");

            if (isExtractCubemaps)
            {
                logText.AppendLine("Extract Cubemaps: True\n");
                logText.AppendLine($"Extract Path: \"{extractPath}\"\n");
                logText.AppendLine(MessageConstants.MessageBspExtractCubemaps);
            }
            else
            {
                logText.AppendLine("Delete Cubemaps: True\n");
                logText.AppendLine(MessageConstants.MessageBspDeleteCubemaps);
            }

            ExecuteBspzip(cubemaps);

            UpdateCubemapsBspGUI();
        }

        #endregion

        #region Events Settings

        #region Events Settings - Save

        private void SettingsSaveButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxImage messageIcon = MessageBoxImage.None;
            string messageText = string.Empty;
            try
            {
                XmlUtils.SerializeSettings(toolSettings);
                messageIcon = MessageBoxImage.Information;
                messageText = MessageConstants.MessageSettingsSaveSuccess;
            }
            catch (SettingsSerializationException ex)
            {
                messageIcon = MessageBoxImage.Warning;
                messageText = ex.GetMessageAndInner();
            }
            finally
            {
                MessageBox.Show(messageText, MessageTitle, MessageBoxButton.OK, messageIcon);
            }
        }

        #endregion

        #region Events Settings - Games

        private void SettingsGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = SettingsGameComboBox.SelectedItem;
            SettingsGameName.DataContext = selected;
            SettingsGameBspzip.DataContext = selected;
            SettingsGameGameinfo.DataContext = selected;
        }

        private void SettingsGameBspzipBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FileUtils.OpenFileDialog(FileFilters.BspZipExe);
            if (fileName != null)
            {
                SettingsGameBspzip.Text = fileName;
                // Need to force updateSource because modifying this way doesn't do the 'LostFocus'
                SettingsGameBspzip.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private void SettingsGameBspzip_Drop(object sender, DragEventArgs e)
        {
            string fileName = DragDropGetPath(e, DragDropMode.BspZipExe);
            if (fileName != null)
            {
                SettingsGameBspzip.Text = fileName;
                // Need to force updateSource because modifying this way doesn't do the 'LostFocus'
                SettingsGameBspzip.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private void SettingsGameGameinfoBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FileUtils.OpenFileDialog(FileFilters.GameinfoTxt);
            if (fileName != null)
            {
                SettingsGameGameinfo.Text = fileName;
                // Need to force updateSource because modifying this way doesn't do the 'LostFocus'
                SettingsGameGameinfo.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
        
        private void SettingsGameGameinfo_Drop(object sender, DragEventArgs e)
        {
            string fileName = DragDropGetPath(e, DragDropMode.GameinfoTxt);
            if (fileName != null)
            {
                SettingsGameGameinfo.Text = fileName;
                // Need to force updateSource because modifying this way doesn't do the 'LostFocus'
                SettingsGameGameinfo.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }
        
        private void SettingsGameAddButton_Click(object sender, RoutedEventArgs e)
        {
            bool wasEmpty = toolSettings.GamesConfigs.Count == 0;
            GameConfig game = GameConfig.GetDefaultGameConfig();
            toolSettings.GamesConfigs.Add(game);
            SettingsGameComboBox.SelectedItem = game;
            if (wasEmpty)
            {
                // If there was no game before adding one, we force the selection on all other windows
                GameSetSelected(game);
                PackBspGameComboBox.SelectedItem = game;
            }
            GamesListUpdateGUI();
        }

        private void SettingsGameDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            object selected = SettingsGameComboBox.SelectedItem;
            if (selected != null)
            {
                GameConfig game = selected as GameConfig;
                toolSettings.GamesConfigs.Remove(game);
                GamesForceSelect();
            }
        }

        #endregion

        #region Events Settings - Custom Folders

        private void SettingsCustomFolderAddButton_Click(object sender, RoutedEventArgs e)
        {
            bool wasEmpty = toolSettings.MapsConfigs.Count == 0;
            MapConfig map = MapConfig.GetDefaultMapConfig();
            toolSettings.MapsConfigs.Add(map);
            SettingsCustomFolderComboBox.SelectedItem = map;
            if (wasEmpty)
            {
                // If there was no custom folder before adding one, we force the selection on the pack bsp part
                PackBspCustomFolderComboBox.SelectedItem = map;
            }
            CustomFoldersListUpdateGUI();
        }

        private void SettingsCustomFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = SettingsCustomFolderComboBox.SelectedItem;
            SettingsCustomFolderName.DataContext = selected;
            SettingsCustomFolderPath.DataContext = selected;
        }

        private void SettingsCustomFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = FileUtils.OpenFolderDialog();
            if (folderName != null)
            {
                SettingsCustomFolderPath.Text = folderName;
                // Need to force updateSource because modifying this way doesn't do the 'LostFocus'
                SettingsCustomFolderPath.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private void SettingsCustomFolderPath_Drop(object sender, DragEventArgs e)
        {
            string folderName = DragDropGetPath(e, DragDropMode.Directory);
            if (folderName != null)
            {
                SettingsCustomFolderPath.Text = folderName;
                // Need to force updateSource because modifying this way doesn't do the 'LostFocus'
                SettingsCustomFolderPath.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        private void SettingsCustomFolderDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            object selected = SettingsCustomFolderComboBox.SelectedItem;
            if (selected != null)
            {
                MapConfig map = selected as MapConfig;
                toolSettings.MapsConfigs.Remove(map);
                CustomFoldersForceSelect();
            }
        }

        #endregion

        #region Events Settings - Whitelist

        private void SettingsWhitelistComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = SettingsWhitelistComboBox.SelectedItem;
            SettingsWhitelistDirName.DataContext = selected;
            SettingsWhitelistExtensions.DataContext = selected;
        }

        private void SettingsWhitelistAddButton_Click(object sender, RoutedEventArgs e)
        {
            DirectoryRestrictions restrictions = DirectoryRestrictions.GetDefaultDirectoryRestrictions();
            toolSettings.DirectoriesRestrictions.Add(restrictions);
            SettingsWhitelistComboBox.SelectedItem = restrictions;
            DirectoryRestrictionsListUpdateGUI();
        }

        private void SettingsWhitelistDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            object selected = SettingsWhitelistComboBox.SelectedItem;
            if (selected != null)
            {
                DirectoryRestrictions restrictions = (DirectoryRestrictions)selected;
                toolSettings.DirectoriesRestrictions.Remove(restrictions);
                DirectoryRestrictionsForceSelect();
            }
        }

        #endregion

        #region Events Settings - Multi Custom Folders

        private void SettingsMultiCustomFolderAddButton_Click(object sender, RoutedEventArgs e)
        {

            bool wasEmpty = toolSettings.MultiMapsConfigs.Count == 0;
            MultiMapConfig map = MultiMapConfig.GetDefaultMultiMapConfig();
            toolSettings.MultiMapsConfigs.Add(map);
            SettingsMultiCustomFolderComboBox.SelectedItem = map;
            if (wasEmpty)
            {
                // If there was no multi custom folder before adding one, we force the selection on the pack bsp part
                MultiPackBspCustomFolderComboBox.SelectedItem = map;
            }
            MultiCustomFoldersListUpdateGUI();
        }

        private void SettingsMultiCustomFolderDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            object selected = SettingsMultiCustomFolderComboBox.SelectedItem;
            if (selected != null)
            {
                MultiMapConfig multiMap = selected as MultiMapConfig;
                toolSettings.MultiMapsConfigs.Remove(multiMap);
                MultiCustomFoldersForceSelect();
            }
        }

        private void SettingsMultiCustomFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object selected = SettingsMultiCustomFolderComboBox.SelectedItem;
            SettingsMultiCustomFolderName.DataContext = selected;
            SettingsMultiCustomFolderBox.DataContext = selected;
        }

        private void SettingsMultiCustomFolderDrag_Drop(object sender, DragEventArgs e)
        {
            string folderName = DragDropGetPath(e, DragDropMode.Directory);
            if (folderName != null)
            {
                SettingsMultiCustomFolderAddDirectories(folderName);
            }
        }

        private void SettingsMultiCustomFolderAddDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = FileUtils.OpenFolderDialog();
            if (folderName != null)
            {
                SettingsMultiCustomFolderAddDirectories(folderName);
            }
        }

        /// <summary>
        /// Add a directory to the current multi folder config
        /// </summary>
        /// <param name="folderName"></param>
        private void SettingsMultiCustomFolderAddDirectories(string folderName)
        {
            if (SettingsMultiCustomFolderComboBox.SelectedItem != null)
            {
                object selected = SettingsMultiCustomFolderComboBox.SelectedItem;
                if (selected != null)
                {
                    MultiMapConfig multiMap = selected as MultiMapConfig;
                    HashSet<string> hashSetDirectories = multiMap.GetHashSetFromList();
                    hashSetDirectories.Add(folderName);
                    multiMap.SetHashSetToList(hashSetDirectories);
                    multiMap.NotifyDirectoryListUpdate();
                }
            }
        }

        private void SettingsMultiCustomFolderBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If at least 1 folder is selected, unlock the Remove button
            SettingsMultiCustomFolderRemoveDirectoryButton.IsEnabled = SettingsMultiCustomFolderBox.SelectedItems.Count > 0;
        }

        private void SettingsMultiCustomFolderRemoveDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingsMultiCustomFolderBox.SelectedItems.Count == 0)
            {
                MessageBox.Show(MessageConstants.MessageMultiCustomFolderSettingsNotSelected, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            object selected = SettingsMultiCustomFolderComboBox.SelectedItem;
            if (selected != null)
            {
                MultiMapConfig multiMap = selected as MultiMapConfig;
                HashSet<string> selectedDirectories = SettingsMultiCustomFolderGetSelectedDirectories();
                for (int i = multiMap.ListPath.Count - 1; i >= 0; i--)
                {
                    if (selectedDirectories.Contains(multiMap.ListPath[i]))
                    {
                        multiMap.ListPath.RemoveAt(i);
                    }
                }
                multiMap.NotifyDirectoryListUpdate();
            }
        }

        /// <summary>
        /// Get the list of selected directories in <see cref="SettingsMultiCustomFolderBox"/>
        /// </summary>
        /// <returns>HashSet of string</returns>
        private HashSet<string> SettingsMultiCustomFolderGetSelectedDirectories()
        {
            HashSet<string> selectedDirectories = new HashSet<string>();
            foreach (object selected in SettingsMultiCustomFolderBox.SelectedItems)
            {
                if (selected is string directory)
                {
                    selectedDirectories.Add(directory);
                }
            }
            return selectedDirectories;
        }

        #endregion

        #endregion

    }

    /// <summary>
    /// Enum to handle drag and dropping of different type of files
    /// </summary>
    public enum DragDropMode
    {
        Any,
        Bsp,
        BspZipExe,
        GameinfoTxt,
        Directory
    }
}
