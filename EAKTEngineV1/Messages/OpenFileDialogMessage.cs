// ========================================================
// File: OpenFileDialogMessage.cs
// ========================================================

using System;

using DevExpress.Mvvm;

namespace EAKTEngineV1.Messages
{
    /// <summary>
    /// Öffnet den Dateiauswahldialog in der Host-Anwendung
    /// </summary>
    public class OpenFileDialogMessage : Messenger
    {
        /// <summary>
        /// Der Titel des Dialogfeldes
        /// </summary>
        public string DialogTitle { get; set; }

        /// <summary>
        /// Der Filter für die Dateiauswahl
        /// </summary>
        public string DialogFilter { get; set; }

        /// <summary>
        /// Die Aktion, die durch den OK-Button ausgeführt wird
        /// </summary>
        public Action<string> Execute { get; set; }

    }
}
