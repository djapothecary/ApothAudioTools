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

using Id3.Net.Internal;

namespace Id3.Net.Frames
{
    [DebuggerDisplay("{ToString()}")]
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class UrlLinkFrame : Id3Frame
    {
        public override void Decode(byte[] data)
        {
            Url = TextEncodingHelper.GetDefaultString(data, 0, data.Length);
        }

        public override byte[] Encode()
        {
            return Url != null ? TextEncodingHelper.GetDefaultEncoding().GetBytes(Url) : new byte[0];
        }

        public override bool Equals(Id3Frame other)
        {
            var urlLink = other as UrlLinkFrame;
            return (urlLink != null) && (Url == urlLink.Url);
        }

        public override string ToString()
        {
            return IsAssigned ? Url : string.Empty;
        }

        public override bool IsAssigned
        {
            get
            {
                return !string.IsNullOrEmpty(Url);
            }
        }

        public string Url { get; set; }
    }
}