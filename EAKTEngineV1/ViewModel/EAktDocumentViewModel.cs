// ========================================================
// File: EAktDocumentViewModel.cs
// ========================================================

using System;
using IO=System.IO;
using System.ComponentModel;

using DevExpress.Mvvm;

using EAKTEngineV1.Model;
using EAKTEngineV1.Messages;

namespace EAKTEngineV1.ViewModel
{
    /// <summary>
    /// Das ViewModel für ein einzelnes EAkt-Dokument
    /// </summary>
    public class EAktDocumentViewModel : ViewModelBase
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private EAktDocument eaktDocument;
        private EAktCommentsViewModel commentsVm;
        private PdfBookmarksViewModel bookmarksVm;
        private bool isReady;
        private int currentPageNumber;
        private string firstPagePath;
        private PdfViewerMessage msgPdfViewer;
        private DelegateCommand<bool> nextPageCommand;
        private DelegateCommand<bool> prevPageCommand;

        /// <summary>
        /// Konstruktor ohne Parameter
        /// </summary>
        public EAktDocumentViewModel()
        {
            // Spielt aktuell keine Rolle
            this.PropertyChanged += EAktDocumentViewModel_PropertyChanged;

            // Instanz der Model-Klasse anlegen
            eaktDocument = new EAktDocument();
            currentPageNumber = 1;
            RaisePropertyChanged("CurrentPageNumber");

            commentsVm = new EAktCommentsViewModel();

            // DelegateCommand-Objekte initialisieren

            // Zur nächsten Seite wechseln
            nextPageCommand = new DelegateCommand<bool>((MouseWheelMode) =>
            {
                // Referenz auf das Eltern-ViewModel holen
                EAktDocumentsViewModel parentVm = (this as ISupportParentViewModel).ParentViewModel as EAktDocumentsViewModel;

                // Aktuelle Seitennummer erhöhen
                currentPageNumber++;
                // Ist aktuelle Seitennummer größer als Gesamtzahl?
                if (currentPageNumber > eaktDocument.PageCount)
                {
                    // Soll im Dokument oder in der Mappe geblättert werden?
                    currentPageNumber = 1;
                    if (parentVm.DocumentScrollMode == ScrollMode.AllDocuments)
                    {
                        // Wechseln zum nächsten Dokument
                        if (parentVm.NextDocumentCommand.CanExecute(MouseWheelMode))
                        {
                            parentVm.NextDocumentCommand.Execute(MouseWheelMode);
                        }
                    }
                }
                else
                {
                    // Nachricht an PdfViewer-Komponente
                    msgPdfViewer = new PdfViewerMessage(PdfViewerAction.NextPage);
                    Messenger.Default.Send<PdfViewerMessage>(msgPdfViewer);
                }
                RaisePropertyChanged("CurrentPageNumber");
            }, (MouseWheelMode) =>
            {
                return true;
            }, true);

            // Wechseln zur vorherigen Seite
            prevPageCommand = new DelegateCommand<bool>((MouseWheelMode) =>
            {
                // Referenz auf das Eltern-ViewModel holen
                EAktDocumentsViewModel parentVm = (this as ISupportParentViewModel).ParentViewModel as EAktDocumentsViewModel;
                // Seitennummer erniedrigen
                currentPageNumber--;
                // Anfang des Dokuments erreicht?
                if (currentPageNumber < 1)
                {
                    // Soll im Dokument oder in der Mappe geblättert werden?
                    if (parentVm.DocumentScrollMode ==  ScrollMode.CurrentDocument)
                    {
                        currentPageNumber = 1;
                    }
                    else
                    {
                        // Vorheriges Dokument zum aktuellen Dokument machen
                        if (parentVm.PrevDocumentCommand.CanExecute(MouseWheelMode))
                        {
                            parentVm.PrevDocumentCommand.Execute(MouseWheelMode);
                            currentPageNumber = parentVm.CurrentDocument.PageCount;
                        }
                    }
                    // Nachricht an PdfViewer-Komponente
                    msgPdfViewer = new PdfViewerMessage(PdfViewerAction.GotoPage);
                    msgPdfViewer.PageNumber = currentPageNumber;
                    Messenger.Default.Send<PdfViewerMessage>(msgPdfViewer);
                }
                else
                {
                    // Nachricht an PdfViewer-Komponente
                    msgPdfViewer = new PdfViewerMessage(PdfViewerAction.PrevPage);
                    Messenger.Default.Send<PdfViewerMessage>(msgPdfViewer);
                }
                RaisePropertyChanged("CurrentPageNumber");
            }, (MouseWheelMode) =>
            {
                return true;
            }, true);
        }

