using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace ApothAudioTools
{
    public class LinkInfo
    {
        #region Properties
        /// <summary>
        /// gets or sets the URL
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// gets or sets the guid
        /// </summary>
        public Guid GUID { get; set; }

        /// <summary>
        /// gets or sets the file name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// gets or sets the video type
        /// </summary>
        public VideoType VideoType { get; set; }

        #endregion

        #region Methods

        public LinkInfo(string url, string fileNameToBeSaved = null, VideoType videoType = VideoType.Mp4)
        {
            this.GUID = Guid.NewGuid();
            this.URL = url;
            this.FileName = fileNameToBeSaved;
            this.VideoType = VideoType;
        }

        #endregion
    }
}
