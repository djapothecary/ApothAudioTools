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

namespace Id3.Net.Frames
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class UnknownFrame : Id3Frame
    {
        private byte[] _data;

        public override void Decode(byte[] data)
        {
            _data = data;
        }

        public override byte[] Encode()
        {
            return _data ?? new byte[0];
        }

        public override bool Equals(Id3Frame other)
        {
            return false;
        }

        public string Id { get; set; }

        public override bool IsAssigned
        {
            get
            {
                return _data != null && _data.Length > 0;
            }
        }
    }
}