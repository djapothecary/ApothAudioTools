using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

using ID3Tagging.ID3Lib.Exceptions;

namespace ID3Tagging.ID3Lib.Utils
{
    #region Global Fields

    /// <summary>
    /// Type of text used in frame
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32")]
    public enum TextCode : byte
    {
        /// <summary>
        /// ASCII(ISO-8859-1)
        /// </summary>
        Ascii = 0x00,

        /// <summary>
        /// Unicode with BOM
        /// </summary>
        Utf16 = 0x01,

        /// <summary>
        /// BigEndian Unicode without BOM
        /// </summary>
        Utf16BE = 0x02,

        /// <summary>
        /// Encoded Unicode
        /// </summary>
        Utf8 = 0x03
    };

    #endregion

    /// <summary>
    /// Manages binary to text and vice versa format conversions.
    /// </summary>
    internal static class TextBuilder
    {
        #region Methods

        /// <summary>
        /// The read text.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="InvalidFrameException">
        /// </exception>
        public static string ReadText(byte[] frame, ref int index, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                    {
                        return ReadASCII(frame, ref index);
                    }

                case TextCode.Utf16:
                    {
                        return ReadUTF16(frame, ref index);
                    }

                case TextCode.Utf16BE:
                    {
                        return ReadUTF16BE(frame, ref index);
                    }

                case TextCode.Utf8:
                    {
                        return ReadUTF8(frame, ref index);
                    }

                default:
                    {
                        throw new InvalidFrameException("Invalid text code string type.");
                    }
            }
        }

