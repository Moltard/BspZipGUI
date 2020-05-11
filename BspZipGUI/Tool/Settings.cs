using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BspZipGUI.Tool
{
    
    /// <summary>
    /// Store all settings of the user
    /// </summary>
    [Serializable]
    [XmlRoot("AppSettings")]
    public class Settings
    {
        
        public Settings()
        {
        }

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
        /// List of games configs (bspzip.exe directory)
        /// </summary>
        [XmlArray(ElementName = "BspZipDirectories", Order = 4)]
        [XmlArrayItem(ElementName = "Game")]
        //public List<GameConfig> GamesConfigs { get; set; }
        public ObservableCollection<GameConfig> GamesConfigs { get; set; }


        /// <summary>
        /// List of maps configs (custom file directory)
        /// </summary>
        [XmlArray(ElementName = "CustomFilesDirectories", Order = 5)]
        [XmlArrayItem(ElementName = "Map")]
        public ObservableCollection<MapConfig> MapsConfigs { get; set; }

        /// <summary>
        /// List of base directories the tool can browse and the file extensions allowed
        /// </summary>
        [XmlArray(ElementName = "WhiteListDirectories", Order = 6)]
        [XmlArrayItem(ElementName = "Directory")]
        public ObservableCollection<DirectoryRestrictions> DirectoriesRestrictions { get; set; }

        /// <summary>
        /// Find the MapConfig (custom files location) corresponding to the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Return the corresponding MapConfig. null if none found</returns>
        public MapConfig FindMapConfig(string name)
        {
            foreach (var mapConfig in MapsConfigs)
            {
                if (name.Equals(mapConfig.Name))
                {
                    return mapConfig;
                }
            }
            return null;
        }

        /// <summary>
        /// Find the GameConfig (bspzip.exe location) corresponding to the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Return the corresponding GameConfig. null if none found</returns>
        public GameConfig FindGameConfig(string name)
        {
            foreach (var gameConfig in GamesConfigs)
            {
                if (name.Equals(gameConfig.Name))
                {
                    return gameConfig;
                }
            }
            return null;
        }

        /// <summary>
        /// Init the attributes that werent init by the xml (cause of missing parameters). And delete invalid ones
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
                LastBsp = "";
            }
            if (LastCustomDirectory == null)
            {
                LastCustomDirectory = "";
            }
            if (LastGame == null)
            {
                LastGame = "";
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
        private void InitDirectoriesRestrictions()
        {
            DirectoriesRestrictions = new ObservableCollection<DirectoryRestrictions>();
        }


        /// <summary>
        /// Save the current setting to settings.xml
        /// </summary>
        public bool SaveSettings()
        {
            return Utils.XmlUtils.SerializeSettings(this);
        }


    }

    /// <summary>
    /// Store a game name and the path to its bspzip.exe
    /// </summary>
    [Serializable]
    public class GameConfig : INotifyPropertyChanged
    {
        public GameConfig()
        {
        }

        /// <summary>
        /// Create a default GameConfig
        /// </summary>
        /// <returns></returns>
        public static GameConfig GetDefaultGameConfig()
        {
            return new GameConfig
            {
                Name = "New Game",
                BspZip = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\bin\bspzip.exe",
                GameInfo = @"C:\Program Files (x86)\Steam\steamapps\common\Counter-Strike Global Offensive\csgo\gameinfo.txt"
            };
        }

        private string name;
        private string bspZip;
        private string gameInfo;


        /// <summary>
        /// Name of the game
        /// </summary>
        public string Name
        {
            get { return name; }
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
            get { return bspZip; }
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
            get { return gameInfo; }
            set
            {
                if (gameInfo != value)
                {
                    gameInfo = value;
                    NotifyPropertyChanged("GameInfo");
                }
            }
        }
        
        /// <summary>
        /// Path to the folder of gameinfo.txt
        /// </summary>
        public string GameInfoFolder { get { return System.IO.Path.GetDirectoryName(GameInfo); } }

        /// <summary>
        /// Return true if all values of the GameConfig are not null, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (Name != null && BspZip != null && GameInfo != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return true if the files used exist
        /// </summary>
        /// <returns></returns>
        public bool FilesExist()
        {
            if (System.IO.File.Exists(BspZip) && System.IO.File.Exists(GameInfo))
            {
                return true;
            }
            return false;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

    /// <summary>
    /// Store a map name and the path to its custom folder
    /// </summary>
    [Serializable]
    public class MapConfig : INotifyPropertyChanged
    {
        public MapConfig()
        {
        }

        /// <summary>
        /// Create a default MapConfig
        /// </summary>
        /// <returns></returns>
        public static MapConfig GetDefaultMapConfig()
        {
            return new MapConfig
            {
                Name = "New Custom Folder",
                Path = @"C:/MyMappingProject/CurrentProject/"
            };
        }

        private string name;
        private string path;

        /// <summary>
        /// Name of the map
        /// </summary>
        public string Name
        {
            get { return name; }
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
            get { return path; }
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
        /// Clean the path of the directory by removing extra '/' within the path
        /// </summary>
        public void CleanPath()
        {
            Path = Utils.FilesUtils.GetDirectoryName(Path + Utils.FilesUtils.slash);
        }

        /// <summary>
        /// Return true if all values of the MapConfig are not null, false otherwise
        /// </summary>
        /// <returns></returns>
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

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

    /// <summary>
    /// Store a directory name and the list of allowed files
    /// </summary>
    [Serializable]
    public class DirectoryRestrictions : INotifyPropertyChanged
    {

        public DirectoryRestrictions()
        {
        }

        /// <summary>
        /// Create a default DirectoryRestrictions
        /// </summary>
        /// <returns></returns>
        public static DirectoryRestrictions GetDefaultDirectoryRestrictions()
        {
            return new DirectoryRestrictions
            {
                DirectoryName = "directory_name",
                AllowedExtension = new List<string>() { ".txt", ".jpg" }
            };
        }

        private string directoryName;

        /// <summary>
        /// Name of the directory (materials, models, ...)
        /// </summary>
        [XmlElement(Order = 1)]
        public string DirectoryName
        {
            get { return directoryName; }
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
        /// String version of the extensions allowed separated by | 
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

        /// <summary>
        /// Return true if all values of the DirectoryRestrictions are not null, false otherwise
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if (DirectoryName != null && AllowedExtension != null)
            {
                return true;
            }
            return false;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

}
