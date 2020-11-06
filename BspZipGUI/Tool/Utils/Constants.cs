using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool.Utils
{
    static class Constants
    {

        #region Constants

        public const string ExtensionBsp = ".bsp";
        public const string ExtensionExe = ".exe";
        public const string ExtensionTxt = ".txt";
        public const string BspZipFile = "bspzip.exe";
        public const string GameinfoFile = "gameinfo.txt";
        public const string FilesListText = "filesList.txt";
        public const char Slash = '/';
        public const char Backslash = '\\';
        
        #endregion

    }

    static class MessageConstants
    {

        #region Constants

        public const string MessageTitle = "BspZipGUI";

        public const string MessageSaveSettingsOK = "Saved settings to settings.xml";
        public const string MessageSaveSettingsFail = "Error trying to save settings";

        public const string MessageFileNotBsp = "The file is not .bsp";
        public const string MessageFileNotFound = "The file doesn't exist";
        public const string MessageCustomFolderNotFound = "The Custom Folder doesn't exist";
        public const string MessageCustomFolderInvalid = "Invalid Custom Folder selected";
        public const string MessageGameNotFound = "Can't find the specified bspzip.exe and/or gameinfo.txt";
        public const string MessageGameInvalid = "Invalid Game selected";

        public const string MessageBspPacking = "Bsp packing in progress...";
        public const string MessageBspRepackCompress = "Bsp compressing in progress...";
        public const string MessageBspRepackDecompress = "Bsp decompressing in progress...";

        public const string MessageBspzipFail = "Error: bspzip.exe process ended unexpectedly";
        public const string MessageCopyBspFail = "Error: Couldn't make a copy of the BSP";
        public const string MessageListFilesNotFound = "Error: Couldn't find the list of files to pack";
        public const string MessageListFilesFail = "Error: Couldn't create the list of files to pack";

        public const string MessageWarningWhitelist = "You unchecked \"Use Directory Whitelist\", it will pack every single files from the directory " +
            "and subdirectories.\nAre you really sure ?\n(Be careful not to use a path like C:\\)";

        #endregion

    }

}
