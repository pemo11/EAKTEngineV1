// ========================================================
// File: GlobalFunctions.cs
// ========================================================

using System;

using NLog;
using EAKTEngineV1.Messages;

using DevExpress.Mvvm;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Enthält allgemeine Funktionen
    /// </summary>
    public static class GlobalFunctions
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private static ILogger logger;
        private static AddStatusMessage msgAddStatus;

        /// <summary>
        /// Statischer Konstruktor
        /// </summary>
        static GlobalFunctions()
        {
            // NLog-Logger initialisieren
            logger = LogManager.GetCurrentClassLogger();
        }

        /// <summary>
        /// Schreiben einer Info-Meldung
        /// </summary>
        /// <param name="Message"></param>
        public static void LogInfo(string Message)
        {
            LogEventInfo eventInfo = new LogEventInfo(LogLevel.Info, "InfoLogger", Message);
            eventInfo.Properties["LogTime"] = DateTime.Now.ToString("dd/MM/yy HH:mm");
            logger.Info(eventInfo);
            // Meldung an Host-Anwendung
            msgAddStatus = new AddStatusMessage
            {
                StatusMessage = new ViewModel.StatusMessageViewModel
                {
                    Message = Message,
                    IsError = false,
                    TimeStamp = DateTime.Now
                }
            };
            Messenger.Default.Send<AddStatusMessage>(msgAddStatus);
        }

        /// <summary>
        /// Schreiben einer Fehlermeldung
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="ex"></param>
        public static void LogError(string Message, SystemException ex)
        {
            if (ex != null)
            {
                Message += " (Exception: " + ex.ToString() + ")";
            }
            LogEventInfo eventInfo = new LogEventInfo(LogLevel.Error, "ErrorLogger", Message);
            eventInfo.Properties["LogTime"] = DateTime.Now.ToString("dd/MM/yy HH:mm");
            logger.Info(eventInfo);
            // Meldung an Host-Anwendung
            msgAddStatus = new AddStatusMessage {
                StatusMessage = new ViewModel.StatusMessageViewModel
                {
                    Message = Message,
                    IsError = true,
                    TimeStamp = DateTime.Now
                }
            };
            Messenger.Default.Send<AddStatusMessage>(msgAddStatus);
        }
    }
}
