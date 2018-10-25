using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApothAudioTools
{
    public class AudioConvertingEventArgs : EventArgs
    {
        public Guid GUID { get; internal set; }

        public string AudioSavedFilePath { get; internal set; }
    }
}
