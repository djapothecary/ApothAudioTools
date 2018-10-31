using System;
using System.Drawing;

using ID3Tagging.ID3Lib.Frames;
using ID3Tagging.ID3Lib.Utils;

namespace ID3Tagging.ID3Lib
{
    /// <summary>
    /// Reduce the complexity the tag model to a simple interface
    /// </summary>
    public class TagHandler
    {
        #region Fields

        private TagModel _frameModel;

        private const TextCode TextCode = TextCode.Ascii; // Default text code

        private const string Language = "eng"; // Default language

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the wrapped FrameModel
        /// </summary>
        /// <remarks>
        /// it would be nice to remove this one day, and completely encapsulate a private FrameModel object
        /// </remarks>
        public TagModel FrameModel
        {
            get
            {
                return _frameModel;
            }

            set
            {
                _frameModel = value;
            }
        }

        /// <summary>
        /// Gets or sets the title/song name/content description.
        /// Song is a synonym of the Title
        /// </summary>
        public string Song
        {
            get
            {
                return Title;
            }

            set
            {
                Title = value;
            }
        }

        /// <summary>
        /// Gets or sets the title / song name / content description.
        /// </summary>
        public string Title
        {
            get
            {
                return GetTextFrame("TIT2");
            }

            set
            {
                SetTextFrame("TIT2", value);
            }
        }

        /// <summary>
        /// Gets or sets the lead performer/soloist.
        /// </summary>
        public string Artist
        {
            get
            {
                return GetTextFrame("TPE1");
            }

            set
            {
                SetTextFrame("TPE1", value);
            }
        }

        /// <summary>
        /// Gets or sets the album title.
        /// </summary>
        public string Album
        {
            get
            {
                return GetTextFrame("TALB");
            }

            set
            {
                SetTextFrame("TALB", value);
            }
        }

        /// <summary>
        /// Gets or sets the production year.
        /// </summary>
        public string Year
        {
            get
            {
                return GetTextFrame("TYER");
            }

            set
            {
                SetTextFrame("TYER", value);
            }
        }

        /// <summary>
        /// Gets or sets the composer.
        /// </summary>
        public string Composer
        {
            get
            {
                return GetTextFrame("TCOM");
            }

            set
            {
                SetTextFrame("TCOM", value);
            }
        }

        /// <summary>
        /// Gets or sets the track genre.
        /// </summary>
        public string Genre
        {
            get
            {
                return GetTextFrame("TCON");
            }

            set
            {
                SetTextFrame("TCON", value);
            }
        }

        /// <summary>
        /// Gets or sets the track number.
        /// </summary>
        public string Track
        {
            get
            {
                return GetTextFrame("TRCK");
            }

            set
            {
                SetTextFrame("TRCK", value);
            }
        }

        /// <summary>
        /// Gets or sets the disc number.
        /// </summary>
        /// <remarks>
        /// The 'Part of a set' frame is a numeric string that describes which
        /// part of a set the audio came from. This frame is used if the source
        /// described in the "TALB" frame is divided into several mediums, e.g. a
        /// double CD. The value MAY be extended with a "/" character and a
        /// numeric string containing the total number of parts in the set. E.g.
        /// "1/2".
        /// </remarks>
        public string Disc
        {
            get
            {
                return GetTextFrame("TPOS");
            }

            set
            {
                SetTextFrame("TPOS", value);
            }
        }

