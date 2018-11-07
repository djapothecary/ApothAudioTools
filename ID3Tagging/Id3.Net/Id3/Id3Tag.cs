#region --- License & Copyright Notice ---
/*
Copyright (c) 2005-2011 Jeevan James
All rights reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;

using Id3.Net.Frames;

namespace Id3.Net
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class Id3Tag : IComparable<Id3Tag>
    {
        #region Private fields
        //Global list of frames
        private Id3FrameList _frames;

        //Specific single instance frames
        private AlbumFrame _album;
        private ArtistsFrame _artists;
        private AudioFileUrlFrame _audioFileUrl;
        private AudioSourceUrlFrame _audioSourceUrl;
        private BandFrame _band;
        private BeatsPerMinuteFrame _beatsPerMinute;
        private ComposersFrame _composers;
        private ConductorFrame _conductor;
        private CopyrightUrlFrame _copyrightUrl;
        private EncoderFrame _encoder;
        private FileOwnerFrame _fileOwner;
        private GenreFrame _genre;
        private PaymentUrlFrame _paymentUrl;
        private PublisherFrame _publisher;
        private RecordingDateFrame _recordingDate;
        private TitleFrame _title;
        private TrackFrame _track;
        private YearFrame _year;

        //Specific multiple instance frames
        private ArtistUrlFrameList _artistUrls;
        private CommentFrameList _comments;
        private CommercialUrlFrameList _commercialUrls;
        private Id3SyncFrameList<CustomTextFrame> _customTexts;
        private Id3SyncFrameList<LyricsFrame> _lyrics;
        private Id3SyncFrameList<PictureFrame> _pictures;
        private PrivateFrameList _privateData;
        #endregion

        //Converts an ID3 tag to another version after resolving the differences between the two
        //versions. The resultant tag will have all the frames from the source tag, but those
        //frames not recognized in the new version will be treated as UnknownFrame objects.
        //Similarly, frames recognized in the output tag version, but not in the source version are
        //converted accordingly.
        public Id3Tag ConvertTo(int majorVersion, int minorVersion)
        {
            if (MajorVersion == majorVersion && MinorVersion == minorVersion)
            {
                return this;
            }
                
            RegisteredId3Handler sourceHandler = Mp3File.RegisteredHandlers.GetHandler(MajorVersion, MinorVersion);
            if (sourceHandler == null)
            {
                return null;
            }
                
            RegisteredId3Handler destinationHandler = Mp3File.RegisteredHandlers.GetHandler(majorVersion, minorVersion);
            if (destinationHandler == null)
            {
                return null;
            }
                
            Id3Tag destinationTag = destinationHandler.Handler.CreateTag();
            foreach (Id3Frame sourceFrame in Frames)
            {
                if (sourceFrame is UnknownFrame)
                {
                    string frameId = ((UnknownFrame)sourceFrame).Id;
                    Id3Frame destinationFrame = destinationHandler.Handler.GetFrameFromFrameId(frameId);
                    destinationTag.Frames.Add(destinationFrame);
                }
                else
                {
                    destinationTag.Frames.Add(sourceFrame);
                }
                    
            }
            return destinationTag;
        }

        public void MergeWith(params Id3Tag[] tags)
        {
            Array.Sort(tags);
            //TODO:
        }

        #region Metadata properties
        public Id3TagFamily Family { get; internal set; }

        //public int MajorVersion { get; internal set; }
        public int MajorVersion { get; set; }

        //public int MinorVersion { get; internal set; }
        public int MinorVersion { get; set; }
        #endregion

        #region Main frames list
        public Id3FrameList Frames
        {
            get
            {
                return _frames ?? (_frames = new Id3FrameList());
            }
        }
        #endregion

        #region Frame properties
        public AlbumFrame Album
        {
            get
            {
                return GetSingleFrame(ref _album);
            }
        }

        public ArtistsFrame Artists
        {
            get
            {
                return GetSingleFrame(ref _artists);
            }
        }

        public ArtistUrlFrameList ArtistUrls
        {
            get
            {
                return _artistUrls ?? (_artistUrls = new ArtistUrlFrameList(Frames));
            }
        }

        public AudioFileUrlFrame AudioFileUrl
        {
            get
            {
                return GetSingleFrame(ref _audioFileUrl);
            }
        }

        public AudioSourceUrlFrame AudioSourceUrl
        {
            get
            {
                return GetSingleFrame(ref _audioSourceUrl);
            }
        }

        public BandFrame Band
        {
            get
            {
                return GetSingleFrame(ref _band);
            }
        }

        public BeatsPerMinuteFrame BeatsPerMinute
        {
            get
            {
                return GetSingleFrame(ref _beatsPerMinute);
            }
        }

        public CommentFrameList Comments
        {
            get
            {
                return _comments ?? (_comments = new CommentFrameList(Frames));
            }
        }

        public CommercialUrlFrameList CommercialUrls
        {
            get
            {
                return _commercialUrls ?? (_commercialUrls = new CommercialUrlFrameList(Frames));
            }
        }

        public ComposersFrame Composers
        {
            get
            {
                return GetSingleFrame(ref _composers);
            }
        }

        public ConductorFrame Conductor
        {
            get
            {
                return GetSingleFrame(ref _conductor);
            }
        }

        public CopyrightUrlFrame CopyrightUrl
        {
            get
            {
                return GetSingleFrame(ref _copyrightUrl);
            }
        }

        public Id3SyncFrameList<CustomTextFrame> CustomTexts
        {
            get
            {
                return GetMultipleFrames(ref _customTexts);
            }
        }

        public EncoderFrame Encoder
        {
            get
            {
                return GetSingleFrame(ref _encoder);
            }
        }

        public FileOwnerFrame FileOwner
        {
            get
            {
                return GetSingleFrame(ref _fileOwner);
            }
        }

        public GenreFrame Genre
        {
            get
            {
                return GetSingleFrame(ref _genre);
            }
        }

        public Id3SyncFrameList<LyricsFrame> Lyrics
        {
            get
            {
                return GetMultipleFrames(ref _lyrics);
            }
        }

        public PaymentUrlFrame PaymentUrl
        {
            get
            {
                return GetSingleFrame(ref _paymentUrl);
            }
        }

        public PublisherFrame Publisher
        {
            get
            {
                return GetSingleFrame(ref _publisher);
            }
        }

        public Id3SyncFrameList<PictureFrame> Pictures
        {
            get
            {
                return GetMultipleFrames(ref _pictures);
            }
        }

        public PrivateFrameList PrivateData
        {
            get
            {
                return _privateData ?? (_privateData = new PrivateFrameList(Frames));
            }
        }

        public RecordingDateFrame RecordingDate
        {
            get
            {
                return GetSingleFrame(ref _recordingDate);
            }
        }

        public TitleFrame Title
        {
            get
            {
                return GetSingleFrame(ref _title);
            }
        }

        public TrackFrame Track
        {
            get
            {
                return GetSingleFrame(ref _track);
            }
        }

        public YearFrame Year
        {
            get
            {
                return GetSingleFrame(ref _year);
            }
        }
        #endregion

        #region IComparable<Id3Tag> implementation
        public int CompareTo(Id3Tag other)
        {
            if (MajorVersion == 0 && other.MajorVersion == 0)
            {
                return Frames.Count.CompareTo(other.Frames.Count);
            }
                
            if (MajorVersion == other.MajorVersion)
            {
                return MinorVersion == other.MinorVersion
                    ? Frames.Count.CompareTo(other.Frames.Count) : MinorVersion.CompareTo(other.MinorVersion);
            }
                
            return MajorVersion.CompareTo(other.MajorVersion);
        }
        #endregion

        #region Private helper methods
        //Retrieves a single-occuring frame from the main frames list. This method is called from
        //the corresponding property getters.
        //Since each frame already has private field declared, we simply need to get a reference
        //to that field, instead of creating a new object. However, if the field is not available,
        //we create a new one with default values, which is then assigned to the private field.
        //Hence the use of a ref parameter.
        private TFrame GetSingleFrame<TFrame>(ref TFrame frame) where TFrame : Id3Frame, new()
        {
            return frame ?? (frame = Frames.GetOrAdd<TFrame>());
        }

        private Id3SyncFrameList<TFrame> GetMultipleFrames<TFrame>(ref Id3SyncFrameList<TFrame> frames) where TFrame : Id3Frame
        {
            return frames ?? (frames = new Id3SyncFrameList<TFrame>(Frames));
        }
        #endregion

        #region Static factory methods
        public static TTag Create<TTag>() where TTag : Id3Tag
        {
            RegisteredId3Handler registeredHandler = Mp3File.RegisteredHandlers.GetHandler<TTag>();
            return registeredHandler != null ? (TTag)registeredHandler.Handler.CreateTag() : null;
        }

        public static Id3Tag Create(int majorVersion, int minorVersion)
        {
            RegisteredId3Handler registeredHandler = Mp3File.RegisteredHandlers.GetHandler(majorVersion, minorVersion);
            return registeredHandler != null ? registeredHandler.Handler.CreateTag() : null;
        }

        public static readonly Id3Tag[] Empty = new Id3Tag[0];
        #endregion

        #region Other static members
        public static Id3Tag Merge(params Id3Tag[] tags)
        {
            if (tags.Length == 0)
            {
                throw new ArgumentNullException("tags", "Specify 2 or more tags to merge");
            }                

            if (tags.Length == 1)
            {
                return tags[0];
            }                

            var tag = new Id3Tag();
            tag.MergeWith(tags);
            return tag;
        }
        #endregion
    }
}