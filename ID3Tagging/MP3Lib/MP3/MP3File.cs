using System.IO;

using ID3Tagging.ID3Lib;

namespace ID3Tagging.MP3Lib.MP3
{
    /// <summary>
    /// Manage MP3 file ID3v2 tags and audio data stream.
    /// </summary>
    public class Mp3File
    {
        #region Fields

        /// <summary>
        /// the <see cref="FileInfo" /> of the source file
        /// </summary>
        private FileInfo _sourceFileInfo;

        /// <summary>
        /// contained data for lazy initialise
        /// </summary>
        private Mp3FileData _mp3FileData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Mp3File"/> class. 
        /// </summary>
        /// <param name="file">
        /// the name of the file to construct from
        /// </param>
        public Mp3File(string file)
            : this(new FileInfo(file))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mp3File"/> class. 
        /// </summary>
        /// <param name="fileinfo">
        /// the <see cref="FileInfo"/> to construct from
        /// </param>
        public Mp3File(FileInfo fileinfo)
        {
            this._sourceFileInfo = fileinfo;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets original filename containing the audio data
        /// </summary>
        public string FileName
        {
            get
            {
                return this._sourceFileInfo.FullName;
            }
        }

        /// <summary>
        /// Gets or sets wrapper for the object containing the audio payload
        /// </summary>
        public IAudio Audio
        {
            get
            {
                return this.Mp3FileData.Audio;
            }

            set
            {
                this.Mp3FileData.Audio = value;
            }
        }

        /// <summary>
        /// Gets or sets ID3v2 tags represented by the Frame Model
        /// </summary>
        public TagModel TagModel
        {
            get
            {
                return this.Mp3FileData.TagModel;
            }

            set
            {
                this.Mp3FileData.TagModel = value;
            }
        }

        /// <summary>
        /// Gets or sets ID3v2 tags wrapped in an interpreter that gives you real properties for each supported frame 
        /// </summary>
        public TagHandler TagHandler
        {
            get
            {
                return this.Mp3FileData.TagHandler;
            }

            set
            {
                this.Mp3FileData.TagHandler = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ID3 tags from stream (if needed) and calculate where the audio must be
        /// </summary>
        /// <remarks>
        /// if this throws an exception, the file is not usable.
        /// </remarks>
        private Mp3FileData Mp3FileData
        {
            get
            {
                if (this._mp3FileData == null)
                {
                    this._mp3FileData = new Mp3FileData(this._sourceFileInfo);
                }

                return this._mp3FileData;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Update ID3V2 and V1 tags in-situ if possible, or rewrite the file to add tags if necessary.
        /// Always creates a backup file.
        /// </summary>
        public void Update()
        {
            this.Update(true);
        }

        /// <summary>
        /// Update ID3V2 and V1 tags in-situ if possible, or rewrite the file to add tags if necessary.
        /// </summary>
        /// <param name="createBackup">
        /// if <c>true</c>, create a backup file
        /// </param>
        public void Update(bool createBackup)
        {
            // cheeky optimisation: 
            // if no changes have been made, nothing to do
            if (this._mp3FileData != null)
            {
                if (this._mp3FileData.Update(createBackup) == Mp3FileData.CacheDataState.eDirty)
                {
                    // clear the data object so it gets re-initialised next time it's needed
                    this._mp3FileData = null;
                }
            }
        }

        /// <summary>
        /// rewrite the file and ID3V2 and V1 tags with no padding.
        /// Always creates a backup file.
        /// </summary>
        public void UpdatePacked()
        {
            this.UpdatePacked(true);
        }

        /// <summary>
        /// rewrite the file and ID3V2 and V1 tags with no padding
        /// </summary>
        /// <param name="createBackup">
        /// if <c>true</c>, create a backup file
        /// </param>
        public void UpdatePacked(bool createBackup)
        {
            if (this.Mp3FileData.UpdatePacked(createBackup) == Mp3FileData.CacheDataState.eDirty)
            {
                // clear the data object so it gets re-initialised next time it's needed
                this._mp3FileData = null;
            }
        }

        /// <summary>
        /// Update file and remove ID3V2 tag (if any); 
        /// update file in-situ if possible, or rewrite the file to remove tag if necessary.
        /// Always creates a backup file.
        /// </summary>
        public void UpdateNoV2tag()
        {
            this.UpdateNoV2tag(true);
        }

        /// <summary>
        /// Update file and remove ID3V2 tag (if any); 
        /// update file in-situ if possible, or rewrite the file to remove tag if necessary.
        /// </summary>
        /// <param name="createBackup">
        /// if <c>true</c>, create a backup file
        /// </param>
        public void UpdateNoV2tag(bool createBackup)
        {
            if (this.Mp3FileData.UpdateNoV2tag(createBackup) == Mp3FileData.CacheDataState.eDirty)
            {
                // clear the data object so it gets re-initialised next time it's needed
                this._mp3FileData = null;
            }
        }

        #endregion
    }
}