        /// <summary>
        /// Gets the length.
        /// the length of the audio file in milliseconds, represented as a numeric string.
        /// </summary>
        public TimeSpan? Length
        {
            get
            {
                string strlen = GetTextFrame("TLEN");
                if (string.IsNullOrEmpty(strlen))
                {
                    return null;
                }                    

                // test for a simple number in the field
                int len;
                if (int.TryParse(strlen, out len))
                {
                    return new TimeSpan(0, 0, 0, 0, len);
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the original padding size.
        /// </summary>
        public uint PaddingSize
        {
            get
            {
                return _frameModel.Header.PaddingSize;
            }
        }

        /// <summary>
        /// Gets the lyrics.
        /// (technically: Unsynchronised lyrics/text transcription)
        /// </summary>
        public string Lyrics
        {
            get
            {
                return GetFullTextFrame("USLT");
            }

            set
            {
                SetFullTextFrame("USLT", value);
            }
        }

        /// <summary>
        /// Gets or sets the track / artist comment.
        /// </summary>
        public string Comment
        {
            get
            {
                return GetFullTextFrame("COMM");
            }

            set
            {
                SetFullTextFrame("COMM", value);
            }
        }

        /// <summary>
        /// Gets or sets the associated picture as System.Drawing.Image, or null reference
        /// </summary>
        /// <value>
        /// The picture.
        /// </value>
        public Image Picture
        {
            get
            {
                var frame = FindFrame("APIC") as FramePicture;
                return frame != null ? frame.Picture : null;
            }

            set
            {
                var frame = FindFrame("APIC") as FramePicture;
                if (frame != null)
                {
                    if (value != null)
                    {
                        frame.Picture = value;
                    }
                    else
                    {
                        _frameModel.Remove(frame);
                    }
                }
                else
                {
                    if (value != null)
                    {
                        var framePic = FrameFactory.Build("APIC") as FramePicture;
                        if (framePic != null)
                        {
                            framePic.Picture = value;
                            this._frameModel.Add(framePic);
                        }
                    }
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the frame text
        /// </summary>
        /// <param name="frameId">
        /// Frame type
        /// </param>
        /// <param name="message">
        /// Value set in frame
        /// </param>
        private void SetTextFrame(string frameId, string message)
        {
            var frame = FindFrame(frameId);
            if (frame != null)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    ((FrameText)frame).Text = message;
                }
                else
                {
                    _frameModel.Remove(frame);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(message))
                {
                    FrameText frameText = (FrameText)FrameFactory.Build(frameId);
                    frameText.Text = message;
                    frameText.TextCode = TextCode;
                    _frameModel.Add(frameText);
                }
            }
        }

        /// <summary>
        /// Get the frame text
        /// </summary>
        /// <param name="frameId">
        /// Frame type
        /// </param>
        /// <returns>
        /// Frame text
        /// </returns>
        private string GetTextFrame(string frameId)
        {
            var frame = FindFrame(frameId);
            if (frame != null)
            {
                return ((FrameText)frame).Text;
            }

            return string.Empty;
        }

        /// <summary>
        /// Set the frame full text
        /// </summary>
        /// <param name="frameId">
        /// Frame type
        /// </param>
        /// <param name="message">
        /// Value set in frame
        /// </param>
        private void SetFullTextFrame(string frameId, string message)
        {
            var frame = FindFrame(frameId);
            if (frame != null)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    FrameFullText framefulltext = (FrameFullText)frame;
                    framefulltext.Text = message;
                    framefulltext.TextCode = TextCode;
                    framefulltext.Description = string.Empty;
                    framefulltext.Language = Language;
                }
                else
                {
                    _frameModel.Remove(frame);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(message))
                {
                    FrameFullText frameLCText = (FrameFullText)FrameFactory.Build(frameId);
                    frameLCText.TextCode = TextCode;
                    frameLCText.Language = "eng";
                    frameLCText.Description = string.Empty;
                    frameLCText.Text = message;
                    _frameModel.Add(frameLCText);
                }
            }
        }

        /// <summary>
        /// Get a full text frame value
        /// </summary>
        /// <param name="frameId">
        /// Frame type
        /// </param>
        /// <returns>
        /// Frame text
        /// </returns>
        private string GetFullTextFrame(string frameId)
        {
            var frame = FindFrame(frameId);
            if (frame != null)
            {
                return ((FrameFullText)frame).Text;
            }

            return string.Empty;
        }

        /// <summary>
        /// Find a frame in the model
        /// </summary>
        /// <param name="frameId">
        /// Frame type
        /// </param>
        /// <returns>
        /// The found frame if found, otherwise null
        /// </returns>
        private FrameBase FindFrame(string frameId)
        {
            foreach (var frame in _frameModel)
            {
                if (frame.FrameId == frameId)
                {
                    return frame;
                }
            }

            return null;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TagHandler"/> class. 
        /// Attach to the TagModel
        /// </summary>
        /// <param name="frameModel">
        /// Frame model to handle
        /// </param>
        public TagHandler(TagModel frameModel)
        {
            _frameModel = frameModel;
        }

        #endregion
    }
}