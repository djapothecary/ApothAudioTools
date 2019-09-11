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

namespace Id3.Net.Id3v2x
{
    internal abstract class Id3v2xHandler : Id3Handler
    {
        internal override Id3TagFamily Family
        {
            get
            {
                return Id3TagFamily.FileStartTag;
            }
        }

        internal override int MajorVersion
        {
            get
            {
                return 2;
            }
        }
    }
}