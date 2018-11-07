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
using System.Globalization;

namespace Id3.Net.Frames
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class DateTimeTextFrame : TextFrame
    {
        public override string ToString()
        {
            return IsAssigned ? AsDateTime.ToString() : string.Empty;
        }

        public DateTime? AsDateTime { get; set; }

        public override string Value
        {
            get
            {
                return AsDateTime.HasValue ? AsDateTime.Value.ToString() : null;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                    AsDateTime = null;
                else
                {
                    DateTime dateTime;
                    if (
                        !DateTime.TryParseExact(value, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces,
                            out dateTime))
                    {
                        AsDateTime = null;
                    }                        
                    else
                    {
                        AsDateTime = dateTime;
                    }                        
                }
            }
        }

        protected abstract string DateTimeFormat { get; }
    }
}