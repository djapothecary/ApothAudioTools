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
using System.Collections.Generic;
using System.Text;

using Id3.Net.Internal;

namespace Id3.Net.Frames
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class CustomUrlLinkFrame : UrlLinkFrame
    {
        private Id3TextEncoding _encodingType = Id3TextEncoding.Unicode;
        private string _description;

        public override void Decode(byte[] data)
        {
            _encodingType = (Id3TextEncoding)data[0];
            byte[][] splitBytes = ByteArrayHelper.SplitBySequence(data, 1, data.Length - 1,
                TextEncodingHelper.GetSplitterBytes(_encodingType));
            string url = null;
            if (splitBytes.Length > 1)
            {
                _description = TextEncodingHelper.GetString(splitBytes[0], 0, splitBytes[0].Length, _encodingType);
                url = TextEncodingHelper.GetDefaultString(splitBytes[1], 0, splitBytes[1].Length);
            }
            else if (splitBytes.Length == 1)
            {
                url = TextEncodingHelper.GetDefaultString(splitBytes[0], 0, splitBytes[0].Length);
            }
                
            Url = url;
        }

        public override byte[] Encode()
        {
            var bytes = new List<byte> {
                (byte)_encodingType
            };

            Encoding encoding = TextEncodingHelper.GetEncoding(_encodingType);
            bytes.AddRange(encoding.GetPreamble());
            if (!string.IsNullOrEmpty(_description))
            {
                bytes.AddRange(encoding.GetBytes(_description));
            }                
            bytes.AddRange(TextEncodingHelper.GetSplitterBytes(_encodingType));
            if (Url != null)
            {
                bytes.AddRange(TextEncodingHelper.GetDefaultEncoding().GetBytes(Url));
            }                

            return bytes.ToArray();
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }

        public Id3TextEncoding EncodingType
        {
            get
            {
                return _encodingType;
            }

            set
            {
                _encodingType = value;
            }
        }
    }
}