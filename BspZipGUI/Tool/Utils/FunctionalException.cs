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
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public FunctionalException() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        public FunctionalException(string message) : base(message) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="inner"><inheritdoc/></param>
        public FunctionalException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Return the Message of Exception and the message of InnerException if available
        /// </summary>
        /// <returns>Exception message and the InnerException message if available</returns>
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
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public SettingsSerializationException() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        public SettingsSerializationException(string message) : base(message) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="inner"><inheritdoc/></param>
        public SettingsSerializationException(string message, Exception inner) : base(message, inner) { }

    }


    /// <summary>
    /// Exception used when an unexpected error happen during the creation of the list of files to pack 
    /// </summary>
    [Serializable]
    internal class FilePackCreationException : FunctionalException
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public FilePackCreationException() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        public FilePackCreationException(string message) : base(message) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="inner"><inheritdoc/></param>
        public FilePackCreationException(string message, Exception inner) : base(message, inner) { }

    }


    /// <summary>
    /// Exception used when an unexpected error happen during the creation of the bsp backup
    /// </summary>
    [Serializable]
    internal class BspBackupCreationException : FunctionalException
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public BspBackupCreationException() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        public BspBackupCreationException(string message) : base(message) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="inner"><inheritdoc/></param>
        public BspBackupCreationException(string message, Exception inner) : base(message, inner) { }

    }

    /// <summary>
    /// Exception used when an unexpected error happen during the bspzip execution
    /// </summary>
    [Serializable]
    internal class BspZipExecutionException : FunctionalException
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public BspZipExecutionException() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        public BspZipExecutionException(string message) : base(message) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="inner"><inheritdoc/></param>
        public BspZipExecutionException(string message, Exception inner) : base(message, inner) { }

    }

    /// <summary>
    /// Exception used when the process encounters one or multiple paths that are longer than <see cref="Constants.MAX_PATH"/>
    /// </summary>
    [Serializable]
    internal class MaxPathSizeLimitException : FunctionalException
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public MaxPathSizeLimitException() { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        public MaxPathSizeLimitException(string message) : base(message) { }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="message"><inheritdoc/></param>
        /// <param name="inner"><inheritdoc/></param>
        public MaxPathSizeLimitException(string message, Exception inner) : base(message, inner) { }

    }

}
