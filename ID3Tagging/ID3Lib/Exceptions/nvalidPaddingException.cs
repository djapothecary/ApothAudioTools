using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ID3Tagging.ID3Lib.Exceptions
{
    /// <summary>
    /// The exception is thrown when the amount of padding 
    /// doesn't match the space left over at the end of the ID3V2 tag.
    /// </summary>
    [Serializable]
    public class InvalidPaddingException : InvalidStructureException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPaddingException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected InvalidPaddingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.Measured = info.GetUInt32("measured");
            this.Specified = info.GetUInt32("specified");
        }

        /// <summary>
        /// </summary>
        /// <param name="info">
        /// </param>
        /// <param name="context">
        /// </param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");

            info.AddValue("measured", this.Measured);
            info.AddValue("specified", this.Specified);
            base.GetObjectData(info, context);
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPaddingException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        public InvalidPaddingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPaddingException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="message">
        /// </param>
        public InvalidPaddingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPaddingException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="message">
        /// </param>
        /// <param name="inner">
        /// </param>
        public InvalidPaddingException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// the number of zero bytes actually found between the last frame in the id3v2 tag, and the first non-zero byte of audio.
        /// </summary>
        public uint Measured { get; private set; }

        /// <summary>
        /// the amount of space between the last frame in the id3v2 tag, and the specified end of the tag block.
        /// </summary>
        public uint Specified { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPaddingException"/> class. 
        /// Initializes a new instance of the class
        /// </summary>
        /// <param name="measured">
        /// </param>
        /// <param name="specified">
        /// </param>
        public InvalidPaddingException(uint measured, uint specified)
            : base("Padding is corrupt, must be zeroes to end of id3 tag.")
        {
            Debug.Assert(measured != specified);
            this.Measured = measured;
            this.Specified = specified;
        }

        /// <summary>
        /// Overrides default message with a specific "Padding is corrupt" one
        /// </summary>
        public override string Message
        {
            get
            {
                if (this.Measured > this.Specified)
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "Padding is corrupt; {0} zero bytes found, but only {1} bytes should be left between last id3v2 frame and the end of the tag",
                        this.Measured,
                        this.Specified);
                else
                    return string.Format(
                        CultureInfo.InvariantCulture,
                        "Padding is corrupt; {1} bytes should be left between last id3v2 frame and the end of the tag, but only {0} zero bytes found.",
                        this.Measured,
                        this.Specified);
            }
        }
    }
}