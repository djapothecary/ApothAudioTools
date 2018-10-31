using ID3Tagging.MP3Lib.Exceptions;
using System;
using System.Text;

namespace ID3Tagging.MP3Lib.Audio
{
    /// <summary>
    /// The audio frame xing header.
    /// </summary>
    class AudioFrameXingHeader : AudioFrame
    {
        #region enums

        /// <summary>
        /// Flags which indicate what fields are present, flags are combined with a logical OR. Field is mandatory.
        /// </summary>
        [FlagsAttribute]
        enum FieldsCode
        {
            /// <summary>
            /// Frames field is present
            /// </summary>
            Frames = 0x0001,

            /// <summary>
            /// Bytes field is present
            /// </summary>
            Bytes = 0x0002,

            /// <summary>
            /// TOC field is present
            /// </summary>
            Toc = 0x0004,

            /// <summary>
            /// Quality indicator field is present
            /// </summary>
            Quality = 0x0008
        }
        #endregion

        #region Fields

        // enum value                   0,  1,  2,  3,  4,  5,  6,  7,  8,  9,  A,  B,  C,  D,  E,  F

        // bytes used
        // 'Xing'                     4   4   4   4   4   4   4   4   4   4   4   4   4   4   4   4
        // Flags                      4   4   4   4   4   4   4   4   4   4   4   4   4   4   4   4
        // Frames                     0   4   0   4   0   4   0   4   0   4   0   4   0   4   0   4
        // Bytes                      0   0   4   4   0   0   4   4   0   0   4   4   0   0   4   4
        // Toc                        0   0   0   0 100 100 100 100   0   0   0   0 100 100 100 100
        // Quality                    0   0   0   0   0   0   0   0   4   4   4   4   4   4   4   4
        static readonly uint[] _offsetFrames = new uint[] { 0, 8, 0, 8, 0, 8, 0, 8, 0, 8, 0, 8, 0, 8, 0, 8 };
        static readonly uint[] _offsetBytes = new uint[] { 0, 0, 8, 12, 0, 0, 8, 12, 0, 0, 8, 12, 0, 0, 8, 12 };
        static readonly uint[] _offsetToc = new uint[] { 0, 0, 0, 0, 8, 12, 12, 16, 0, 0, 0, 0, 8, 12, 12, 16 };
        static readonly uint[] _offsetQuality = new uint[] { 0, 0, 0, 0, 0, 0, 0, 0, 8, 12, 12, 16, 108, 112, 112, 116 };

        #endregion

        #region Internal Properties

        string Tag
        {
            get
            {
                return Encoding.ASCII.GetString(_frameBuffer, (int)_headerBytes, 4);
            }
        }

        // 0          1          2          3
        // space[headersize]                                                                 01234567 89012345 67890123 45678901
        // VBRI header mark
        // 'Xing' or 'Info'                                                               // XING.... ........ ........ ........

        // Flags which indicate what fields are present, flags are combined with a logical OR. Field is mandatory.
        // 0x0001 - Frames field is present
        // 0x0002 - Bytes field is present
        // 0x0004 - TOC field is present
        // 0x0008 - Quality indicator field is present
        FieldsCode Fields
        {
            get
            {
                return (FieldsCode)ParseBigEndianDWORD(_frameBuffer, _headerBytes + 4);
            }
        }

        // Number of Frames (including this header frame) as Big-Endian DWORD (optional)
        uint? XingNumFileFrames
        {
            get
            {
                uint offset = _offsetFrames[(int)Fields];
                if (offset > 0)
                {
                    return ParseBigEndianDWORD(_frameBuffer, _headerBytes + offset);
                }                    
                else
                {
                    return null;
                }                    
            }
        }

        // Number of Bytes in file (including this header frame) as Big-Endian DWORD (optional)
        // or if not present, the number of bytes in audio block, as reported by the caller
        uint? XingNumFileBytes
        {
            get
            {
                uint offset = _offsetBytes[(int)Fields];
                if (offset > 0)
                {
                    return ParseBigEndianDWORD(_frameBuffer, _headerBytes + offset);
                }                    
                else
                {
                    return null;
                }                    
            }
        }

        // 100 TOC entries for seeking as integral BYTE (optional)
        uint TocOffset
        {
            get
            {
                return _offsetToc[(int)Fields];
            }
        }

