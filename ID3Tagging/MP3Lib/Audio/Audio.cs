using ID3Tagging.MP3Lib.Exceptions;
using ID3Tagging.MP3Lib.MP3;
using System;
using System.Diagnostics;
using System.IO;

namespace ID3Tagging.MP3Lib.Audio
{
    /// <summary>
    /// The audio.
    /// </summary>
    internal abstract class Audio : IAudio
    {
        #region Fields

        /// <summary>
        /// first audio frame; could be could be xing or vbri header
        /// </summary>
        protected AudioFrame _firstFrame;

        /// <summary>
        /// duration of the audio block, as parsed from the optional ID3v2 "TLEN" tag
        /// the Xing/VBRI header is more authoritative for bitrate calculations, if present.
        /// </summary>
        protected TimeSpan? _id3DurationTag;

        /// <summary>
        /// number of frames and audio bytes - obtained by counting the entire file - slow!
        /// </summary>
        private AudioStats _audioStats;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Audio"/> class. 
        /// construct object to wrap the audio
        /// passing in audio size and id3 length tag (if any) to help with bitrate calculations
        /// </summary>
        /// <param name="firstFrame">
        /// The <see cref="AudioFrame"/> to save as first frame
        /// </param>
        /// <param name="id3DurationTag">
        /// length from ID3v2, if any
        /// </param>
        protected Audio(AudioFrame firstFrame, TimeSpan? id3DurationTag)
        {
            if (firstFrame == null)
            {
                throw new InvalidAudioFrameException("MPEG Audio Frame not found");
            }

            this._firstFrame = firstFrame;
            this._id3DurationTag = id3DurationTag;

            /*_hasInconsistencies = false;*/
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the mp3 frame header number of bytes of audio data in AudioStream
        /// </summary>
        public AudioFrameHeader Header
        {
            get
            {
                return this._firstFrame.Header;
            }
        }

        /// <summary>
        /// text info, e.g. the encoding standard of audio data in AudioStream
        /// </summary>
        public virtual string DebugString
        {
            get
            {
                // ----AudioFrame----
                // Payload: 10336 frames in 4750766 bytes
                // Length: 270 seconds
                // 140 kbit
                // Counted: 10567 frames in 4750766 bytes
                string retval =
                    string.Format(
                        "{0}\n----Audio----\n  Payload: {1} frames in {2} bytes\n  Length: {3:N3} seconds\n  {4:N4} kbit",
                        this._firstFrame.DebugString,
                        this.NumPayloadFrames,
                        this.NumPayloadBytes,
                        this.Duration,
                        this.BitRate);

                if (this._audioStats._numFrames > 0)
                {
                    retval += string.Format(
                        "\n  Counted: {0} frames, {1} bytes",
                        this._audioStats._numFrames,
                        this._audioStats._numBytes);
                }

                return retval;
            }
        }

        /// <summary>
        /// Gets if it is a VBR file? i.e. is it better encoding quality than cbr at the same bitrate?
        /// first we make a guess based on the audio header found in the first frame.
        /// </summary>
        /// <remarks>
        /// if the frame didn't have any strong opinions,
        /// we don't check if the mp3 audio header bitrate is the same as the calculated bitrate
        /// because a truncated file shows up as vbr (because the bitrates don't match)
        /// and we just return false.
        /// </remarks>
        public virtual bool IsVbr
        {
            get
            {
                bool? frameVbr = this._firstFrame.IsVbr;
                if (frameVbr != null)
                {
                    return frameVbr.Value;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets if it has a VBR (VBRI, XING, INFO, LAME) header
        /// </summary>
        public bool HasVbrHeader
        {
            get
            {
                // return true if it's no longer the base class 'AudioFrame'
                return this._firstFrame.GetType() != typeof(AudioFrame);
            }
        }

        /// <summary>
        /// Gets the number of bytes of data in the Audio payload
        /// always the real size of the file
        /// supplied by AudioFile or AudioBuffer
        /// </summary>
        public virtual uint NumPayloadBytes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the Number of bytes playable audio, VBR header priority, best for calculating bitrates
        /// </summary>
        /// <remarks>
        /// if there is no xing/vbri header, it's the same as NumPayloadBytes
        /// if the xing header doesn't have the audio bytes filled in, 
        /// it can still return 'don't know, but you need to take one header off the file length'
        /// </remarks>
        public uint NumAudioBytes
        {
            get
            {
                uint? numAudioBytes = this._firstFrame.NumAudioBytes;
                if (numAudioBytes != null)
                {
                    return numAudioBytes.Value;
                }

                if (this.HasVbrHeader)
                {
                    // vbr header will never be free bitrate, because they're vbr instead
                    uint? frameLengthInBytes = this._firstFrame.Header.FrameLengthInBytes;
                    if (frameLengthInBytes == null)
                    {
                        throw new InvalidAudioFrameException("VBR files cannot be 'free' bitrate");
                    }

                    return this.NumPayloadBytes - (uint)this._firstFrame.Header.FrameLengthInBytes;
                }

                return this.NumPayloadBytes;
            }
        }

        /// <summary>
        /// Gets the Number of Frames in file (including the header frame)
        /// VBR header priority, best for calculating bitrates
        /// or if not present, calculated from the number of bytes in the audio block, as reported by the caller
        /// This will be correct for CBR files, at least.
        /// </summary>
        public uint NumPayloadFrames
        {
            get
            {
                uint? numPayloadFrames = this._firstFrame.NumPayloadFrames;
                if (numPayloadFrames.HasValue)
                {
                    return numPayloadFrames.Value;
                }

                double? framelength = this.Header.IdealisedFrameLengthInBytes;
                if (framelength.HasValue)
                {
                    return (uint)Math.Round(this.NumPayloadBytes / framelength.Value);
                }

                return this.NumPayloadBytes / this._firstFrame.FrameLengthInBytes;
            }
        }

        /// <summary>
        /// Gets the Number of Frames of playable audio
        /// </summary>
        /// <remarks>
        /// if there is no xing/vbri header, it's the same as NumPayloadFrames
        /// if the xing header doesn't have the audio frames filled in, 
        /// it can still return 'don't know, but you need to take one header off the file length'
        /// </remarks>
        public uint NumAudioFrames
        {
            get
            {
                if (this.HasVbrHeader)
                {
                    return this.NumPayloadFrames - 1;
                }
                else
                {
                    return this.NumPayloadFrames;
                }
            }
        }

        /// <summary>
        /// Gets the number of seconds for bitrate calculations.
        /// first get it from the xing/vbri headers,
        /// then from the id3 TLEN tag,
        /// then from the file size and initial frame bitrate.
        /// </summary>
        public double Duration
        {
            get
            {
                double? headerDuration = this._firstFrame.Duration;
                if (headerDuration != null)
                {
                    return headerDuration.Value;
                }
                else if (this._id3DurationTag != null)
                {
                    return this._id3DurationTag.Value.TotalSeconds;
                }
                else
                {
                    return this.NumAudioFrames * this.Header.SecondsPerFrame;
                }
            }
        }

        /// <summary>
        /// Gets the bitrate calculated from the id3 length tag, and the length of the audio
        /// </summary>
        public double? BitRateCalc
        {
            get
            {
                if (this._id3DurationTag == null)
                {
                    return null;
                }

                return this.NumPayloadBytes * 8 / this._id3DurationTag.Value.TotalSeconds;
            }
        }

        /// <summary>
        /// Gets the bitrate published in the standard mp3 header
        /// </summary>
        public uint? BitRateMp3
        {
            get
            {
                return this.Header.BitRate;
            }
        }

        /// <summary>
        /// Gets the vbr bitrate from xing or vbri header frame
        /// audio without xing or vbri header returns null
        /// </summary>
        public double? BitRateVbr
        {
            get
            {
                return this._firstFrame.BitRateVbr;
            }
        }

        /// <summary>
        /// Gets the overall best guess of bitrate; there's always a way of guessing it
        /// </summary>
        public double BitRate
        {
            get
            {
                // get best guess at duration from derived classes, or id3 TLEN tag, or first frame bitrate
                double duration = this.Duration;

                // get best guess at numbytes from derived classes, or audio length
                uint numBytes = this.NumAudioBytes;

                // bitrate is size / time
                return numBytes / duration * 8;
            }
        }

        /// <summary>
        /// Gets whether it parsed without any errors
        /// </summary>
        public bool HasInconsistencies { get; private set; }

        /// <summary>
        /// Gets the error from the parse operation, if any
        /// </summary>
        /// <remarks>
        /// should the parse operation save all thrown exceptions here,
        /// and not generate it on demand?
        /// </remarks>
        public Exception ParsingError
        {
            get
            {
                uint? vbrPayloadBytes = this._firstFrame.NumPayloadBytes;
                if (vbrPayloadBytes != null && vbrPayloadBytes != this.NumPayloadBytes)
                {
                    return new InvalidVbrSizeException(this.NumPayloadBytes, vbrPayloadBytes.Value);
                }
                else
                {
                    return null;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The open audio stream.
        /// </summary>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public virtual Stream OpenAudioStream()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The calculate audio cr c 32.
        /// </summary>
        /// <returns>
        /// The <see cref="uint"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public virtual uint CalculateAudioCRC32()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The calculate audio sh a 1.
        /// </summary>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// </exception>
        public virtual byte[] CalculateAudioSHA1()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Count frames and bytes of file to see who's telling porkies
        /// </summary>
        public void ScanWholeFile()
        {
            this._audioStats._numFrames = 0;
            this._audioStats._numBytes = 0;

            using (Stream stream = this.OpenAudioStream())
            {
                uint payloadStart = (uint)stream.Position;
                try
                {
                    while (true)
                    {
                        uint pos = (uint)stream.Position;
                        uint used = pos - payloadStart;
                        uint remainingBytes = this.NumPayloadBytes - used;

                        AudioFrame frame = AudioFrameFactory.CreateFrame(stream, remainingBytes);
                        if (frame == null)
                        {
                            break;
                        }

                        ++this._audioStats._numFrames;
                        this._audioStats._numBytes += frame.FrameLengthInBytes;

                        // Trace.WriteLine(string.Format("frame {0} ({1} bytes) found at {2}",
                        // _audioStats._numFrames,
                        // frame.Header.FrameLengthInBytes,
                        // stream.Position - frame.Header.FrameLengthInBytes));
                    }
                }
                catch (Exception e)
                {
                    this.HasInconsistencies = true;
                    Trace.WriteLine(e.Message);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the consistency check result.
        /// </summary>
        protected void CheckConsistency()
        {
            Exception ex = this.ParsingError;
            if (ex != null)
            {
                this.HasInconsistencies = true;
                Trace.WriteLine(ex.Message);
            }
        }

        #endregion

        private struct AudioStats
        {
            #region Fields

            /// <summary>
            /// The _num frames.
            /// </summary>
            public uint _numFrames;

            /// <summary>
            /// The _num bytes.
            /// </summary>
            public uint _numBytes;

            #endregion
        }
    }
}