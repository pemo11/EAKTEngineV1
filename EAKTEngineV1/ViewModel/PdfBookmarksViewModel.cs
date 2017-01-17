// ============================================================
// File: PdfBookmarksViewModel.cs
// ============================================================

using System;
using System.IO;
using System.Collections.ObjectModel;
using Pdf = Aspose.Pdf;

using DevExpress.Mvvm;
using Facades = Aspose.Pdf.Facades;

using EAKTEngineV1.Helper;

namespace EAKTEngineV1.ViewModel
{
    /// <summary>
    /// Das ViewModel für alle Lesemarken eines Dokuments
    /// </summary>
    public class PdfBookmarksViewModel : ViewModelBase
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private ObservableCollection<PdfBookmarkViewModel> bookmarks;

        /// <summary>
        /// Konstruktor mit Pfad der Pdf-Datei
        /// </summary>
        /// <param name="PdfPath"></param>
        public PdfBookmarksViewModel(string PdfPath)
        {
            try
            {
                bookmarks = new ObservableCollection<PdfBookmarkViewModel>();
                // Alle Lesemarken aus der Pdf-Datei extrahieren
                Facades.PdfBookmarkEditor bookmarkEditor = new Aspose.Pdf.Facades.PdfBookmarkEditor();
                bookmarkEditor.BindPdf(PdfPath);
                Facades.Bookmarks pdfBookmarks = bookmarkEditor.ExtractBookmarks();
                // Alle Lesemarken als PdfBookmarkViewModel-Objekte anlegen
                foreach (Facades.Bookmark pdfBookmark in pdfBookmarks)
                {
                    bookmarks.Add(new PdfBookmarkViewModel
                    {
                        PageNr = pdfBookmark.PageNumber,
                        Title = pdfBookmark.Title,
                        Action = pdfBookmark.Action
                    });
                }
            }
            catch (SystemException ex)
            {
                // TODO: Provisorisch
                throw ex;
            }
        }

        /// <summary>
        /// Steht für alle Lesemarken des Dokuments
        /// </summary>
        public ObservableCollection<PdfBookmarkViewModel> Bookmarks
        {
            get { return bookmarks; }
        }

    }
}
