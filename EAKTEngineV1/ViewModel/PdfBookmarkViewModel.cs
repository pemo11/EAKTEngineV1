// ============================================================
// File: PdfBookmarkViewModel.cs
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Mvvm;

using EAKTEngineV1.Messages;
using EAKTEngineV1.Model;

namespace EAKTEngineV1.ViewModel
{
    /// <summary>
    /// Das ViewModel für eine Lesemarke in einem Pdf-Dokument
    /// </summary>
    public class PdfBookmarkViewModel : ViewModelBase
    {
        private PdfBookmark pdfBookmark;

        /// <summary>
        /// Konstruktor ohne Parameter
        /// </summary>
        public PdfBookmarkViewModel()
        {
            pdfBookmark = new PdfBookmark();
        }

        /// <summary>
        /// Seitennummer der Lesemarke
        /// </summary>
        public int PageNr
        {
            get { return pdfBookmark.PageNr; }
            set {
                pdfBookmark.PageNr = value;
                RaisePropertyChanged("PageNr");
            }
        }

        /// <summary>
        /// Titel der Lesemarke
        /// </summary>
        public string Title
        {
            get { return pdfBookmark.Title; }
            set {
                pdfBookmark.Title = value;
                RaisePropertyChanged("Title");
            }
        }

        /// <summary>
        /// Die mit der Lesemarke verknüpfte Aktion
        /// </summary>
        public string Action
        {
            get { return pdfBookmark.Action; }
            set
            {
                pdfBookmark.Action = value;
                RaisePropertyChanged("Action");
            }
        }

    }
}
