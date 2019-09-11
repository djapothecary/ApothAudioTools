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
    public abstract class Id3Frame : IEquatable<Id3Frame>
    {
        //Accepts the raw frame bytes, decodes them and assigns the values to properties
        public abstract void Decode(byte[] data);

        //Encodes the properties of the class into the raw bytes of the frame
        public abstract byte[] Encode();

        public abstract bool Equals(Id3Frame other);

        public abstract bool IsAssigned { get; }

        public static implicit operator string(Id3Frame frame)
        {
            return frame.ToString();
        }
    }
}