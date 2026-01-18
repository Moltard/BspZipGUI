using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BspZipGUI.Tool.Xml
{

    #region Class - Settings

    /// <summary>
    /// Store all settings of the user
    /// </summary>
    [Serializable]
    [XmlRoot("AppSettings")]
    public class ToolSettings
    {

        #region Attributes

        /// <summary>
        /// Last BSP loaded in the tool
        /// </summary>
        [XmlElement(ElementName = "LastBsp", Order = 1)]
        public string LastBsp { get; set; }

        /// <summary>
        /// Last game name loaded by the tool
        /// </summary>
        [XmlElement(ElementName = "LastGame", Order = 2)]
        public string LastGame { get; set; }

        /// <summary>
        /// Last custom files directory name loaded by the tool
        /// </summary>
        [XmlElement(ElementName = "LastCustomDirectory", Order = 3)]
        public string LastCustomDirectory { get; set; }

        /// <summary>
        /// Last directory path loaded by the tool for extractions
        /// </summary>
        [XmlElement(ElementName = "LastExtractDirectory", Order = 4)]
        public string LastExtractDirectory { get; set; }

        /// <summary>
        /// Is the bspzip output written synchronously (1) or asynchronously (0)
        /// </summary>
        [XmlIgnore]
        public bool IsSyncLogs { get; private set; }

        /// <summary>
        /// Serialized value to represent <see cref="IsSyncLogs"/>
        /// </summary>
        [XmlElement(ElementName = "IsSyncLogs", Order = 5)]
        public string IsAsyncLogsSerialize
        {
            // This getter is automatically called when the xml file is serialized
            get { return IsSyncLogs ? "True" : "False"; }
            set
            {
                // This setter is automatically called when the xml file is deserialized
                if ("True".Equals(value))
                    IsSyncLogs = true;
                else if ("False".Equals(value))
                    IsSyncLogs = false;
                else
                    IsSyncLogs = false; // Force Async if not defined in the config
            }
        }

        /// <summary>
        /// Should the new bspzipplusplus arguments be used when executing bspzip (1) or not (0)<br/>
        /// Since using them on the default bspzip.exe is gonna cause it to not work, i added this to globally
        /// turn on and off the usage of all the following arguments
        /// </summary>
        [XmlIgnore]
        public bool UseBspZipPlusPlusArguments { get; private set; }

        /// <summary>
        /// Serialized value to represent <see cref="UseVerbosePack"/>
        /// </summary>
        [XmlElement(ElementName = "UseBspZipPlusPlusArguments", Order = 6)]
        public string UseBspZipPlusPlusArgumentsSerialize
        {
            // This getter is automatically called when the xml file is serialized
            get { return UseBspZipPlusPlusArguments ? "True" : "False"; }
            set
            {
                // This setter is automatically called when the xml file is deserialized
                if ("True".Equals(value))
                    UseBspZipPlusPlusArguments = true;
                else if ("False".Equals(value))
                    UseBspZipPlusPlusArguments = false;
                else
                    UseBspZipPlusPlusArguments = false; // Force No Use if not defined in the config
            }
        }

        /// <summary>
        /// Will the bspzip "PACK" execution use the -verbose parameter (1) or not (0)
        /// </summary>
        [XmlIgnore]
        public bool UseVerboseForPack { get; private set; }

        /// <summary>
        /// Serialized value to represent <see cref="UseVerbosePack"/>
        /// </summary>
        [XmlElement(ElementName = "UseVerboseForPack", Order = 7)]
        public string UseVerboseForPackSerialize
        {
            // This getter is automatically called when the xml file is serialized
            get { return UseVerboseForPack ? "True" : "False"; }
            set
            {
                // This setter is automatically called when the xml file is deserialized
                if ("True".Equals(value))
                    UseVerboseForPack = true;
                else if ("False".Equals(value))
                    UseVerboseForPack = false;
                else
                    UseVerboseForPack = false; // Force No Verbose if not defined in the config
            }
        }

        /// <summary>
        /// Will the bspzip "REPACK" execution use the -verbose parameter (1) or not (0)
        /// </summary>
        [XmlIgnore]
        public bool UseVerboseForRepack { get; private set; }

        /// <summary>
        /// Serialized value to represent <see cref="UseVerboseForRepack"/>
        /// </summary>
        [XmlElement(ElementName = "UseVerboseForRepack", Order = 8)]
        public string UseVerboseForRepackSerialize
        {
            // This getter is automatically called when the xml file is serialized
            get { return UseVerboseForRepack ? "True" : "False"; }
            set
            {
                // This setter is automatically called when the xml file is deserialized
                if ("True".Equals(value))
                    UseVerboseForRepack = true;
                else if ("False".Equals(value))
                    UseVerboseForRepack = false;
                else
                    UseVerboseForRepack = false; // Force No Verbose if not defined in the config
            }
        }

        /// <summary>
        /// Will the bspzip "EXTRACT" execution use the -verbose parameter (1) or not (0)
        /// </summary>
        [XmlIgnore]
        public bool UseVerboseForExract { get; private set; }

        /// <summary>
        /// Serialized value to represent <see cref="UseVerboseForExract"/>
        /// </summary>
        [XmlElement(ElementName = "UseVerboseForExract", Order = 9)]
        public string UseVerboseForExractSerialize
        {
            // This getter is automatically called when the xml file is serialized
            get { return UseVerboseForExract ? "True" : "False"; }
            set
            {
                // This setter is automatically called when the xml file is deserialized
                if ("True".Equals(value))
                    UseVerboseForExract = true;
                else if ("False".Equals(value))
                    UseVerboseForExract = false;
                else
                    UseVerboseForExract = false; // Force No Verbose if not defined in the config
            }
        }

        /// <summary>
        /// Will the bspzip "CUBEMAPS" execution use the -verbose parameter (1) or not (0)
        /// </summary>
        [XmlIgnore]
        public bool UseVerboseForCubemaps { get; private set; }

        /// <summary>
        /// Serialized value to represent <see cref="UseVerboseForCubemaps"/>
        /// </summary>
        [XmlElement(ElementName = "UseVerboseForCubemaps", Order = 10)]
        public string UseVerboseForCubemapsSerialize
        {
            // This getter is automatically called when the xml file is serialized
            get { return UseVerboseForCubemaps ? "True" : "False"; }
            set
            {
                // This setter is automatically called when the xml file is deserialized
                if ("True".Equals(value))
                    UseVerboseForCubemaps = true;
                else if ("False".Equals(value))
                    UseVerboseForCubemaps = false;
                else
                    UseVerboseForCubemaps = false; // Force No Verbose if not defined in the config
            }
        }

        /// <summary>
        /// Extra parameters that will be added at the end of the bspzip command for "PACK"
        /// </summary>
        [XmlElement(ElementName = "ExtraParametersPack", Order = 11)]
        public string ExtraParametersPack { get; set; }

        /// <summary>
        /// Extra parameters that will be added at the end of the bspzip command for "REPACK"
        /// </summary>
        [XmlElement(ElementName = "ExtraParametersRepack", Order = 12)]
        public string ExtraParametersRepack { get; set; }

        /// <summary>
        /// Extra parameters that will be added at the end of the bspzip command for "EXTRACT"
        /// </summary>
        [XmlElement(ElementName = "ExtraParametersExtract", Order = 13)]
        public string ExtraParametersExtract { get; set; }

        /// <summary>
        /// Extra parameters that will be added at the end of the bspzip command for "CUBEMAPS"
        /// </summary>
        [XmlElement(ElementName = "ExtraParametersCubemaps", Order = 14)]
        public string ExtraParametersCubemaps { get; set; }

        /// <summary>
        /// List of games configs (bspzip.exe directory)
        /// </summary>
        [XmlArray(ElementName = "BspZipDirectories", Order = 15)]
        [XmlArrayItem(ElementName = "Game")]
        //public List<GameConfig> GamesConfigs { get; set; }
        public ObservableCollection<GameConfig> GamesConfigs { get; set; }

        /// <summary>
        /// List of maps configs (custom file directory)
        /// </summary>
        [XmlArray(ElementName = "CustomFilesDirectories", Order = 16)]
        [XmlArrayItem(ElementName = "Map")]
        public ObservableCollection<MapConfig> MapsConfigs { get; set; }

        /// <summary>
        /// List of base directories the tool can browse and the file extensions allowed
        /// </summary>
        [XmlArray(ElementName = "WhiteListDirectories", Order = 17)]
        [XmlArrayItem(ElementName = "Directory")]
        public ObservableCollection<DirectoryRestrictions> DirectoriesRestrictions { get; set; }

        /// <summary>
        /// List of multi maps configs (multiple custom file directories)
        /// </summary>
        [XmlArray(ElementName = "MultiCustomFilesDirectories", Order = 18)]
        [XmlArrayItem(ElementName = "Map")]
        public ObservableCollection<MultiMapConfig> MultiMapsConfigs { get; set; }

        /// <summary>
        /// Last multi custom files directory name loaded by the tool
        /// </summary>
        [XmlElement(ElementName = "LastMultiCustomDirectory", Order = 19)]
        public string LastMultiCustomDirectory { get; set; }

        #endregion

        #region Constructor

        public ToolSettings()
        {
        }

        #endregion

        #region Methods - Init Attributes

        /// <summary>
        /// Initialize the attributes that werent automatically initialized wtih the xml file (cause of missing parameters).<br/>
        /// And delete any invalid attribute
        /// </summary>
        public void InitAllAttributes()
        {
            if (GamesConfigs == null)
            {
                InitGamesConfigs();
            }
            else
            {
                // Delete any invalid game config (missing data that was removed from the xml)
                for (int i = GamesConfigs.Count - 1; i >= 0; i--)
                {
                    if (!GamesConfigs[i].IsValid())
                    {
                        GamesConfigs.RemoveAt(i);
                    }
                }
            }
            if (MapsConfigs == null)
            {
                InitMapsConfigs();
            }
            else
            {
                // Delete any invalid map config (missing data that was removed from the xml)
                for (int i = MapsConfigs.Count - 1; i >= 0; i--)
                {
                    if (!MapsConfigs[i].IsValid())
                    {
                        MapsConfigs.RemoveAt(i);
                    }
                }
            }
            if (MultiMapsConfigs == null)
            {
                InitMultiMapsConfigs();
            }
            else
            {
                // Delete any invalid map config (missing data that was removed from the xml)
                for (int i = MultiMapsConfigs.Count - 1; i >= 0; i--)
                {
                    if (!MultiMapsConfigs[i].IsValid())
                    {
                        MultiMapsConfigs.RemoveAt(i);
                    }
                    else
                    {
                        // Delete any invalid directory in the list
                        for (int j = MultiMapsConfigs[i].ListPath.Count - 1; j >= 0; j--)
                        {
                            if (MultiMapsConfigs[i].ListPath[j] == null)
                            {
                                MultiMapsConfigs[i].ListPath.RemoveAt(j);
                            }
                        }
                    }
                }
            }

            if (DirectoriesRestrictions == null)
            {
                InitDirectoriesRestrictions();
            }
            else
            {
                // Delete any invalid directory restriction (missing data that was removed from the xml)
                for (int i = DirectoriesRestrictions.Count - 1; i >= 0; i--)
                {
                    if (!DirectoriesRestrictions[i].IsValid())
                    {
                        DirectoriesRestrictions.RemoveAt(i);
                    }
                }
            }

            if (LastBsp == null)
            {
                LastBsp = string.Empty;
            }
            if (LastCustomDirectory == null)
            {
                LastCustomDirectory = string.Empty;
            }
            if (LastMultiCustomDirectory == null)
            {
                LastMultiCustomDirectory = string.Empty;
            }
            if (LastGame == null)
            {
                LastGame = string.Empty;
            }
            if (LastExtractDirectory == null)
            {
                LastExtractDirectory = string.Empty;
            }
            if (ExtraParametersPack == null)
            {
                ExtraParametersPack = string.Empty;
            }
            if (ExtraParametersRepack == null)
            {
                ExtraParametersRepack = string.Empty;
            }
            if (ExtraParametersExtract == null)
            {
                ExtraParametersExtract = string.Empty;
            }
            if (ExtraParametersCubemaps == null)
            {
                ExtraParametersCubemaps = string.Empty;
            }
        }

        private void InitGamesConfigs()
        {
            GamesConfigs = new ObservableCollection<GameConfig>();
        }
        private void InitMapsConfigs()
        {
            MapsConfigs = new ObservableCollection<MapConfig>();
        }
        private void InitMultiMapsConfigs()
        {
            MultiMapsConfigs = new ObservableCollection<MultiMapConfig>();
        }
        private void InitDirectoriesRestrictions()
        {
            DirectoriesRestrictions = new ObservableCollection<DirectoryRestrictions>();
        }

        #endregion

        #region Methods - Find Configs

        /// <summary>
        /// Find the first MapConfig (custom files location) corresponding to the given name
        /// </summary>
        /// <param name="name">name to search</param>
        /// <returns>Return the corresponding MapConfig. null if none found</returns>
        public MapConfig FindMapConfig(string name)
        {
            foreach (MapConfig mapConfig in MapsConfigs)
            {
                if (name.Equals(mapConfig.Name))
                {
                    return mapConfig;
                }
            }
            return null;
        }

        /// <summary>
        /// Find the first MultiMapConfig (custom files location) corresponding to the given name
        /// </summary>
        /// <param name="name">name to search</param>
        /// <returns>Return the corresponding MultiMapConfig. null if none found</returns>
        public MultiMapConfig FindMultiMapConfig(string name)
        {
            foreach (MultiMapConfig multiMapConfig in MultiMapsConfigs)
            {
                if (name.Equals(multiMapConfig.Name))
                {
                    return multiMapConfig;
                }
            }
            return null;
        }

        /// <summary>
        /// Find the first GameConfig (bspzip.exe location) corresponding to the given name
        /// </summary>
        /// <param name="name">name to search</param>
        /// <returns>Return the corresponding GameConfig. null if none found</returns>
        public GameConfig FindGameConfig(string name)
        {
            foreach (GameConfig gameConfig in GamesConfigs)
            {
                if (name.Equals(gameConfig.Name))
                {
                    return gameConfig;
                }
            }
            return null;
        }

        #endregion

    }

    #endregion

    #region Class - GameConfig

    /// <summary>
    /// Store a game name and the path to its bspzip.exe
    /// </summary>
    [Serializable]
    public class GameConfig : INotifyPropertyChanged
    {

        #region Attributes

        private string name;
        private string bspZip;
        private string gameInfo;

        /// <summary>
        /// Name of the game
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Path to bspzip.exe
        /// </summary>
        public string BspZip
        {
            get => bspZip;
            set
            {
                if (bspZip != value)
                {
                    bspZip = value;
                    NotifyPropertyChanged("BspZip");
                }
            }
        }

        /// <summary>
        /// Path to the gameinfo.txt
        /// </summary>
        public string GameInfo
        {
            get => gameInfo;
            set
            {
                if (gameInfo != value)
                {
                    gameInfo = value;
                    NotifyPropertyChanged("GameInfo");
                }
            }
        }

        #endregion

        #region Constructor

        public GameConfig()
        {
        }

        /// <summary>
        /// Create a default GameConfig
        /// </summary>
        /// <returns>A new instance of <see cref="GameConfig"/></returns>
        public static GameConfig GetDefaultGameConfig()
        {
            return new GameConfig
            {
                Name = "New Game",
                BspZip = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\bin\bspzip.exe",
                GameInfo = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\gameinfo.txt"
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Path to the folder of gameinfo.txt
        /// </summary>
        public string GameInfoFolder => System.IO.Path.GetDirectoryName(GameInfo);

        /// <summary>
        /// Verify if all values of the GameConfig are not null
        /// </summary>
        /// <returns><c>true</c> if all value are not null, <c>false</c> otherwise</returns>
        public bool IsValid()
        {
            if (Name != null && BspZip != null && GameInfo != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verify if the bspzip.exe and gameinfo.txt files exist
        /// </summary>
        /// <returns><c>true</c> if the files exist, <c>false</c> otherwise</returns>
        public bool FilesExist()
        {
            if (System.IO.File.Exists(BspZip) && System.IO.File.Exists(GameInfo))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

    }

    #endregion

    #region Class - MapConfig

    /// <summary>
    /// Store a map name and the path to its custom folder
    /// </summary>
    [Serializable]
    public class MapConfig : INotifyPropertyChanged
    {

        #region Attributes

        private string name;
        private string path;

        /// <summary>
        /// Name of the map
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Path to the custom folder with files to pack
        /// </summary>
        public string Path
        {
            get => path;
            set
            {
                if (path != value)
                {
                    path = value;
                    NotifyPropertyChanged("Path");
                }
            }
        }

        /// <summary>
        /// Extra string that will contains the directory path cleaned from extra character
        /// </summary>
        [XmlIgnore]
        public string CleanedPath { get; private set; }

        #endregion

        #region Constructor

        public MapConfig()
        {
            CleanedPath = null;
        }

        /// <summary>
        /// Create a default MapConfig
        /// </summary>
        /// <returns>A new instance of <see cref="MapConfig"/></returns>
        public static MapConfig GetDefaultMapConfig()
        {
            return new MapConfig
            {
                Name = "New Custom Folder",
                Path = @"C:\MyMappingProject\CurrentProject"
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clean the path of the directory by removing extra '/' within the path
        /// and storing it in <see cref="CleanedPath"/>
        /// </summary>
        public void CleanPath()
        {
            CleanedPath = Utils.FileUtils.CleanDirectoryPath(Path);
        }

        /// <summary>
        /// Verify if all values of the MapConfig are not null
        /// </summary>
        /// <returns><c>true</c> if all value are not null, <c>false</c> otherwise</returns>
        public bool IsValid()
        {
            if (Name != null && Path != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if the custom directory exist
        /// </summary>
        /// <returns></returns>
        public bool DirectoryExists()
        {
            if (System.IO.Directory.Exists(Path))
            {
                return true;
            }
            return false;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

    }

    #endregion

    #region Class - MultiMapConfig

    /// <summary>
    /// Store a map name and the paths to its custom folders
    /// </summary>
    [Serializable]
    public class MultiMapConfig : INotifyPropertyChanged
    {

        #region Attributes

        private string name;

        /// <summary>
        /// Name of the map
        /// </summary>
		[XmlElement(Order = 1)]
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// List of paths to the custom folders with files to pack
        /// </summary>
        [XmlArray(ElementName = "Paths", Order = 2)]
        [XmlArrayItem(ElementName = "Path")]
        public ObservableCollection<string> ListPath { get; set; }

        /// <summary>
        /// List of custom folders paths separated by a new line.<br/>
        /// Used by the Textbox
        /// </summary>
        [XmlIgnore]
        public string Path
        {
            get
            {
                const string separator = "\n";
                return string.Join(separator, ListPath);
            }
        }
        
        /// <summary>
        /// List of custom folders paths as a dashed list.<br/>
        /// Used for logs
        /// </summary>
        [XmlIgnore]
        public string PathAsDashedList
        {
            get
            {
                // Non breaking space = \u00A0
                const string separator = "\n-\u00A0";
                return (ListPath.Count > 0 ? "-\u00A0" : "") + string.Join(separator, ListPath);
            }
        }

        /// <summary>
        /// List that will contains all the directories paths cleaned from any extra character.<br/>
        /// Needed because modifying asynchronously a ObservableCollection is not allowed
        /// </summary>
        [XmlIgnore]
        public HashSet<string> HashSetCleanedPath { get; private set; }

        #endregion

        #region Constructor

        public MultiMapConfig()
        {
            HashSetCleanedPath = new HashSet<string>();
        }

        /// <summary>
        /// Create a default MultiMapConfig
        /// </summary>
        /// <returns>A new instance of <see cref="MultiMapConfig"/></returns>
        public static MultiMapConfig GetDefaultMultiMapConfig()
        {
            return new MultiMapConfig
            {
                Name = "New Custom Folders",
                ListPath = new ObservableCollection<string>()
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Verify if all values of the MultiMapConfig are not null
        /// </summary>
        /// <returns><c>true</c> if all value are not null, <c>false</c> otherwise</returns>
        public bool IsValid()
        {
            if (Name != null && ListPath != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the list of path is not empty, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool IsNotEmpty()
        {
            return ListPath.Count > 0;
        }

        /// <summary>
        /// Verify if all custom directories exist
        /// </summary>
        /// <returns>True if all custom directories exist, False otherwise</returns>
        public bool DirectoriesExists()
        {
            foreach (string directory in ListPath)
            {
                if (!System.IO.Directory.Exists(directory))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get a list of all the non existing directories, in the Multi Custom folder config
        /// </summary>
        /// <returns>List of directories</returns>
        public List<string> GetNonExistingDirectories()
        {
            List<string> listDirectories = new List<string>();
            foreach (string directory in ListPath)
            {
                if (!System.IO.Directory.Exists(directory))
                {
                    listDirectories.Add(directory);
                }
            }
            return listDirectories;
        }

        /// <summary>
        /// Create a HashSet with all unique directories paths from <see cref="ListPath"/>
        /// </summary>
        /// <returns>A HashSet of string</returns>
        public HashSet<string> GetHashSetFromList()
        {
            return new HashSet<string>(ListPath);
        }

        /// <summary>
        /// Clear <see cref="ListPath"/> and insert every element from the HashSet parameter
        /// </summary>
        /// <param name="directoriesHashSet">List of unique directory path</param>
        public void SetHashSetToList(HashSet<string> directoriesHashSet)
        {
            ListPath.Clear();
            foreach(string directory in directoriesHashSet)
            {
                ListPath.Add(directory);
            }
        }

        /// <summary>
        /// Clean the paths of the directories by removing extra '/' within the list of paths 
        /// and storing them in <see cref="HashSetCleanedPath"/>
        /// </summary>
        public void CleanPaths()
        {
            HashSetCleanedPath.Clear();
            foreach (string path in ListPath)
            {
                HashSetCleanedPath.Add(Utils.FileUtils.CleanDirectoryPath(path));
            }
        }

        /// <summary>
        /// Force an GUI update on the list of directory, in the MainWindow
        /// </summary>
        public void NotifyDirectoryListUpdate()
        {
            // To update the ListBox
            NotifyPropertyChanged("ListPath");
            // To update the TextBox
            NotifyPropertyChanged("Path");
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

    }

    #endregion
    
    #region Class - DirectoryRestrictions

    /// <summary>
    /// Store a directory name and the list of allowed files
    /// </summary>
    [Serializable]
    public class DirectoryRestrictions : INotifyPropertyChanged
    {

        #region Attributes

        private string directoryName;

        /// <summary>
        /// Name of the directory (materials, models, ...)
        /// </summary>
        [XmlElement(Order = 1)]
        public string DirectoryName
        {
            get => directoryName;
            set
            {
                if (directoryName != value)
                {
                    directoryName = value;
                    NotifyPropertyChanged("DirectoryName");
                }
            }
        }

        /// <summary>
        /// Files Extensions allowed
        /// </summary>
        [XmlArray(ElementName = "Extensions", Order = 2)]
        [XmlArrayItem(ElementName = "Extension")]
        public List<string> AllowedExtension { get; set; }

        /// <summary>
        /// String version of the extensions allowed separated by <c>|</c>
        /// </summary>
        [XmlIgnore]
        public string ExtensionStr
        {
            get
            {
                const string separator = "|";
                return string.Join(separator, AllowedExtension);
            }
            set
            {
                const char separator = '|';
                AllowedExtension = new List<string>(value.Split(separator));
            }
        }

        #endregion

        #region Constructor

        public DirectoryRestrictions()
        {
        }

        /// <summary>
        /// Create a default DirectoryRestrictions
        /// </summary>
        /// <returns>A new instance of <see cref="DirectoryRestrictions"/></returns>
        public static DirectoryRestrictions GetDefaultDirectoryRestrictions()
        {
            return new DirectoryRestrictions
            {
                DirectoryName = "directory_name",
                AllowedExtension = new List<string>() { ".txt", ".jpg" }
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Verify if all values of the DirectoryRestrictions are not null
        /// </summary>
        /// <returns><c>true</c> if all value are not null, <c>false</c> otherwise</returns>
        public bool IsValid()
        {
            if (DirectoryName != null && AllowedExtension != null)
            {
                return true;
            }
            return false;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

    }

    #endregion

}