        //  Quality indicator as Big-Endian DWORD
        // from 0 - best quality to 100 - worst quality (optional)
        uint? QualityIndicator
        {
            get
            {
                uint offset = _offsetQuality[(int)Fields];
                if (offset > 0)
                {
                    return ParseBigEndianDWORD(_frameBuffer, _headerBytes + offset);
                }                    
                else
                {
                    return null;
                }                    
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// some text to show we decoded it correctly
        /// </summary>
        public override string DebugString
        {
            get
            {
                // ----Xing Header----
                // 10567 Frames
                // 4750766 Bytes
                // Quality: 0
                string retval = base.DebugString + "\n----Xing Header----";

                uint? numFrames = NumPayloadFrames;
                if (numFrames != null)
                {
                    retval += string.Format("\n  {0} Frames", numFrames.Value);
                }

                uint? numBytes = NumPayloadBytes;
                if (numBytes != null)
                {
                    retval += string.Format(", {0} Bytes", numBytes.Value);
                }

                uint? quality = QualityIndicator;
                if (quality != null)
                {
                    retval += string.Format("\n  Quality: {0}", quality.Value);
                }

                bool? vbr = IsVbr;
                if (vbr != null)
                {
                    retval += string.Format(", {0}", IsVbr.Value ? "VBR" : "CBR");
                }

                return retval;
            }
        }

        /// <summary>
        /// is it a VBR file?
        /// A file with a 'Xing' header is VBR, an 'Info' header is CBR
        /// </summary>
        public override bool? IsVbr
        {
            get
            {
                string tag = Tag;

                if (tag == "Xing")
                {
                    return true;
                }                    
                else if (tag == "Info")
                {
                    return false;
                }                    
                else
                {
                    throw new InvalidAudioFrameException("Xing/Info tag not recognised");
                }                    
            }
        }

        /// <summary>
        /// Number of Bytes in file (including this header frame) (optional)
        /// or if not present, the number of bytes in audio block, as reported by the caller
        /// </summary>
        public override uint? NumPayloadBytes
        {
            get
            {
                return XingNumFileBytes;
            }
        }

        /// <summary>
        /// Number of bytes playable audio (i.e. excluding this header frame, 
        /// but including the standard header bytes of normal audio frames))
        /// </summary>
        public override uint? NumAudioBytes
        {
            get
            {
                uint? xingBytes = XingNumFileBytes;
                if (xingBytes == null)
                {
                    return null;
                }                    
                else
                {
                    return xingBytes.Value - Header.FrameLengthInBytes;
                }                    
            }
        }

        /// <summary>
        /// Number of Frames in file (including this header frame)
        /// or if not present, null
        /// </summary>
        public override uint? NumPayloadFrames
        {
            get
            {
                return XingNumFileFrames;
            }
        }

        /// <summary>
        /// Number of Frames of playable audio (i.e. excluding this header frame)
        /// </summary>
        public override uint? NumAudioFrames
        {
            get
            {
                uint? xingFrames = XingNumFileFrames;
                if (xingFrames != null)
                {
                    return xingFrames.Value;
                }                    
                else
                {
                    return null;
                }                    
            }
        }

        /// <summary>
        /// Number of seconds from xing header
        /// if not present, then the id3 TLEN tag
        /// </summary>
        public override double? Duration
        {
            get
            {
                uint? numFrames = XingNumFileFrames;
                if (numFrames != null)
                {
                    return (numFrames.Value - 1) * Header.SecondsPerFrame;
                }                    
                else
                {
                    return base.Duration;
                }                    
            }
        }

        /// <summary>
        /// vbr bitrate from xing header frame
        /// returns null where xing header doesn't have both frames and bytes
        /// </summary>
        public override double? BitRateVbr
        {
            get
            {
                uint? numFrames = NumAudioFrames;
                if (numFrames == null)
                {
                    return null;
                }                    

                uint? xingStreamBytes = NumAudioBytes;
                if (xingStreamBytes == null)
                {
                    return null;
                }                    

                double vbrDuration = numFrames.Value * Header.SecondsPerFrame;

                // bitrate is size / time
                return xingStreamBytes.Value / vbrDuration * 8;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioFrameXingHeader"/> class. 
        /// construct XingHeader frame from a pre-existing raw frame; "downcast".
        /// </summary>
        /// <param name="baseclass">
        /// </param>
        public AudioFrameXingHeader(AudioFrame baseclass)
            : base(baseclass)
        {
        }

        #endregion
    }
}