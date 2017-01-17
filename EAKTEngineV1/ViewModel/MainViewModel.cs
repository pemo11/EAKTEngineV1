// ========================================================
// File: MainViewModel.cs
// ========================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Practices.Unity;

using EAKTEngineV1.Messages;
using EAKTEngineV1.Helper;

using DevExpress.Mvvm;

namespace EAKTEngineV1.ViewModel
{
    /// <summary>
    /// Die zentrale MainViewModel-Klasse
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        // DelegateCommand-Variablen
        private StatusMessagesViewModel messages;
        private DelegateCommand openEAktCommand;
        private DelegateCommand<string> loadEAktCommand;
        private AsyncCommand openEAktAsyncCommand;
        private AsyncCommand<string> loadEAktAsyncCommand;
        private DelegateCommand<string> newEAktCommand;
        private AsyncCommand<string> readDirectoryAsyncCommand;
        private DelegateCommand<EAktDocumentFilter> applyFilterCommand;
        private DelegateCommand saveFilterCommand;
        private DelegateCommand clearFilterCommand;
        private DelegateCommand copySelectedTextCommand;
        private DelegateCommand zoomInCommand;
        private DelegateCommand zoomOutCommand;
        private DelegateCommand<double> zoomChangedCommand;

        // Variablen für die Dokumente
        private EAktDocumentsViewModel documentsVm;
        private ObservableCollection<EAktDocumentViewModel> allDocuments;
        private LRUDocumentsViewModel lruDocuments;

        // Container-Variable
        private UnityContainer unityContainer;

        // Weitere Variablen
        private DateTime startTime;
        private TimeSpan duration;
        private TimeSpan totalDuration;
        private string statusMessage;
        private int lruLimit = 3;
        private double currentZoomValue;

        /// <summary>
        /// Dient nur zum Test der Datenbindung
        /// </summary>
        public string AppTitle {  get { return "Powered by EAKTEngineV1"; } }

        /// <summary>
        /// Liefert den DispatcherService, der in der Host-Anwendung registriert wurde
        /// </summary>
        private IDispatcherService DispatcherService
        {
            get { return GetService<IDispatcherService>(); }
        }

        private IOpenFileDialogService OpenFileDialogService
        {
            get { return GetService<IOpenFileDialogService>(); }
        }

        /// <summary>
        /// Legt fest, ob beim Indizieren der zeilenbasierte Index verwendet wird
        /// Aktuell nur für das Testen ein Thema
        /// </summary>
        public bool UseLineIndex { get; set; }

