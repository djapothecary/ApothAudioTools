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
using System.Collections.ObjectModel;
using System.Linq;

namespace Id3.Net.Frames
{
#if !SILVERLIGHT
    [Serializable]
#endif
    public class Id3SyncFrameList<TFrame> : Collection<TFrame>
        where TFrame : Id3Frame
    {
        //Reference to the main frame list
        private readonly Id3FrameList _mainList;

        //If true, indicates that an internal list update is taking place, and the list change
        //notification methods should not perform the corresponding updates on the main list.
        private readonly bool _internalUpdate;

        public Id3SyncFrameList(Id3FrameList mainList)
        {
            _mainList = mainList;

            //Extract all frames of type TFrame and store then in this collection.
            //The list change notification events should not be fired during this process.
            _internalUpdate = true;
            try
            {
                TFrame[] frames = _mainList.GetAll<TFrame>();
                foreach (TFrame frame in frames)
                {
                    Add(frame);
                }                    
            }
            finally
            {
                _internalUpdate = false;
            }
        }

        #region Public utility methods
        public TFrame Find(Func<TFrame, bool> predicate)
        {
            return this.FirstOrDefault(predicate);
        }

        public TFrame[] FindAll(Func<TFrame, bool> predicate)
        {
            var result = new List<TFrame>();
            for (int frameIdx = 0; frameIdx < Count; frameIdx++)
            {
                if (predicate(this[frameIdx]))
                {
                    result.Add(this[frameIdx]);
                }                    
            }
            return result.ToArray();
        }

        public int IndexOf(Func<TFrame, bool> predicate)
        {
            for (int frameIdx = 0; frameIdx < Count; frameIdx++)
            {
                if (predicate(this[frameIdx]))
                {
                    return frameIdx;
                }                    
            }
            return -1;
        }
        #endregion

        #region List change notification methods
        protected override void ClearItems()
        {
            if (!_internalUpdate)
            {
                TFrame[] frames = _mainList.GetAll<TFrame>();
                foreach (TFrame frame in frames)
                {
                    _mainList.Remove(frame);
                }                    
            }
            base.ClearItems();
        }

        protected override void InsertItem(int index, TFrame item)
        {
            if (!_internalUpdate)
            {
                _mainList.Add(item);
            }
                
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            if (!_internalUpdate)
            {
                TFrame frame = this[index];
                _mainList.Remove(frame);
            }
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, TFrame item)
        {
            if (!_internalUpdate)
            {
                TFrame frame = this[index];
                int mainIndex = _mainList.IndexOf(frame);
                _mainList[mainIndex] = item;
            }
            base.SetItem(index, item);
        }
        #endregion
    }
}