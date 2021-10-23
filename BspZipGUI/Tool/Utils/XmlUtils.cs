using BspZipGUI.Tool.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BspZipGUI.Tool.Utils
{
    internal static class XmlUtils
    {
        #region Constants

        /// <summary>
        /// Name of the Settings file
        /// </summary>
        private const string xmlSettings = "settings.xml";

        /// <summary>
        /// Path to the default settings, stored in the App
        /// </summary>
        private const string xmlSettingsBackup = "BspZipGUI.settings_backup.txt";

        #endregion

        #region Methods

        /// <summary>
        /// Get the settings of the application by reading settings.xml
        /// </summary>
        /// <exception cref="SettingsSerializationException">If an error happens during the parsingof the file</exception>
        /// <returns>The initialized Settings or null if the file doesn't exist</returns>
        public static ToolSettings GetSettingsFromFile()
        {
            if (System.IO.File.Exists(xmlSettings))
            {
                // If settings.xml exist, we load it
                return DeserializeSettingsFromFile(xmlSettings);
            }
            return null;
        }


        /// <summary>
        /// Read and parse the given settings file
        /// </summary>
        /// <param name="filename">Name of the file to parse</param>
        /// <exception cref="SettingsSerializationException">If an error happens during the parsing</exception>
        /// <returns>Parsed settings</returns>
        private static ToolSettings DeserializeSettingsFromFile(string filename)
        {
            try
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(filename))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ToolSettings));
                    return (ToolSettings)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new SettingsSerializationException(MessageConstants.MessageSettingsReadError, ex);
            }
        }

        /// <summary>
        /// Get the default settings stored in the .exe
        /// </summary>
        /// <exception cref="SettingsSerializationException">If an error happens when reading and parsing the default settings</exception>
        /// <returns></returns>
        public static ToolSettings GetSettingsFromResource()
        {
            // If it doesn't exist, we load it from the embedded ressource and recreate it
            string xmlText;
            try
            {
                xmlText = FileUtils.ReadResourceFile(xmlSettingsBackup);
                //System.IO.File.WriteAllText(xmlSettings, xmlText);
            }
            catch (Exception ex)
            {
                throw new SettingsSerializationException(MessageConstants.MessageSettingsDefaultReadError, ex);
            }
            return DeserializeSettingsFromText(xmlText);
        }


        /// <summary>
        /// Read and parse the given string into Settings
        /// </summary>
        /// <param name="xmlText">the text to parse</param>
        /// <exception cref="SettingsSerializationException">If an error happens during the parsing</exception>
        /// <returns>Parsed settings</returns>
        private static ToolSettings DeserializeSettingsFromText(string xmlText)
        {
            try
            {
                using (System.IO.StringReader reader = new System.IO.StringReader(xmlText))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ToolSettings));
                    return (ToolSettings)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new SettingsSerializationException(MessageConstants.MessageSettingsDefaultReadError, ex);
            }
        }

        /// <summary>
        /// Save the given settings in settings.xml
        /// </summary>
        /// <param name="settings">The settings to save</param>
        /// <exception cref="SettingsSerializationException">If an error happens during the serialization of the file</exception>
        /// <returns>true if successful, false if the Settings are null</returns>
        public static bool SerializeSettings(ToolSettings settings)
        {
            if (settings != null)
            {
                System.Xml.XmlWriterSettings xmlWritterSettings =
                    new System.Xml.XmlWriterSettings() { Indent = true };
                try
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(xmlSettings))
                    using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(writer, xmlWritterSettings))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ToolSettings));
                        serializer.Serialize(xmlWriter, settings);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    throw new SettingsSerializationException(MessageConstants.MessageSettingsSaveError, ex);
                }
            }
            return false;
        }

        #endregion

    }



}