        /// <summary>
        /// Legt fest, ob Vorschauseiten angelegt werden
        /// Spielt für das Testen eine Rolle
        /// </summary>
        public bool CreatePreviewPages { get; set; }
        /// <summary>
        /// Das Hauptview-Model mit allen Commands und Eigenschaften für die Datenbindung
        /// </summary>
        public MainViewModel()
        {
            // Unity-Container initialisieren
            unityContainer = new UnityContainer();
            unityContainer.RegisterType<IDocumentService, FileDocumentService>();
            unityContainer.RegisterType<EAktDocumentsViewModel>("FileService", new InjectionConstructor(new FileDocumentService(), ""));

            unityContainer.RegisterType <EAktDocumentsViewModel> (new InjectionConstructor(typeof(string)));

            messages = new StatusMessagesViewModel();

            // Lizenzdateien einlesen
            Aspose.Pdf.License AspPdfLic = new Aspose.Pdf.License();
            AspPdfLic.SetLicense("Aspose.Pdf-1-351582.lic");
            Aspose.Words.License AspWordsLic = new Aspose.Words.License();
            AspWordsLic.SetLicense("Aspose.Words-4.lic");

            statusMessage = "Lizenzdateien wurden initialisiert";
            LogStatus(statusMessage, false, true);

            // LRU-Liste initialisieren
            lruDocuments = new LRUDocumentsViewModel(lruLimit);

            // Nachrichten registrieren
            /*
            Messenger.Default.Register<AddStatusMessage>(this, (m) =>
            {
                LogStatus(m.StatusMessage.Message, m.StatusMessage.IsError);
            });
            */

            // DelegateCommands implementieren

            // Öffnen einer EAkt-Datei
            openEAktCommand = new DelegateCommand(() =>
            {
                OpenFileDialogMessage msgFile = new OpenFileDialogMessage();
                msgFile.DialogTitle = "EAkt-Datei auswählen";
                msgFile.DialogFilter = "EAkt-Datei (*.eakt)|*.eakt|Xml-Dateien|*.xml|Alle Dateien|*.*";
                msgFile.Execute = (path) =>
                {
                    LoadEAktFile(path);
                };
                Messenger.Default.Send<OpenFileDialogMessage>(msgFile);
                RaisePropertyChanged("Messages");
            });

            // Laden einer EAkt-Datei über den Pfad
            loadEAktCommand = new DelegateCommand<String>((EAktPath) =>
            {
                LoadEAktFile(EAktPath);
                RaisePropertyChanged("DocumentsVm");
            });

            // Asynchrones Laden einer EAkt-Datei
            openEAktAsyncCommand = new AsyncCommand(() =>
            {
                DocumentConvertRecord record = null;
                ClearStatusLog();
                Task tMain = Task.Factory.StartNew(() =>
                {
                    // OpenFileDialogService.ShowDialog(@"H:\\Aktenviewer\Testdokumente");
                    OpenFileDialogMessage msgFile = new OpenFileDialogMessage();
                    msgFile.DialogTitle = "EAkt-Datei auswählen";
                    msgFile.DialogFilter = "EAkt-Datei (*.eakt)|*.eakt|Xml-Dateien|*.xml|Alle Dateien|*.*";
                    msgFile.Execute = (eaktPath) =>
                    {
                        Task tStep1 = Task.Factory.StartNew(() =>
                        {
                            // Schritt 1: EAkt-Datei laden und in ein EAktDocumentsViewModel konvertieren
                            startTime = DateTime.Now;
                            // documentsVm = EAktFunctions.LoadEAkt(eaktPath);
                            documentsVm = unityContainer.Resolve<EAktDocumentsViewModel>("FileService", new ParameterOverride("EAktPath", eaktPath));
                            RaisePropertyChanged("DocumentsVm");
                            duration = DateTime.Now - startTime;
                            totalDuration += duration;
                            DispatcherService.BeginInvoke(() =>
                            {
                                statusMessage = String.Format("{0} Dokumente wurden in {1:n2}s geladen.", documentsVm.Documents.Count, duration.TotalSeconds);
                                LogStatus(statusMessage);
                            });
                        }).ContinueWith((tStep2) =>
                        {
                            // Schritt 2: Dokumente nach Pdf konvertieren
                            startTime = DateTime.Now;
                            record = EAktConverter.ConvertDocuments(documentsVm);
                            duration = DateTime.Now - startTime;
                            totalDuration += duration;
                            DispatcherService.BeginInvoke(() =>
                            {
                                statusMessage = String.Format("{0} Pdf-Dokumente und {1} andere Dokumenttypen wurden mit {2} Fehlern in {3:n2}s konvertiert.", record.PdfDocumentCount, record.DocumentConvertedCount - record.PdfDocumentCount, record.ConvertErrorCount, duration.TotalSeconds);
                                LogStatus(statusMessage);
                            });
                        }).ContinueWith((tStep3) =>
                        {
                            // Schritt 3: Dokumente für Volltextsuche indizieren
                            startTime = DateTime.Now;
                            if (this.UseLineIndex)
                            {
                                LuceneFunctions.CreateLineIndex(documentsVm.Documents);
                            }
                            else
                            {
                                LuceneFunctions.CreateIndex(documentsVm.Documents);
                            }
                            duration = DateTime.Now - startTime;
                            totalDuration += duration;
                            DispatcherService.BeginInvoke(() =>
                            {
                                statusMessage = String.Format("{0} Dokumente wurden in {1:n2}s indiziert.",
                                    record.DocumentConvertedCount, duration.TotalSeconds);
                                LogStatus(statusMessage);
                            });
                        }).ContinueWith((tStep4) =>
                        {
                            startTime = DateTime.Now;
                            int anzahlVorschauSeiten = PreviewFunctions.CreateDocumentsPreview(documentsVm.Documents);
                            duration = DateTime.Now - startTime;
                            totalDuration += duration;
                            DispatcherService.BeginInvoke(() =>
                            {
                                statusMessage = String.Format("Für {0} Dokumente wurden {1} Vorschauseiten in {2:n2}s angelegt.",
                                    record.DocumentConvertedCount, anzahlVorschauSeiten, duration.TotalSeconds);
                                LogStatus(statusMessage);
                            });
                        }).ContinueWith((tStep5) =>
                        {
                            CommentFunctions.LoadEAktComments(documentsVm);
                            DispatcherService.BeginInvoke(() =>
                            {
                                statusMessage = String.Format("{0} Kommentare eingelesen.", documentsVm.AllComments.Count);
                                LogStatus(statusMessage);
                            });
                        }).ContinueWith((tStep6) =>
                        {
                            DispatcherService.BeginInvoke(() =>
                            {
                                statusMessage = String.Format("OpenEaktCommand wurde vollständig ausgeführt. Dauer: {0:n2}s", totalDuration.TotalSeconds);
                                LogStatus(statusMessage);
                            });
                            // Soll das aktuelle Dokument setzen und die Datenbindung aktualisieren
                            RaisePropertyChanged("DocumentsVm");
                            this.DocumentsVm.SetCurrentDocument();
                        });
                    };
                    Messenger.Default.Send<OpenFileDialogMessage>(msgFile);
                });
                return tMain;
            });

            // Asynchrones Laden einer EAkt-Mappe
            loadEAktAsyncCommand = new AsyncCommand<string>((eaktPath) =>
            {
                ClearStatusLog();
                Task tMain = LoadEAktAsync(eaktPath);
                RaisePropertyChanged("DocumentsVm");
                return tMain;
            }); 



             // Neue EAkt-Mappe anlegen
             newEAktCommand = new DelegateCommand<string>((EAktName) =>
            {
                // TODO: Über DI-Container mit EAkt-Namen als Parameter
                // documentsVm = new EAktDocumentsViewModel(EAktName);
                documentsVm = unityContainer.Resolve<EAktDocumentsViewModel>(new ParameterOverride("EAktName", EAktName));
                GlobalVars.EAktName = EAktName;
            });

            // Einlesen einer Verzeichnisstruktur
            readDirectoryAsyncCommand = new AsyncCommand<string>((DirectoryPath) =>
            {
                return ReadDirectoryAsync(DirectoryPath);
            });

            // Anwenden des Dokumentefilters
            applyFilterCommand = new DelegateCommand<EAktDocumentFilter>((filter) =>
            {
                try
                {
                    CultureInfo ci = Thread.CurrentThread.CurrentCulture;

                    // filterDocumentsVm = new EAktDocumentsViewModel();
                    // allDocumentsVm = new EAktDocumentsViewModel { Documents = documentsVm.Documents };
                    allDocuments = documentsVm.Documents.ToObservable();

                    // Ist der Ersteller-Filter gesetzt?
                    if (!String.IsNullOrEmpty(filter.Ersteller))
                    {
                        documentsVm.Documents = (documentsVm.Documents.Where((d) => ci.CompareInfo.IndexOf(d.Ersteller, filter.Ersteller,
                             CompareOptions.IgnoreCase) >= 0).ToObservable());
                    }

                    // Ist der Schriftsatzfilter gesetzt?
                    if (!String.IsNullOrEmpty(filter.Schriftsatz))
                    {
                        documentsVm.Documents = (documentsVm.Documents.Where((d) => ci.CompareInfo.IndexOf(d.Schriftsatz, filter.Schriftsatz, CompareOptions.IgnoreCase) >= 0).ToObservable());
                    }

                    DateTime fromDate = DateTime.Now;
                    DateTime untilDate = DateTime.Now;

                    // Ist der Eingang-Filter gesetzt?
                    if (!(String.IsNullOrEmpty(filter.EingangVon) && String.IsNullOrEmpty(filter.EingangBis)))
                    {
                        // TODO: Aufruf mit ci und zweitem Argument nur ein Versuch wegen VSOnline-Fehler
                        if (!DateTime.TryParse(filter.EingangVon, ci, DateTimeStyles.AssumeLocal, out fromDate))
                        {
                            // Sollte nicht vorkommen
                        }
                        if (!DateTime.TryParse(filter.EingangBis, ci, DateTimeStyles.AssumeLocal, out untilDate))
                        {
                            // Sollte nicht vorkommen
                        }

                        documentsVm.Documents = documentsVm.Documents.Where((d) => DateTime.Parse(d.Eingang) >= fromDate &&
                            DateTime.Parse(d.Eingang) <= untilDate).ToObservable();
                    }

                    // Ist der Ampel-Filter gesetzt?
                    if (filter.Ampel.HasValue)
                    {
                        documentsVm.Documents = (documentsVm.Documents.Where((d) => d.Indicator == filter.Ampel).ToObservable());
                    }

                    if (documentsVm.Documents.Count == 0)
                    {
                        statusMessage = "Filter führt zu keinem Ergebnis - Auswahl wird zurückgesetzt.";
                        documentsVm.Documents = allDocuments;
                    }
                    else if (documentsVm.Documents.Count == 1)
                    {
                        statusMessage = "Ein Dokument in der Auswahl.";
                    }
                    else
                    {
                        statusMessage = String.Format("{0} Dokumente in der Auswahl", documentsVm.Documents.Count);
                    }

                    // Datenbindung aktualisieren
                    RaisePropertyChanged("Documents");
                    GlobalFunctions.LogInfo(statusMessage);

                }
                catch (SystemException ex)
                {
                    statusMessage = "Fehler beim Anwenden des Dokumentefilters";
                    GlobalFunctions.LogError(statusMessage, ex);
                }

            });

            // Speichern des Dokumentfilters in einer EAkt-Datei
            saveFilterCommand = new DelegateCommand(() =>
            {
                try
                {
                    string EAktFilterpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                        this.AppInfo.EAktName + "_FilterAuswahl.eakt");
                    if (!String.IsNullOrEmpty(GlobalVars.FilterPath))
                    {
                        EAktFilterpath = GlobalVars.FilterPath;
                    }
                    EAktFunctions.SaveEAktDocument(documentsVm, EAktFilterpath);
                    statusMessage = "Die Filterauswahl wurde unter " + EAktFilterpath + " gespeichert.";
                    GlobalFunctions.LogInfo(statusMessage);
                }
                catch (SystemException ex)
                {
                    statusMessage = "Fehler beim SaveFilter-Command";
                    GlobalFunctions.LogError(statusMessage, ex);
                }
            });

