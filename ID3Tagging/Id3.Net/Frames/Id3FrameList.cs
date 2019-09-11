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
using System.Collections.ObjectModel;
using System.Linq;

namespace Id3.Net.Frames
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class Id3FrameList : Collection<Id3Frame>
    {
        public TFrame Get<TFrame>() where TFrame : Id3Frame
        {
            Id3Frame frame = this.FirstOrDefault(f => f as TFrame != null);
            return frame != null ? frame as TFrame : null;
        }

        public TFrame Get<TFrame>(Func<TFrame, bool> predicate) where TFrame : Id3Frame
        {
            Id3Frame frame = this.FirstOrDefault(f => {
                var typedFrame = f as TFrame;
                return (typedFrame != null) && (predicate(typedFrame));
            });
            return frame != null ? frame as TFrame : null;
        }

        public Id3Frame Get(Type frameType)
        {
            Id3Frame frame = this.FirstOrDefault(f => f.GetType() == frameType);
            return frame;
        }

        public Id3Frame Get(Type frameType, Func<Id3Frame, bool> predicate)
        {
            Id3Frame frame = this.FirstOrDefault(f => f.GetType() == frameType && predicate(f));
            return frame;
        }

        public TFrame[] GetAll<TFrame>() where TFrame : Id3Frame
        {
            TFrame[] frames = this.Where(f => f is TFrame).Select(f => (TFrame)f).ToArray();
            return frames.ToArray();
        }

        public TFrame GetOrAdd<TFrame>() where TFrame : Id3Frame, new()
        {
            var frame = Get<TFrame>();
            if (frame == null)
            {
                frame = new TFrame();
                Add(frame);
            }
            return frame;
        }

        public TFrame GetOrAdd<TFrame>(Func<TFrame, bool> predicate) where TFrame : Id3Frame, new()
        {
            TFrame frame = Get(predicate);
            if (frame == null)
            {
                frame = new TFrame();
                Add(frame);
            }
            return frame;
        }
    }
}