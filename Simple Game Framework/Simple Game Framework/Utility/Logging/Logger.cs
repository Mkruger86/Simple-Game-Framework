using System;
using System.Diagnostics;

namespace Simple_Game_Framework.Utility.Logging
{
    public static class Logger
    {
        private static readonly TraceSource TraceSource = ListenerManager.TraceSource;
        private static TraceEventType _minimumLogLevel = TraceEventType.Information;
        private static int _logIdCounter = 1; // Counter for unique log IDs

        static Logger()
        {
            // Add a default listener (e.g., Console)
            ListenerManager.AddListener(new ConsoleTraceListener());
        }

        /// <summary>
        /// Logs a message with the specified event type and context.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="eventType">The type of event (e.g., Information, Warning).</param>
        /// <param name="context">The context of the log message (optional).</param>
        /// <remarks>
        /// This method interacts with the <see cref="ListenerManager"/> to send log messages to configured listeners.
        /// </remarks>
        public static void Log(string message, TraceEventType eventType = TraceEventType.Information, string? context = null)
        {
            if (eventType < _minimumLogLevel) return;

            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {context}: {message}";
            TraceSource.TraceEvent(eventType, _logIdCounter++, logMessage);
        }

        public static void SetMinimumLogLevel(TraceEventType level)
        {
            _minimumLogLevel = level;
        }

        public static void LogPlayerAction(string playerName, string action, string details)
        {
            Log($"{playerName} performed action: {action}. Details: {details}", TraceEventType.Information, "PlayerAction");
        }

        public static void LogWorldState(string stateDescription)
        {
            Log($"World state updated: {stateDescription}", TraceEventType.Information, "WorldState");
        }
    }
}

