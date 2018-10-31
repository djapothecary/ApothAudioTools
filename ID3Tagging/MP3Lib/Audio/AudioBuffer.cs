using System;
using System.IO;
using System.Security.Cryptography;

namespace ID3Tagging.MP3Lib.Audio
{
    /// <summary>
    /// The audio buffer.
    /// </summary>
    class AudioBuffer : Audio
    {
        #region Fields

        /// <summary>
        /// holds audio stream locked so we can rely on it when saving
        /// </summary>
        private byte[] _sourceBuffer;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioBuffer"/> class. 
        /// construct audio file
        /// passing in audio size and id3 length tag (if any) to help with bitrate calculations
        /// </summary>
        /// <param name="sourceBuffer">
        /// </param>
        /// <param name="id3DurationTag">
        /// </param>
        public AudioBuffer(byte[] sourceBuffer, TimeSpan? id3DurationTag)
            : base(AudioFrameFactory.CreateFrame(sourceBuffer), id3DurationTag)
        {
            _sourceBuffer = sourceBuffer;

            CheckConsistency();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the source buffer.
        /// </summary>
        public byte[] SourceBuffer
        {
            get
            {
                return _sourceBuffer;
            }

            set
            {
                _sourceBuffer = value;
            }
        }

        /// <summary>
        /// text info, e.g. the encoding standard of audio data in AudioStream
        /// /// </summary>
        public override string DebugString
        {
            get
            {
                // ----AudioFile----
                // Header starts: 12288 bytes
                // FileSize: 4750766 bytes
                string retval = string.Format("{0}\n----AudioBuffer----",
                                              base.DebugString);
                return retval;
            }
        }

        #endregion

        #region IAudio Functions

        /// <summary>
        /// the number of bytes of data in AudioStream, always the real size of the file
        /// </summary>
        public override uint NumPayloadBytes
        {
            get
            {
                return (uint)_sourceBuffer.Length;
            }
        }

        /// <summary>
        /// the stream containing the audio data, wound to the start
        /// </summary>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        public override Stream OpenAudioStream()
        {
            MemoryStream str = new MemoryStream(_sourceBuffer);
            return str;
        }

        /// <summary>
        /// calculate sha-1 of the audio data
        /// </summary>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        public override byte[] CalculateAudioSHA1()
        {
            // This is one implementation of the abstract class SHA1.
            SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] result = sha.ComputeHash(this._sourceBuffer);
            return result;
        }

        #endregion
    }
}