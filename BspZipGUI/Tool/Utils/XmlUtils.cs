using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BspZipGUI.Tool.Utils
{
    static class XmlUtils
    {
        public const string xmlSettings = "settings.xml";
        private const string xmlSettingsBackup = "BspZipGUI.settings_backup.txt";

        /// <summary>
        /// Get the settings of the application or recreate them if they don't exist
        /// </summary>
        /// <returns></returns>
        public static Settings GetSettings()
        {
            Settings settings = null;
            if (System.IO.File.Exists(xmlSettings))
            {
                // If settings.xml exist, we load it
                settings = DeserializeSettings();
            }
            else
            {
                // If it doesn't exist, we load it from the embedded ressource and recreate it
                string xmlText = FilesUtils.ReadResourceFile(xmlSettingsBackup);
                settings = DeserializeSettings(xmlText);
                FilesUtils.WriteAllText(xmlSettings, xmlText);
            }
            return settings;
        }

        
        /// <summary>
        /// Read and parse settings.xml
        /// </summary>
        /// <returns>Returns the parsed settings</returns>
        public static Settings DeserializeSettings()
        {
            Settings settings = null;
            try
            {
                using (var reader = new System.IO.StreamReader(xmlSettings))
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    settings = (Settings)serializer.Deserialize(reader);
                }
            }
            catch { }
            return settings;
        }

        /// <summary>
        /// Read and parse the given string of settings.xml
        /// </summary>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static Settings DeserializeSettings(string xmlText)
        {
            Settings settings = null;
            try
            {
                using (var reader = new System.IO.StringReader(xmlText))
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    settings = (Settings)serializer.Deserialize(reader);
                }
            }
            catch { }
            return settings;
        }


        /// <summary>
        /// Save the given settings in settings.xml
        /// </summary>
        /// <param name="settings"></param>
        public static bool SerializeSettings(Settings settings)
        {
            if (settings != null)
            {
                var xmlWritterSettings = new System.Xml.XmlWriterSettings() { Indent = true };
                try
                {
                    using (var writer = new System.IO.StreamWriter(xmlSettings))
                    using (var xmlWriter = System.Xml.XmlWriter.Create(writer, xmlWritterSettings))
                    {
                        var serializer = new XmlSerializer(typeof(Settings));
                        serializer.Serialize(xmlWriter, settings);
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

    }
}
