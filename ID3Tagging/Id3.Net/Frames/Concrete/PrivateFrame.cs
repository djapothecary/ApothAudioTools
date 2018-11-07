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

using Id3.Net.Internal;

namespace Id3.Net.Frames
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class PrivateFrame : Id3Frame
    {
        private string _ownerId;
        private byte[] _data;

        public override void Decode(byte[] data)
        {
            byte[] splitterSequence = TextEncodingHelper.GetSplitterBytes(Id3TextEncoding.Iso8859_1);
            byte[] ownerIdBytes = ByteArrayHelper.GetBytesUptoSequence(data, 0, splitterSequence);
            _ownerId = TextEncodingHelper.GetString(ownerIdBytes, 0, ownerIdBytes.Length, Id3TextEncoding.Iso8859_1);
            _data = new byte[data.Length - ownerIdBytes.Length - splitterSequence.Length];
            Array.Copy(data, ownerIdBytes.Length + splitterSequence.Length - 1, _data, 0, _data.Length);
        }

        public override byte[] Encode()
        {
            var bytes = new List<byte>();

            bytes.AddRange(TextEncodingHelper.GetEncoding(Id3TextEncoding.Iso8859_1).GetBytes(_ownerId));
            bytes.AddRange(TextEncodingHelper.GetSplitterBytes(Id3TextEncoding.Iso8859_1));
            bytes.AddRange(_data ?? new byte[0]);
            return bytes.ToArray();
        }

        public override bool Equals(Id3Frame other)
        {
            var privateFrame = other as PrivateFrame;
            return (privateFrame != null) && (privateFrame.OwnerId == OwnerId) && (ByteArrayHelper.AreEqual(privateFrame.Data, Data));
        }

        #region Public properties
        public byte[] Data
        {
            get
            {
                return _data;
            }

            set
            {
                _data = value;
            }
        }

        public override bool IsAssigned
        {
            get
            {
                return Data != null && Data.Length > 0;
            }
        }

        public string OwnerId
        {
            get
            {
                return _ownerId;
            }

            set
            {
                _ownerId = value;
            }
        }
        #endregion
    }

#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class PrivateFrameList : Id3SyncFrameList<PrivateFrame>
    {
        internal PrivateFrameList(Id3FrameList mainList)
            : base(mainList)
        {
        }

        public PrivateFrame[] ByOwnerId(string ownerId)
        {
            return FindAll(frame => frame.OwnerId.Equals(ownerId, StringComparison.OrdinalIgnoreCase));
        }
    }
}