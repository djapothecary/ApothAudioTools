using System;

namespace ID3Tagging.Utils
{
    /// <summary>
    /// The hi perf timer wrapper.
    /// </summary>
    public class HiPerfTimerWrapper : IDisposable
    {
        private readonly HiPerfTimer _timer;

        private readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="HiPerfTimerWrapper"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public HiPerfTimerWrapper(string message)
        {
            _message = message;
            _timer = new HiPerfTimer();
        }

        #region IDisposable pattern

        /// <summary>
        /// Dispose of (clean up and deallocate) resources used by this class.
        /// </summary>
        /// <param name="fromUser">
        /// True if called directly or indirectly from user code.
        /// False if called from the finalizer (i.e. from the class' destructor).
        /// </param>
        /// <remarks>
        /// When called from user code, it is safe to clean up both managed and unmanaged objects.
        /// When called from the finalizer, it is only safe to dispose of unmanaged objects.
        /// This method should expect to be called multiple times without causing an exception.
        /// </remarks>
        private void Dispose(bool fromUser)
        {
            if (fromUser)
            {
                // Called from user code rather than the garbage collector
                // log output
                System.Diagnostics.Trace.WriteLine(string.Format("{0} took {1:F6} seconds", _message, _timer.Duration));

                // Dispose of managed resources (only safe if called directly or indirectly from user code).
                GC.SuppressFinalize(this);  // No need for the Finalizer to do all this again.
            }
        }

        /// <summary>
        /// Dispose of all resources (both managed and unmanaged) used by this class.
        /// </summary>
        public void Dispose()
        {
            // Call our private Dispose method, indicating that the call originated from user code.
            // Diagnostics.TraceInfo("Disposed by user code.");
            this.Dispose(true);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="HiPerfTimerWrapper"/> class. 
        /// Destructor, called by the finalizer during garbage collection.
        /// There is no guarantee that this method will be called. For example, if <see cref="Dispose"/> has already
        /// been called in user code for this object, then finalization may have been suppressed.
        /// </summary>
        ~HiPerfTimerWrapper()
        {
            // Call our private Dispose method, indicating that the call originated from the finalizer.
            // Diagnostics.TraceInfo("Finalizer is disposing HiPerfTimerWrapper instance");
            this.Dispose(false);
        }

        #endregion
    }
}