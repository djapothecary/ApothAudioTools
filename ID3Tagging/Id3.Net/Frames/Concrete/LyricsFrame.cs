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

using Id3.Net.Internal;

namespace Id3.Net.Frames
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class LyricsFrame : Id3Frame
    {
        private Id3TextEncoding _encodingType = Id3TextEncoding.Unicode;
        private Id3Language _language = Id3Language.eng;
        private string _description;
        private string _lyrics;

        public override void Decode(byte[] data)
        {
            _encodingType = (Id3TextEncoding)data[0];

            string language = TextEncodingHelper.GetDefaultEncoding().GetString(data, 1, 3).ToLowerInvariant();
            if (!Enum.IsDefined(typeof(Id3Language), language))
            {
                _language = Id3Language.eng;
            }                
            else
            {
                _language = (Id3Language)Enum.Parse(typeof(Id3Language), language, true);
            }                

            string[] splitStrings = TextEncodingHelper.GetSplitStrings(data, 4, data.Length - 4, _encodingType);
            if (splitStrings.Length > 1)
            {
                _description = splitStrings[0];
                _lyrics = splitStrings[1];
            }
            else if (splitStrings.Length == 1)
            {
                _lyrics = splitStrings[0];
            }                
        }

        public override byte[] Encode()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool Equals(Id3Frame other)
        {
            var lyricsFrame = other as LyricsFrame;
            return (lyricsFrame != null) && (lyricsFrame.Language == Language) && (lyricsFrame.Description == Description);
        }

        #region Public properties
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

        public override bool IsAssigned
        {
            get
            {
                return !string.IsNullOrEmpty(_lyrics);
            }
        }

        public Id3Language Language
        {
            get
            {
                return _language;
            }

            set
            {
                _language = value;
            }
        }

        public string Lyrics
        {
            get
            {
                return _lyrics;
            }

            set
            {
                _lyrics = value;
            }
        }
        #endregion
    }
}