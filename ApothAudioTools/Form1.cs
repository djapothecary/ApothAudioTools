using ApothAudioTools.Utilities;
using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoLibrary;
using YoutubeExtractor;

namespace ApothAudioTools
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Not implemented
        }

        private void btnDownload_Click_1(object sender, EventArgs e)
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;

            if (!cbxAddList.Checked && tbxUrl.Text != "")
            {
                SingleSyncDownload(tbxUrl.Text);
            }
            else if (cbxAddList.Checked)
            {
                //TODO: Parse list
                var linkList = tbxLinkList.Text;

                var listParser = new Utilities.ListParser();
                var linkinfoList = listParser.BuildList(linkList);

                AsyncDownload(linkinfoList);
            }            

            tbxUrl.ResetText();

        }

        private void Downloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            //update the progress bar
            Invoke(new MethodInvoker(delegate ()
            {
                progressBar.Value = (int)e.ProgressPercentage;
                lblPercentage.Text = $"{string.Format("{0:0.##}", e.ProgressPercentage)}%";
                progressBar.Update();
            }));
        }

        public void VideoDownloader(IEnumerable<VideoInfo> videos)
        {
            
            //VideoInfo video = videos.First(p => p.VideoType == VideoType.Mp4 && p.Resolution == Convert.ToInt32(cmbxResolution.Text));
            VideoInfo video = videos.First(p => p.VideoType == VideoType.Mp4 && p.Resolution == 360);

            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            //download video
            VideoDownloader downloader = new VideoDownloader(video, Path.Combine(@"C:\Users\david.waidmann\Downloads\", video.Title + video.VideoExtension));
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;

            //create a new thread to download the file
            Thread thread = new Thread(() => { downloader.Execute(); }) { IsBackground = true };
            thread.Start();
        }

        public void AudioDownloader(IEnumerable<VideoInfo> videos)
        {
            /*
             * We want the first extractable video with the highest audio quality.
             */
            VideoInfo video = videos
                //.Where(info => info.CanExtractAudio)
                .OrderByDescending(info => info.AudioBitrate)
                .First();

            /*
             * If the video has a decrypted signature, decipher it
             */
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            /*
             * Create the audio downloader.
             * The first argument is the video where the audio should be extracted from.
             * The second argument is the path to save the audio file.
             */
            var audioDownloader = new AudioDownloader(video, Path.Combine(@"C:\Users\david.waidmann\Downloads\", video.Title + video.AudioExtension));

            // Register the progress events. We treat the download progress as 85% of the progress and the extraction progress only as 15% of the progress,
            // because the download will take much longer than the audio extraction.
            audioDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage * 0.85);
            audioDownloader.AudioExtractionProgressChanged += (sender, args) => Console.WriteLine(85 + args.ProgressPercentage * 0.15);

            /*
             * Execute the audio downloader.
             * For GUI applications note, that this method runs synchronously.
             */
            audioDownloader.Execute();
        }

        public void MediaDownloader()
        {
            var source = @"C:\Users\david.waidmann\Downloads\";
            var youtube = YouTube.Default;
            //var vid = youtube.GetVideo("<video url>");

            //sender.ProtocolVersion = HttpVersion.Version10; // THIS DOES THE TRICK
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            var vid = youtube.GetVideo(tbxUrl.Text);
            File.WriteAllBytes(source + vid.FullName, vid.GetBytes());

            var inputFile = new MediaFile { Filename = source + vid.FullName };
            var outputFile = new MediaFile { Filename = $"{source + vid.FullName}.mp3" };

            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
            }
        }

        public void SingleSyncDownload(string link)
        {
            string exportVideoPath = @"C:\Users\david.waidmann\Downloads\Productivity\";
            string exportAudioPath = @"C:\Users\david.waidmann\Downloads\Productivity\";

            var link1 = new LinkInfo(link);

            var downloader = new YTDownloaderBuilder()
                .SetExportAudioPath(exportAudioPath)    //required
                .SetExportVideoPath(exportVideoPath)    //required
                .SetExportOptions(ExportOptions.ExportVideo | ExportOptions.ExportAudio)    //default
                .SetSkipDownloadIfFileExists(false) //default
                .SetLinks(link1)    //check other overloads
                .Build();           

            DownloadResult[] results = downloader.DownloadLinks();  //process the download

            foreach (var res in results)
            {
                Console.WriteLine(res.AudioSavedFilePath);
                Console.WriteLine(res.VideoSavedFilePath);
                Console.WriteLine(res.FileBaseName);
                Console.WriteLine(res.GUID);
                Console.WriteLine(res.DownloadSkipped);
            }

            downloader.AddDownloadStartedAction(link1.GUID, (evArgs) => 
            {
                Console.WriteLine("DOWNLOAD STARTED");
            });

            downloader.AddDownloadFinishedAction(link1.GUID, (evArgs) => 
            {
                Console.WriteLine("DOWNLOAD FINISHED");
            });

            downloader.AddDownloadProgressChangedAction(link1.GUID, (progressArgs) =>
            {
                Invoke(new MethodInvoker(delegate ()
                {
                    progressBar.Value = (int)progressArgs.ProgressPercentage;
                    lblPercentage.Text = $"{string.Format("{0:0.##}", progressArgs.ProgressPercentage)}%";
                    progressBar.Update();
                }));

                Console.WriteLine("Download for link:  " + link1.URL + " " + progressArgs.ProgressPercentage + "%");
            });

            downloader.AddDownloadStartedAction(link1.GUID, (convertArgs) =>
            {
                //Console.WriteLine("Converting audio to path:  " + convertArgs.AudioSavedFilePath);
            });

            downloader.AddAudioConvertingEndedAction(link1.GUID, (convertArgs) =>
            {
                Console.WriteLine("Converting audio complete");
            });
        }

        public void AsyncDownload(List<LinkInfo> linkinfoList)
        {
            string exportVideoPath = @"C:\Users\david.waidmann\Downloads\Productivity\";
            string exportAudioPath = @"C:\Users\david.waidmann\Downloads\Productivity\";

            var downloader = new YTDownloaderBuilder()
                .SetExportAudioPath(exportAudioPath)    //required
                .SetExportVideoPath(exportVideoPath)    //required
                .SetExportOptions(ExportOptions.ExportVideo | ExportOptions.ExportAudio)    //default
                .SetSkipDownloadIfFileExists(false) //default
                .SetLinks(linkinfoList)    //check other overloads
                .Build();            

            Task<DownloadResult[]> results = downloader.DownloadLinksAsync(CancellationToken.None); // process download

            foreach (var link in linkinfoList)
            {
                downloader.AddDownloadStartedAction(link.GUID, (evArgs) =>
                {
                    Console.WriteLine("DOWNLOAD STARTED");
                });

                downloader.AddDownloadFinishedAction(link.GUID, (evArgs) =>
                {
                    Console.WriteLine("DOWNLOAD FINISHED");
                });

                downloader.AddDownloadProgressChangedAction(link.GUID, (progressArgs) =>
                {
                    Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar.Value = (int)progressArgs.ProgressPercentage;
                        lblPercentage.Text = $"{string.Format("{0:0.##}", progressArgs.ProgressPercentage)}%";
                        progressBar.Update();
                    }));

                    //Console.WriteLine("Download for link:  " + link.URL + " " + progressArgs.ProgressPercentage + "%");
                });

                downloader.AddDownloadStartedAction(link.GUID, (convertArgs) =>
                {
                    //Console.WriteLine("Converting audio to path:  " + convertArgs.AudioSavedFilePath);
                });

                //downloader.AddAudioConvertingEndedAction(link.GUID, (convertArgs) =>
                //{
                //    Console.WriteLine("Converting audio for link:  " + link.FileName + " completed.");
                //    //tbxUrl.ResetText();
                //    //progressBar.Value = 0;
                //    //progressBar.Refresh();
                //});
            }
        }

        private void cbxAddList_CheckedChanged(object sender, EventArgs e)
        {
            if (tbxLinkList.Visible)
            {
                tbxLinkList.Visible = false;
            }
            else
            {
                tbxLinkList.Visible = true;
            }
        }
    }
}
