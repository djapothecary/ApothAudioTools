﻿#region --- License & Copyright Notice ---
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

using System.Text;

namespace Id3.Net.Id3v23
{
    internal static class AsciiEncoding
    {
        internal static string GetString(byte[] bytes, int index, int count)
        {
#if SILVERLIGHT
            var sb = new StringBuilder(bytes.Length);
            foreach (byte b in bytes)
                sb.Append(b <= 0x7f ? (char)b : '?');
            return sb.ToString();
#else
            return Encoding.ASCII.GetString(bytes, index, count);
#endif
        }

        internal static byte[] GetBytes(string str)
        {
#if SILVERLIGHT
            var retval = new byte[str.Length];
            for (int ix = 0; ix < str.Length; ++ix)
            {
                char ch = str[ix];
                if (ch <= 0x7f)
                    retval[ix] = (byte)ch;
                else
                    retval[ix] = (byte)'?';
            }
            return retval;
#else
            return Encoding.ASCII.GetBytes(str);
#endif
        }
    }
}