// ========================================================
// File: SetProgressbarMessage.cs
// ========================================================

using System;

using DevExpress.Mvvm;

namespace EAKTEngineV1.Messages
{
    /// <summary>
    /// Aktualisiert den Fortschrittsbalken in der Host-Anwendung
    /// </summary>
    public class SetProgressbarMessage : Messenger
    {
        /// <summary>
        /// Der aktuelle Wert des Fortschrittsbalkens
        /// </summary>
        public int Value { get; set; }

    }
}
