// ========================================================
// File: LuceneFunctions.cs
// ========================================================

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using IO = System.IO;

using LucNet = Lucene.Net.Documents;
using LuceneDe = Lucene.Net.Analysis.De;
using Lucene.Net.Index;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.QueryParsers;

using DevExpress.Mvvm;

using AsposePdfKit = Aspose.Pdf.Facades;
using AsposePdf = Aspose.Pdf;

using EAKTEngineV1.ViewModel;
using EAKTEngineV1.Messages;
using EAKTEngineV1.Helper;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Stellt Funktionen für die Volltextsuche zur Verfügung
    /// </summary>
    public static class LuceneFunctions
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private static string statusMessage;
        private static SetProgressbarMessage msgProgress;

        /// <summary>
        /// Index anlegen mit dem Inhalt der gesamten Datei
        /// </summary>
        /// <param name="Dokumente"></param>
        public static void CreateIndex(ObservableCollection<EAktDocumentViewModel> Documents)
        {
            FSDirectory indexStore = null;
            WhitespaceAnalyzer analyzer = null;
            // LuceneDe.GermanAnalyzer analyzer = null;
            IndexWriter writer = null;
            int docIndex = 0;

            // Indexverzeichnispfad mit Temp-Pfad und EAkt-Name bilden
            string indexPath = IO.Path.Combine(GlobalVars.TempPath, GlobalVars.EAktName);
            indexPath = IO.Path.Combine(indexPath, "Index");

            // Indexverzeichnis anlegen, wenn es noch nicht existiert
            if (!IO.Directory.Exists(indexPath))
            {
                IO.Directory.CreateDirectory(indexPath);
            }

            // Index-Verzeichnispfad global speichern
            GlobalVars.IndexPath = indexPath;

            try
            {
                LucNet.Document luceneDoc = null;
                // Index-Verzeichnis öffnen
                indexStore = FSDirectory.Open(indexPath);
                // analyzer = new LuceneDe.GermanAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                // Analyzer anlegen
                analyzer = new WhitespaceAnalyzer();
                // Index-Writer anlegen
                writer = new IndexWriter(indexStore, analyzer, IndexWriter.MaxFieldLength.LIMITED);
                foreach (EAktDocumentViewModel EAktDoc in Documents)
                {
                    docIndex++;

                    // Fortschrittsbalken aktualisieren
                    double progressValue = (double)docIndex / Documents.Count * 100;
                    msgProgress = new SetProgressbarMessage { Value = Convert.ToInt32(progressValue) };
                    Messenger.Default.Send<SetProgressbarMessage>(msgProgress);

                    // Ungültige Dokumente auslassen
                    if (!EAktDoc.IsValid)
                        continue;
                    try
                    {
                        // Neues Lucene-Dokument anlegen
                        luceneDoc = new LucNet.Document();
                        // Passwortgeschützte Pdf-Dateien auslassen
                        AsposePdfKit.PdfFileInfo pdfInfo = new AsposePdfKit.PdfFileInfo(EAktDoc.PdfPath);
                        if (!pdfInfo.IsEncrypted)
                        {
                            AsposePdf.Document pdfDoc = new AsposePdf.Document(EAktDoc.PdfPath);
                            StringBuilder sb = new StringBuilder();

                            // Text aus Pdf-Datei per Aspose-Bibliothek extrahieren
                            AsposePdf.Text.TextAbsorber textAbsorber = new AsposePdf.Text.TextAbsorber();
                            pdfDoc.Pages.Accept(textAbsorber);
                            string pdfText = textAbsorber.Text;
                            // \r und \n aus dem Text entfernen
                            pdfText = pdfText.ToString().Replace('\r', ' ').Replace('\n', ' ');
                            // Zwischenräume aus Leerzeichen entfernen
                            pdfText = pdfText.Replace("   ", "");
                            pdfText = pdfText.Trim();

                            // TODO: Wichtig: Inhaltsfeld wird nicht mehr im Index gespeichert
                            // Feld für Dokumentinhalt anlegen
                            LucNet.Field ContentField = new LucNet.Field("DokContent", pdfText,
                                LucNet.Field.Store.NO,
                                LucNet.Field.Index.ANALYZED,
                                LucNet.Field.TermVector.YES);
                            luceneDoc.Add(ContentField);

                            // Feld für den Pdf-Dateipfad anlegen
                            LucNet.Field PathField = new LucNet.Field("DokPfad", EAktDoc.PdfPath,
                                LucNet.Field.Store.YES,
                                LucNet.Field.Index.NOT_ANALYZED,
                                LucNet.Field.TermVector.NO);
                            luceneDoc.Add(PathField);

                            // Feld für Dateipfad anlegen
                            LucNet.Field NameField = new LucNet.Field("DokName", EAktDoc.Name,
                                LucNet.Field.Store.YES,
                                LucNet.Field.Index.NOT_ANALYZED,
                                LucNet.Field.TermVector.NO);
                            luceneDoc.Add(NameField);

                            // Feld für Schriftsatz anlegen
                            LucNet.Field SchriftsatzField = new LucNet.Field("Schriftsatz", EAktDoc.Schriftsatz,
                                LucNet.Field.Store.YES,
                                LucNet.Field.Index.NOT_ANALYZED,
                                LucNet.Field.TermVector.NO);
                            luceneDoc.Add(SchriftsatzField);

                            // Feld für Ersteller anlegen
                            LucNet.Field ErstellerField = new LucNet.Field("Ersteller", EAktDoc.Ersteller,
                                LucNet.Field.Store.YES,
                                LucNet.Field.Index.NOT_ANALYZED,
                                LucNet.Field.TermVector.NO);
                            luceneDoc.Add(ErstellerField);

                            // Feld für Eingang anlegen
                            LucNet.Field EingangField = new LucNet.Field("Eingang", EAktDoc.Eingang,
                                LucNet.Field.Store.YES,
                                LucNet.Field.Index.NOT_ANALYZED,
                                LucNet.Field.TermVector.NO);
                            luceneDoc.Add(EingangField);

                            // Feld für Dokumentnummer anlegen
                            LucNet.Field NumberField = new LucNet.Field("DokNr", EAktDoc.Id.ToString(),
                                LucNet.Field.Store.YES,
                                LucNet.Field.Index.NOT_ANALYZED,
                                LucNet.Field.TermVector.NO);
                            luceneDoc.Add(NumberField);

                            writer.AddDocument(luceneDoc);
                        }
                    }
                    catch (AsposePdf.Exceptions.PdfException)
                    {
                        statusMessage = "Aspose-Fehler beim Indizieren des Pdf-Dokuments.";
                        GlobalFunctions.LogError(statusMessage, null);

                        // Weiter zum nächsten Dokument
                        continue;
                    }
                    catch (SystemException ex)
                    {
                        statusMessage = "Allgemeiner Fehler beim Extrahieren eines Pdf-Dokuments.";
                        GlobalFunctions.LogError(statusMessage, ex);

                        // Weiter zum nächsten Dokument
                        continue;
                    }
                }
            }
            catch (SystemException ex)
            {
                statusMessage = "Allgemeiner Fehler beim Anlegen des Dokumenteindex.";
                GlobalFunctions.LogError(statusMessage, ex);
            }
            finally
            {
                // Index-Writer abschließen
                writer.Optimize();
                // NEW: Ist nicht erforderlich
                // writer.Flush(true, true, true);
                writer.Dispose();
                indexStore.Dispose();
            }

            // Fortschrittsbalken zurücksetzen
            msgProgress = new SetProgressbarMessage { Value = 0 };
            Messenger.Default.Send<SetProgressbarMessage>(msgProgress);
        }

        /// <summary>
        /// Index anlegen mit jeder Zeile einer Pdf-Datei
        /// </summary>
        /// <param name="Dokumente"></param>
        public static void CreateLineIndex(ObservableCollection<EAktDocumentViewModel> Documents)
        {
            FSDirectory indexStore = null;
            WhitespaceAnalyzer analyzer = null;
            // LuceneDe.GermanAnalyzer analyzer = null;
            IndexWriter writer = null;
            int docIndex = 0;

            // Index-Verzeichnispfad mit Temp-Pfad und EAkt-Name bilden
            string indexPath = IO.Path.Combine(GlobalVars.TempPath, GlobalVars.EAktName);
            indexPath = IO.Path.Combine(indexPath, "Index");

            // Index-Verzeichnis anlegen
            if (!IO.Directory.Exists(indexPath))
            {
                IO.Directory.CreateDirectory(indexPath);
            }

            // Index-Verzeichnispfad speichern
            GlobalVars.IndexPath = indexPath;
            try
            {
                LucNet.Document luceneDoc = null;
                // Index-Verzeichnis öffnen
                indexStore = FSDirectory.Open(indexPath);
                // Analyzer anlegen
                // analyzer = new LuceneDe.GermanAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
                analyzer = new WhitespaceAnalyzer();
                // Index-Writer anlegen
                writer = new IndexWriter(indexStore, analyzer, IndexWriter.MaxFieldLength.LIMITED);
                // Alle Dokumente durchgehen
                foreach (EAktDocumentViewModel EAktDoc in Documents)
                {
                    docIndex++;

                    // Fortschrittsbalken aktualisieren
                    double progressValue = (double)docIndex / Documents.Count * 100;
                    msgProgress = new SetProgressbarMessage { Value = Convert.ToInt32(progressValue) };
                    Messenger.Default.Send<SetProgressbarMessage>(msgProgress);

                    // Ungültige Dokumente auslassen
                    if (!EAktDoc.IsValid)
                        continue;
                    try
                    {
                        // Passwortgeschützte Pdf-Dateien auslassen
                        AsposePdfKit.PdfFileInfo pdfInfo = new AsposePdfKit.PdfFileInfo(EAktDoc.PdfPath);
                        if (!pdfInfo.IsEncrypted)
                        {
                            // Text aus Pdf-Datei per Aspose-Bibliothek extrahieren
                            AsposePdf.Text.TextAbsorber textAbsorber = new AsposePdf.Text.TextAbsorber();
                            AsposePdf.Document pdfDoc = new AsposePdf.Document(EAktDoc.PdfPath);
                            pdfDoc.Pages.Accept(textAbsorber);
                            string pdfText = textAbsorber.Text;
                            // Text in Zeilen zerlegen
                            string[] lines = pdfText.Split('\r');
                            // Alle Zeilen des Dokuments lesen
                            for (int i = 0; i <= lines.Length; i++)
                            {
                                // Alle Leerzeichen abtrennen
                                string line = lines[i].Trim();
                                // Leere Zeilen auslassen
                                if (!String.IsNullOrEmpty(line))
                                {
                                    line = line.Replace('\r', ' ').Replace('\n', ' ');
                                    luceneDoc = new LucNet.Document();
                                    // Feld für Dokumentinhalt anlegen
                                    LucNet.Field ContentField = new LucNet.Field("DokContent", line,
                                        LucNet.Field.Store.YES,
                                        LucNet.Field.Index.ANALYZED,
                                        LucNet.Field.TermVector.YES);
                                    luceneDoc.Add(ContentField);

                                    // Feld für den Pdf-Dateipfad anlegen
                                    LucNet.Field PathField = new LucNet.Field("DokPfad", EAktDoc.PdfPath,
                                        LucNet.Field.Store.YES,
                                        LucNet.Field.Index.NOT_ANALYZED,
                                        LucNet.Field.TermVector.NO);
                                    luceneDoc.Add(PathField);

                                    // Feld für Dateipfad anlegen
                                    LucNet.Field NameField = new LucNet.Field("DokName", EAktDoc.Name,
                                        LucNet.Field.Store.YES,
                                        LucNet.Field.Index.NOT_ANALYZED,
                                        LucNet.Field.TermVector.NO);
                                    luceneDoc.Add(NameField);

                                    // Feld für die Zeilennummer
                                    LucNet.Field LineField = new LucNet.Field("Line", (i++).ToString(),
                                        LucNet.Field.Store.YES,
                                        LucNet.Field.Index.NOT_ANALYZED,
                                        LucNet.Field.TermVector.NO);
                                    luceneDoc.Add(NameField);

                                    // Feld für Schriftsatz anlegen
                                    LucNet.Field SchriftsatzField = new LucNet.Field("Schriftsatz", EAktDoc.Schriftsatz,
                                        LucNet.Field.Store.YES,
                                        LucNet.Field.Index.NOT_ANALYZED,
                                        LucNet.Field.TermVector.NO);
                                    luceneDoc.Add(SchriftsatzField);

                                    // Feld für Ersteller anlegen
                                    LucNet.Field ErstellerField = new LucNet.Field("Ersteller", EAktDoc.Ersteller,
                                        LucNet.Field.Store.YES,
                                        LucNet.Field.Index.NOT_ANALYZED,
                                        LucNet.Field.TermVector.NO);
                                    luceneDoc.Add(ErstellerField);

                                    // Feld für Eingang anlegen
                                    LucNet.Field EingangField = new LucNet.Field("Eingang", EAktDoc.Eingang,
                                        LucNet.Field.Store.YES,
                                        LucNet.Field.Index.NOT_ANALYZED,
                                        LucNet.Field.TermVector.NO);
                                    luceneDoc.Add(EingangField);

                                    // Feld für Dokumentnummer anlegen
                                    LucNet.Field NumberField = new LucNet.Field("DokNr", EAktDoc.Id.ToString(),
                                        LucNet.Field.Store.YES,
                                        LucNet.Field.Index.NOT_ANALYZED,
                                        LucNet.Field.TermVector.NO);
                                    luceneDoc.Add(NumberField);

                                    writer.AddDocument(luceneDoc);
                                }
                            }
                        }
                    }
                    catch (AsposePdf.Exceptions.PdfException)
                    {
                        statusMessage = "Aspose-Fehler beim Indizieren des Pdf-Dokuments.";
                        GlobalFunctions.LogError(statusMessage, null);

                        // Weiter zum nächsten Dokument
                        continue;
                    }
                    catch (SystemException ex)
                    {
                        statusMessage = "Allgemeiner Fehler beim Extrahieren eines Pdf-Dokuments.";
                        GlobalFunctions.LogError(statusMessage, ex);

                        // Weiter zum nächsten Dokument
                        continue;
                    }
                }
            }
            catch (SystemException ex)
            {
                statusMessage = "Allgemeiner Fehler beim Anlegen des Dokumenteindex.";
                GlobalFunctions.LogError(statusMessage, ex);
            }
            finally
            {
                // Index-Writer abschließen
                writer.Optimize();
                // writer.Flush(true, true, true);
                writer.Dispose();
                indexStore.Dispose();
            }

            // Fortschrittsbalken zurücksetzen
            msgProgress = new SetProgressbarMessage { Value = 0 };
            Messenger.Default.Send<SetProgressbarMessage>(msgProgress);
        }

        /// <summary>
        /// Volltextsuche nach einem Suchbegriff
        /// </summary>
        /// <param name="Suchwort"></param>
        /// <returns></returns>
        public static List<SearchResultViewModel> StartSearch(string SearchWord)
        {
            // Private Variablen
            List<SearchResultViewModel> results = null;

            try
            {
                // Verzeichnis für die Indexdateien wurde bereits beim Index anlegen angelegt
                results = new List<SearchResultViewModel>();
                DateTime startTime = DateTime.Now;
                // Index-Verzeichnis öffnen
                FSDirectory indexStore = FSDirectory.Open(GlobalVars.IndexPath);
                // Analyzer anlegen
                WhitespaceAnalyzer analyzer = new WhitespaceAnalyzer();
                // LuceneDe.GermanAnalyzer analyzer = new LuceneDe.GermanAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

                // Suche über Indexsearcher
                Searcher searcher = new IndexSearcher(indexStore);
                QueryParser queryParser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "DokContent", analyzer);
                queryParser.AllowLeadingWildcard = true;
                // Abfrage erstellen
                Query query = queryParser.Parse(SearchWord);
                int maxResults = 100;
                // Suche durchführen
                TopDocs resultDocs = searcher.Search(query, maxResults);
                // Alle Ergebnisse durchgehen
                for (int i = 0; i < resultDocs.TotalHits; i++)
                {
                    LucNet.Document resultDoc = searcher.Doc(i);
                    // ViewModel für Treffer anlegen
                    SearchResultViewModel trefferVm = new SearchResultViewModel();
                    trefferVm.HitId = i;
                    trefferVm.DocumentName = resultDoc.Get("DokName");
                    trefferVm.DocumentPath = resultDoc.Get("DokPfad");
                    trefferVm.Schriftsatz = resultDoc.Get("Schriftsatz");
                    trefferVm.ResultScore = resultDocs.ScoreDocs[i].Score;
                    int DocumentNr = 0;
                    Int32.TryParse(resultDoc.Get("DokNr"), out DocumentNr);
                    trefferVm.DocumentId = DocumentNr;
                    string content = resultDoc.Get("DokContent");
                    if (!String.IsNullOrEmpty(content))
                        trefferVm.Result = content; // .Substring(0, Math.Min(content.Length, 80));
                    else
                        trefferVm.Result = "Kein Treffer";
                    results.Add(trefferVm);
                }
                // Such-Objekte aufräumen
                searcher.Dispose();
                indexStore.Dispose();

                // Trefferliste sortieren
                results = results.OrderByDescending((t) => t.ResultScore).ToList();
                TimeSpan Dauer = DateTime.Now - startTime;
                statusMessage = String.Format("{0} Treffer in {1:n2}s", results.Count, Dauer.TotalSeconds);
                GlobalFunctions.LogInfo(statusMessage);

            }
            catch (SystemException ex)
            {
                statusMessage = "Fehler bei der Volltextsuche (" + ex.Message + ")";
                GlobalFunctions.LogError(statusMessage, ex);
            }
            return results;

        }
    }
}