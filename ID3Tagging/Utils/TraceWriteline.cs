using System.Diagnostics;

namespace ID3Tagging.Utils
{
    /// <summary>
    /// Trace Function
    /// </summary>
    public class TraceF
    {
        /// <summary>
        /// Writes a message to the trace listeners in the System.Diagnostics.Trace.Listeners
        /// collection.
        /// </summary>
        /// <param name="message">
        /// A message to write.
        /// </param>
        [Conditional("TRACE")]
        public static void WriteLine(string message)
        {
            System.Diagnostics.Trace.WriteLine(message);
        }

        /// <summary>
        /// Writes a message to the trace listeners in the System.Diagnostics.Trace.Listeners
        /// collection.
        /// </summary>
        /// <param name="format">
        /// The format.
        /// </param>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        [Conditional("TRACE")]
        public static void WriteLine(string format, params object[] arguments)
        {
            System.Diagnostics.Trace.WriteLine(string.Format(format, arguments));
        }
    }
}