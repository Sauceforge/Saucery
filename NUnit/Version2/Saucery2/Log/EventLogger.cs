using System;
using System.Diagnostics;

namespace Saucery2.Log {
    internal class EventLogger {
        private static readonly EventLog EventLog = new EventLog(string.Empty);
        private const string Source = "Saucery";
        private static readonly bool AreLogging = true;

        static EventLogger() {
            try {
                if(!EventLog.SourceExists(Source)) {
                    EventLog.CreateEventSource(Source, Source);
                }
                EventLog.Source = Source;
            } catch(Exception) {
                AreLogging = false;
            }
        }

        public static void WriteInfo(string message) {
            WriteLog(message, EventLogEntryType.Information);
        }

        public static void WriteWarning(string message) {
            WriteLog(message, EventLogEntryType.Warning);
        }

        public static void WriteError(string message) {
            WriteLog(message, EventLogEntryType.Error);
        }

        public static void WriteFailureAudit(string message) {
            WriteLog(message, EventLogEntryType.FailureAudit);
        }

        public static void WriteSuccessAudit(string message) {
            WriteLog(message, EventLogEntryType.SuccessAudit);
        }

        private static void WriteLog(string message, EventLogEntryType entryType) {
            if(AreLogging) {
                EventLog.WriteEntry(message, entryType);
            }
        }
    }
}