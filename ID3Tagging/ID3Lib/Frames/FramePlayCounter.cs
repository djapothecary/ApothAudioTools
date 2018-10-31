using ID3Tagging.ID3Lib.Utils;
using System.IO;

namespace ID3Tagging.ID3Lib.Frames
{
    /// <summary>
    /// Manage play counter frames.
    /// </summary>
    /// <remarks>
    ///   This frame's purpose is to be able to identify the audio file in a
    ///   database, that may provide more information relevant to the content.
    /// </remarks>
    [Frame("PCNT")]
    public class FramePlayCounter : FrameBase
    {
        #region Fields

        private byte[] _counter = { 0 };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FramePlayCounter"/> class. 
        /// Create a Play Counter frame.
        /// </summary>
        /// <param name="frameId">
        /// ID3v2 PCNT frame
        /// </param>
        public FramePlayCounter(string frameId)
            : base(frameId)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// get the number of time the song has been played
        /// </summary>
        public ulong Counter
        {
            get
            {
                return Memory.ToInt64(_counter);
            }

            set
            {
                _counter = Memory.GetBytes(value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parse the binary PCNT frame
        /// </summary>
        /// <param name="frame">
        /// binary frame
        /// </param>
        public override void Parse(byte[] frame)
        {
            const int Index = 0;
            _counter = Memory.Extract(frame, Index, frame.Length - Index);
        }

        /// <summary>
        /// Create a binary PCNT frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            writer.Write(_counter);
            return buffer.ToArray();
        }

        /// <summary>
        /// Unique Tag Identifer description 
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return null;
        }

        #endregion
    }
}
