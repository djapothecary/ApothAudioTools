using System;
using System.Runtime.Serialization;

namespace ID3Tagging.ID3Lib.Exceptions
{
    /// <summary>
    /// The exception is thrown when some component of an mp3 file is permanently corrupt.
    /// Re-reading the file will always give you the same error, 
    /// as opposed to I/O or permission errors that can be usefully retried.
    /// </summary>
    /// <remarks>
    /// <see href="http://www.codeproject.com/KB/architecture/exceptionbestpractices.aspx#Exceptionsshouldbemarked[Serializable]22">Exceptions should be marked [Serializable]</see>
    /// </remarks>
    [Serializable]
    public class InvalidStructureException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStructureException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        public InvalidStructureException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStructureException" /> class.
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidStructureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStructureException" /> class.
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InvalidStructureException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidStructureException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected InvalidStructureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}