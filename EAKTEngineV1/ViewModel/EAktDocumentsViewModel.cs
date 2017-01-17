// ========================================================
// File: EAktDocumentsViewModel.cs
// ========================================================

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using EAKTEngineV1.Messages;
using EAKTEngineV1.Helper;

using DevExpress.Mvvm;

namespace EAKTEngineV1.ViewModel
{
    /// <summary>
    /// Repräsentiert die verschiedenen Scroll-Varianten
    /// </summary>
    public enum ScrollMode
    {
        CurrentDocument,
        AllDocuments
    }

    /// <summary>
    /// Das ViewModel für eine EAkt-Mappe
    /// </summary>
    public class EAktDocumentsViewModel : ViewModelBase
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private string statusMessage;
        private ObservableCollection<EAktDocumentViewModel> documents;
        private EAktDocumentViewModel currentDocument;
        private int currentDocumentIndex;
        private int errorCount;
        private string eaktName;
        private ScrollMode documentScrollMode;

        /// DelegateCommand-Variablen
        private DelegateCommand<bool> nextDocumentCommand;
        private DelegateCommand<bool> prevDocumentCommand;
        private DelegateCommand<int> gotoDocumentCommand;
        private DelegateCommand switchScrollModeCommand;

        /// <summary>
        /// Nur provisorisch
        /// </summary>
        public void SetCurrentDocument()
        {
            currentDocument = documents[0];
            RaisePropertyChanged("CurrentDocument");
        }

        /// <summary>
        /// Konstruktor ohne Parameter für die Initialisierung von Variablen
        /// </summary>
        public EAktDocumentsViewModel()
        {
            /// Anlegen einer Documents-Collection mit EAktDocumentViewModel-Objekten
            documents = new ObservableCollection<EAktDocumentViewModel>();

            /// DelegateCommand-Objekte initialisieren

            // Wechseln zum nächsten Dokument
            nextDocumentCommand = new DelegateCommand<bool>((MouseWheelMode) =>
            {
                // TODO: MouseWheelModel-Parameter spielt für das Blättern in der Mappe aktuell keine Rolle
                // Kann vermutlich wieder raus
                currentDocumentIndex++;
                // Ende der Mappe erreicht?
                if (currentDocumentIndex == documents.Count)
                {
                    // Erstes Dokument zum aktuellen Dokument machen
                    currentDocumentIndex = 0;
                }

                // Aktuelles Dokument festlegen
                currentDocument= documents[currentDocumentIndex];
                RaisePropertyChanged("CurrentDocument");
            }, (MouseWheelMode) =>
            {
                return true;
            }, true);

            // Wechseln zum vorherigen Dokument
            prevDocumentCommand = new DelegateCommand<bool>((MouseWheelMode) =>
            {
                // TODO: MouseWheelModel-Parameter spielt für das Blättern in der Mappe aktuell keine Rolle
                // Kann vermutlich wieder raus
                currentDocumentIndex--;
                // Anfang der Mappe erreicht?
                if (currentDocumentIndex < 0)
                {
                    // Letztes Dokument zum aktuellen Dokument machen
                    currentDocumentIndex = documents.Count -1;
                }

                // Aktuelles Dokument festlegen
                currentDocument = documents[currentDocumentIndex];
                RaisePropertyChanged("CurrentDocument");

            }, (MouseWheelMode) =>
            {
                return true;
            }, true);

            // Zu einem bestimmten Dokument wechseln
            gotoDocumentCommand = new DelegateCommand<int>((docNr) =>
            {
                // Dokument lokalisieren
                EAktDocumentViewModel eaktDoc = (from d in documents where d.Id == docNr select d).SingleOrDefault();
                if (eaktDoc != null)
                {
                    currentDocument = eaktDoc;
                    RaisePropertiesChanged("CurrentDocument");
                }
            }, (nr) =>
            {
                return true;
            }, true);

            // Umschalten des Scroll-Modus für das Scrollen
            switchScrollModeCommand = new DelegateCommand(() =>
            {
                switch (documentScrollMode)
                {
                    case ScrollMode.CurrentDocument:
                        {
                            documentScrollMode = ScrollMode.AllDocuments;
                            break;
                        }
                    case ScrollMode.AllDocuments:
                        {
                            documentScrollMode = ScrollMode.CurrentDocument;
                            break;
                        }
                    default:
                        {
                            documentScrollMode = ScrollMode.CurrentDocument;
                            break;
                        }
                }
            }, true);
        }

        /// <summary>
        /// Konstruktor mit dem Pfad einer EAkt-Mappendatei
        /// </summary>
        /// <param name="EAktPath"></param>
        public EAktDocumentsViewModel(IDocumentService DocumentService, string EAktPath) : this()
        {
            eaktName = Path.GetFileNameWithoutExtension(EAktPath);
            documents = DocumentService.GetDocuments(EAktPath);

            // TODO: Nur ein Versuch - bei allen EAktDocumentViewModel-Instanzen die ParentViewModel-Eigenschaft setzen
            // Damit ist ein Zugriff z.B. auf die DelegateCommands des EAKtDocumentViewModels möglich
            documents.ToList().ForEach((d) => { ((ISupportParentViewModel)d).ParentViewModel = this; });
        }

        /// <summary>
        /// Konstruktor mit dem Namen der EAkt-Datei
        /// </summary>
        /// <param name="EAktName"></param>
        public EAktDocumentsViewModel(string EAktName)
        {
            eaktName = EAktName;
            documents = new ObservableCollection<EAktDocumentViewModel>();
        }

