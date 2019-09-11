using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApothAudioTools
{
    public class YTDownloaderBuilder
    {
        #region Properties

        /// <summary>
        /// gets or sets the video export directory
        /// </summary>
        public string ExportVideoDirPath { get; private set; }

        /// <summary>
        /// gets or sets the audio export directory
        /// </summary>
        public string ExportAudioDirPath { get; private set; }

        /// <summary>
        /// gets or sets the export options
        /// </summary>
        public ExportOptions ExportOptions { get; private set; }

        /// <summary>
        /// gets or sets the skipped videos
        /// </summary>
        public bool SkipVideosWhichExists { get; private set; }

        private IList<LinkInfo> links;

        #endregion

        #region Methods

        public YTDownloaderBuilder()
        {
            links = new List<LinkInfo>();
            this.ExportOptions = ExportOptions.ExportVideo | ExportOptions.ExportAudio;
            this.ExportAudioDirPath = null;
            this.ExportVideoDirPath = null;
            //this.SkipVideosWhichExists = false;
            this.SkipVideosWhichExists = true;
        }

        public YTDownloader Build()
        {
            CheckDirPath(ExportAudioDirPath);
            CheckDirPath(ExportVideoDirPath);

            return new YTDownloader(this, links);
        }

        public YTDownloaderBuilder SetLinks(IList<LinkInfo> links)
        {
            this.links.Clear();

            foreach (var link in links)
            {
                this.links.Add(link);
            }

            return this;
        }

        public YTDownloaderBuilder SetLinks(params LinkInfo[] links)
        {
            this.links.Clear();

            foreach (var link in links)
            {
                this.links.Add(link);
            }

            return this;
        }

        public YTDownloaderBuilder SetLinks(params string[] urls)
        {
            this.links.Clear();

            foreach (var url in urls)
            {
                this.links.Add(new LinkInfo(url));
            }

            return this;
        }

        public YTDownloaderBuilder SetExportOptions(ExportOptions opt)
        {
            this.ExportOptions = opt;
            return this;
        }

        public YTDownloaderBuilder SetExportAudioPath(string path)
        {
            this.ExportAudioDirPath = path;
            return this;
        }

        public YTDownloaderBuilder SetExportVideoPath(string path)
        {
            this.ExportVideoDirPath = path;
            return this;
        }

        public YTDownloaderBuilder SetSkipDownloadIfFileExists(bool skipDownloadIfFileExists)
        {
            this.SkipVideosWhichExists = skipDownloadIfFileExists;
            return this;
        }

        public void CheckDirPath(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new InvalidOperationException("Path to export directory was not set");
            }

            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format("Directory path {0} is invalid:  ", path) + ex.Message);
            }
        }
        #endregion
    }
}