            // Zurücksetzen des Filters
            clearFilterCommand = new DelegateCommand(() =>
            {
                documentsVm.Documents = allDocuments;
                RaisePropertyChanged("Documents");
            });

            // Kopieren des selektierten Textes im PdfViewer
            // TODO: Nur provisorisch - eventuell besser in EAKTDocumentsViewModel?
            copySelectedTextCommand = new DelegateCommand(() =>
            {
                PdfViewerMessage msgPdfViewer = new PdfViewerMessage(PdfViewerAction.CopyText);
                Messenger.Default.Send<PdfViewerMessage>(msgPdfViewer);
            });

            zoomInCommand = new DelegateCommand(() =>
            {
                PdfViewerMessage msgPdfViewer = new PdfViewerMessage(PdfViewerAction.ZoomIn);
                Messenger.Default.Send<PdfViewerMessage>(msgPdfViewer);
            });

            zoomOutCommand = new DelegateCommand(() =>
            {
                PdfViewerMessage msgPdfViewer = new PdfViewerMessage(PdfViewerAction.ZoomOut);
                Messenger.Default.Send<PdfViewerMessage>(msgPdfViewer);
            });

            zoomChangedCommand = new DelegateCommand<double>((zoomValue) =>
            {
                currentZoomValue = zoomValue;
                RaisePropertyChanged("CurrentZoomValue");
            });

