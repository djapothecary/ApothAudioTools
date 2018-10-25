using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApothAudioTools
{
    public class AudioConvertingEventArgs : EventArgs
    {
        /// <summary>
        /// gets or sets the GUID
        /// </summary>
        public Guid GUID { get; internal set; }

        /// <summary>
        /// gets or sets the audio save path
        /// </summary>
        public string AudioSavedFilePath { get; internal set; }
    }
}
