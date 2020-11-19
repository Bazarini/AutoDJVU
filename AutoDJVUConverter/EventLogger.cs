using System.Diagnostics;

namespace AutoDJVUConverter
{
    public static class EventLogger
    {
        static EventLog Log;
        public static void InitLogger(EventLog log)
        {
            Log = log;
        }
        public static void LogMessage(string message, EventLogEntryType eventLogEntryType)
        {
            Log.WriteEntry(message, eventLogEntryType);
        }
    }
}
