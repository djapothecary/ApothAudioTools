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
    //Used on frame classes to specify version-specific frame IDs for the given frame type.
    //Useful when certain ID3 frames have different frame IDs depending on the ID3 version
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class Id3FrameAttribute : Attribute
    {
        private readonly int _majorVersion;
        private readonly int _minorVersion;
        private readonly string _frameId;

        public Id3FrameAttribute(int majorVersion, int minorVersion, string frameId)
        {
            _majorVersion = majorVersion;
            _minorVersion = minorVersion;
            _frameId = frameId;
        }

        public string FrameId
        {
            get
            {
                return _frameId;
            }
        }

        public int MajorVersion
        {
            get
            {
                return _majorVersion;
            }
        }

        public int MinorVersion
        {
            get
            {
                return _minorVersion;
            }
        }
    }
}