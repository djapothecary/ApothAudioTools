using System;

namespace ID3Tagging.ID3Lib.Frames
{
    // <summary>
    /// Define the type of frame
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class FrameAttribute : Attribute
    {
        #region Fields

        /// <summary>
        /// gets the FrameId
        /// </summary>
        public string FrameId { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameAttribute"/> class. 
        /// The frameId represented
        /// </summary>
        /// <param name="frameId">
        /// a frameId
        /// </param>
        public FrameAttribute(string frameId)
        {
            if (frameId == null)
            {
                throw new ArgumentNullException("frameId");
            }

            this.FrameId = frameId;
        }
    }
}
