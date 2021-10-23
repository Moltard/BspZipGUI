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

                // Set the last BSP loaded (if possible)
                BspFileTextBox.Text = toolSettings.LastBsp;
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
            GameComboBox.DataContext = toolSettings.GamesConfigs;
            RepackGameComboBox.DataContext = toolSettings.GamesConfigs;
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
        /// <param name="game"></param>
        private void GameSetSelected(GameConfig game)
        {
            GameComboBox.SelectedItem = game;
            RepackGameComboBox.SelectedItem = game;
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
                GameComboBox.IsEnabled = true;
                RepackGameComboBox.IsEnabled = true;
                SettingsGameComboBox.IsEnabled = true;
                SettingsGamesContainer.Visibility = Visibility.Visible;
            }
            else
            {
                GameComboBox.IsEnabled = false;
                RepackGameComboBox.IsEnabled = false;
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
            CustomFolderComboBox.DataContext = toolSettings.MapsConfigs;
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
            CustomFolderComboBox.SelectedItem = map;
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
                CustomFolderComboBox.IsEnabled = true;
                SettingsCustomFolderComboBox.IsEnabled = true;
                SettingsCustomFolderContainer.Visibility = Visibility.Visible;
            }
            else
            {
                CustomFolderComboBox.IsEnabled = false;
                SettingsCustomFolderComboBox.IsEnabled = false;
                SettingsCustomFolderContainer.Visibility = Visibility.Hidden;
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
        /// <returns>The path is a matching file found, null otherwise</returns>
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
                    case DragDropMode.Folder:
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

        private void LockNonLogTabs()
        {
            BspPackerTab.IsEnabled = false;
            RepackBspTab.IsEnabled = false;
            SettingsTab.IsEnabled = false;
        }

        private void UnlockNonLogTabs()
        {
            BspPackerTab.IsEnabled = true;
            RepackBspTab.IsEnabled = true;
            SettingsTab.IsEnabled = true;
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

        #endregion

        #region Events Generic Bspzip

        /// <summary>
        /// Execute Bspzip with the given mode (Pack / Repack / Extract / Cubemaps)
        /// </summary>
        /// <param name="bspzip">A variable that inherit from Bspzip class</param>
        private async void ExecuteBspzip(Bspzip bspzip)
        {
            // Change the current tab and lock all others
            MainTabControl.SelectedItem = LogsTab;
            LockNonLogTabs();

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
                messageIcon = MessageBoxImage.Error;
                messageText = MessageConstants.MessageListFilesFail + MessageConstants.MessageSeeLogs;
            }
            catch (BspBackupCreationException ex)
            {
                // BSP Backup Creation Error
                sw.Stop();
                logText.AppendLine(ex.GetMessageAndInner());
                messageIcon = MessageBoxImage.Error;
                messageText = MessageConstants.MessageCopyBspFail + MessageConstants.MessageSeeLogs;
            }
            catch (BspZipExecutionException ex)
            {
                // bspzip.exe Execution Error
                sw.Stop();
                logText.AppendLine(ex.GetMessageAndInner());
                messageIcon = MessageBoxImage.Error;
                messageText = MessageConstants.MessageBspzipFail + MessageConstants.MessageSeeLogs;
            }
            finally
            {
                logText.AppendLine($"Time elapsed: {sw.ElapsedMilliseconds} ms");
                LogsTextBox.ScrollToEnd();
                MessageBox.Show(messageText, MessageTitle, MessageBoxButton.OK, messageIcon);
            }

            // Unlock the tabs
            UnlockNonLogTabs();
        }

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

        #region Events PackBsp

        /// <summary>
        /// Enable or disable the Pack Bsp button based on which fields are currently valid
        /// </summary>
        private void UpdatePackBspGUI()
        {
            bool isGameSelected = GameComboBox.SelectedValue != null;
            bool isFolderSelected = CustomFolderComboBox.SelectedValue != null;
            bool isBspSelected = !string.IsNullOrEmpty(BspFileTextBox.Text);
            if (isGameSelected && isFolderSelected && isBspSelected)
            {
                PackBspButton.IsEnabled = true;
            }
            else
            {
                PackBspButton.IsEnabled = false;
            }
        }

        private void GameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePackBspGUI();
        }

        private void CustomFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePackBspGUI();
            // Set the datacontext of the folder path textbox to the current selection (or null)    
            CustomFolderPath.DataContext = CustomFolderComboBox.SelectedItem;
        }

        private void BspFileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePackBspGUI();
        }

        private void BspFileTextBox_Drop(object sender, DragEventArgs e)
        {
            string fileName = DragDropGetPath(e, DragDropMode.Bsp);
            if (fileName != null)
            {
                BspFileTextBox.Text = fileName;
            }
        }

        private void BspFileBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FileUtils.OpenFileDialog(FileFilters.Bsp);
            if (fileName != null)
            {
                BspFileTextBox.Text = fileName;
            }
        }

        private void PackBspButton_Click(object sender, RoutedEventArgs e)
        {
            GameConfig game = GameComboBox.SelectedItem != null ? (GameConfig)GameComboBox.SelectedItem : null;
            MapConfig mapContent = CustomFolderComboBox.SelectedItem != null ? (MapConfig)CustomFolderComboBox.SelectedItem : null;
            string bspPath = BspFileTextBox.Text;
            bool restrictions = (bool)WhitelistCheckbox.IsChecked;
            bool overrideBsp = (bool)OverrideBSPCheckbox.IsChecked;

            if (game != null)
            {
                if (game.FilesExist())
                {
                    if (mapContent != null)
                    {
                        if (mapContent.DirectoryExists())
                        {
                            if (System.IO.File.Exists(bspPath))
                            {
                                if (FileUtils.IsExtension(bspPath, Constants.ExtensionBsp))
                                {
                                    string outputBsp = bspPath;
                                    if (overrideBsp)
                                    {
                                        outputBsp = FileUtils.SaveFileDialog(FileFilters.Bsp,
                                            System.IO.Path.GetFileNameWithoutExtension(bspPath),
                                            Constants.ExtensionBsp,
                                            System.IO.Path.GetDirectoryName(bspPath));
                                    }
                                    if (outputBsp != null)
                                    {
                                        bool continueProcess = true;
                                        if (!restrictions)
                                        {
                                            MessageBoxResult result = MessageBox.Show(MessageConstants.MessageWarningWhitelist,
                                                MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                            switch (result)
                                            {
                                                case MessageBoxResult.Yes:
                                                    break;
                                                case MessageBoxResult.No:
                                                    continueProcess = false;
                                                    break;
                                            }
                                        }

                                        if (continueProcess)
                                        {
                                            string mapName = System.IO.Path.GetFileName(bspPath);
                                            string message;
                                            if (overrideBsp)
                                            {
                                                string outputMapName = System.IO.Path.GetFileName(outputBsp);
                                                message = $"Pack \"{mapName}\" into \"{outputMapName}\" using \"{mapContent.Path}\" ?";
                                            }
                                            else
                                            {
                                                message = $"Pack \"{mapName}\" using \"{mapContent.Path}\" ?";
                                            }
                                            MessageBoxResult result = MessageBox.Show(message, MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                                            switch (result)
                                            {
                                                case MessageBoxResult.Yes:
                                                    ExecutePackBsp(game, bspPath, mapContent, outputBsp, restrictions);
                                                    break;
                                                case MessageBoxResult.No:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(MessageConstants.MessageFileNotBsp, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                            }
                            else
                            {
                                MessageBox.Show(MessageConstants.MessageFileNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show(MessageConstants.MessageCustomFolderNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageConstants.MessageCustomFolderInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(MessageConstants.MessageGameNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show(MessageConstants.MessageGameInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExecutePackBsp(GameConfig game, string bspPath, MapConfig mapContent, string outputBsp, bool hasRestrictions)
        {
            Pack pack = new Pack(toolSettings, game, bspPath, logText, mapContent, outputBsp, hasRestrictions);
            PackBspButton.IsEnabled = false;

            logText.Clear();
            logText.AppendLine($"Game: \"{game.Name}\"\nBSP: \"{bspPath}\"\nCustom Folder: \"{mapContent.Path}\"\nOutput BSP: \"{outputBsp}\"\nUse Directory Whitelist: {hasRestrictions}\n");
            logText.AppendLine(MessageConstants.MessageBspPacking);

            ExecuteBspzip(pack);

            UpdatePackBspGUI();
        }

        #endregion

        #region Events RepackBsp

        /// <summary>
        /// Enable or disable the Repack Bsp button based on which fields are currently valid
        /// </summary>
        private void UpdateRepackBspGUI()
        {
            bool isGameSelected = RepackGameComboBox.SelectedValue != null;
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
            RepackCompressDecompressBsp(true);
        }

        private void RepackBspDecompressButton_Click(object sender, RoutedEventArgs e)
        {
            RepackCompressDecompressBsp(false);
        }

        private void RepackCompressDecompressBsp(bool isCompress)
        {
            GameConfig game = RepackGameComboBox.SelectedItem != null ? (GameConfig)RepackGameComboBox.SelectedItem : null;
            string bspPath = RepackBspFileTextBox.Text;
            if (game != null)
            {
                if (game.FilesExist())
                {
                    if (System.IO.File.Exists(bspPath))
                    {
                        if (FileUtils.IsExtension(bspPath, Constants.ExtensionBsp))
                        {
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
                        else
                        {
                            MessageBox.Show(MessageConstants.MessageFileNotBsp, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageConstants.MessageFileNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(MessageConstants.MessageGameNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show(MessageConstants.MessageGameInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private void ExecuteRepackBsp(GameConfig game, string bspPath, bool isCompress)
        {
            Repack repack = new Repack(toolSettings, game, bspPath, logText, isCompress);
            RepackBspCompressButton.IsEnabled = false;
            RepackBspDecompressButton.IsEnabled = false;

            logText.Clear();
            logText.AppendLine($"Game: \"{game.Name}\"\nBSP: \"{bspPath}\"\nCompress BSP: {isCompress}\nDecompress BSP: {!isCompress}\n");
            logText.AppendLine(isCompress ? MessageConstants.MessageBspRepackCompress : MessageConstants.MessageBspRepackDecompress);
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
            string fileName = DragDropGetPath(e, DragDropMode.Folder);
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

        private void ExtractBspZipDirectory(bool isExtractToZip)
        {
            GameConfig game = ExtractBspGameComboBox.SelectedItem != null ? (GameConfig)ExtractBspGameComboBox.SelectedItem : null;
            string bspPath = ExtractBspFileTextBox.Text;
            string extractPath = ExtractBspFolderTextBox.Text;
            if (game != null)
            {
                if (game.FilesExist())
                {
                    if (System.IO.File.Exists(bspPath))
                    {
                        if (FileUtils.IsExtension(bspPath, Constants.ExtensionBsp))
                        {
                            if (isExtractToZip)
                            {
                                // Extracting to a zip file

                                // Save Menu
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
                                // Extracting to a folder
                                if (!System.IO.Directory.Exists(extractPath))
                                {
                                    MessageBox.Show(MessageConstants.MessageFolderNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
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
                        else
                        {
                            MessageBox.Show(MessageConstants.MessageFileNotBsp, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageConstants.MessageFileNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(MessageConstants.MessageGameNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show(MessageConstants.MessageGameInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExecuteExtractFiles(GameConfig game, string bspPath, bool isExtractToZip, string extractPath)
        {
            Extract extract = new Extract(toolSettings, game, bspPath, logText, isExtractToZip, extractPath);
            PackBspButton.IsEnabled = false;

            logText.Clear();
            logText.AppendLine($"Game: \"{game.Name}\"\nBSP: \"{bspPath}\"\nExtract to Zip file: {isExtractToZip}\nExtract to directory: {!isExtractToZip}\nExtract Path: \"{extractPath}\"\n");
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
            string fileName = DragDropGetPath(e, DragDropMode.Folder);
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

        private void CubemapsBspExtractDelete(bool isExtractCubemaps)
        {
            GameConfig game = CubemapsBspGameComboBox.SelectedItem != null ? (GameConfig)CubemapsBspGameComboBox.SelectedItem : null;
            string bspPath = CubemapsBspFileTextBox.Text;
            string extractPath = CubemapsBspFolderTextBox.Text;
            if (game != null)
            {
                if (game.FilesExist())
                {
                    if (System.IO.File.Exists(bspPath))
                    {
                        if (FileUtils.IsExtension(bspPath, Constants.ExtensionBsp))
                        {
                            string mapName = System.IO.Path.GetFileName(bspPath);
                            string message;
                            if (isExtractCubemaps)
                            {
                                // Extracting cubemaps to a folder
                                if (System.IO.Directory.Exists(extractPath))
                                {
                                    message = $"Extract \"{mapName}\" cubemaps to \"{extractPath}\" ?";
                                }
                                else
                                {
                                    MessageBox.Show(MessageConstants.MessageFolderNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                                    return;
                                }
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
                        else
                        {
                            MessageBox.Show(MessageConstants.MessageFileNotBsp, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageConstants.MessageFileNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(MessageConstants.MessageGameNotFound, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show(MessageConstants.MessageGameInvalid, MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExecuteCubemaps(GameConfig game, string bspPath, bool isExtractCubemaps, string extractPath = null)
        {
            Cubemaps cubemaps = new Cubemaps(toolSettings, game, bspPath, logText, isExtractCubemaps, extractPath);
            PackBspButton.IsEnabled = false;

            logText.Clear();
            logText.AppendLine($"Game: \"{game.Name}\"\nBSP: \"{bspPath}\"\nExtract Cubemaps: {isExtractCubemaps}\n" +
                "Extract Path: " + (isExtractCubemaps ? $"\"{extractPath}\"" : "N/A") + $"\nDelete Cubemaps: {!isExtractCubemaps}\n");
            logText.AppendLine(isExtractCubemaps ? MessageConstants.MessageBspExtractCubemaps : MessageConstants.MessageBspDeleteCubemaps);

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
                GameComboBox.SelectedItem = game;
            }
            GamesListUpdateGUI();
        }

        private void SettingsGameDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            object selected = SettingsGameComboBox.SelectedItem;
            if (selected != null)
            {
                GameConfig game = (GameConfig)selected;
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
                CustomFolderComboBox.SelectedItem = map;
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
            string folderName = DragDropGetPath(e, DragDropMode.Folder);
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
                MapConfig map = (MapConfig)selected;
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

        #endregion

    }

    public enum DragDropMode
    {
        Any,
        Bsp,
        BspZipExe,
        GameinfoTxt,
        Folder
    }
}
