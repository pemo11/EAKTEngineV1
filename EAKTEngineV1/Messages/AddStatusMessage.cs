// ========================================================
// File: AddStatusMessage.cs
// ========================================================

using System;

using EAKTEngineV1.ViewModel;

using DevExpress.Mvvm;

namespace EAKTEngineV1.Messages
{
    /// <summary>
    /// Schickt eine Statusmeldung an die Host-Anwendung
    /// </summary>
    public class AddStatusMessage : Messenger
    {
        /// <summary>
        /// Die Meldung als Zeichenkette
        /// </summary>
        public StatusMessageViewModel StatusMessage { get; set; }

    }
}
