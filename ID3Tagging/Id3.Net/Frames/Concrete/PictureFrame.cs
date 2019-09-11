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
    public sealed class PictureFrame : Id3Frame
    {
        public PictureFrame()
        {
            EncodingType = Id3TextEncoding.Unicode;
        }

        public override void Decode(byte[] data)
        {
            EncodingType = (Id3TextEncoding)data[0];

            byte[] mimeType = ByteArrayHelper.GetBytesUptoSequence(data, 1, new byte[] { 0x00 });
            if (mimeType == null)
            {
                MimeType = "image/";
                return;
            }
            MimeType = TextEncodingHelper.GetDefaultString(mimeType, 0, mimeType.Length);

            int currentPos = mimeType.Length + 2;
            PictureType = (PictureType)data[currentPos];

            currentPos++;
            byte[] description = ByteArrayHelper.GetBytesUptoSequence(data, currentPos, TextEncodingHelper.GetSplitterBytes(EncodingType));
            if (description == null)
            {
                return;
            }
                
            Description = TextEncodingHelper.GetString(description, 0, description.Length, EncodingType);

            currentPos += description.Length + TextEncodingHelper.GetSplitterBytes(EncodingType).Length;
            PictureData = new byte[data.Length - currentPos];
            Array.Copy(data, currentPos, PictureData, 0, PictureData.Length);
        }

        public override byte[] Encode()
        {
            var bytes = new List<byte> {
                (byte)EncodingType
            };

            Encoding defaultEncoding = TextEncodingHelper.GetDefaultEncoding();
            bytes.AddRange(!string.IsNullOrEmpty(MimeType) ? defaultEncoding.GetBytes(MimeType) : defaultEncoding.GetBytes("image/"));

            bytes.Add(0);
            bytes.Add((byte)PictureType);

            Encoding descriptionEncoding = TextEncodingHelper.GetEncoding(EncodingType);
            bytes.AddRange(descriptionEncoding.GetPreamble());
            if (!string.IsNullOrEmpty(Description))
            {
                bytes.AddRange(descriptionEncoding.GetBytes(Description));
            }
                
            bytes.AddRange(TextEncodingHelper.GetSplitterBytes(EncodingType));

            if (PictureData != null && PictureData.Length > 0)
            {
                bytes.AddRange(PictureData);
            }                

            return bytes.ToArray();
        }

        public override bool Equals(Id3Frame other)
        {
            var picture = other as PictureFrame;
            return (picture != null) && (PictureType == picture.PictureType);
        }

        public string GetExtension()
        {
            if (string.IsNullOrEmpty(MimeType))
            {
                return "jpg";
            }                
            string[] parts = MimeType.Split('/');
            if (parts.Length < 2 || string.IsNullOrEmpty(parts[1]))
            {
                return "jpg";
            }
                
            return parts[1];
        }

        public override bool IsAssigned
        {
            get
            {
                return PictureData != null && PictureData.Length > 0;
            }
        }

        public string Description { get; set; }

        public Id3TextEncoding EncodingType { get; set; }

        public string MimeType { get; set; }

        public byte[] PictureData { get; set; }

        public PictureType PictureType { get; set; }
    }

    public enum PictureType : byte
    {
        Other = 0x00,
        FileIcon = 0x01,
        OtherFileIcon = 0x02,
        FrontCover = 0x03,
        BackCover = 0x04,
        LeafletPage = 0x05,
        Media = 0x06,
        LeadArtistPerformerSoloist = 0x07,
        ArtistOrPerformer = 0x08,
        Conductor = 0x09,
        BandOrOrchestra = 0x0A,
        Composer = 0x0B,
        LyricistOrTextWriter = 0x0C,
        RecordingLocation = 0x0D,
        DuringRecording = 0x0E,
        DuringPerformance = 0x0F,
        MovieOrVideoScreenCapture = 0x10,
        ABrightColouredFish = 0x11,
        Illustration = 0x12,
        BandOrArtistLogotype = 0x13,
        PublisherOrStudioLogotype = 0x14
    }
}