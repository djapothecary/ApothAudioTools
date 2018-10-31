using System;
using System.Runtime.Serialization;

namespace ID3Tagging.ID3Lib.Exceptions
{
    /// <summary>
    /// The exception is thrown when a frame is corrupt.
    /// </summary>
    [Serializable]
    public class InvalidFrameException : InvalidStructureException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFrameException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected InvalidFrameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFrameException"/> class. 
        /// 
        /// </summary>
        public InvalidFrameException()
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFrameException"/> class. 
        /// </summary>
        /// <param name="message">
        /// </param>
        public InvalidFrameException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidFrameException"/> class. 
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="inner">
        /// </param>
        public InvalidFrameException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}