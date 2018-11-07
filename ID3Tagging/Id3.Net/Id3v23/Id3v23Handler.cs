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
using System.IO;

using Id3.Net.Frames;
using Id3.Net.Id3v2x;
using Id3.Net.Internal;

namespace Id3.Net.Id3v23
{
    internal sealed class Id3v23Handler : Id3v2xHandler
    {
        internal override void DeleteTag(Stream stream)
        {
            if (!HasTag(stream))
            {
                return;
            }                

            var buffer = new byte[BufferSize];
            int tagSize = GetTagSize(stream);
            int readPos = tagSize, writePos = 0;
            int bytesRead;
            do
            {
                stream.Seek(readPos, SeekOrigin.Begin);
                bytesRead = stream.Read(buffer, 0, BufferSize);
                if (bytesRead == 0)
                    continue;
                stream.Seek(writePos, SeekOrigin.Begin);
                stream.Write(buffer, 0, bytesRead);
                readPos += bytesRead;
                writePos += bytesRead;
            } while (bytesRead == BufferSize);

            stream.SetLength(stream.Length - tagSize);
            stream.Flush();
        }

        internal override byte[] GetTagBytes(Stream stream)
        {
            if (!HasTag(stream))
            {
                return null;
            }                

            var sizeBytes = new byte[4];
            stream.Seek(6, SeekOrigin.Begin);
            stream.Read(sizeBytes, 0, 4);
            int tagSize = SyncSafeNumber.DecodeSafe(sizeBytes, 0, 4);

            var tagBytes = new byte[tagSize + 10];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(tagBytes, 0, tagBytes.Length);
            return tagBytes;
        }

        internal override bool HasTag(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);

            var headerBytes = new byte[5];
            stream.Read(headerBytes, 0, 5);

            string magic = AsciiEncoding.GetString(headerBytes, 0, 3);
            return magic == "ID3" && headerBytes[3] == 3;
        }

        internal override Id3Tag ReadTag(Stream stream)
        {
            if (!HasTag(stream))
            {
                return null;
            }                

            var tag = Id3Tag.Create<Id3v23Tag>();

            stream.Seek(4, SeekOrigin.Begin);
            var headerBytes = new byte[6];
            stream.Read(headerBytes, 0, 6);

            tag.Header.Revision = headerBytes[0];

            byte flags = headerBytes[1];
            tag.Header.Unsyncronization = (flags & 0x80) > 0;
            tag.Header.ExtendedHeader = (flags & 0x40) > 0;
            tag.Header.Experimental = (flags & 0x20) > 0;

            int tagSize = SyncSafeNumber.DecodeSafe(headerBytes, 2, 4);
            var tagData = new byte[tagSize];
            stream.Read(tagData, 0, tagSize);

            int currentPos = 0;
            if (tag.Header.ExtendedHeader)
            {
                SyncSafeNumber.DecodeSafe(tagData, currentPos, 4);
                currentPos += 4;

                tag.ExtendedHeader.PaddingSize = SyncSafeNumber.DecodeNormal(tagData, currentPos + 2, 4);

                if ((tagData[currentPos] & 0x80) > 0)
                {
                    tag.ExtendedHeader.Crc32 = SyncSafeNumber.DecodeNormal(tagData, currentPos + 6, 4);
                    currentPos += 10;
                } else
                {
                    currentPos += 6;
                }                    
            }

            while (currentPos < tagSize && tagData[currentPos] != 0x00)
            {
                string frameId = AsciiEncoding.GetString(tagData, currentPos, 4);
                currentPos += 4;

                int frameSize = SyncSafeNumber.DecodeNormal(tagData, currentPos, 4);
                currentPos += 4;

                var frameFlags = (ushort)((tagData[currentPos] << 0x08) + tagData[currentPos + 1]);
                currentPos += 2;

                var frameData = new byte[frameSize];
                Array.Copy(tagData, currentPos, frameData, 0, frameSize);

                Id3Frame frame = GetFrameFromFrameId(frameId);
                if (frame != null)
                {
                    frame.Decode(frameData);
                    tag.Frames.Add(frame);
                }

                currentPos += frameSize;
            }

            return tag;
        }

        //internal override bool WriteTag(Stream stream, Id3Tag tag)
        //{
        //    var tag23 = (Id3v23Tag)tag;
        //    byte[] tagBytes = GetTagBytes(tag23);
        //    int requiredTagSize = tagBytes.Length;
        //    if (HasTag(stream))
        //    {
        //        int currentTagSize = GetTagSize(stream);
        //        if (requiredTagSize > currentTagSize)
        //        {
        //            MakeSpaceForTag(stream, currentTagSize, requiredTagSize);
        //        }

