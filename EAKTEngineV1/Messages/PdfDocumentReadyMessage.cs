// ========================================================
// File: PdfDocumentReadyMessage.cs
// ========================================================
using System;
using EAKTEngineV1.ViewModel;

using DevExpress.Mvvm;

namespace EAKTEngineV1.Messages
{
    /// <summary>
    /// Meldet, dass ein Pdf-Dokument fertig ist und vom Pdf-Viewer angezeigt werden kann
    /// </summary>
    public class PdfDocumentReadyMessage : Messenger
    {
        /// <summary>
        /// Der Pfad des Pdf-Dokuments
        /// </summary>
        public string PdfPath { get; set; }

        /// <summary>
        /// Der Pfad der erste Seite
        /// </summary>
        public string FirstPagePath { get; set; }

    }
}
