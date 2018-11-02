using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApothAudioTools
{
    public class DownloadResult
    {
        /// <summary>
        /// gets or sets if the download is skipped
        /// </summary>
        public bool DownloadSkipped { get; set; }

        /// <summary>
        /// gets or sets the video save path
        /// </summary>
        public string VideoSavedFilePath { get; set; }

        /// <summary>
        /// gets or sets the audio save path
        /// </summary>
        public string AudioSavedFilePath { get; set; }

        /// <summary>
        /// gets or sets the base file name
        /// </summary>
        public string FileBaseName { get; set; }

        /// <summary>
        /// gets or sets the GUID
        /// </summary>
        public Guid GUID { get; set; }

        /// <summary>
        /// gets or sets if the Download Result has been tagged
        /// allow nulls
        /// </summary>
        public bool? IsId3Tagged { get; set; }
    }
}