        /// <summary>
        /// Stellt alle Dokumente der Mappe zur Verfügung
        /// </summary>
        public ObservableCollection<EAktDocumentViewModel> Documents
        {
            get { return documents; }
            set
            {
                documents = value;
                // Wenn mind. ein Dokument vorhanden, aktuelles Dokument festlegen
                if (documents.Count > 0)
                {
                    currentDocument = documents[0];
                    RaisePropertyChanged("Documents");
                    RaisePropertyChanged("CurrentDocument");
                }
                else
                {
                    // TODO: Fehler oder Zustand kein Dokument loggen?
                    throw new EAKTEngineV1Exception("Kein Dokument in der Auflistung enthalten!");
                }
            }
        }

        /// <summary>
        /// Stellt alle Kommentare zur Verfügung
        /// </summary>
        public List<EAktCommentViewModel> AllComments
        {
            // Alle Kommentare aller Dokumente zurückgeben
            get
            {
                return documents.SelectMany(d => d.CommentsVm.Comments).ToList();
            }
        }

        /// <summary>
        /// Fügt ein neues EAkt-Dokument zur aktuellen Mappe hinzu  
        /// </summary>
        /// <returns></returns>
        public EAktDocumentViewModel AddEAktDocument()
        {
            // EAktDocumentVm anlegen
            EAktDocumentViewModel eaktDoc = new EAktDocumentViewModel();
            eaktDoc.Id = documents.Count + 1;
            documents.Add(eaktDoc);
            return eaktDoc;
        }

        /// <summary>
        /// Entfernt ein EAkt-Dokument aus der Mappe
        /// </summary>
        /// <param name="Id"></param>
        public void RemoveEAktDocument(int Id)
        {
            EAktDocumentViewModel eaktDoc = (from d in documents where d.Id == Id select d).SingleOrDefault();
            // Gibt es ein Dokument mit der Id?
            if (eaktDoc != null)
            {
                documents.Remove(eaktDoc);
            }
        }

        /// <summary>
        /// Speichert alle Dokumentkommentare in der Standardkommentardatei
        /// </summary>
        public void SaveComments()
        {
            try
            {
                CommentFunctions.SaveEAktComments(this);
            }
            catch (SystemException ex)
            {
                // Fehler wird nur weitergereicht
                throw ex;
            }
        }

        /// <summary>
        /// Speichert alle Dokumentkommentare in einer angegebenen Kommentardatei
        /// </summary>
        /// <param name="CommentPath"></param>
        public void SaveComments(string CommentPath)
        {
            try
            {
                CommentFunctions.SaveEAktComments(this, CommentPath);
            }
            catch (SystemException ex)
            {
                // Fehler wird nur weitergereicht
                throw ex;
            }
        }

        /// <summary>
        /// Durchsucht alle Kommentare der EAkt-Mappe nach einem Suchbegriff
        /// </summary>
        /// <param name="documentsVm"></param>
        /// <param name="SearchTerm"></param>
        /// <returns></returns>
        public List<EAktCommentViewModel> SearchComments(CommentSearchRecord record )
        {
            List<EAktCommentViewModel> tmpComments = null;
            try
            {
                // Alle Kommentare zusammenfassen
                tmpComments = documents.SelectMany(d => d.CommentsVm.Comments).ToList();
                // Kommentartexte durchsuchen 
                // Aktuell nur nach dem Suchwort
                tmpComments = tmpComments.Where((c) => c.Comment.Contains(record.SearchWord)).ToList();
                statusMessage = String.Format("{0} Kommentare gefunden.", tmpComments.Count);
            }
            catch (SystemException ex)
            {
                statusMessage = "Allgemeiner Fehler beim Durchsuchen der Aktenkommentare.";
                GlobalFunctions.LogError(statusMessage, ex);
            }
            return tmpComments;
        }

        /// <summary>
        /// Steht für den Namen der EAkt-Mappe
        /// </summary>
        public string EAktName
        {
            get { return eaktName; }
        }

        /// <summary>
        /// Steht für das aktuelle Dokument in der Mappe für die Datenbindung
        /// </summary>
        public EAktDocumentViewModel CurrentDocument
        {
            get { return currentDocument; }
            set
            {
                currentDocument = value;
                RaisePropertyChanged("CurrentDocument");
            }
        }

        /// <summary>
        /// Liefert die Anzahl der ungültigen Dokumente
        /// </summary>
        public int ErrorCount
        {
            get { return documents.Where((d) => { return !d.IsValid; }).Count(); }
        }

        /// <summary>
        /// Steht für den aktuellen Blätter-Modus - im Dokument oder innerhalb der Mappe
        /// </summary>
        public ScrollMode DocumentScrollMode
        {
            get { return documentScrollMode; }
            set
            {
                documentScrollMode = value;
            }
        }

        /// <summary>
        /// Das nächste Dokument in der Mappe zum aktuellen Dokument machen
        /// </summary>
        public DelegateCommand<bool> NextDocumentCommand
        {
            get { return nextDocumentCommand; }
        }

        /// <summary>
        /// Das vorherige Dokument in der Mappe zum aktuellen Dokument machen
        /// </summary>
        public DelegateCommand<bool> PrevDocumentCommand
        {
            get { return prevDocumentCommand; }
        }

        /// <summary>
        /// Ein bestimmtes Dokument anhand seiner Nr zum aktuellen Dokument machen
        /// </summary>
        public DelegateCommand<int> GotoDocumentCommand
        {
            get { return gotoDocumentCommand; }
        }

        /// <summary>
        /// Umschalten zwischen Blättern in der Mappe und in einem Dokument
        /// </summary>
        public DelegateCommand SwitchScrollModeCommand
        {
            get { return switchScrollModeCommand; }
        }
    }
}
