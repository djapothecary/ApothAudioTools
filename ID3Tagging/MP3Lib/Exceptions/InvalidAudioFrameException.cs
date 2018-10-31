using System;

using ID3Tagging.ID3Lib.Exceptions;

namespace ID3Tagging.MP3Lib.Exceptions
{
    /// <summary>
    /// The exception is thrown when an audio frame is corrupt.
    /// </summary>
    [Serializable]
    public class InvalidAudioFrameException : InvalidStructureException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAudioFrameException"/> class. 
        /// 
        /// </summary>
        public InvalidAudioFrameException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAudioFrameException"/> class. 
        /// </summary>
        /// <param name="message">
        /// </param>
        public InvalidAudioFrameException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidAudioFrameException"/> class. 
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="inner">
        /// </param>
        public InvalidAudioFrameException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}