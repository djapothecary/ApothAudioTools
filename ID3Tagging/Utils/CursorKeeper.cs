using System;
using System.Windows.Forms;

namespace ID3Tagging.Utils
{
    /// <summary>
    /// The cursor keeper.
    /// </summary>
    public class CursorKeeper : IDisposable
    {
        private readonly Cursor _originalCursor;

        private bool _isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CursorKeeper"/> class.
        /// </summary>
        /// <param name="newCursor">
        /// The new cursor.
        /// </param>
        public CursorKeeper(Cursor newCursor)
        {
            _originalCursor = Cursor.Current;
            Cursor.Current = newCursor;
        }

        #region " IDisposable Support "

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Cursor.Current = _originalCursor;
                }
            }

            _isDisposed = true;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}