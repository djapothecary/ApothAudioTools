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
using System.Diagnostics;
using System.Text;

using Id3.Net.Internal;

namespace Id3.Net.Frames
{
    [DebuggerDisplay("{ToString()}")]
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class TextFrame : Id3Frame
    {
        private Id3TextEncoding _encodingType = Id3TextEncoding.Unicode;

        public override void Decode(byte[] data)
        {
            byte encodingByte = data[0];
            string value;
            if (encodingByte == 0 || encodingByte == 1)
            {
                _encodingType = (Id3TextEncoding)encodingByte;
                Encoding encoding = TextEncodingHelper.GetEncoding(_encodingType);
                value = encoding.GetString(data, 1, data.Length - 1);
                if (value.Length > 0 && _encodingType == Id3TextEncoding.Unicode && (value[0] == '\xFFFE' || value[0] == '\xFEFF'))
                {
                    value = value.Remove(0, 1);
                }                    
            }
            else
            {
                _encodingType = Id3TextEncoding.Iso8859_1;
                Encoding encoding = TextEncodingHelper.GetEncoding(_encodingType);
                value = encoding.GetString(data, 0, data.Length);
            }
            Value = value;
        }

        public override byte[] Encode()
        {
            Encoding encoding = TextEncodingHelper.GetEncoding(_encodingType);
            byte[] preamble = encoding.GetPreamble();
            byte[] textBytes = encoding.GetBytes(Value);
            var data = new byte[1 + preamble.Length + textBytes.Length];
            data[0] = (byte)_encodingType;
            preamble.CopyTo(data, 1);
            textBytes.CopyTo(data, preamble.Length + 1);
            return data;
        }

        public override bool Equals(Id3Frame other)
        {
            var text = other as TextFrame;
            return (text != null) && (Value == text.Value);
        }

        public override string ToString()
        {
            return IsAssigned ? Value : string.Empty;
        }

        public override bool IsAssigned
        {
            get
            {
                return !string.IsNullOrEmpty(Value);
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

        public virtual string Value { get; set; }
    }
}