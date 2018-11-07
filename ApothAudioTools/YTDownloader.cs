using Id3.Net;
using Id3.Net.Frames;
using Id3.Net.Id3v23;
using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace ApothAudioTools
{
    public class YTDownloader
    {
        #region Settings

        /// <summary>
        /// gets or sets the video export directory
        /// </summary>
        private string ExportVideoDirPath { get; set; }

        /// <summary>
        /// gets or sets the audio export directory
        /// </summary>
        private string ExportAudioDirPath { get; set; }

        /// <summary>
        /// gets or sets the export options
        /// </summary>
        private ExportOptions ExportOptions { get; set; }

        /// <summary>
        /// gets or sets the skipped video
        /// </summary>
        private bool SkipVideosWhichExists { get; set; }
        #endregion

        private IDictionary<Guid, Engine> engines = new Dictionary<Guid, Engine>();

        private IDictionary<Guid, VideoDownloader> videoDownloaders = new Dictionary<Guid, VideoDownloader>();

        private IList<Task<DownloadResult>> tasks = new List<Task<DownloadResult>>();

        private IList<LinkInfo> linksToProcess = new List<LinkInfo>();

        private Dictionary<Guid, Action<AudioConvertingEventArgs>> beforeConvertingActions = new Dictionary<Guid, Action<AudioConvertingEventArgs>>();

        private Dictionary<Guid, Action<AudioConvertingEventArgs>> afterConvertingActions = new Dictionary<Guid, Action<AudioConvertingEventArgs>>();

        #region AudioEngineAPI

        public void AddAudioConvertingStartedAction(Guid guid, Action<AudioConvertingEventArgs> action)
        {
            beforeConvertingActions.Add(guid, action);
        }

        public void AddAudioConvertingEndedAction(Guid guid, Action<AudioConvertingEventArgs> action)
        {
            afterConvertingActions.Add(guid, action);
        }

        #endregion

        #region VideoDownloaderAPI

        public void AddDownloadProgressChangedAction(Guid guid, Action<ProgressEventArgs> action)
        {
            videoDownloaders[guid].DownloadProgressChanged += (sender, args) => { action.Invoke(args); };
        }

        public void AddDownloadStartedAction(Guid guid, Action<EventArgs> action)
        {
            videoDownloaders[guid].DownloadStarted += (sender, args) => { action.Invoke(args); };
        }

        public void AddDownloadFinishedAction(Guid guid, Action<EventArgs> action)
        {
            videoDownloaders[guid].DownloadFinished += (sender, args) => { action.Invoke(args); };
        }

        #endregion

        public YTDownloader(YTDownloaderBuilder builder, IList<LinkInfo> links)
        {
            InitBuilder(builder);

            foreach (var linkInfo in links)
            {
                engines.Add(linkInfo.GUID, new Engine());
                videoDownloaders.Add(linkInfo.GUID, new VideoDownloader());
                linksToProcess.Add(linkInfo);
            }
        }

        public YTDownloader(YTDownloaderBuilder builder, string[] urls)
        {
            InitBuilder(builder);

            foreach (var url in urls)
            {
                var linkInfo = new LinkInfo(url);

                engines.Add(linkInfo.GUID, new Engine());
                videoDownloaders.Add(linkInfo.GUID, new VideoDownloader());
                linksToProcess.Add(linkInfo);
            }
        }

        private void InitBuilder(YTDownloaderBuilder builder)
        {
            this.ExportAudioDirPath = builder.ExportAudioDirPath;
            this.ExportVideoDirPath = builder.ExportVideoDirPath;
            this.ExportOptions = builder.ExportOptions;
            this.SkipVideosWhichExists = builder.SkipVideosWhichExists;
        }

        public DownloadResult[] DownloadLinks()
        {
            return ProcessDownloads(CancellationToken.None).Result;
        }

        public async Task<DownloadResult[]> DownloadLinksAsync(CancellationToken token)
        {
            return await ProcessDownloads(token);
        }

        private async Task<DownloadResult[]> ProcessDownloads(CancellationToken token)
        {
            foreach (var linkInfo in linksToProcess)
            {
                videoDownloaders[linkInfo.GUID].DownloadProgressChanged += (sender, args) => { token.ThrowIfCancellationRequested(); };

                Task<DownloadResult> downloadTask = Task.Run(() =>
                 {
                     string videoName;
                     string videoFilePath;

                     var videoInfo = GetVideoInfo(linkInfo, out videoName, out videoFilePath);

                     //test if the video file exists
                     if (File.Exists(videoFilePath) && SkipVideosWhichExists)
                     {
                         Console.WriteLine("Skipping download for file:  " + videoInfo.Title.ToString());
                         return new DownloadResult()
                         {
                             VideoSavedFilePath = videoFilePath,
                             GUID = linkInfo.GUID,
                             AudioSavedFilePath = null,
                             FileBaseName = videoName,
                             DownloadSkipped = true,
                             IsId3Tagged = false
                         };
                     }

                     //test if the converted audio exists
                     //TODO: try building skip mechanics here
                     var testFile = videoFilePath.Replace(".mp4", ".mp3");
                     if (File.Exists(testFile))
                     {
                         //don't download
                         Console.WriteLine("Skipping download for file:  " + videoInfo.Title.ToString());
                         return new DownloadResult()
                         {
                             VideoSavedFilePath = videoFilePath,
                             GUID = linkInfo.GUID,
                             AudioSavedFilePath = null,
                             FileBaseName = videoName,
                             DownloadSkipped = true,
                             IsId3Tagged = false
                         };
                     }

                     DownloadVideo(videoInfo, linkInfo.GUID, videoFilePath);

                     if (ExportOptions.HasFlag(ExportOptions.ExportAudio))
                     {
                         if (videoInfo != null)
                         {
                             DownloadAudio(linkInfo, videoFilePath, videoInfo.VideoExtension);
                         }
                         else
                         {
                             //Console.WriteLine("Video is empty, abandoning task...");
                             Console.WriteLine("Video is empty, skipping task...");
                             //tasks.RemoveAt((int)Task.CurrentId);
                             return new DownloadResult()
                             {
                                 VideoSavedFilePath = videoFilePath,
                                 GUID = linkInfo.GUID,
                                 AudioSavedFilePath = null,
                                 FileBaseName = videoName,
                                 DownloadSkipped = true,
                                 IsId3Tagged = false
                             };
                         }                         
                     }

                     if (ExportOptions.HasFlag(ExportOptions.ExportAudio))
                     {
                         if (!string.IsNullOrEmpty(videoFilePath))
                         {
                             File.Delete(videoFilePath);
                         }
                         else
                         {
                             Console.WriteLine("Video is empty, abandoning task...");
                             tasks.RemoveAt((int)Task.CurrentId);
                             return new DownloadResult()
                             {
                                 VideoSavedFilePath = videoFilePath,
                                 GUID = linkInfo.GUID,
                                 AudioSavedFilePath = null,
                                 FileBaseName = videoName,
                                 DownloadSkipped = true,
                                 IsId3Tagged = false
                             };
                         }
                     }

                     var transformedAudioPath = TransformToAudioPath(videoFilePath, videoInfo.VideoExtension);
                     //// tag the download
                     ID3TagSingleDownload(transformedAudioPath, videoInfo);

                     return new DownloadResult()
                     {
                         VideoSavedFilePath = videoFilePath,
                         GUID = linkInfo.GUID,
                         AudioSavedFilePath = transformedAudioPath,
                         FileBaseName = videoName,
                         DownloadSkipped = false,
                         IsId3Tagged = true
                     };

                 }, token);

                tasks.Add(downloadTask);
            }
            return await Task.WhenAll(tasks);
        }

        private VideoInfo GetVideoInfo(LinkInfo linkInfo, out string videoName, out string videoFilePath)
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(linkInfo.URL, false);

            // Select the first video by type with highest AudioBitrate
            VideoInfo videoInfo = videoInfos
                ?.OrderByDescending(info => info.AudioBitrate)
                ?.First();

            string fileBaseName = string.Empty;
            videoFilePath = string.Empty;
            videoName = string.Empty;

            //wrap whole thing in null object check
            if (videoInfo != null)
            {
                // This is must, cause we decrypting only this video
                if (videoInfo.RequiresDecryption)
                {
                    DownloadUrlResolver.DecryptDownloadUrl(videoInfo);
                }
                
                fileBaseName = linkInfo.FileName == null ? videoInfo.Title.ToSafeFileName() : linkInfo.FileName.ToSafeFileName();
                videoName = fileBaseName;
                videoFilePath = Path.Combine(ExportVideoDirPath, fileBaseName + videoInfo.VideoExtension);
                return videoInfo;
            }

            return videoInfo;
        }

        private void DownloadVideo(VideoInfo videoInfo, Guid linkGUID, string videoFilePath)
        {
            var videoDownloader = videoDownloaders[linkGUID];

            videoDownloader.Init(videoInfo, videoFilePath);
            videoDownloader.Execute();
        }

        private void DownloadAudio(LinkInfo linkInfo, string videoFilePath, string videoExtension)
        {
            string audioOutputPath = TransformToAudioPath(videoFilePath, videoExtension);

            Action<AudioConvertingEventArgs> beforeAction;

            if (beforeConvertingActions.TryGetValue(linkInfo.GUID, out beforeAction))
            {
                beforeConvertingActions[linkInfo.GUID].Invoke(new AudioConvertingEventArgs() { GUID = linkInfo.GUID, AudioSavedFilePath = audioOutputPath });
            }                

            var inputFile = new MediaFile { Filename = videoFilePath };
            var outputFile = new MediaFile { Filename = audioOutputPath };

            var engine = engines[linkInfo.GUID];

            engine.Convert(inputFile, outputFile);

            Action<AudioConvertingEventArgs> afterAction;

            if (afterConvertingActions.TryGetValue(linkInfo.GUID, out afterAction))
            {
                afterConvertingActions[linkInfo.GUID].Invoke(new AudioConvertingEventArgs() { GUID = linkInfo.GUID, AudioSavedFilePath = audioOutputPath });
            }                

            engine.Dispose();
        }

        private string TransformToAudioPath(string videoFilePath, string videoExtension)
        {
            string audioOutputPath = videoFilePath
                                    .Replace(ExportVideoDirPath, ExportAudioDirPath)
                                    .ReplaceLastOccurrence(videoExtension, ".mp3");
            return audioOutputPath;
        }

        private void ID3TagSingleDownload(string musicFile, VideoInfo videoinfo)
        {
            //  make sure there is a file
            if (musicFile != null)
            {
                //  tag it
                using (var mp3 = new Mp3File(musicFile))
                {
                    //Id3Tag tag = mp3.GetTag(Id3TagFamily.FileEndTag);
                    //Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2x);
                    //Id3Tag tag = Id3Tag.Create(2, 1);
                    Id3Tag tag = Id3Tag.Create<Id3v23Tag>();

                    Id3FrameBuilder id3FrameBuilder = new Id3FrameBuilder();

                    //  build major and minor versions first off
                    tag.MajorVersion = 1;
                    tag.MinorVersion = 1;

                    tag.Artists.Value = id3FrameBuilder.BuildArtistFrame(videoinfo.Title);
                    //tag.AudioFileUrl //not implemented yet
                        
                    tag.Title.Value = id3FrameBuilder.BuildTitleFrame(videoinfo.Title);

                    //  TODO:   write the tag now that it has values
                    mp3.WriteTag(tag, WriteConflictAction.NoAction);
                    //mp3.WriteTag(tag, 2, 1, WriteConflictAction.NoAction);
                }
            }
        }

        private void ID3TagDownloadDirectory(string[] musicFiles)
        {
            //set the scan directory if musicFiles is empty
            if (musicFiles == null)
            {
                //  TODO:   set the directory in the app/web page
                musicFiles = Directory.GetFiles(@"C:\Users\david.waidmann\Downloads\Productivity\", "*.mp3");
            }

            foreach (string musicFile in musicFiles)
            {
                using (var mp3 = new Mp3File(musicFile))
                {
                    Id3Tag tag = mp3.GetTag(Id3TagFamily.FileStartTag);
                    Console.WriteLine("Title: {0}", tag.Title.Value);
                    Console.WriteLine("Artist: {0}", tag.Artists.Value);
                    Console.WriteLine("Album: {0}", tag.Album.Value);
                }
            }
        }
    }
}
