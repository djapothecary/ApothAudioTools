﻿using System;
using System.Diagnostics;

using ID3Tagging.ID3Lib.Exceptions;

namespace ID3Tagging.MP3Lib.Exceptions
{
    /// <summary>
    /// The exception is for when the vbr header claims the audio
    /// is a different length to the file size.
    /// This can happen if the file has been truncated at some point in its history,
    /// but could also be if unrecognised tags (non-id3, e.g. monkey audio) are added to the file.
    /// It is not thrown as such, because it's not an error that needs the parse to fail.
    /// </summary>
    [Serializable]
    public class InvalidVbrSizeException : InvalidStructureException
    {
        /// <summary>
        /// the number of zero bytes found after the last frame in the id3v2 tag
        /// </summary>
        public uint Measured { get; private set; }

        /// <summary>
        /// the amount of space left over after the last frame in the id3v2 tag
        /// </summary>
        public uint Specified { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidVbrSizeException"/> class. 
        /// </summary>
        /// <param name="measured">
        /// </param>
        /// <param name="specified">
        /// </param>
        public InvalidVbrSizeException(uint measured, uint specified)
            : base("VBR header claims audio size is not payload size.")
        {
            Debug.Assert(measured != specified);
            this.Measured = measured;
            this.Specified = specified;
        }

        /// <summary>
        /// overrides default message with a specific "Padding is corrupt" one
        /// </summary>
        public override string Message
        {
            get
            {
                if (this.Specified > this.Measured)
                {
                    // audio has been truncated due to file system error?
                    return
                        string.Format(
                            "VBR header states the audio size is {0} bytes, but the payload is only {1} bytes, so {2} bytes have been lost from the end",
                            this.Specified,
                            this.Measured,
                            this.Specified - this.Measured);
                }
                else
                {
                    // maybe something's added a tag we don't understand, but could still be file system error
                    return
                        string.Format(
                            "VBR header states audio size is only {0} bytes, but the payload is {1} bytes, so {2} bytes have been added to the end",
                            this.Specified,
                            this.Measured,
                            this.Measured - this.Specified);
                }
            }
        }
    }
}