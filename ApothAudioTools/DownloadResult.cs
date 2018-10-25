﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApothAudioTools
{
    public class DownloadResult
    {
        public bool DownloadSkipped { get; set; }

        public string VideoSavedFilePath { get; set; }

        public string AudioSavedFilePath { get; set; }

        public string FileBaseName { get; set; }

        public Guid GUID { get; set; }
    }
}
