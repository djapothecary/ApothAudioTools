using ID3Tagging.ID3Lib.Utils;
using System;
using System.IO;

namespace ID3Tagging.ID3Lib.Frames
{/// <summary>
 /// Manages predefined URL W*** (not WXXX) Frames
 /// </summary>
 /// <remarks>
 /// URL text string
 /// </remarks>
    [Frame("W")]
    public class FrameUrl : FrameBase
    {
        #region Fields

        private Uri _uri;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameUrl"/> class. 
        /// Create a URL frame
        /// </summary>
        /// <param name="frameId">
        /// Type of URL frame
        /// </param>
        public FrameUrl(string frameId)
            : base(frameId)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The URL page location
        /// </summary>
        public string Url
        {
            get
            {
                return _uri.AbsoluteUri;
            }
        }

        /// <summary>
        ///  The Uri page location
        /// </summary>
        public Uri Uri
        {
            get
            {
                return _uri;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }                    

                this._uri = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parse the binary frame
        /// </summary>
        /// <param name="frame">
        /// binary frame
        /// </param>
        public override void Parse(byte[] frame)
        {
            // TODO: Handle this invalid tag
            if (frame.Length < 1)
            {
                return;
            }                

            var url = TextBuilder.ReadTextEnd(frame, 0, TextCode.Ascii);
            if (Uri.TryCreate(url, UriKind.Absolute, out _uri) == false)
                _uri = null;
        }

        /// <summary>
        /// Create a binary frame
        /// </summary>
        /// <returns>binary frame</returns>
        public override byte[] Make()
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            var url = _uri != null ? _uri.AbsoluteUri : string.Empty;
            writer.Write(TextBuilder.WriteTextEnd(url, TextCode.Ascii));
            return buffer.ToArray();
        }

        /// <summary>
        /// Default frame description
        /// </summary>
        /// <returns>URL text</returns>
        public override string ToString()
        {
            return _uri != null ? _uri.AbsoluteUri : string.Empty;
        }

        #endregion
    }
}