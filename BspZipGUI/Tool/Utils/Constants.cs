namespace BspZipGUI.Tool.Utils
{
    /// <summary>
    /// Class containing constants used in the app
    /// </summary>
    internal static class Constants
    {

        #region Constants

        /// <summary>
        /// The max path length correctly supported by Windows.<br/>
        /// Trying to pack a file that use a path longer than this will break the bspzip.exe process itself.<br/>
        /// It won't pack any file after encountering the limit.<br/>
        /// This will only be used to show a warning to the user after the execution.
        /// </summary>
        public const int MAX_PATH = 260;

        public const string ExtensionBsp = ".bsp";
        public const string ExtensionExe = ".exe";
        public const string ExtensionTxt = ".txt";
        public const string ExtensionZip = ".zip";

        public const string BspZipFile = "bspzip.exe";
        public const string GameinfoFile = "gameinfo.txt";

        /// <summary>
        /// The name of the text file listing all files to pack
        /// </summary>
        public const string FilesListText = "filesList.txt";

        public const char Slash = '/';
        public const char Backslash = '\\';

        #endregion

    }

    /// <summary>
    /// Class containing constants message used in the GUI
    /// </summary>
    internal static class MessageConstants
    {

        #region Constants

        //public const string MessageToolNotWork = "The tool will not work correctly";

        public const string MessageSettingsSaveSuccess = "Saved settings to settings.xml";
        public const string MessageSettingsSaveError = "Error trying to save settings";
        public const string MessageSettingsCreateError = "Error creating settings.xml";
        public const string MessageSettingsReadError = "Error while reading the settings";
        public const string MessageSettingsUseDefault = "Trying to use the default settings instead";
        public const string MessageSettingsDefaultReadError = "Error while reading the default settings";
        public const string MessageSettingsUseEmpty = "Generating empty settings";

        public const string MessageFileNotBsp = "The file is not .bsp";
        public const string MessageBspFileNotFound = "The .bsp file doesn't exist";
        public const string MessageDirectoryNotFound = "The directory doesn't exist";
        public const string MessageCustomFolderNotFound = "The Custom Folder doesn't exist";
        public const string MessageMultiCustomFolderEmpty = "The list of Custom Folders is empty";
        public const string MessageMultiCustomFolderNotFound = "One or multiple Custom Folder(s) don't exist";
        public const string MessageSimpleMultiCustomFolderNotSelected = "Invalid state: No Custom Folder(s) were selected";
        public const string MessageMultiCustomFolderSettingsNotSelected = "No Custom Folders selected";
        public const string MessageCustomFolderInvalid = "Invalid Custom Folder selected";
        public const string MessageGameNotFound = "Can't find the specified bspzip.exe and/or gameinfo.txt";
        public const string MessageGameInvalid = "Invalid Game selected";

        public const string MessageBspPacking = "Bsp packing in progress...";
        public const string MessageBspRepackCompress = "Bsp compressing in progress...";
        public const string MessageBspRepackDecompress = "Bsp decompressing in progress...";
        public const string MessageBspExtractFile = "Bsp extraction in progress...";
        public const string MessageBspExtractCubemaps = "Cubemaps extraction in progress...";
        public const string MessageBspDeleteCubemaps = "Cubemaps deletion in progress...";

        public const string MessageSeeLogs = "\nSee logs for more details";

        public const string MessageBspzipSuccess = "Success";
        public const string MessageBspzipFail = "Error: bspzip.exe process ended unexpectedly";
        public const string MessageCopyBspFail = "Error: Couldn't make a copy of the BSP";
        public const string MessageListFilesNotFound = "Error: Couldn't find the list of files to pack";
        public const string MessageListFilesFail = "Error: Couldn't create the list of files to pack";
        public const string MessageMaxPathSizeWarning = "One or more file(s) path(s) are longer than 260 characters (MAX_PATH)";
        public const string MessageMaxPathSizeSuggestion = "Suggestion: Move your custom directory to a shorter path";
        public const string MessageBspzipPackingWarning = "Warning: bspzip.exe may not have packed all the files correctly";

        public const string MessageWhitelistWarning = "You unchecked \"Use Directory Whitelist\", it will pack every single files from the directory " +
            "and its subdirectories.\nAre you really sure ?\n(Be careful not to use a path like C:\\)";

        #endregion

    }

}
