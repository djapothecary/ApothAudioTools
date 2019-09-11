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
    public sealed class CommentFrame : Id3Frame
    {
        private Id3TextEncoding _encodingType = Id3TextEncoding.Unicode;
        private Id3Language _language = Id3Language.eng;
        private string _description;
        private string _comment;

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
                _comment = splitStrings[1];
            }
            else if (splitStrings.Length == 1)
            {
                _comment = splitStrings[0];
            }                
        }

        public override byte[] Encode()
        {
            var bytes = new List<byte> {
                (byte)_encodingType
            };

            bytes.AddRange(TextEncodingHelper.GetDefaultEncoding().GetBytes(_language.ToString()));

            Encoding encoding = TextEncodingHelper.GetEncoding(_encodingType);
            bytes.AddRange(encoding.GetPreamble());
            if (!string.IsNullOrEmpty(_description))
            {
                bytes.AddRange(encoding.GetBytes(_description));
            }                
            bytes.AddRange(TextEncodingHelper.GetSplitterBytes(_encodingType));
            bytes.AddRange(encoding.GetPreamble());
            if (!string.IsNullOrEmpty(_comment))
            {
                bytes.AddRange(encoding.GetBytes(_comment));
            }                

            return bytes.ToArray();
        }

        public override bool Equals(Id3Frame other)
        {
            var comment = other as CommentFrame;
            return (comment != null) && (comment.Language == Language) && (comment.Description == Description);
        }

        #region Public properties
        public string Comment
        {
            get
            {
                return _comment;
            }

            set
            {
                _comment = value;
            }
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

        public override bool IsAssigned
        {
            get
            {
                return !string.IsNullOrEmpty(_comment);
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
        #endregion
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class CommentFrameList : Id3SyncFrameList<CommentFrame>
    {
        public CommentFrameList(Id3FrameList mainList)
            : base(mainList)
        {
        }

        public CommentFrame[] ByLanguage(Id3Language language)
        {
            return FindAll(commentFrame => commentFrame.Language == language);
        }

        public CommentFrame[] ByDescription(string description)
        {
            return FindAll(frame => frame.Description.Equals(description, StringComparison.OrdinalIgnoreCase));
        }

        public CommentFrame ByLanguageAndDescription(Id3Language language, string description)
        {
            return Find(frame => (frame.Language == language) && frame.Description.Equals(description, StringComparison.OrdinalIgnoreCase));
        }
    }
}