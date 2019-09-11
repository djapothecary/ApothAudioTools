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

namespace Id3.Net.Frames
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract class NumericTextFrame : TextFrame
    {
        public int? AsInt { get; set; }

        public override string Value
        {
            get
            {
                return AsInt.HasValue ? AsInt.Value.ToString() : null;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                    AsInt = null;
                else
                {
                    int asInt;
                    AsInt = !int.TryParse(value, out asInt) ? (int?)null : asInt;
                }
            }
        }
    }
}