            statusMessage = "MainViewModel wurde initialsiert.";
            LogStatus(statusMessage);
            RaisePropertyChanged("Messages");
        }


        /// <summary>
        /// Asynchrones Laden einer EAkt-Mappendatei
        /// </summary>
        /// <param name="EAktPath"></param>
        /// <returns></returns>
        public Task LoadEAktAsync(string EAktPath)
        {
            DocumentConvertRecord record = null;
            Task tMain = Task.Factory.StartNew(() =>
            {
                // Schritt 1: EAkt-Datei laden und in ein EAktDocumentsViewModel konvertieren
                startTime = DateTime.Now;
                // documentsVm = EAktFunctions.LoadEAkt(EAktPath);
                documentsVm = unityContainer.Resolve<EAktDocumentsViewModel>("FileService", new ParameterOverride("EAktPath", EAktPath));
                RaisePropertyChanged("DocumentsVm");
                duration = DateTime.Now - startTime;
                totalDuration += duration;

                // Dokument zur LRU-Liste hinzufügen
                lruDocuments.AddDocument(EAktPath);

                if (DispatcherService != null)
                {
                    DispatcherService.BeginInvoke(() =>
                    {
                        statusMessage = String.Format("{0} Dokumente wurden in {1:n2}s geladen.", documentsVm.Documents.Count, duration.TotalSeconds);
                        LogStatus(statusMessage);
                    });
                }
            }).ContinueWith((tStep2) =>
            {
                // Schritt 2: Dokumente nach Pdf konvertieren
                startTime = DateTime.Now;
                record = EAktConverter.ConvertDocuments(documentsVm);
                duration = DateTime.Now - startTime;
                totalDuration += duration;
                if (DispatcherService != null)
                {
                    DispatcherService.BeginInvoke(() =>
                    {
                        statusMessage = String.Format("{0} Pdf-Dokumente und {1} andere Dokumenttypen wurden mit {2} Fehlern in {3:n2}s konvertiert.", record.PdfDocumentCount, record.DocumentConvertedCount - record.PdfDocumentCount, record.ConvertErrorCount, duration.TotalSeconds);
                        LogStatus(statusMessage);
                    });
                }
            }).ContinueWith((tStep3) =>
            {
                // Schritt 3: Dokumente für Volltextsuche indizieren
                startTime = DateTime.Now;
                LuceneFunctions.CreateIndex(documentsVm.Documents);
                duration = DateTime.Now - startTime;
                totalDuration += duration;
                if (DispatcherService != null)
                {
                    DispatcherService.BeginInvoke(() =>
                    {
                        statusMessage = String.Format("{0} Dokumente wurden in {1:n2}s indiziert.",
                            record.DocumentConvertedCount, duration.TotalSeconds);
                        LogStatus(statusMessage);
                    });
                }
            }).ContinueWith((tStep4) =>
            {
                startTime = DateTime.Now;
                int anzahlVorschauSeiten = PreviewFunctions.CreateDocumentsPreview(documentsVm.Documents);
                duration = DateTime.Now - startTime;
                totalDuration += duration;
                if (DispatcherService != null)
                {
                    DispatcherService.BeginInvoke(() =>
                    {
                        statusMessage = String.Format("Für {0} Dokumente wurden {1} Vorschauseiten in {2:n2}s angelegt.",
                            record.DocumentConvertedCount, anzahlVorschauSeiten, duration.TotalSeconds);
                        LogStatus(statusMessage);
                    });
                }
            }).ContinueWith((tStep5) =>
            {
                CommentFunctions.LoadEAktComments(documentsVm);
                if (DispatcherService != null)
                {
                    DispatcherService.BeginInvoke(() =>
                    {
                        statusMessage = String.Format("{0} Kommentare eingelesen.", documentsVm.AllComments.Count);
                        LogStatus(statusMessage);
                    });
                }
            }).ContinueWith((tStep6) =>
            {
                // Lesemarken einlesen
                foreach(EAktDocumentViewModel EAktDocument in documentsVm.Documents)
                {
                    // Ist das Dokument gültig?
                    if (EAktDocument.IsValid)
                    {
                        EAktDocument.BookmarksVm = new PdfBookmarksViewModel(EAktDocument.PdfPath);
                    }
                }
            }).ContinueWith((tStep7) =>
            {
                if (DispatcherService != null)
                {
                    DispatcherService.BeginInvoke(() =>
                    {
                        statusMessage = String.Format("OpenEaktCommand wurde vollständig ausgeführt. Dauer: {0:n2}s", totalDuration.TotalSeconds);
                        LogStatus(statusMessage);
                    });
                }
            });
            Task.WaitAll(tMain);
            return tMain;

        }

        /// <summary>
        /// Synchrones Laden einer EAkt-Datei
        /// </summary>
        /// <param name="EAktPath"></param>
        private void LoadEAktFile(string EAktPath)
        {
            // Statusmeldungen löschen
            messages.Clear();

            // Schritt 1: EAkt-Datei laden und in ein EAktDocumentsViewModel konvertieren
            startTime = DateTime.Now;
            // documentsVm = EAktFunctions.LoadEAkt(EAktPath);
            // ViewModel wird über DI container geholt
            documentsVm = unityContainer.Resolve<EAktDocumentsViewModel>("FileService", new ParameterOverride("EAktPath", EAktPath));

            duration = DateTime.Now - startTime;
            totalDuration += duration;
            statusMessage = String.Format("{0} Dokumente wurden in {1:n2}s geladen.", documentsVm.Documents.Count, duration.TotalSeconds);
            LogStatus(statusMessage);

            // Dokument zur LRU-Liste hinzufügen
            lruDocuments.AddDocument(EAktPath);

            // Schritt 2: Alle Nicht-Pdf-Dokumente nach Pdf konvertieren und Pdf-Dokumente prüfen und Anzahl Seiten holen
            startTime = DateTime.Now;
            DocumentConvertRecord record = EAktConverter.ConvertDocuments(documentsVm);
            duration = DateTime.Now - startTime;
            totalDuration += duration;
            statusMessage = String.Format("{0} Pdf-Dokumente und {1} andere Dokumenttypen wurden mit {2} Fehlern in {3:n2}s konvertiert.", record.PdfDocumentCount, record.DocumentConvertedCount - record.PdfDocumentCount, record.ConvertErrorCount, duration.TotalSeconds);
            LogStatus(statusMessage);

            // Schritt 3: Dokumente für Volltextsuche indizieren
            startTime = DateTime.Now;
            if (this.UseLineIndex)
            {
                LuceneFunctions.CreateLineIndex(documentsVm.Documents);
            }
            else
            {
                LuceneFunctions.CreateIndex(documentsVm.Documents);
            }
            duration = DateTime.Now - startTime;
            totalDuration += duration;
            statusMessage = String.Format("{0} Dokumente wurden in {1:n2}s indiziert.",
                record.DocumentConvertedCount, duration.TotalSeconds);
            LogStatus(statusMessage);

            // Schritt 4: Vorschauseiten anlegen
            if (this.CreatePreviewPages)
            {
                startTime = DateTime.Now;
                int anzahlVorschauSeiten = PreviewFunctions.CreateDocumentsPreview(documentsVm.Documents);
                duration = DateTime.Now - startTime;
                totalDuration += duration;
                statusMessage = String.Format("Für {0} Dokumente wurden {1} Vorschauseiten in {2:n2}s angelegt.",
                    record.DocumentConvertedCount, anzahlVorschauSeiten, duration.TotalSeconds);
                LogStatus(statusMessage);
            }
            else
            {
                statusMessage = "Anlegen von Vorschauseiten wurde ausgelassen.";
                LogStatus(statusMessage);
            }

            // Schritt 5: Lesemarken einlesen
            foreach(EAktDocumentViewModel EAktDocument in documentsVm.Documents)
            {
                // Nur für gültige Dokumente
                if (EAktDocument.IsValid)
                {
                    EAktDocument.BookmarksVm = new PdfBookmarksViewModel(EAktDocument.PdfPath);
                    statusMessage = String.Format("{0} Bookmarks für Dokument {1} eingelesen.",
                        EAktDocument.BookmarksVm.Bookmarks.Count, EAktDocument.PdfPath);
                    GlobalFunctions.LogInfo(statusMessage);
                }
            }

            // Schritt 6: Kommentare einlesen
            CommentFunctions.LoadEAktComments(documentsVm);
            statusMessage = String.Format("{0} Kommentare eingelesen.", documentsVm.AllComments.Count);
            LogStatus(statusMessage);

            // Aktuelles Dokument festlegen
            // Wichtig: Während der Dokumentkonvertierung wird immer das gerade konvertierte Dokument
            // zum aktuellen Dokument, um die Datenbindung zu aktualisieren
            if (documentsVm.Documents.Count > 0)
            {
                documentsVm.CurrentDocument = documentsVm.Documents[0];
            }

            statusMessage = String.Format("OpenEaktCommand wurde vollständig ausgeführt. Dauer: {0:n2}s", totalDuration.TotalSeconds);
            LogStatus(statusMessage);

        }

        /// <summary>
        /// Wird in erster Linie für die Tests benötigt
        /// </summary>
        /// <param name="DirectoryPath"></param>
        /// <returns></returns>
        public Task ReadDirectoryAsync(string DirectoryPath)
        {
            // EAktPath muss aus dem letzten Namen des Verzeichnisses gebildet werden
            DirectoryPath = Path.GetFullPath(DirectoryPath).TrimEnd(Path.DirectorySeparatorChar);
            string EAktPath = Path.Combine(DirectoryPath, DirectoryPath.Split(Path.DirectorySeparatorChar).Last() + ".Eakt");
            try
            {
                return Task.Factory.StartNew(() =>
                {
                    EAktFunctions.ReadEAktDirectory(DirectoryPath, EAktPath);
                });
            }
            catch (SystemException ex)
            {
                statusMessage = "Fehler beim Einlesen der Verzeichnisstruktur " + DirectoryPath;
                GlobalFunctions.LogError(statusMessage, ex);
                return null;
            }
        }


        /// <summary>
        /// Wird in erster Linie für die Tests benötigt
        /// </summary>
        /// <param name="DirectoryPath"></param>
        /// <returns></returns>
        public Task ReadDirectoryAsync(string DirectoryPath, string EAktPath)
        {
            try
            {
                return Task.Factory.StartNew(() =>
                {
                    EAktFunctions.ReadEAktDirectory(DirectoryPath, EAktPath);
                });
            }
            catch (SystemException ex)
            {
                statusMessage = "Fehler beim Einlesen der Verzeichnisstruktur " + DirectoryPath;
                GlobalFunctions.LogError(statusMessage, ex);
                return null;
            }
        }

        /// <summary>
        /// Entfernt alle Nachrichten aus dem Statuslog
        /// </summary>
        private void ClearStatusLog()
        {
            messages.Clear();
        }

        /// <summary>
        /// Status-Meldung schreiben
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="IsError"></param>
        private void LogStatus(string Message, bool IsError = false, bool WriteToLog = false)
        {
            // Wichtig: Statusmeldungen von einem anderen Thread müssen per DispatcherService durchgeführt werden
            if (DispatcherService != null)
            {
                DispatcherService.BeginInvoke(() =>
                {
                    if (WriteToLog)
                    {
                        if (IsError)
                        {
                            GlobalFunctions.LogError(Message, null);
                        }
                        else
                        {
                            GlobalFunctions.LogInfo(Message);
                        }
                    }
                    messages.AddMessage(Message, IsError);
                    RaisePropertyChanged("MessagesVm");
                });
            }
            else
            {
                AddStatusMessage msgStatus = new AddStatusMessage
                {
                    StatusMessage = new StatusMessageViewModel
                    {
                        IsError = IsError,
                        Message = Message,
                        TimeStamp = DateTime.Now
                    }
                };
                Messenger.Default.Send<AddStatusMessage>(msgStatus);
            }
        }

        /// <summary>
        /// Stellt alle Statusmeldungen als ViewModel zur Verfügung
        /// </summary>
        public StatusMessagesViewModel MessagesVm
        {
            get { return messages; }
        }

        /// <summary>
        /// Stellt die gesamte Akte mit Dokumenten und Kommentaren zur Verfügung
        /// </summary>
        public EAktDocumentsViewModel DocumentsVm
        {
            get { return documentsVm; }
        }

        /// <summary>
        /// Stellt Infos über die App, z.B. für Tests, zur Verfügung
        /// </summary>
        public AppInfo AppInfo
        {
            get
            {
                return new AppInfo
                {
                    EAktName = GlobalVars.EAktName,
                    TempPath = GlobalVars.TempPath,
                    IndexPath = GlobalVars.IndexPath,
                    FilterPath = GlobalVars.FilterPath,
                    AppVersion = GlobalVars.AppVersion,
                    AppName = GlobalVars.AppName
                };
            }
        }

        /// <summary>
        /// Anzahl der maximal in der LRU-Liste gespeicherten Einträge
        /// </summary>
        public int LRULimit
        {
            get { return lruLimit; }
        }

        /// <summary>
        /// Gibt die Liste der zuletzt benutzten Dokumente zurück
        /// </summary>
        public LRUDocumentsViewModel LRUDocuments
        {
            get { return lruDocuments; }
        }

        /// <summary>
        /// Steht für den aktuellen Zoomwert des Pdf Viewer
        /// </summary>
        public double CurrentZoomValue
        {
            get { return currentZoomValue; }
        }

        /// <summary>
        /// Kommando für das Öffnen einer EAkt-Datei
        /// </summary>
        public DelegateCommand OpenEAktCommand
        {
            get { return openEAktCommand; }
        }

        /// <summary>
        /// Kommando für das direkte Laden einer EAkt-Datei
        /// </summary>
        public DelegateCommand<string> LoadEAktCommand
        {
            get { return loadEAktCommand; }
        }


        /// <summary>
        /// Kommando für das asynchrone Öffnen einer EAkt-Datei
        /// </summary>
        public AsyncCommand OpenEAktAsyncCommand
        {
            get { return openEAktAsyncCommand; }
        }

        /// <summary>
        /// Kommando für das asynchrone Laden einer EAkt-Datei
        /// </summary>
        public AsyncCommand<string> LoadEAktAsyncCommand
        {
            get { return loadEAktAsyncCommand; }
        }

        /// <summary>
        /// Liest eine Verzeichnisstruktur ein
        /// </summary>
        public AsyncCommand<string> ReadDirectoryAsyncCommand
        {
            get { return readDirectoryAsyncCommand; }
        }

        /// <summary> 
        /// Kommando für das neu Anlegen einer EAkt-Mappe
        /// </summary>
        public DelegateCommand<string> NewEAktCommand
        {
            get { return newEAktCommand; }
        }

        /// <summary>
        /// Kommando für das Anwenden des Filters
        /// </summary>
        public DelegateCommand<EAktDocumentFilter> ApplyFilterCommand
        {
            get { return applyFilterCommand; }
        }

        /// <summary>
        /// Kommando für das Speichern des Filters
        /// </summary>
        public DelegateCommand SaveFilterCommand
        {
            get { return saveFilterCommand; }
        }

        /// <summary>
        /// Kommando für das Zurücksetzen des Filters
        /// </summary>
        public DelegateCommand ClearFilterCommand
        {
            get { return clearFilterCommand; }
        }

        /// <summary>
        /// Kopieren des selektierten Textes im PdfViewer
        /// </summary>
        public DelegateCommand CopySelectedTextCommand
        {
            get { return copySelectedTextCommand; }
        }

        /// <summary>
        /// Zoom In beim PdfViewer
        /// </summary>
        public DelegateCommand ZoomInCommand
        {
            get { return zoomInCommand; }
        }

        /// <summary>
        /// Zoom Out beim PdfViewer
        /// </summary>
        public DelegateCommand ZoomOutCommand
        {
            get { return zoomOutCommand; }
        }

        public DelegateCommand<Double> ZoomChangedCommand
        {
            get { return zoomChangedCommand; }
        }

     
    }
}
