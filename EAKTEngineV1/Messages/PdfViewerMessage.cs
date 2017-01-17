// ============================================================
// File: PdfViewerMessage.cs
// ============================================================

using System;
using DevExpress.Mvvm;

namespace EAKTEngineV1.Messages
{
    /// <summary>
    /// Mögliche Aktionen für die PdfViewer-Komponente
    /// </summary>
    public enum PdfViewerAction
    {
        NextPage,
        PrevPage,
        FirstPage,
        LastPage,
        GotoPage,
        LoadDocument,
        SetZoom,
        CopyText,
        ZoomIn,
        ZoomOut
    }

    /// <summary>
    /// Repräsentiert eine Meldung an die PdfViewer-Komponente
    /// </summary>
    public class PdfViewerMessage : Messenger
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private PdfViewerAction action;
        private int pageNumber;

        /// <summary>
        /// Konstruktor mit Parameter
        /// </summary>
        /// <param name="Action"></param>
        public PdfViewerMessage(PdfViewerAction Action)
        {
            action = Action;
        }

        /// <summary>
        /// Steht für die Aktion, die die PdfViewer-Komponente ausführen soll
        /// </summary>
        public PdfViewerAction Action
        {
            get { return action; }
        }

        /// <summary>
        /// Steht für die Seitennummer, die angezeigt werden soll
        /// </summary>
        public int PageNumber
        {
            get {  return PageNumber;}
            set
            {
                pageNumber = value;
            }
        }
    }
}
