using System;
using System.Diagnostics;
using System.IO;

using ID3Tagging.ID3Lib;
using ID3Tagging.ID3Lib.Exceptions;
using ID3Tagging.MP3Lib.Audio;
using ID3Tagging.MP3Lib.Exceptions;
using ID3Tagging.MP3Lib.Utils;

namespace ID3Tagging.MP3Lib.MP3
{
    /// <summary>
    /// Manage MP3 file data with ID3v2 tags and audio data stream.
    /// </summary>
    internal class Mp3FileData
    {
        #region Fields

        /// <summary>
        /// name of source file
        /// </summary>
        private readonly FileInfo _sourceFileInfo;

        /// <summary>
        /// Current MP3Audio object - if re-assigned, owned by assigner.
        /// </summary>
        private IAudio _audio;

        /// <summary>
        /// set to true if the audio is replaced
        /// </summary>
        private bool _audioReplaced;

        /// <summary>
        /// offset from start of original stream that the original audio started at
        /// </summary>
        private uint _audioStart;

        /// <summary>
        /// ID3v2 tag model at start of file
        /// </summary>
        private TagHandler _tagHandler;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Mp3FileData"/> class. 
        /// Construct from file info; parse ID3 tags from stream and calculate where the audio must be
        /// </summary>
        /// <param name="fileinfo">
        /// </param>
        public Mp3FileData(FileInfo fileinfo)
        {
            this._sourceFileInfo = fileinfo;

            // create an empty frame model, to use if we don't parse anything better
            TagModel tagModel = new TagModel();

            // don't know how big the audio is until we've parsed the tags
            UInt32 audioNumBytes;

            using (FileStream sourceStream = fileinfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // all the header calculations use UInt32; 
                // this guarantees all the file offsets we have to deal with fit in a UInt32
                if (sourceStream.Length > UInt32.MaxValue)
                {
                    throw new InvalidAudioFrameException("MP3 file can't be bigger than 4gb");
                }

                // in the absence of any recognised tags,
                // audio starts at the start
                this._audioStart = 0;

                // audio is entire file length
                audioNumBytes = (UInt32)sourceStream.Length;

                // try to read an ID3v1 block.
                // If ID3v2 block exists, its values overwrite these
                // Otherwise, if ID3V1 block exists, its values are used
                // The audio is anything that's left after all the tags are excluded.
                try
                {
                    ID3v1 id3v1 = new ID3v1();
                    id3v1.Deserialize(sourceStream);

                    // fill in ID3v2 block from the ID3v1 data
                    tagModel = id3v1.FrameModel;

                    // audio is shorter by the length of the id3v1 tag
                    audioNumBytes -= ID3v1.TagLength;
                }
                catch (TagNotFoundException)
                {
                    // ignore "no ID3v1 block"
                    // everything else isn't caught here, and throws out to the caller
                }

                try
                {
                    sourceStream.Seek(0, SeekOrigin.Begin);
                    tagModel = TagManager.Deserialize(sourceStream);

                    // audio starts after the tag
                    this._audioStart = (uint)sourceStream.Position;

                    // audio is shorter by the length of the id3v2 tag
                    audioNumBytes -= this._audioStart;
                }
                catch (TagNotFoundException)
                {
                    // ignore "no ID3v2 block"
                    // everything else isn't caught here, and throws out to the caller
                }

                // create a taghandler to hold the tagmodel we've parsed, if any
                this._tagHandler = new TagHandler(tagModel);
            }

            // closes sourceStream

            // save the location of the audio in the original file
            // passing in audio size and id3 length tag (if any) to help with bitrate calculations
            this._audio = new AudioFile(fileinfo, this._audioStart, audioNumBytes, this._tagHandler.Length);
            this._audioReplaced = false;
        }

        #endregion

        #region CacheDataState enum

        /// <summary>
        /// possible return values for update functions
        /// </summary>
        public enum CacheDataState
        {
            /// <summary>
            /// data is still valid and can be used again
            /// </summary>
            eClean,

            /// <summary>
            /// data is now out of date; delete and recreate it
            /// </summary>
            eDirty
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets wrapper for the object containing the audio payload
        /// </summary>
        public IAudio Audio
        {
            get
            {
                return this._audio;
            }

            set
            {
                // this destroys the original one
                // but as it only holds names we don't need to dispose it
                this._audio = value;

                // set a flag so we know we can't update in-situ when they call update
                this._audioReplaced = true;
            }
        }

        /// <summary>
        /// Gets or sets ID3v2 tags represented by the Frame Model
        /// </summary>
        public TagModel TagModel
        {
            get
            {
                return this._tagHandler.FrameModel;
            }

            set
            {
                this._tagHandler.FrameModel = value;
            }
        }

