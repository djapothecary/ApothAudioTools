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

namespace Id3.Net.Info
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class InfoProviderProperties
    {
        private readonly string _name;
        private readonly string _url;
        private readonly string _registrationUrl;
        private readonly FrameTypeList _availableOutputs = new FrameTypeList();
        private readonly FrameTypeList _requiredInputs = new FrameTypeList();
        private readonly FrameTypeList _optionalInputs = new FrameTypeList();

        public InfoProviderProperties(string name, string url)
            : this(name, url, null)
        {
        }

        public InfoProviderProperties(string name, string url, string registrationUrl)
        {
            _name = name;
            _url = url;
            _registrationUrl = registrationUrl;
        }

        public FrameTypeList AvailableOutputs
        {
            get
            {
                return _availableOutputs;
            }
        }

        public bool CanOmitTag { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public FrameTypeList OptionalInputs
        {
            get
            {
                return _optionalInputs;
            }
        }

        public string RegistrationUrl
        {
            get
            {
                return _registrationUrl;
            }
        }

        public FrameTypeList RequiredInputs
        {
            get
            {
                return _requiredInputs;
            }
        }

        public bool RequiresFilename { get; set; }

        public bool RequiresStream { get; set; }

        public string Url
        {
            get
            {
                return _url;
            }
        }
    }
}