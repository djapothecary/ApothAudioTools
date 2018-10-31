using System;
//using System.Data.SqlServerCe;
using System.IO;

using ID3Tagging.ID3Lib;
using ID3Tagging.ID3Lib.Frames;
using ID3Tagging.ID3Editor;
//using ID3Tagging.ID3Extract.Repository;
using ID3Tagging.ID3Extract;
using ID3Tagging.ED3Extract;
using System.Data.SqlClient;

namespace ID3Tagging.ID3Extract
{
    public class ReopositoryAdapter
    {
        readonly RepositoryDataContext _dataContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReopositoryAdapter"/> class.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        public ReopositoryAdapter(SqlConnection connection)
        {
            _dataContext = new RepositoryDataContext(connection);
        }

        /// <summary>
        /// The publish frames.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public void PublishFrames(string fileName)
        {
            using (Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    //var tagModel = TagManager.Deserialization(stream);
                    var tagModel = TagManager.Deserialize(stream);

                    SourceFile sourceFile = new SourceFile
                    {
                        FileName = Path.GetFileName(fileName),
                        FilePath = Path.GetDirectoryName(fileName)
                    };

                    foreach (var frame in tagModel)
                    {
                        SourceFileTag fileTag = new SourceFileTag
                        {
                            Tag = frame.FrameId
                        };

                        sourceFile.SourceFileTag.Add(fileTag);

                        var frameText = frame as FrameText;

                        if (frameText != null)
                        {
                            fileTag.TagText = new TagText
                            {
                                Text = frameText.Text,
                                TextEncodingId = (byte)frameText.TextCode
                            };
                            continue;
                        }

                        var frameTextUserDef = frame as FrameTextUserDef;

                        if (frameTextUserDef != null)
                        {
                            fileTag.TagText = new TagText
                            {
                                Text = frameTextUserDef.Text,
                                Description = frameTextUserDef.Description,
                                TextEncodingId = (byte)frameTextUserDef.TextCode
                            };
                            continue;
                        }

                        var frameUrl = frame as FrameUrl;

                        if (frameUrl != null)
                        {
                            fileTag.TagText = new TagText
                            {
                                TextReader = frameUrl.Url
                            };
                            continue;
                        }

                        var frameFullText = frame as FrameFullText;

                        if (frameFullText != null)
                        {
                            fileTag.TagFullText = new TagFullText
                            {
                                TextEncodingId = (byte)frameFullText.TextCode,
                                Description = frameFullText.Description,
                                TextLanguage = frameFullText.Language,
                                Comment = frameFullText.Text
                            };
                            continue;
                        }

                        var framePicture = frame as FramePicture;

                        if (framePicture != null)
                        {
                            fileTag.TagPicture = new TagPicture
                            {
                                PictureTypeId = (byte)framePicture.PictureType,
                                MimeType = framePicture.Mime,
                                Description = framePicture.Description,
                                BinaryImage = framePicture.PictureData
                            };
                            continue;
                        }

                        var frameBinary = frame as FrameBinary;

                        if (frameBinary != null)
                        {
                            fileTag.TagBinary = new TagBinary
                            {
                                TextEncodingId = (byte)frameBinary.TextEncoding,
                                MimeType = frame,
                                frameBinary.Mime,
                                Description = frameBinary.Description,
                                BinaryObject = frameBinary.ObjectData
                            };
                        }
                    }

                    //  sql commit; may not use
                    _dataContext.SourceFile.InsertOnSubmit(sourceFile);
                    _dataContext.SubmitChanges();
                }
                catch (NotImplementedException e)
                {
                    Console.WriteLine("{0}:{1}", fileName, e.Message);
                }
            }
        }
    }
}