        /// <summary>
        /// Gets or sets ID3v2 tags wrapped in an interpreter that gives you real properties for each supported frame 
        /// </summary>
        public TagHandler TagHandler
        {
            get
            {
                return this._tagHandler;
            }

            set
            {
                this._tagHandler = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Update ID3V2 and V1 tags in-situ if possible, or rewrite the file to add tags if necessary.
        /// Always creates a backup.
        /// </summary>
        /// <returns><c>CacheDataState.eDirty</c> if the MP3FileData object is dirty after this</returns>
        public CacheDataState Update()
        {
            return this.Update(true);
        }

        /// <summary>
        /// Update ID3V2 and V1 tags in-situ if possible, or rewrite the file to add tags if necessary.
        /// </summary>
        /// <param name="createBackup">
        /// if <c>true</c>, create a backup file
        /// </param>
        /// <returns>
        /// <c>CacheDataState.eDirty</c> if the MP3FileData object is dirty after this
        /// </returns>
        public CacheDataState Update(bool createBackup)
        {
            if (!this.TagModel.IsValid)
            {
                // the standard does not allow an id3v2 tag with no frames in, 
                // so we must remove it completely.
                return this.UpdateNoV2tag(createBackup);
            }
            else
            {
                this.TagModel.UpdateSize();
                uint tagSizeComplete = this.TagModel.Header.TagSizeWithHeaderFooter;

                if (tagSizeComplete <= this._audioStart && !this._audioReplaced)
                {
                    this.UpdateInSitu(tagSizeComplete);
                    return CacheDataState.eClean;
                }
                else
                {
                    // calculate enough padding to round final file to 2k cluster size.
                    uint minLength = tagSizeComplete + this._audio.NumPayloadBytes + ID3v1.TagLength;
                    uint newLength = (minLength + 2047) & 0xFFFFF800; // round up to whole 2k cluster
                    this.TagModel.Header.PaddingSize = newLength - minLength;

                    this.RewriteFile(createBackup ? this.CreateBackupFileInfo() : null);
                    return CacheDataState.eDirty;
                }
            }
        }

        /// <summary>
        /// rewrite the file and ID3V2 and V1 tags with no padding.
        /// Always creates a backup.
        /// </summary>
        /// <returns><c>CacheDataState.eDirty</c> if the MP3FileData object is dirty after this</returns>
        public CacheDataState UpdatePacked()
        {
            return this.UpdatePacked(true);
        }

        /// <summary>
        /// rewrite the file and ID3V2 and V1 tags with no padding.
        /// </summary>
        /// <param name="createBackup">
        /// if <c>true</c>, create a backup file
        /// </param>
        /// <returns>
        /// <c>CacheDataState.eDirty</c> if the MP3FileData object is dirty after this
        /// </returns>
        public CacheDataState UpdatePacked(bool createBackup)
        {
            if (this.TagModel.Count == 0)
            {
                // the standard does not allow an id3v2 tag with no frames in, 
                // so we must remove it completely.
                // this removes the padding too, of course.
                return this.UpdateNoV2tag(createBackup);
            }
            else
            {
                this.TagModel.UpdateSize();
                this.TagModel.Header.PaddingSize = 0;

                this.RewriteFile(createBackup ? this.CreateBackupFileInfo() : null);
                return CacheDataState.eDirty;
            }
        }

        /// <summary>
        /// Update file and remove ID3V2 tag (if any); 
        /// update file in-situ if possible, or rewrite the file to remove tag if necessary.
        /// Always creates a backup.
        /// </summary>
        /// <returns><c>CacheDataState.eDirty</c> if the MP3FileData object is dirty after this</returns>
        public CacheDataState UpdateNoV2tag()
        {
            return this.UpdateNoV2tag(true);
        }

        /// <summary>
        /// Update file and remove ID3V2 tag (if any); 
        /// update file in-situ if possible, or rewrite the file to remove tag if necessary.
        /// Always creates a backup.
        /// </summary>
        /// <param name="createBackup">
        /// if <c>true</c>, create a backup file
        /// </param>
        /// <returns>
        /// <c>CacheDataState.eDirty</c> if the MP3FileData object is dirty after this
        /// </returns>
        public CacheDataState UpdateNoV2tag(bool createBackup)
        {
            if (this._audioStart == 0 && !this._audioReplaced)
            {
                // no v2 tag to start with; just update v1 tag
                this.UpdateInSituNoV2tag();
                return CacheDataState.eClean;
            }
            else
            {
                this.RewriteFileNoV2tag(createBackup ? this.CreateBackupFileInfo() : null);
                return CacheDataState.eDirty;
            }
        }

        /// <summary>
        /// The copy audio stream.
        /// </summary>
        /// <param name="writeStream">
        /// The write stream.
        /// </param>
        public void CopyAudioStream(FileStream writeStream)
        {
            // open original mp3 file stream and seek to the start of the audio
            // or, if the audio's been replaced, create a memorystream from the buffer.
            using (Stream audio = this._audio.OpenAudioStream())
            {
                // Copy mp3 stream
                // this will also copy the original ID3v1 tag if present, 
                // but it's only 128 bytes extra so it won't matter.
                const int Size = 4096;
                byte[] bytes = new byte[4096];
                int numBytes;
                while ((numBytes = audio.Read(bytes, 0, Size)) > 0)
                {
                    writeStream.Write(bytes, 0, numBytes);
                }
            }
        }

        /// <summary>
        /// append or overwrite ID3v1 tag at the end of the audio
        /// </summary>
        /// <param name="stream">
        /// </param>
        public void WriteID3v1(Stream stream)
        {
            ID3v1 v1tag = new ID3v1();
            v1tag.FrameModel = this.TagModel;

            stream.Seek(this._audioStart + this._audio.NumPayloadBytes, SeekOrigin.Begin);
            v1tag.Write(stream);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates a <see cref="FileInfo"/> to use for backup
        /// </summary>
        /// <returns>
        /// a new <see cref="FileInfo"/>
        /// </returns>
        private FileInfo CreateBackupFileInfo()
        {
            string bakName = Path.ChangeExtension(this._sourceFileInfo.FullName, "bak");
            return new FileInfo(bakName);
        }

        /// <summary>
        /// UpdateInSitu
        /// </summary>
        /// <remarks>
        /// doesn't make a backup as it's only modifying the tags not re-writing the audio
        /// </remarks>
        /// <param name="tagSizeComplete">
        /// </param>
        private void UpdateInSitu(uint tagSizeComplete)
        {
            Debug.Assert(this.TagModel.Header.Footer == false, "if tag is before the audio, it shouldn't have a footer");

            // calculate enough padding to fill the gap between tag and audio start
            this.TagModel.Header.PaddingSize = this._audioStart - tagSizeComplete;

            // open source file in readwrite mode
            using (FileStream writeStream = this._sourceFileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                // write header, tags and padding to start of audio
                TagManager.Serialize(this.TagModel, writeStream);

                Debug.Assert(writeStream.Position == this._audioStart, "we haven't filled the gap exactly");

                // now overwrite or append the ID3V1 tag
                this.WriteID3v1(writeStream);
            }
        }

        /// <summary>
        /// create new output file stream and write the ID3v2 block to it
        /// </summary>
        /// <remarks>
        /// makes a backup as it's modifying the tags and re-writing the audio.
        /// Always need to reinitialise the mp3 file wrapper if you use it after this runs
        /// </remarks>
        /// <param name="bakFileInfo">
        /// location of backup file - must be on same drive
        /// could be null if no backup required
        /// </param>
        private void RewriteFile(FileInfo bakFileInfo)
        {
            // generate a temp filename in the target's directory
            string tempName = Path.ChangeExtension(this._sourceFileInfo.FullName, "$$$");
            FileInfo tempFileInfo = new FileInfo(tempName);

            using (FileStream writeStream = tempFileInfo.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                // write an ID3v2 tag to new file
                TagManager.Serialize(this.TagModel, writeStream);

                uint newAudioStart = (uint)writeStream.Position;

                this.CopyAudioStream(writeStream);

                // if the stream copies without error, update the start of the audio
                this._audioStart = newAudioStart;

                // now overwrite or append the ID3v1 tag to new file
                this.WriteID3v1(writeStream);
            }

            // replace the original file, delete new file if fail
            try
            {
                FileMover.FileMove(tempFileInfo, this._sourceFileInfo, bakFileInfo);
            }
            catch
            {
                tempFileInfo.Delete();
                throw;
            }
        }

        /// <summary>
        /// UpdateInSituNoV2tag
        /// </summary>
        /// <remarks>
        /// doesn't make a backup as it's only modifying the tags not re-writing the audio
        /// </remarks>
        private void UpdateInSituNoV2tag()
        {
            // open source file in readwrite mode
            using (FileStream writeStream = this._sourceFileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                // just overwrite or append the ID3V1 tag
                this.WriteID3v1(writeStream);
            }
        }

        /// <summary>
        /// create new output file stream and don't write the ID3v2 block to it
        /// </summary>
        /// <remarks>
        /// makes a backup as it's re-writing the audio
        /// Always need to re-initialise the mp3 file wrapper if you use it after this runs
        /// </remarks>
        /// <param name="bakFileInfo">
        /// location of backup file - must be on same drive
        /// could be null if no backup required
        /// </param>
        private void RewriteFileNoV2tag(FileInfo bakFileInfo)
        {
            // generate a temp filename in the target's directory
            string tempName = Path.ChangeExtension(this._sourceFileInfo.FullName, "$$$");
            FileInfo tempFileInfo = new FileInfo(tempName);

            using (FileStream writeStream = tempFileInfo.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                this.CopyAudioStream(writeStream);

                // if the stream copies without error, update the start of the audio
                this._audioStart = 0;

                // now overwrite or append the ID3v1 tag to new file
                this.WriteID3v1(writeStream);
            }

            // replace the original file, delete new file if fail
            try
            {
                FileMover.FileMove(tempFileInfo, this._sourceFileInfo, bakFileInfo);
            }
            catch
            {
                tempFileInfo.Delete();
                throw;
            }
        }

        #endregion
    }
}