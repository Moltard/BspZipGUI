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

namespace BspZipGUI
{
   
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        #region Attributes

        private StringHolder stringHolderLog = new StringHolder();
        private Settings toolSettings;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();
           
            InitSettings();

            InitLogs();
        }

        #endregion

        #region Initialize GUI

        private void InitLogs()
        {
            LogsTextBox.DataContext = stringHolderLog;
        }

        private void InitSettings()
        {
            toolSettings = XmlUtils.GetSettings();
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

                // Init the whitelist combobox
                InitDirectoryRestrictionsComboBox();

            }
        }

        #endregion

        #region Handlers Games GUI

        /// <summary>
        /// Init the datacontext of the game combobox and set the selection to a default value
        /// </summary>
        private void InitGamesComboBox()
        {
            GameComboBox.DataContext = toolSettings.GamesConfigs;
            RepackGameComboBox.DataContext = toolSettings.GamesConfigs;
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
        /// Set the selected game config of the combobox to the given game
        /// </summary>
        /// <param name="map"></param>
        private void GameSetSelected(GameConfig game)
        {
            GameComboBox.SelectedItem = game;
            RepackGameComboBox.SelectedItem = game;
            SettingsGameComboBox.SelectedItem = game;
        }

        /// <summary>
        /// Force the game combobox to the first element if possible
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
                        foreach (var path in droppedFilePaths)
                        {
                            // Get the first BSP dragged if multiple files are dragged
                            if (FilesUtils.IsExtension(path, Constants.ExtensionBsp))
                            {
                                filePath = path;
                                break;
                            }
                        }
                        break;
                    case DragDropMode.BspZipExe:
                        foreach (var path in droppedFilePaths)
                        {
                            // Get the first bspzip.exe dragged if multiple files are dragged
                            if (FilesUtils.IsFileName(path, Constants.BspZipFile))
                            {
                                filePath = path;
                                break;
                            }
                            // Otherwise we get any exe file that was dragged
                            if (FilesUtils.IsExtension(path, Constants.ExtensionExe))
                            {
                                filePath = path;
                            }

                        }
                        break;
                    case DragDropMode.GameinfoTxt:
                        foreach (var path in droppedFilePaths)
                        {
                            // Get the first gameinfo.txt dragged if multiple files are dragged
                            if (FilesUtils.IsFileName(path, Constants.GameinfoFile))
                            {
                                filePath = path;
                                break;
                            }
                            // Otherwise we get any txt file that was dragged
                            if (FilesUtils.IsExtension(path, Constants.ExtensionTxt))
                            {
                                filePath = path;
                            }

                        }
                        break;
                    case DragDropMode.Folder:
                        foreach (var path in droppedFilePaths)
                        {
                            // Get the directory path of the dragged files
                            if (System.IO.Directory.Exists(path))
                            {
                                filePath = path;
                                break;
                            }
                            else if (System.IO.File.Exists(path))
                            {
                                filePath = FilesUtils.GetDirectoryName(path);
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
            BspRepackTab.IsEnabled = false;
            SettingsTab.IsEnabled = false;
        }

        private void UnlockNonLogTabs()
        {
            BspPackerTab.IsEnabled = true;
            BspRepackTab.IsEnabled = true;
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
            string fileName = FilesUtils.OpenFileDialog(FileFilters.Bsp);
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
                                if (FilesUtils.IsExtension(bspPath, Constants.ExtensionBsp))
                                {
                                    bool continueProcess = true;
                                    if (!restrictions)
                                    {
                                        MessageBoxResult result = MessageBox.Show(MessageConstants.MessageWarningWhitelist, 
                                            MessageConstants.MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);
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
                                        string message = string.Format("Pack {0} using {1} ?", mapName, mapContent.Path);
                                        MessageBoxResult result = MessageBox.Show(message, MessageConstants.MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                                        switch (result)
                                        {
                                            case MessageBoxResult.Yes:
                                                ExecutePackBsp(game, mapContent, bspPath, restrictions);
                                                break;
                                            case MessageBoxResult.No:
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(MessageConstants.MessageFileNotBsp, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                            }
                            else
                            {
                                MessageBox.Show(MessageConstants.MessageFileNotFound, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show(MessageConstants.MessageCustomFolderNotFound, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageConstants.MessageCustomFolderInvalid, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(MessageConstants.MessageGameNotFound, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show(MessageConstants.MessageGameInvalid, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        private async void ExecutePackBsp(GameConfig game, MapConfig mapContent, string bspPath, bool hasRestrictions)
        {
            Pack pack = new Pack(toolSettings, game, bspPath, mapContent, hasRestrictions);

            // Change the current tab and lock all others
            MainTabControl.SelectedItem = LogsTab;
            LockNonLogTabs();
            PackBspButton.IsEnabled = false;

            stringHolderLog.Text = MessageConstants.MessageBspPacking;

            Task<bool> task = pack.Start();
            bool success = await task;

            stringHolderLog.Text = pack.GetLogsOutput();

            // Unlock the tabs
            UnlockNonLogTabs();
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
                RepackCompressBspButton.IsEnabled = true;
                RepackDecompressBspButton.IsEnabled = true;
            }
            else
            {
                RepackCompressBspButton.IsEnabled = false;
                RepackDecompressBspButton.IsEnabled = false;
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
            string fileName = FilesUtils.OpenFileDialog(FileFilters.Bsp);
            if (fileName != null)
            {
                RepackBspFileTextBox.Text = fileName;
            }
        }
        
        private void RepackCompressBspButton_Click(object sender, RoutedEventArgs e)
        {
            RepackCompressDecompressBsp(true);
        }

        private void RepackDecompressBspButton_Click(object sender, RoutedEventArgs e)
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
                        if (FilesUtils.IsExtension(bspPath, Constants.ExtensionBsp))
                        {
                            string mapName = System.IO.Path.GetFileName(bspPath);
                            string message = isCompress ? string.Format("Compress {0} ?", mapName) : string.Format("Decompress {0} ?", mapName);
                            MessageBoxResult result = MessageBox.Show(message, MessageConstants.MessageTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                            MessageBox.Show(MessageConstants.MessageFileNotBsp, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show(MessageConstants.MessageFileNotFound, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show(MessageConstants.MessageGameNotFound, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show(MessageConstants.MessageGameInvalid, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }

        private async void ExecuteRepackBsp(GameConfig game, string bspPath, bool isCompress)
        {
            Repack repack = new Repack(toolSettings, game, bspPath, isCompress);

            // Change the current tab and lock all others
            MainTabControl.SelectedItem = LogsTab;
            LockNonLogTabs();
            RepackCompressBspButton.IsEnabled = false;
            RepackDecompressBspButton.IsEnabled = false;

            stringHolderLog.Text = isCompress ? MessageConstants.MessageBspRepackCompress : MessageConstants.MessageBspRepackDecompress;
            
            Task<bool> task = repack.Start();
            bool success = await task;

            stringHolderLog.Text = repack.GetLogsOutput();

            // Unlock the tabs
            UnlockNonLogTabs();
            UpdateRepackBspGUI();
        }


        #endregion

        #region Events Settings

        #region Events Settings - Save

        private void SettingsSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (toolSettings.SaveSettings())
            {
                MessageBox.Show(MessageConstants.MessageSaveSettingsOK, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(MessageConstants.MessageSaveSettingsFail, MessageConstants.MessageTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region Events Settings - Games

        private void SettingsGameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = SettingsGameComboBox.SelectedItem;
            SettingsGameName.DataContext = selected;
            SettingsGameBspzip.DataContext = selected;
            SettingsGameGameinfo.DataContext = selected;
        }
        

        private void SettingsGameBspzipBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string fileName = FilesUtils.OpenFileDialog(FileFilters.BspZipExe);
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
            string fileName = FilesUtils.OpenFileDialog(FileFilters.GameinfoTxt);
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
                // If there was no game before adding one, we force the selection on the pack bsp part
                GameComboBox.SelectedItem = game;
            }
            GamesListUpdateGUI();
        }

        private void SettingsGameDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = SettingsGameComboBox.SelectedItem;
            if (selected != null)
            {
                var game = (GameConfig)selected;
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
            var selected = SettingsCustomFolderComboBox.SelectedItem;
            SettingsCustomFolderName.DataContext = selected;
            SettingsCustomFolderPath.DataContext = selected;
        }

        private void SettingsCustomFolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string folderName = FilesUtils.OpenFolderDialog();
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
            var selected = SettingsCustomFolderComboBox.SelectedItem;
            if (selected != null)
            {
                var map = (MapConfig)selected;
                toolSettings.MapsConfigs.Remove(map);
                CustomFoldersForceSelect();
            }
        }

        #endregion

        #region Events Settings - Whitelist

        private void SettingsWhitelistComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = SettingsWhitelistComboBox.SelectedItem;
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
            var selected = SettingsWhitelistComboBox.SelectedItem;
            if (selected != null)
            {
                var restrictions = (DirectoryRestrictions)selected;
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
