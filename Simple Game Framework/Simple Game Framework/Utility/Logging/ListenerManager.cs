using System.Diagnostics;

namespace Simple_Game_Framework.Utility.Logging
{
    public static class ListenerManager
    {
        public static readonly TraceSource TraceSource = new TraceSource("SimpleGameFramework");

        public static void AddListener(TraceListener listener)
        {
            TraceSource.Listeners.Add(listener);
        }

        public static void RemoveListener(TraceListener listener)
        {
            TraceSource.Listeners.Remove(listener);
        }

        public static void ClearListeners()
        {
            TraceSource.Listeners.Clear();
        }

        /// <summary>
        /// Configures file-based logging by adding a file trace listener.
        /// </summary>
        /// <param name="filePath">The path to the log file.</param>
        /// <remarks>
        /// This method ensures that only one file listener is active at a time.
        /// It interacts with the <see cref="TextWriterTraceListener"/> class.
        /// </remarks>
        public static void ConfigureFileLogging(string filePath)
        {
            // Set the SourceSwitch level to allow all logs
            TraceSource.Switch = new SourceSwitch("SourceSwitch", "All");

            // Remove existing file listeners to avoid duplicates
            foreach (var listener in TraceSource.Listeners)
            {
                if (listener is TextWriterTraceListener)
                {
                    TraceSource.Listeners.Remove((TraceListener)listener);
                    break;
                }
            }

            // Add a new file listener
            TraceSource.Listeners.Add(new TextWriterTraceListener(filePath));
        }

        public static void Flush()
        {
            TraceSource.Flush();
        }
    }
}