        //    }
        //    else
        //    {
        //        MakeSpaceForTag(stream, 0, requiredTagSize);
        //    }

        //    stream.Seek(0, SeekOrigin.Begin);
        //    stream.Write(tagBytes, 0, requiredTagSize);
        //    stream.Flush();

        //    return true;
        //}

        internal override bool WriteTag(MemoryStream stream, Id3Tag tag)
        {
            var tag23 = (Id3v23Tag)tag;
            byte[] tagBytes = GetTagBytes(tag23);
            int requiredTagSize = tagBytes.Length;
            if (HasTag(stream))
            {
                int currentTagSize = GetTagSize(stream);
                if (requiredTagSize > currentTagSize)
                {
                    MakeSpaceForTag(stream, currentTagSize, requiredTagSize);
                }

            }
            else
            {
                MakeSpaceForTag(stream, 0, requiredTagSize);
            }

            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(tagBytes, 0, requiredTagSize);
            stream.Flush();

            return true;
        }

        internal override int MinorVersion
        {
            get
            {
                return 3;
            }
        }

        internal override Type TagType
        {
            get
            {
                return typeof(Id3v23Tag);
            }
        }

        protected override void BuildFrameIdMappings(FrameIdMappings mappings)
        {
            mappings.Add<AlbumFrame>("TALB");
            mappings.Add<ArtistsFrame>("TPE1");
            mappings.Add<ArtistUrlFrame>("WOAR");
            //mappings.Add<AudioEncryptionFrame>("AENC");
            mappings.Add<AudioFileUrlFrame>("WOAF");
            mappings.Add<AudioSourceUrlFrame>("WOAS");
            mappings.Add<BandFrame>("TPE2");
            mappings.Add<BeatsPerMinuteFrame>("TBPM");
            mappings.Add<CommentFrame>("COMM");
            //mappings.Add<CommercialFrame>("COMR");
            mappings.Add<CommercialUrlFrame>("WCOM");
            mappings.Add<ComposersFrame>("TCOM");
            mappings.Add<ConductorFrame>("TPE3");
            //mappings.Add<ContentGroupDescriptionFrame>("TIT1");
            //mappings.Add<CopyrightFrame>("TCOP");
            mappings.Add<CopyrightUrlFrame>("WCOP");
            mappings.Add<CustomTextFrame>("TXXX");
            mappings.Add<CustomUrlLinkFrame>("WXXX");
            mappings.Add<EncoderFrame>("TENC");
            //mappings.Add<EncodingSettingsFrame>("TSSE");
            //mappings.Add<EncryptionMethodRegistrationFrame>("ENCR");
            //mappings.Add<EqualizationFrame>("EQUA");
            //mappings.Add<EventTimingCodesFrame>("ETCO");
            mappings.Add<FileOwnerFrame>("TOWN");
            //mappings.Add<FileTypeFrame>("TFLT");
            //mappings.Add<GeneralEncapsulationObjectFrame>("GEOB");
            mappings.Add<GenreFrame>("TCON");
            //mappings.Add<GroupIdentificationRegistrationFrame>("GRID");
            //mappings.Add<InitialKeyFrame>("TKEY");
            //mappings.Add<InvolvedPeopleFrame>("IPLS");
            //mappings.Add<LanguagesFrame>("TLAN");
            //mappings.Add<LengthFrame>("TLEN");
            //mappings.Add<LinkedInformationFrame>("LINK");
            //mappings.Add<LyricistFrame>("TEXT");
            mappings.Add<LyricsFrame>("USLT");
            //mappings.Add<MediaTypeFrame>("TMED");
            //mappings.Add<ModifiersFrame>("TPE4");
            //mappings.Add<MusicCDIdentifierFrame>("MCDI");
            //mappings.Add<MPEGLocationLookupTableFrame>("MLLT");
            //mappings.Add<OriginalAlbumFrame>("TOAL");
            //mappings.Add<OriginalArtistsFrame>("TOPE");
            //mappings.Add<OriginalFilenameFrame>("TOFN");
            //mappings.Add<OriginalLyricistFrame>("TOLY");
            //mappings.Add<OriginalReleaseYearFrame>("TORY");
            //mappings.Add<OwnershipFrame>("OWNE");
            //mappings.Add<PartOfASetFrame>("TPOS");
            mappings.Add<PaymentUrlFrame>("WPAY");
            mappings.Add<PictureFrame>("APIC");
            //mappings.Add<PlayCounterFrame>("PCNT");
            //mappings.Add<PlaylistDelayFrame>("TDLY");
            //mappings.Add<PopularimeterFrame>("POPM");
            //mappings.Add<PositionSynchronizationFrame>("POSS");
            mappings.Add<PrivateFrame>("PRIV");
            mappings.Add<PublisherFrame>("TPUB");
            //mappings.Add<PublisherUrlFrame>("WPUB");
            //mappings.Add<RadioStationNameFrame>("TRSN");
            //mappings.Add<RadioStationOwnerFrame>("TRSO");
            //mappings.Add<RadioStationUrlFrame>("WORS");
            //mappings.Add<RecommendedBufferSizeFrame>("RBUF");
            mappings.Add<RecordingDateFrame>("TDAT");
            //mappings.Add<RecordingDatesFrame>("TRDA");
            //mappings.Add<RelativeVolumeAdjustmentFrame>("RVAD");
            //mappings.Add<ReverbFrame>("RVRB");
            //mappings.Add<SizeFrame>("TSIZ");
            //mappings.Add<StandardRecordingCodeFrame>("TSRC");
            //mappings.Add<SubtitleFrame>("TIT3");
            //mappings.Add<SynchronizedTempoCodesFrame>("SYTC");
            //mappings.Add<SynchronizedTextFrame>("SYLT");
            //mappings.Add<TermsOfUseFrame>("USER");
            //mappings.Add<TimeFrame>("TIME");
            mappings.Add<TitleFrame>("TIT2");
            mappings.Add<TrackFrame>("TRCK");
            //mappings.Add<UniqueFileIDFrame>("UFID");
            mappings.Add<YearFrame>("TYER");
        }

