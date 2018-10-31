using ID3Tagging.ID3Lib.Utils;
using System;
using System.IO;

namespace ID3Tagging.ID3Lib.Frames
{/// <summary>
 /// Manage unique identifier frames.
 /// </summary>
 /// <remarks>
 ///   This frame's purpose is to be able to identify the audio file in a
 ///   database, that may provide more information relevant to the content.
 /// </remarks>
    [Frame("UFID")]
    public class FrameUniqueIdentifier : FrameBase
    {
        #region Fields

        private string _description;

        private byte[] _identifier;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameUniqueIdentifier"/> class. 
        /// Create a FrameGEOB frame.
        /// </summary>
        /// <param name="frameId">
        /// ID3v2 UFID frame
        /// </param>
        public FrameUniqueIdentifier(string frameId)
            : base(frameId)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Frame description
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        /// <summary>
        /// Binary representation of the object
        /// </summary>
        public byte[] Identifier
        {
            get
            {
                return _identifier;
            }

            set
            {
                if (value.Length > 64)
                {
                    throw new ArgumentOutOfRangeException("value", "The identifier can't be more than 64 bytes");
                }

                _identifier = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parse the binary UFID frame
        /// </summary>
        /// <param name="frame">
        /// binary frame
        /// </param>
        public override void Parse(byte[] frame)
        {
            int index = 0;
            _description = TextBuilder.ReadASCII(frame, ref index);
            _identifier = Memory.Extract(frame, index, frame.Length - index);
        }

        /// <summary>
        /// Create a binary UFID frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write(TextBuilder.WriteASCII(_description));
            writer.Write(_identifier);
            return buffer.ToArray();
        }

        /// <summary>
        /// Unique Tag Identifier description 
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return _description;
        }

        #endregion
    }
}
