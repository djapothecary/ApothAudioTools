using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Id3.Net.Frames
{
    public class Id3FrameBuilder
    {
        #region Fields

        public AlbumFrame albumFrame { get; set; }
        public ArtistsFrame artistFrame { get; set; }
        public ArtistUrlFrame artistUrlFrame { get; set; }
        public AudioFileUrlFrame audioFileUrlFrame { get; set; }
        public AudioSourceUrlFrame audioSourceUrlFrame { get; set; }
        public BandFrame bandFrame { get; set; }
        public BeatsPerMinuteFrame beatsPerMinuteFrame { get; set; }
        public CommentFrame commentsFrame { get; set; }
        public CommercialUrlFrame commercialUrlFrame { get; set; }
        public ComposersFrame composerFrame { get; set; }
        public ConductorFrame conductorFrame { get; set; }
        public CopyrightUrlFrame copyrightUrlFrame { get; set; }
        public CustomTextFrame customTextFrame { get; set; }
        public CustomUrlLinkFrame customUrlLinkFrame { get; set; }
        public EncoderFrame encoderFrame { get; set; }
        public FileOwnerFrame fileOwnerFrame { get; set; }
        public GenreFrame genreFrame { get; set; }
        public LyricsFrame lyricsFrame { get; set; }
        public PaymentUrlFrame paymentUrlFrame { get; set; }
        public PictureFrame pictureFrame { get; set; }
        public PrivateFrame privateFrame { get; set; }
        public PublisherFrame publisherFrame { get; set; }
        public RecordingDateFrame recordingDateFrame { get; set; }
        public TitleFrame titleFrame { get; set; }
        public TrackFrame trackFrame { get; set; }
        public UnknownFrame unknownFrame { get; set; }
        public YearFrame yearFrame { get; set; }

        #endregion

        #region Methods

        public AlbumFrame BuildAlbumFrame()
        {
            return null;
        }

        public ArtistsFrame BuildArtistFrame(string videoInfoTitle = null)
        {
            //  TODO:   add parsing to split the title at the first hyphen
            if (videoInfoTitle != null)
            {
                string[] splitFileName = videoInfoTitle.Split(new[] { " - " }, StringSplitOptions.None);
                if (splitFileName.Length <= 1)
                {
                    Console.WriteLine("Multiple hyphens detected in title, returning empty for safety...");
                    return null;
                }
                artistFrame = new ArtistsFrame
                {
                    Value = splitFileName[0].Trim()
                };
                //artistFrame.Value = splitFileName[0].Trim();

                return artistFrame;
            }
            return null;
        }

        public ArtistUrlFrame BuildArtistUrlFrame()
        {
            return null;
        }

        public AudioFileUrlFrame BuildAudioFileUrlFrame()
        {
            return null;
        }

        public AudioSourceUrlFrame BuildAudioSourceUrlFrame()
        {
            return null;
        }

        public BandFrame BuildBandFrame()
        {
            return null;
        }

        public BeatsPerMinuteFrame BuildBeatsPerMinuteFrame()
        {
            return null;
        }

        public CommentFrame BuildCommentFrame()
        {
            return null;
        }

        public CommercialUrlFrame BuildCommercialUrlFrame()
        {
            return null;
        }

        public ComposersFrame BuildComposersFrame()
        {
            return null;
        }

        public ConductorFrame BuildConductorFrame()
        {
            return null;
        }

        public CopyrightUrlFrame BuildCopyrightUrlFrame()
        {
            return null;
        }

        public CustomTextFrame BuildCustomTextFrame()
        {
            return null;
        }

        public CustomUrlLinkFrame BuildCustomUrlLinkFrame()
        {
            return null;
        }

        public EncoderFrame BuildEncoderFrame()
        {
            return null;
        }

        public FileOwnerFrame BuildFileOwnerFrame()
        {
            return null;
        }

        public GenreFrame BuildGenreFrame()
        {
            return null;
        }

        public LyricsFrame BuildLyricsFrame()
        {
            return null;
        }

        public PaymentUrlFrame BuildPaymentUrlFrame()
        {
            return null;
        }

        public PictureFrame BuildPictureFrame()
        {
            return null;
        }

        public PrivateFrame BuildPrivateFrame()
        {
            return null;
        }

        public PublisherFrame BuildPublisherFrame()
        {
            return null;
        }

        public RecordingDateFrame BuildRecordingDateFrame()
        {
            return null;
        }

        public TitleFrame BuildTitleFrame(string videoInfoTitle = null)
        {
            if (videoInfoTitle != null)
            {
                string[] splitTitle = videoInfoTitle.Split(new[] { " - " }, StringSplitOptions.None);
                titleFrame = new TitleFrame();
                if (splitTitle.Length <= 1)
                {
                    Console.WriteLine("Multiple hyphens detected, setting Title to full name");
                    titleFrame.Value = videoInfoTitle;
                }
                else
                {
                    titleFrame.Value = splitTitle[1].Trim();
                }

                return titleFrame;
            }

            return titleFrame;
        }

        public TrackFrame BuildTrackFrame()
        {
            return null;
        }

        public UnknownFrame BuildUnknownFrame()
        {
            return null;
        }

        public YearFrame BuildYearFrame()
        {
            return null;
        }

        #endregion
    }
}