        private byte[] GetTagBytes(Id3v23Tag tag)
        {
            var bytes = new List<byte>();
            bytes.AddRange(AsciiEncoding.GetBytes("ID3"));
            bytes.AddRange(new byte[] { 3, 0, 0 });
            foreach (Id3Frame frame in tag.Frames)
            {
                if (frame.IsAssigned)
                {
                    byte[] frameBytes = frame.Encode();
                    bytes.AddRange(AsciiEncoding.GetBytes(GetFrameIdFromFrame(frame)));
                    bytes.AddRange(SyncSafeNumber.EncodeNormal(frameBytes.Length));
                    bytes.AddRange(new byte[] { 0, 0 });
                    bytes.AddRange(frameBytes);
                }
            }
            int framesSize = bytes.Count - 6;
            bytes.InsertRange(6, SyncSafeNumber.EncodeSafe(framesSize));
            return bytes.ToArray();
        }

        private static int GetTagSize(Stream stream)
        {
            stream.Seek(6, SeekOrigin.Begin);
            var sizeBytes = new byte[4];
            stream.Read(sizeBytes, 0, 4);
            int tagSize = SyncSafeNumber.DecodeSafe(sizeBytes, 0, 4) + 10;
            return tagSize;
        }

        //private static void MakeSpaceForTag(Stream stream, int currentTagSize, int requiredTagSize)
        //{
        //    if (currentTagSize >= requiredTagSize)
        //    {
        //        return;
        //    }                

        //    int increaseRequired = requiredTagSize - currentTagSize;
        //    var readPos = (int)stream.Length;
        //    int writePos = readPos + increaseRequired;
        //    stream.SetLength(writePos);

        //    var buffer = new byte[BufferSize];
        //    while (readPos > currentTagSize)
        //    {
        //        int bytesToRead = (readPos - BufferSize < currentTagSize) ? readPos - currentTagSize : BufferSize;
        //        readPos -= bytesToRead;
        //        stream.Seek(readPos, SeekOrigin.Begin);
        //        stream.Read(buffer, 0, bytesToRead);
        //        writePos -= bytesToRead;
        //        stream.Seek(writePos, SeekOrigin.Begin);
        //        stream.Write(buffer, 0, bytesToRead);
        //    }
        //}

        private static void MakeSpaceForTag(MemoryStream stream, int currentTagSize, int requiredTagSize)
        {
            if (currentTagSize >= requiredTagSize)
            {
                return;
            }

            int increaseRequired = requiredTagSize - currentTagSize;
            var readPos = (int)stream.Length;
            int writePos = readPos + increaseRequired;
            stream.SetLength(writePos);

            var buffer = new byte[BufferSize];
            while (readPos > currentTagSize)
            {
                int bytesToRead = (readPos - BufferSize < currentTagSize) ? readPos - currentTagSize : BufferSize;
                readPos -= bytesToRead;
                stream.Seek(readPos, SeekOrigin.Begin);
                stream.Read(buffer, 0, bytesToRead);
                writePos -= bytesToRead;
                stream.Seek(writePos, SeekOrigin.Begin);
                stream.Write(buffer, 0, bytesToRead);
            }
        }

        private const int BufferSize = 8192;
    }
}