        /// <summary>
        /// The read text end.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="InvalidFrameException">
        /// </exception>
        public static string ReadTextEnd(byte[] frame, int index, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                    {
                        return ReadASCIIEnd(frame, index);
                    }

                case TextCode.Utf16:
                    {
                        return ReadUTF16End(frame, index);
                    }

                case TextCode.Utf16BE:
                    {
                        return ReadUTF16BEEnd(frame, index);
                    }

                case TextCode.Utf8:
                    {
                        return ReadUTF8End(frame, index);
                    }

                default:
                    {
                        throw new InvalidFrameException("Invalid text code string type.");
                    }
            }
        }

        /// <summary>
        /// The read ascii.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="InvalidFrameException">
        /// </exception>
        public static string ReadASCII(byte[] frame, ref int index)
        {
            string text = null;
            int count = Memory.FindByte(frame, 0, index);

            if (count == -1)
            {
                throw new InvalidFrameException("Invalid ASCII string size");
            }                

            if (count > 0)
            {
                var encoding = Encoding.GetEncoding(1252); // Should be ASCII
                text = encoding.GetString(frame, index, count);
                index += count; // add the read bytes
            }

            index++; // jump an end of line byte
            return text;
        }

        /// <summary>
        /// The read ut f 16.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="InvalidFrameException">
        /// </exception>
        public static string ReadUTF16(byte[] frame, ref int index)
        {
            // check for empty string first, and throw a useful exception
            // otherwise we'll get an out-of-range exception when we look for the BOM
            if (index >= frame.Length - 2)
            {
                throw new InvalidFrameException("ReadUTF16: string must be terminated");
            }                

            if (frame[index] == 0xfe && frame[index + 1] == 0xff)
            {
                // Big Endian
                index += 2;
                return ReadUTF16BE(frame, ref index);
            }

            if (frame[index] == 0xff && frame[index + 1] == 0xfe)
            {
                // Little Endian
                index += 2;
                return ReadUTF16LE(frame, ref index);
            }

            if (frame[index] == 0x00 && frame[index + 1] == 0x00)
            {
                // empty string
                index += 2;
                return string.Empty;
            }

            throw new InvalidFrameException("Invalid UTF16 string.");

        }

        /// <summary>
        /// The read ut f 16 be.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="InvalidFrameException">
        /// </exception>
        public static string ReadUTF16BE(byte[] frame, ref int index)
        {
            UnicodeEncoding encoding = new UnicodeEncoding(true, false);
            int count = Memory.FindShort(frame, 0, index);

            if (count == -1)
            {
                throw new InvalidFrameException("Invalid UTF16BE string size");
            }                

            // we can safely let count==0 fall through
            string text = encoding.GetString(frame, index, count);
            index += count; // add the bytes read
            index += 2;     // skip the EOL
            return text;
        }

        private static string ReadUTF16LE(byte[] frame, ref int index)
        {
            UnicodeEncoding encoding = new UnicodeEncoding(false, false);
            int count = Memory.FindShort(frame, 0, index);

            if (count == -1)
            {
                throw new InvalidFrameException("Invalid UTF16LE string size");
            }                

            // we can safely let count==0 fall through
            string text = encoding.GetString(frame, index, count);
            index += count; // add the bytes read
            index += 2;     // skip the EOL
            return text;
        }

        /// <summary>
        /// The read ut f 8.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="InvalidFrameException">
        /// </exception>
        public static string ReadUTF8(byte[] frame, ref int index)
        {
            string text = null;
            int count = Memory.FindByte(frame, 0, index);
            if (count == -1)
            {
                throw new InvalidFrameException("Invalid UTF8 string size");
            }

            if (count > 0)
            {
                text = Encoding.UTF8.GetString(frame, index, count);
                index += count; // add the read bytes
            }

            index++; // jump an end of line byte
            return text;
        }

        /// <summary>
        /// The read ascii end.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReadASCIIEnd(byte[] frame, int index)
        {
            Encoding encoding = Encoding.GetEncoding(1252); // Should be ASCII
            return encoding.GetString(frame, index, frame.Length - index);
        }

        /// <summary>
        /// The read ut f 16 end.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="InvalidFrameException">
        /// </exception>
        public static string ReadUTF16End(byte[] frame, int index)
        {
            // check for empty string first
            // otherwise we'll get an exception when we look for the BOM
            // SourceForge bug ID: 2686976
            if (index >= frame.Length - 2)
            {
                return string.Empty;
            }                

            if (frame[index] == 0xfe && frame[index + 1] == 0xff) // Big Endian
            {
                return ReadUTF16BEEnd(frame, index + 2);
            }                

            if (frame[index] == 0xff && frame[index + 1] == 0xfe) // Little Endian
            {
                return ReadUTF16LEEnd(frame, index + 2);
            }                

            throw new InvalidFrameException("Invalid UTF16 string.");
        }

        /// <summary>
        /// The read ut f 16 be end.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReadUTF16BEEnd(byte[] frame, int index)
        {
            var encoding = new UnicodeEncoding(true, false);
            return encoding.GetString(frame, index, frame.Length - index);
        }

        private static string ReadUTF16LEEnd(byte[] frame, int index)
        {
            var encoding = new UnicodeEncoding(false, false);
            return encoding.GetString(frame, index, frame.Length - index);
        }

        /// <summary>
        /// The read ut f 8 end.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string ReadUTF8End(byte[] frame, int index)
        {
            return Encoding.UTF8.GetString(frame, index, frame.Length - index);
        }

        // Write routines
        /// <summary>
        /// The write text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        /// <exception cref="InvalidFrameException">
        /// </exception>
        public static byte[] WriteText(string text, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                    {
                        return WriteASCII(text);
                    }

                case TextCode.Utf16:
                    {
                        return WriteUTF16(text);
                    }

                case TextCode.Utf16BE:
                    {
                        return WriteUTF16BE(text);
                    }

                case TextCode.Utf8:
                    {
                        return WriteUTF8(text);
                    }

                default:
                    {
                        throw new InvalidFrameException("Invalid text code string type.");
                    }
            }
        }

        /// <summary>
        /// The write text end.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        /// <exception cref="InvalidFrameException">
        /// </exception>
        public static byte[] WriteTextEnd(string text, TextCode code)
        {
            switch (code)
            {
                case TextCode.Ascii:
                    {
                        return WriteASCIIEnd(text);
                    }

                case TextCode.Utf16:
                    {
                        return WriteUTF16End(text);
                    }

                case TextCode.Utf16BE:
                    {
                        return WriteUTF16BEEnd(text);
                    }

                case TextCode.Utf8:
                    {
                        return WriteUTF8End(text);
                    }

                default:
                    {
                        throw new InvalidFrameException("Invalid text code string type.");
                    }
            }
        }

        /// <summary>
        /// The write ascii.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        public static byte[] WriteASCII(string text)
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            if (string.IsNullOrEmpty(text))
            {
                // Write a null string
                writer.Write((byte)0);
                return buffer.ToArray();
            }

            var encoding = Encoding.GetEncoding(1252); // Should be ASCII
            writer.Write(encoding.GetBytes(text));
            writer.Write((byte)0); // EOL
            return buffer.ToArray();
        }

        /// <summary>
        /// The write ut f 16.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        public static byte[] WriteUTF16(string text)
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            if (string.IsNullOrEmpty(text))
            {
                // Write a null string
                writer.Write((ushort)0);
                return buffer.ToArray();
            }

            writer.Write((byte)0xff); // Little endian, we have UTF16BE for big endian
            writer.Write((byte)0xfe);
            var encoding = new UnicodeEncoding(false, false);
            writer.Write(encoding.GetBytes(text));
            writer.Write((ushort)0);
            return buffer.ToArray();
        }

        /// <summary>
        /// The write ut f 16 be.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        public static byte[] WriteUTF16BE(string text)
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            var encoding = new UnicodeEncoding(true, false);
            if (string.IsNullOrEmpty(text))
            {
                // Write a null string
                writer.Write((ushort)0);
                return buffer.ToArray();
            }

            writer.Write(encoding.GetBytes(text));
            writer.Write((ushort)0);
            return buffer.ToArray();
        }

        /// <summary>
        /// The write ut f 8.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        public static byte[] WriteUTF8(string text)
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            if (string.IsNullOrEmpty(text))
            {
                // Write a null string
                writer.Write((byte)0);
                return buffer.ToArray();
            }

            writer.Write(Encoding.UTF8.GetBytes(text));
            writer.Write((byte)0);
            return buffer.ToArray();
        }

        /// <summary>
        /// The write ascii end.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        public static byte[] WriteASCIIEnd(string text)
        {
            var buffer = new MemoryStream();
            var writer = new BinaryWriter(buffer);
            if (string.IsNullOrEmpty(text))
            {
                return buffer.ToArray();
            }

            Encoding encoding = Encoding.GetEncoding(1252); // Should be ASCII
            writer.Write(encoding.GetBytes(text));
            return buffer.ToArray();
        }

        /// <summary>
        /// The write ut f 16 end.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        public static byte[] WriteUTF16End(string text)
        {
            MemoryStream buffer = new MemoryStream(text.Length + 2);
            BinaryWriter writer = new BinaryWriter(buffer);
            if (string.IsNullOrEmpty(text))
            {
                // Write a null string
                return buffer.ToArray();
            }

            writer.Write((byte)0xff); // Little endian
            writer.Write((byte)0xfe);
            UnicodeEncoding encoding = new UnicodeEncoding(false, false);
            writer.Write(encoding.GetBytes(text));
            return buffer.ToArray();
        }

        /// <summary>
        /// The write ut f 16 be end.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        public static byte[] WriteUTF16BEEnd(string text)
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            if (string.IsNullOrEmpty(text))
            {
                // Write a null string
                return buffer.ToArray();
            }

            UnicodeEncoding encoding = new UnicodeEncoding(true, false);
            writer.Write(encoding.GetBytes(text));
            return buffer.ToArray();
        }

        /// <summary>
        /// The write ut f 8 end.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/> array.
        /// </returns>
        public static byte[] WriteUTF8End(string text)
        {
            MemoryStream buffer = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(buffer);
            if (string.IsNullOrEmpty(text))
            {
                // Write a null string
                return buffer.ToArray();
            }

            writer.Write(Encoding.UTF8.GetBytes(text));
            return buffer.ToArray();
        }

        #endregion
    }
}