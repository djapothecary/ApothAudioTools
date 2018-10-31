using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ID3Tagging.Utils
{
    /// <summary>
    /// The hi perf timer.
    /// </summary>
    public class HiPerfTimer
    {
        [DllImport("Kernel32.dll")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "To match Win32 API")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "To match Win32 API")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private readonly long _startTime;

        private static readonly long Freq = GetFreq();

        static long GetFreq()
        {
            long freq;

            if (QueryPerformanceFrequency(out freq) == false)
            {
                // high-performance counter not supported
                throw new Win32Exception();
            }

            return freq;
        }

        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="HiPerfTimer"/> class.
        /// </summary>
        public HiPerfTimer()
        {
            _startTime = 0;

            // _stopTime = 0;
            QueryPerformanceCounter(out _startTime);
        }

        //// Start the timer

        // public void Start()
        // {
        // // lets do the waiting threads there work

        // Thread.Sleep(0);

        // QueryPerformanceCounter(out _startTime);
        // }

        //// Stop the timer

        // public void Stop()
        // {
        // QueryPerformanceCounter(out _stopTime);
        // }

        // Returns the duration of the timer (in seconds)

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public double Duration
        {
            get
            {
                long stopTime;
                QueryPerformanceCounter(out stopTime);

                return (stopTime - this._startTime) / (double)Freq;
            }
        }
    }
}