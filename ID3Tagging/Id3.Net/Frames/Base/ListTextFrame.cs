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

namespace Id3.Net.Frames
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class ListTextFrame : TextFrame
    {
        private readonly List<string> _values = new List<string>();
        private const char Separator = '/';

        public override string Value
        {
            get
            {
                var sb = new StringBuilder();
                foreach (string value in _values)
                {
                    sb.Append(value + Separator);
                }
                    
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                    
                return sb.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _values.Clear();
                }                    
                else
                {
                    string[] breakup = value.Split(Separator);
                    _values.AddRange(breakup);
                }
            }
        }

        public List<string> Values
        {
            get
            {
                return _values;
            }
        }
    }
}