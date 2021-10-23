using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BspZipGUI.Tool.Utils
{

    /// <summary>
    /// Abstract Exception that other functional Exception will inherit
    /// </summary>
    internal abstract class FunctionalException : Exception
    {
        public FunctionalException() { }

        public FunctionalException(string message) : base(message) { }

        public FunctionalException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Return the Message of Exception and the message of InnerException if available
        /// </summary>
        /// <returns></returns>
        public string GetMessageAndInner()
        {
            string errorMessage = Message;
            if (InnerException != null)
            {
                errorMessage += "\n" + InnerException.Message;
            }
            return errorMessage;
        }
    }

    /// <summary>
    /// Exception used when an unexpected error happen during the serialization / deserialization of the settings 
    /// </summary>
    [Serializable]
    internal class SettingsSerializationException : FunctionalException
    {
        public SettingsSerializationException() { }

        public SettingsSerializationException(string message) : base(message) { }

        public SettingsSerializationException(string message, Exception inner) : base(message, inner) { }

    }


    /// <summary>
    /// Exception used when an unexpected error happen during the creation of the list of files to pack 
    /// </summary>
    [Serializable]
    internal class FilePackCreationException : FunctionalException
    {
        public FilePackCreationException() { }

        public FilePackCreationException(string message) : base(message) { }

        public FilePackCreationException(string message, Exception inner) : base(message, inner) { }

    }


    /// <summary>
    /// Exception used when an unexpected error happen during the creation of the bsp backup
    /// </summary>
    [Serializable]
    internal class BspBackupCreationException : FunctionalException
    {
        public BspBackupCreationException() { }

        public BspBackupCreationException(string message) : base(message) { }

        public BspBackupCreationException(string message, Exception inner) : base(message, inner) { }

    }

    /// <summary>
    /// Exception used when an unexpected error happen during the bspzip execution
    /// </summary>
    [Serializable]
    internal class BspZipExecutionException : FunctionalException
    {
        public BspZipExecutionException() { }

        public BspZipExecutionException(string message) : base(message) { }

        public BspZipExecutionException(string message, Exception inner) : base(message, inner) { }

    }

}
