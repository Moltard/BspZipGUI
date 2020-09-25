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
        private Settings ToolSettings;
        
        public StringHolder sh = new StringHolder();
        //public StringBuilderHolder sbh = new StringBuilderHolder();

        private const string MessageBoxTitle = "BspZipGUI";

        public MainWindow()
        {
            InitializeComponent();
           
            InitSettings();

            InitLogs();

        }

        //-------------------------------------
        //------------- Init GUI --------------
        //-------------------------------------

        private void InitLogs()
        {
            LogsTextBox.DataContext = sh;
            //LogsTextBox.DataContext = sbh;
        }

        private void InitSettings()
        {
            ToolSettings = XmlUtils.GetSettings();
            if (ToolSettings != null)
            {
                // Init all attributes that were null, and delete the game and mapconfigs that are invalid
                ToolSettings.InitAllAttributes();

                // Init the Games combobox and set the last game used (if possible)
                InitGamesComboBox();

                // Init the Custom Folders combobox and set the last folder used (if possible)
                InitCustomFoldersComboBox();

                // Set the last BSP loaded (if possible)
                BspFileTextBox.Text = ToolSettings.LastBsp;

                // Init the whitelist combobox
                InitDirectoryRestrictionsComboBox();

            }
        }

        // **************
        //     Games
        // **************

        /// <summary>
        /// Init the datacontext of the game combobox and set the selection to a default value
        /// </summary>
        private void InitGamesComboBox()
        {
            GameComboBox.DataContext = ToolSettings.GamesConfigs;
            SettingsGameComboBox.DataContext = ToolSettings.GamesConfigs;

            GameConfig game = ToolSettings.FindGameConfig(ToolSettings.LastGame);
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
            SettingsGameComboBox.SelectedItem = game;
        }

        /// <summary>
        /// Force the game combobox to the first element if possible
        /// </summary>
        private void GamesForceSelect()
        {
            if (ToolSettings.GamesConfigs.Count > 0)
            {
                GameSetSelected(ToolSettings.GamesConfigs[0]);
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
            if (ToolSettings.GamesConfigs.Count > 0)
            {
                GameComboBox.IsEnabled = true;
                SettingsGameComboBox.IsEnabled = true;
                SettingsGamesContainer.Visibility = Visibility.Visible;
            }
            else
            {
                GameComboBox.IsEnabled = false;
                SettingsGameComboBox.IsEnabled = false;
                SettingsGamesContainer.Visibility = Visibility.Hidden;
            }
        }

        // **************
        // Custom Folder
        // **************

        /// <summary>
        /// Init the datacontext of the custom folder combobox and set the selection to a default value
        /// </summary>
        private void InitCustomFoldersComboBox()
        {
            CustomFolderComboBox.DataContext = ToolSettings.MapsConfigs;
            SettingsCustomFolderComboBox.DataContext = ToolSettings.MapsConfigs;

            MapConfig map = ToolSettings.FindMapConfig(ToolSettings.LastCustomDirectory);
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
            if (ToolSettings.MapsConfigs.Count > 0)
            {
                CustomFolderSetSelected(ToolSettings.MapsConfigs[0]);
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
            if (ToolSettings.MapsConfigs.Count > 0)
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


        // **********
        // Whitelist
        // **********

        /// <summary>
        /// Init the datacontext of the whitelist directory combobox and set the selection to a default value
        /// </summary>
        private void InitDirectoryRestrictionsComboBox()
        {
            SettingsWhitelistComboBox.DataContext = ToolSettings.DirectoriesRestrictions;
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
            if (ToolSettings.DirectoriesRestrictions.Count > 0)
            {
                DirectoryRestrictionsSetSelected(ToolSettings.DirectoriesRestrictions[0]);
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
            if (ToolSettings.DirectoriesRestrictions.Count > 0)
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

        //-------------------------------------
        //------- Events Drag and Drop --------
        //-------------------------------------

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
                            if (FilesUtils.IsExtension(path, FilesUtils.ExtensionBsp))
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
                            if (FilesUtils.IsFileName(path, FilesUtils.BspZipFile))
                            {
                                filePath = path;
                                break;
                            }
                            // Otherwise we get any exe file that was dragged
                            if (FilesUtils.IsExtension(path, FilesUtils.ExtensionExe))
                            {
                                filePath = path;
                            }

                        }
                        break;
                    case DragDropMode.GameinfoTxt:
                        foreach (var path in droppedFilePaths)
                        {
                            // Get the first gameinfo.txt dragged if multiple files are dragged
                            if (FilesUtils.IsFileName(path, FilesUtils.GameinfoFile))
                            {
                                filePath = path;
                                break;
                            }
                            // Otherwise we get any txt file that was dragged
                            if (FilesUtils.IsExtension(path, FilesUtils.ExtensionTxt))
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

        //--------------------------------------
        //---------- Events PackBsp ------------
        //--------------------------------------

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

        private void GameLinkButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = SettingsTab;
            SettingsTabControl.SelectedItem = SettingsGamesTab;
        }

        private void CustomFolderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePackBspGUI();
            var selected = CustomFolderComboBox.SelectedItem;
            // Set the datacontext of the folder path textbox to the current selection (or null)    
            CustomFolderPath.DataContext = selected;
        }

        private void CustomFolderLinkButton_Click(object sender, RoutedEventArgs e)
        {
            MainTabControl.SelectedItem = SettingsTab;
            SettingsTabControl.SelectedItem = SettingsCustomFoldersTab;
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
            MapConfig map = CustomFolderComboBox.SelectedItem != null ? (MapConfig)CustomFolderComboBox.SelectedItem : null;
            string bspPath = BspFileTextBox.Text;
            bool restrictions = (bool)WhitelistCheckbox.IsChecked;

            if (game != null)
            {
                if (game.FilesExist())
                {
                    if (map != null)
                    {
                        if (map.DirectoryExists())
                        {
                            if (System.IO.File.Exists(bspPath))
                            {
                                if (FilesUtils.IsExtension(bspPath, FilesUtils.ExtensionBsp))
                                {
                                    bool continueProcess = true;
                                    if (!restrictions)
                                    {
                                        string message = string.Format("You unchecked \"Use Directory Whitelist\", it will pack every single files from the directory and subdirectories.\nAre you really sure ?\n(Be careful not to use a path like C:\\)", map.Path);
                                        MessageBoxResult result = MessageBox.Show(message, MessageBoxTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);
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
                                        string message = string.Format("Pack {0} using {1} ?", mapName, map.Path);
                                        MessageBoxResult result = MessageBox.Show(message, MessageBoxTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                                        switch (result)
                                        {
                                            case MessageBoxResult.Yes:
                                                ExecutePackBsp(game, map, bspPath, restrictions);
                                                break;
                                            case MessageBoxResult.No:
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("The file is not .bsp", MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                                }
                            }
                            else
                            {
                                MessageBox.Show("The file doesn't exist", MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                        else
                        {
                            MessageBox.Show("The Custom Folder doesn't exist", MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Custom Folder selected", MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Can't find the specified bspzip.exe and/or gameinfo.txt", MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Invalid Game selected", MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }

        private async void ExecutePackBsp(GameConfig game, MapConfig map, string bspPath, bool restrictions)
        {
            Pack pack = new Pack(ToolSettings, game, map, bspPath, restrictions);

            // Change the current tab and lock all others
            MainTabControl.SelectedItem = LogsTab;
            PackBspButton.IsEnabled = false;
            BspPackerTab.IsEnabled = false;
            SettingsTab.IsEnabled = false;

            sh.Text = "Packing in process...";

            Task<bool> task = pack.Start();
            bool success = await task;

            sh.Text = success ? pack.LogsOutput.ToString() : pack.LogsError.ToString();

            // Unlock the tabs
            BspPackerTab.IsEnabled = true;
            SettingsTab.IsEnabled = true;
            UpdatePackBspGUI();
        }

        //-------------------------------------
        //---------- Settings Events ----------
        //-------------------------------------

        private void SettingsSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ToolSettings.SaveSettings())
            {
                MessageBox.Show("Saved settings to settings.xml", MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Error trying to save settings", MessageBoxTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        // ******
        // Games
        // ******

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
            bool wasEmpty = ToolSettings.GamesConfigs.Count == 0;
            GameConfig game = GameConfig.GetDefaultGameConfig();
            ToolSettings.GamesConfigs.Add(game);
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
                ToolSettings.GamesConfigs.Remove(game);
                GamesForceSelect();
            }
        }


        // ***************
        // Custom Folders
        // ***************
        
        private void SettingsCustomFolderAddButton_Click(object sender, RoutedEventArgs e)
        {
            bool wasEmpty = ToolSettings.MapsConfigs.Count == 0;
            MapConfig map = MapConfig.GetDefaultMapConfig();
            ToolSettings.MapsConfigs.Add(map);
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
                ToolSettings.MapsConfigs.Remove(map);
                CustomFoldersForceSelect();
            }
        }

        // **********
        // Whitelist
        // **********

        private void SettingsWhitelistComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = SettingsWhitelistComboBox.SelectedItem;
            SettingsWhitelistDirName.DataContext = selected;
            SettingsWhitelistExtensions.DataContext = selected;
        }

        private void SettingsWhitelistAddButton_Click(object sender, RoutedEventArgs e)
        {
            DirectoryRestrictions restrictions = DirectoryRestrictions.GetDefaultDirectoryRestrictions();
            ToolSettings.DirectoriesRestrictions.Add(restrictions);
            SettingsWhitelistComboBox.SelectedItem = restrictions;
            DirectoryRestrictionsListUpdateGUI();
        }
        
        private void SettingsWhitelistDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var selected = SettingsWhitelistComboBox.SelectedItem;
            if (selected != null)
            {
                var restrictions = (DirectoryRestrictions)selected;
                ToolSettings.DirectoriesRestrictions.Remove(restrictions);
                DirectoryRestrictionsForceSelect();
            }
        }

        

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
