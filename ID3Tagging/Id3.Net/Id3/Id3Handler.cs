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
using System.IO;
using System.Linq;

using Id3.Net.Frames;

namespace Id3.Net
{
    //Represents an ID3 handler, which can manipulate ID3 tags in an MP3 file stream.
    //There is a derived class for each version of ID3 supported by this framework.
    internal abstract class Id3Handler
    {
        //Contains the mappings from the frame ID string to the corresponding ID3 frame class type
        //for this handler.
        private FrameIdMappings _frameIdMappings;

        internal Id3Tag CreateTag()
        {
            var tag = (Id3Tag)Activator.CreateInstance(TagType);
            tag.MajorVersion = MajorVersion;
            tag.MinorVersion = MinorVersion;
            tag.Family = Family;
            return tag;
        }

        internal Id3Frame GetFrameFromFrameId(string frameId)
        {
            Type frameType;
            if (!FrameIdMappings.TryGetValue(frameId, out frameType))
            {
                var unknownFrame = new UnknownFrame {
                    Id = frameId
                };
                return unknownFrame;
            }
            var frame = (Id3Frame)Activator.CreateInstance(frameType);
            return frame;
        }

        internal string GetFrameIdFromFrame(Id3Frame frame)
        {
            var unknownFrame = frame as UnknownFrame;
            if (unknownFrame != null)
            {
                return unknownFrame.Id;
            }                

            Type frameType = frame.GetType();

            //Check whether custom frame IDs have been defined for this frame type, and if they're
            //defined for this handler's version, then return that.
            if (frameType.IsDefined(typeof(Id3FrameAttribute), false))
            {
                var frameAttributes = (Id3FrameAttribute[])frameType.GetCustomAttributes(typeof(Id3FrameAttribute), false);
                Id3FrameAttribute frameAttribute = frameAttributes.FirstOrDefault(fa => fa.MajorVersion == MajorVersion && fa.MinorVersion == MinorVersion);
                if (frameAttribute != null)
                {
                    return frameAttribute.FrameId;
                }                    
            }

            return (from mapping in FrameIdMappings
                where mapping.Value == frameType
                select mapping.Key).FirstOrDefault();
        }

        //Stream-manipulation overrides for the handler
        internal abstract void DeleteTag(Stream stream);
        internal abstract byte[] GetTagBytes(Stream stream);
        internal abstract bool HasTag(Stream stream);
        internal abstract Id3Tag ReadTag(Stream stream);
        //internal abstract bool WriteTag(Stream stream, Id3Tag tag);
        internal abstract bool WriteTag(MemoryStream stream, Id3Tag tag);

        //ID3 tag properties for the handler
        internal abstract Id3TagFamily Family { get; }
        internal abstract int MajorVersion { get; }
        internal abstract int MinorVersion { get; }
        internal abstract Type TagType { get; }

        //Override this in each derived handler to specify the valid frame types for the handler and
        //the frame IDs to which they correspond.
        protected abstract void BuildFrameIdMappings(FrameIdMappings mappings);

        protected FrameIdMappings FrameIdMappings
        {
            get
            {
                if (_frameIdMappings == null)
                {
                    _frameIdMappings = new FrameIdMappings();
                    BuildFrameIdMappings(_frameIdMappings);
                }
                return _frameIdMappings;
            }
        }
    }
}