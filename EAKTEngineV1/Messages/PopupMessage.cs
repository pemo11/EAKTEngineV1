// ========================================================
// File: PopupMessage.cs
// ========================================================

using System;

using DevExpress.Mvvm;

namespace EAKTEngineV1.Messages
{
    /// <summary>
    /// Zeigt eine Popup-Meldung in der Host-Anwendung an
    /// </summary>
    public class PopupMessage : Messenger
    {
        /// <summary>
        /// Die anzuzeigende Meldung
        /// </summary>
        public string Message { get; set; }

    }
}
