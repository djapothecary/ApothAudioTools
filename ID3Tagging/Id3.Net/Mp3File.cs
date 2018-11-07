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

using Id3.Net.Id3v1;
using Id3.Net.Id3v23;
using Id3.Net.Internal;
using Id3.Net.Resources;

namespace Id3.Net
{
    public sealed class Mp3File : IDisposable
    {
        #region Private fields
        //MP3 file stream-related
        private readonly Stream _stream;
        //private readonly MemoryStream _stream = null;  
        //private readonly MemoryStream _stream;
        private readonly bool _streamOwned;

        //Audio stream properties
        private AudioStreamProperties _audioProperties;

        //ID3 Handler management
        private RegisteredId3Handlers _existingHandlers;
        #endregion

        #region Construction & destruction
        //By default, files are opened in the most restrictive mode - read-only. Read-write access
        //must be explicitely specified using the next constructor.
        public Mp3File(string filename)
            : this(filename, FileAccess.Read)
        {
        }

        public Mp3File(string filename, FileAccess access)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }                

            _stream = new FileStream(filename, FileMode.Open, access, FileShare.Read);

            //Since we created the stream, we are responsible for disposing it when we're done
            _streamOwned = true;
        }

        //public Mp3File(Stream stream)
        //    : this(stream, FileAccess.Read)
        //{
        //}
        public Mp3File(Stream stream)
            : this(stream.ToString(), FileAccess.Read)
        {
        }

        //public Mp3File(Stream stream, FileAccess access)
        //{
        //    if (stream == null)
        //    {
        //        throw new ArgumentNullException("stream");
        //    }

        //    if (access == FileAccess.Write)
        //    {
        //        access = FileAccess.ReadWrite;
        //    }                

        //    if (!stream.CanRead || !stream.CanSeek)
        //    {
        //        throw new Id3Exception(Id3Messages.StreamNotReadableOrSeekable);
        //    }

        //    if (access == FileAccess.ReadWrite && !stream.CanWrite)
        //    {
        //        throw new Id3Exception(Id3Messages.StreamNotWritable);
        //    }                

        //    _stream = stream;

        //    //The stream is owned by the caller, so it is their responsibility to dispose it.
        //    _streamOwned = false;
        //}

        public Mp3File(Stream stream, FileAccess access)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (access == FileAccess.Write)
            {
                access = FileAccess.ReadWrite;
            }

            if (!stream.CanRead || !stream.CanSeek)
            {
                throw new Id3Exception(Id3Messages.StreamNotReadableOrSeekable);
            }

            if (access == FileAccess.ReadWrite && !stream.CanWrite)
            {
                throw new Id3Exception(Id3Messages.StreamNotWritable);
            }

            _stream = stream;

            //The stream is owned by the caller, so it is their responsibility to dispose it.
            _streamOwned = false;
        }

        public void Dispose()
        {
            if (_streamOwned && _stream != null)
            {
                _stream.Dispose();
            }                
        }
        #endregion

        #region Tag deleting methods
        public void DeleteTag(int majorVersion, int minorVersion)
        {
            RegisteredId3Handler registeredHandler = ExistingHandlers.GetHandler(majorVersion, minorVersion);
            if (registeredHandler != null)
            {
                registeredHandler.Handler.DeleteTag(_stream);
                InvalidateExistingHandlers();
            }
        }

        public void DeleteTag(Id3TagFamily family)
        {
            RegisteredId3Handler[] registeredHandlers = ExistingHandlers.GetHandlers(family);
            if (registeredHandlers.Length > 0)
            {
                Id3Handler handler = registeredHandlers[0].Handler;
                handler.DeleteTag(_stream);
                InvalidateExistingHandlers();
            }
        }

        public void DeleteAllTags()
        {
            foreach (RegisteredId3Handler existingHandler in ExistingHandlers)
            {
                existingHandler.Handler.DeleteTag(_stream);
            }
                
            InvalidateExistingHandlers();
        }
        #endregion

        #region Tag retrieval methods
        public Id3Tag[] GetAllTags()
        {
            var tags = new Id3Tag[ExistingHandlers.Count];
            for (int tagIdx = 0; tagIdx < tags.Length; tagIdx++)
            {
                Id3Handler handler = ExistingHandlers[tagIdx].Handler;
                tags[tagIdx] = handler.ReadTag(_stream);
            }
            return tags;
        }

        public Id3Tag GetTag(Id3TagFamily family)
        {
            RegisteredId3Handler[] familyHandlers = ExistingHandlers.GetHandlers(family);
            if (familyHandlers.Length == 0)
            {
                return null;
            }
                
            Id3Handler handler = familyHandlers[0].Handler;
            Id3Tag tag = handler.ReadTag(_stream);
            return tag;
        }

        public Id3Tag GetTag(int majorVersion, int minorVersion)
        {
            RegisteredId3Handler registeredHandler = ExistingHandlers.GetHandler(majorVersion, minorVersion);
            return registeredHandler != null ? registeredHandler.Handler.ReadTag(_stream) : null;
        }

        //Retrieves the tag as a byte array. This method does not attempt to read the tag data,
        //it simply reads the header and if present the tag bytes are read directly from the
        //stream. This means that typical exceptions that get thrown in a tag read will not
        //occur in this method.
        public byte[] GetTagBytes(int majorVersion, int minorVersion)
        {
            RegisteredId3Handler registeredHandler = RegisteredHandlers.GetHandler(majorVersion, minorVersion);
            byte[] tagBytes = registeredHandler.Handler.GetTagBytes(_stream);
            return tagBytes;
        }
        #endregion

        #region Tag querying methods
        public Version[] GetVersions()
        {
            Version[] versions = ExistingHandlers.Select(handler => new Version(handler.Handler.MajorVersion, handler.Handler.MinorVersion)).ToArray();
            return versions;
        }

        public bool HasTagOfFamily(Id3TagFamily family)
        {
            return ExistingHandlers.Any(handler => handler.Handler.Family == family);
        }

        public bool HasTagOfVersion(int majorVersion, int minorVersion)
        {
            return ExistingHandlers.Any(handler => handler.Handler.MajorVersion == majorVersion && handler.Handler.MinorVersion == minorVersion);
        }

        public bool HasTags
        {
            get
            {
                return ExistingHandlers.Count > 0;
            }
        }
        #endregion

        #region Tag writing methods
        public bool WriteTag(Id3Tag tag, WriteConflictAction conflictAction)
        {
            if (tag == null)
            {
                throw new ArgumentNullException("tag");
            }
                

            //The tag should specify major version number
            if (tag.MajorVersion == 0)
            {
                throw new ArgumentException(Id3Messages.MajorTagVersionMissing, "tag");
            }                

            //Get any existing handlers from the same family as the tag
            RegisteredId3Handler[] familyHandlers = ExistingHandlers.GetHandlers(tag.Family);

            //If a tag already exists from the same family, but is a different version than the passed tag,
            //delete it if conflictAction is Replace.
            if (familyHandlers.Length > 0 &&
                (familyHandlers[0].Handler.MajorVersion != tag.MajorVersion || familyHandlers[0].Handler.MinorVersion != tag.MinorVersion))
            {
                if (conflictAction == WriteConflictAction.NoAction)
                {
                    return false;
                }
                    
                if (conflictAction == WriteConflictAction.Replace)
                {
                    Id3Handler familyHandler = familyHandlers[0].Handler;
                    familyHandler.DeleteTag(_stream);
                }
            }

            //Write the tag to the file. The handler will know how to overwrite itself.
            RegisteredId3Handler registeredHandler = RegisteredHandlers.GetHandler(tag.MajorVersion, tag.MinorVersion);

            //initalize handler if it's null
            if (registeredHandler == null)
            {
                registeredHandler = new RegisteredId3Handler();
                registeredHandler = RegisteredHandlers.GetHandler(tag.MajorVersion, tag.MinorVersion);

            }
            //added cast to memory stream
            //bool writeSuccessful = registeredHandler.Handler.WriteTag((System.IO.MemoryStream)_stream, tag);
            // convert the stream the right way
            //MemoryStream memStream = ConvertStreamFile(_stream);
            MemoryStream memStream = new MemoryStream();
            _stream.CopyTo(memStream);
            //bool writeSuccessful = registeredHandler.Handler.WriteTag(_stream, tag);
            bool writeSuccessful = registeredHandler.Handler.WriteTag(memStream, tag);
            if (writeSuccessful)
            {
                InvalidateExistingHandlers();
            }
                
            return writeSuccessful;
        }

        public bool WriteTag(Id3Tag tag, int majorVersion, int minorVersion, WriteConflictAction conflictAction)
        {
            tag.MajorVersion = majorVersion;
            tag.MinorVersion = minorVersion;
            return WriteTag(tag, conflictAction);
        }
        #endregion

        #region Audio stream members
        public byte[] GetAudioStream()
        {
            byte[] startBytes = null, endBytes = null;
            foreach (RegisteredId3Handler registeredHandler in ExistingHandlers)
            {
                if (registeredHandler.Handler.Family == Id3TagFamily.FileStartTag)
                {
                    startBytes = registeredHandler.Handler.GetTagBytes(_stream);
                }                    
                else
                {
                    endBytes = registeredHandler.Handler.GetTagBytes(_stream);
                }
                    
            }

            long audioStreamLength = _stream.Length - (startBytes != null ? startBytes.Length : 0) -
                (endBytes != null ? endBytes.Length : 0);
            var audioStream = new byte[audioStreamLength];
            _stream.Seek(startBytes == null ? 0 : startBytes.Length, SeekOrigin.Begin);
            _stream.Read(audioStream, 0, (int)audioStreamLength);
            return audioStream;
        }

        public AudioStreamProperties Audio
        {
            get
            {
                if (_audioProperties == null)
                {
                    byte[] audioStream = GetAudioStream();
                    if (audioStream == null || audioStream.Length == 0)
                    {
                        throw new AudioStreamException(Id3Messages.AudioStreamMissing);
                    }
                        
                    _audioProperties = new AudioStream(audioStream).Calculate();
                }
                return _audioProperties;
            }
        }
        #endregion

        #region ID3 handler registration/management
        private void InvalidateExistingHandlers()
        {
            _existingHandlers = null;
        }

        //The list of registered ID3 handlers for existing tags in the file. This list is
        //dynamically built and is the basis for most of the GetXXXX methods.
        //Whenever the MP3 stream is changed (such as when WriteTag or DeleteTag is called), the
        //_existingHandlers field should be reset to null so that this list can be recreated the
        //next time it is accessed.
        private RegisteredId3Handlers ExistingHandlers
        {
            get
            {
                if (_existingHandlers == null)
                {
                    _existingHandlers = new RegisteredId3Handlers();
                    foreach (RegisteredId3Handler registeredHandler in RegisteredHandlers)
                    {
                        Id3Handler handler = registeredHandler.Handler;
                        if (handler.HasTag(_stream))
                        {
                            _existingHandlers.Add(registeredHandler);
                        }                            
                    }
                }
                return _existingHandlers;
            }
        }

        private static readonly RegisteredId3Handlers _registeredHandlers;

        static Mp3File()
        {
            _registeredHandlers = new RegisteredId3Handlers(2);
            _registeredHandlers.Register<Id3v23Handler>();
            _registeredHandlers.Register<Id3v1Handler>();
        }

        internal static RegisteredId3Handlers RegisteredHandlers
        {
            get
            {
                return _registeredHandlers;
            }
        }
        #endregion
    }
}