        private void EAktDocumentViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Wird aktuell nicht benötigt
        }

        /// <summary>
        /// Die Id des Dokuments
        /// </summary>
        public int Id
        {
            get { return eaktDocument.Id; }
            set
            {
                eaktDocument.Id = value;
                RaisePropertyChanged("Id");
            }
        }

        /// <summary>
        /// Der Pfad des Dokuments
        /// </summary>
        public string Path
        {
            get { return eaktDocument.Path; }
            set {
                eaktDocument.Path = value;
                // Auch die Name-Eigenschaft belegen
                eaktDocument.Name = IO.Path.GetFileName(value);
                // Lesemarken für Pdf-Dateien einlesen, wenn es ein gültiges Dokument ist
                // und dies noch nicht geschehen ist
                if (IO.Path.GetExtension(value) == ".pdf" && this.IsValid &&  bookmarksVm == null)
                {
                    bookmarksVm = new PdfBookmarksViewModel(value);
                }

                // Auch die Type-Eigenschaft belegen
                // TODO: GlobalFunctions.GetDocType(eaktDocument.Path)
                RaisePropertyChanged("Name");
                RaisePropertyChanged("Path");
            }
        }

        /// <summary>
        /// Der Pfad der Pdf-Datei
        /// </summary>
        public string PdfPath
        {
            get { return eaktDocument.PdfPath; }
            set
            {
                eaktDocument.PdfPath = value;
                RaisePropertyChanged("PdfPath");
            }
        }

        /// <summary>
        /// Steht für den Pfad der ersten/einzigen Pdf-Datei
        /// </summary>
        public string FirstPagePath
        {
            get { return firstPagePath; }
            set
            {
                firstPagePath = value;
                RaisePropertyChanged("FirstPagePath");
            }
        }


        /// <summary>
        /// Der Name des Dokuments
        /// </summary>
        public string Name
        {
            get { return eaktDocument.Name; }
            set
            {
                eaktDocument.Name = value;
                RaisePropertyChanged("Name");
            }
        }

        /// <summary>
        /// Ersteller-Eigenschaft des Dokuments
        /// </summary>
        public string Ersteller
        {
            get { return eaktDocument.Ersteller; }
            set
            {
                eaktDocument.Ersteller = value;
                RaisePropertyChanged("Ersteller");
            }
        }

        /// <summary>
        /// Schriftsatz-Eigenschaft des Dokuments
        /// </summary>
        public string Schriftsatz
        {
            get { return eaktDocument.Schriftsatz; }
            set
            {
                eaktDocument.Schriftsatz = value;
                RaisePropertyChanged("Schriftsatz");
            }
        }

        /// <summary>
        /// Eingang-Eigenschaft des Dokuments
        /// </summary>
        public string Eingang
        {
            get { return eaktDocument.Eingang; }
            set
            {
                eaktDocument.Eingang = value;
                RaisePropertyChanged("Eingang");
            }
        }

        /// <summary>
        /// Ampel-Eigenschaft des Dokuments
        /// </summary>
        public DocumentIndicator Indicator
        {
            get { return eaktDocument.Indicator; }
            set
            {
                eaktDocument.Indicator = value;
                RaisePropertyChanged("Indicator");
            }
        }

        /// <summary>
        /// Typ des Dokuments
        /// </summary>
        public DocumentType Type
        {
            get { return eaktDocument.Type; }
            set
            {
                eaktDocument.Type = value;
                RaisePropertyChanged("Type");
            }
        }

        /// <summary>
        /// Gibt ab, ob das Dokumentformat gültig bzw. das Dokument lesbar ist
        /// </summary>
        public bool IsValid
        {
            get { return eaktDocument.IsValid; }
            set
            {
                eaktDocument.IsValid = value;
                RaisePropertyChanged("IsValid");
            }
        }

        /// <summary>
        /// Gibt ab, ob die Datei aus der Mappe vorhanden ist
        /// </summary>
        public bool IsPresent
        {
            get { return eaktDocument.IsPresent; }
            set
            {
                eaktDocument.IsPresent = value;
                RaisePropertyChanged("IsPresent");
            }
        }

        /// <summary>
        /// Gibt an, ob das Dokument angezeigt werden kann
        /// </summary>
        public bool IsReady
        {
            get { return isReady; }
            set
            {
                isReady = value;
                RaisePropertyChanged("IsReady");
            }
        }
        /// <summary>
        /// Gibt an, ob ein Dokument selektiert wurde - gibt es nur im ViewModel
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Anzahl der Seiten des Dokuments
        /// </summary>
        public int PageCount
        {
            get { return eaktDocument.PageCount; }
            set
            {
                eaktDocument.PageCount = value;
                RaisePropertyChanged("PageCount");
            }
        }

        /// <summary>
        /// Steht für die aktuelle Seitennummer im Rahmen der Datenbindung
        /// </summary>
        public int CurrentPageNumber
        {
            get { return currentPageNumber; }
        }

        /// <summary>
        /// Stellt alle Kommentare des Dokuments als ViewModel zur Verfügung
        /// </summary>
        public EAktCommentsViewModel CommentsVm
        {
            get { return commentsVm; }
        }

        /// <summary>
        /// Für die Lesemarken des Dokuments
        /// </summary>
        public PdfBookmarksViewModel BookmarksVm
        {
            get { return bookmarksVm; }
            set
            {
                bookmarksVm = value;
                RaisePropertiesChanged("BookmarksVm");
            }
        }

        /// <summary>
        /// Macht die vorherige Seite des Dokuments zur aktuellen Seitennummer
        /// </summary>
        public DelegateCommand<bool> NextPageCommand
        {
            get { return nextPageCommand; }
        }

        /// <summary>
        /// Macht die nächste Seite des Dokuments zur aktuellen Seitennummer
        /// </summary>
        public DelegateCommand<bool> PrevPageCommand
        {
            get { return prevPageCommand; }
        }
    }

}
