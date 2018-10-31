using System;
using System.Runtime.Serialization;

namespace ID3Tagging.ID3Lib.Exceptions
{
    /// <summary>
    /// The exception is thrown when the tag is corrupt.
    /// </summary>
    [Serializable]
    public class InvalidTagException : InvalidStructureException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTagException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected InvalidTagException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTagException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        public InvalidTagException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTagException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="message">
        /// </param>
        public InvalidTagException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidTagException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="inner">
        /// </param>
        public InvalidTagException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}