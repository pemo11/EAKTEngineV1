// ========================================================
// File: PdfFunctions.cs
// ========================================================

using System;
using System.IO;
using System.Linq;
using System.Text;

using AsposePdf = Aspose.Pdf;
using AsposeKit = Aspose.Pdf.Facades;
using AsposePdfEx = Aspose.Pdf.Exceptions;
using AsposePdfInteractive = Aspose.Pdf.InteractiveFeatures;

using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Methoden für den Umgang mit Pdf-Dateien
    /// </summary>
    public static class PdfFunctions
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private static string statusMessage;

        /// <summary>
        /// Holt die Anzahl Seiten einer Pdf-Datei - Passwort ist optional
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static int GetPageCount(string Path, string Password = "")
        {
            int pageCount = 0;
            try
            {
                // Gibt es kein Passwort?
                if (Password == "")
                {
                    using (AsposePdf.Document doc = new AsposePdf.Document(Path))
                    {
                        pageCount = doc.Pages.Count;
                    }
                }
                else
                {
                    // Dokument mit Passwort öffnen
                    using (AsposePdf.Document doc = new AsposePdf.Document(Path, Password))
                    {
                        pageCount = doc.Pages.Count;
                    }
                }
            }
            catch (AsposePdfEx.InvalidPdfFileFormatException ex)
            {
                pageCount = -1;
                statusMessage = "Aspose-Fehler in GetPageCount: " + ex.Message;
                GlobalFunctions.LogError(statusMessage, new SystemException("AposeInvalidPdfFileFormat", ex));
            }
            catch (SystemException ex)
            {
                statusMessage = "Allgemeiner Fehler in GetPageCount";
                GlobalFunctions.LogError(statusMessage, ex);
            }
            return pageCount;
        }

        /// <summary>
        /// Gibt ab, ob eine Pdf-Datei mit einem Kennwort geschützt ist
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static bool CheckPasswordProtected(string PathPath)
        {
            return false;
        }


        /// <summary>
        /// Gibt an, ob Pdf-Dokument eine Signatur enthält
        /// </summary>
        /// <param name="PdfPath"></param>
        /// <returns></returns>
        public static bool CheckSignature(string PdfPath)
        {
            try
            {
                // Pdf-Dateisignatur anlegen
                using (AsposeKit.PdfFileSignature pdfSignature = new AsposeKit.PdfFileSignature())
                {
                    pdfSignature.BindPdf(PdfPath);
                    return pdfSignature.IsContainSignature();
                }
            }
            catch (AsposePdfEx.InvalidPdfFileFormatException pdfEx)
            {
                statusMessage = "Aspose/CheckPdfSignature: Fehlerhaftes Dateiformat bei " + PdfPath;
                GlobalFunctions.LogError(statusMessage, new SystemException("Fehlerhaftes Pdf-Dateiformat", pdfEx));
                return false;
            }
            catch (AsposePdfEx.PdfException pdfEx)
            {
                statusMessage = "Aspose/CheckPdfSignature: Spezieller Fehler beim Zugriff auf eine Pdf-Signatur.";
                GlobalFunctions.LogError(statusMessage, new SystemException("Allgemeine Pdf-Exception", pdfEx)); ;
                return false;
            }
            catch (SystemException ex)
            {
                statusMessage = "CheckPdfSignature: Allgemeiner Fehler beim Zugriff auf eine Pdf-Signatur.";
                GlobalFunctions.LogError(statusMessage, ex);
                return false;
            }
        }

        /// <summary>
        /// Entfernt die Signaturen einer Pdf-Datei und legt von der Datei mit Signatur eine Kopie an
        /// </summary>
        /// <param name="PdfPath"></param>
        public static bool RemoveSignature(EAktDocumentViewModel EAktDoc)
        {
            string pdfPath = EAktDoc.PdfPath;
            string tmpPath = Path.Combine(GlobalVars.TempPath, Path.GetFileNameWithoutExtension(pdfPath) + "_OhneSignatur.pdf");
            try
            {
                // Pdf-Datei mit Signatur in temporäres Verzeichnis kopieren, um Signatur zu entfernen
                File.Copy(pdfPath, tmpPath, true);
                // Pdf-Pfad anpassen, so dass das Dokument auf die Pdf-Datei ohne Signatur verweist
                EAktDoc.PdfPath = tmpPath;
                statusMessage = pdfPath + " wurde nach " + tmpPath + " kopiert.";
                GlobalFunctions.LogInfo(statusMessage);
            }
            catch (SystemException ex)
            {
                statusMessage = "Fehler beim Kopieren von " + pdfPath + " nach " + tmpPath;
                GlobalFunctions.LogError(statusMessage, ex);
                return false;
            }

            // Entfernen der Signaturen aus der kopierten Datei
            try
            {
                using (AsposeKit.PdfFileSignature pdfSig = new AsposeKit.PdfFileSignature())
                {
                    pdfSig.BindPdf(tmpPath);
                    // Alle Signaturen durchgehen und entfernen
                    int signatureCount = pdfSig.GetSignNames().Count;
                    foreach (string signName in pdfSig.GetSignNames())
                    {
                        pdfSig.RemoveSignature(signName, true);
                    }

                    // Pdf-Dokument ohne Signaturen im temporären Verzeichnis speichern
                    pdfSig.Save(tmpPath);
                    statusMessage = String.Format("{0} Signatur(en) aus {1} entfernt.", signatureCount, tmpPath);
                    GlobalFunctions.LogInfo(statusMessage);
                }
                return true;
            }
            catch (AsposePdfEx.PdfException pdfEx)
            {
                statusMessage = "Aspose/CheckPdfSignature: Fehler beim Entfernen der Pdf-Signatur bei " + tmpPath + ".";
                GlobalFunctions.LogError(statusMessage, new SystemException("PdfException beim Entfernen einer Signatur", pdfEx));
                return false;
            }
            catch (SystemException ex)
            {
                statusMessage = "Allgemeiner Fehler beim Entfernen der Pdf-Signatur in " + tmpPath;
                GlobalFunctions.LogError(statusMessage, ex);
                return false;
            }
        }

        /// <summary>
        /// Fasst alle Dokumente einer EAkt-Mappe zu einer Pdf-Datei zusammen
        /// </summary>
        /// <param name="documentsVm"></param>
        /// <returns></returns>
        public static PdfExportRecord EAktPdfMerge(EAktDocumentsViewModel documentsVm, PdfExportRecord record)
        {
            AsposePdf.Document pdfMergeDoc = null;
            // Seitenzahlen für Rückgabewert mitzählen
            int pageCount = 0;
            int docIndex = 0;
            int pageIndex = 0;

            // Pfade initialisieren
            // Wurde ein Pfad per record-Parameter übergeben?
            string pdfExportDirectoryPath = Directory.Exists(record.ExportDirectoryPath) ? record.ExportDirectoryPath :  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string pdfExportPageDirectoryPath  = Path.Combine(pdfExportDirectoryPath, GlobalVars.EAktName + "_PdfExportSeiten");
            string pdfExportFilePath = Path.Combine(pdfExportDirectoryPath, GlobalVars.EAktName + "_Merge.pdf");

            // Soll Pdf-Dokument auch seitenweise gespeichert werden?
            if (record.PageMode)
            {
                try
                {
                    // Verzeichnis für Seitenexport anlegen
                    if (Directory.Exists(pdfExportPageDirectoryPath))
                    {
                        Directory.Delete(pdfExportPageDirectoryPath, true);
                    }
                    Directory.CreateDirectory(pdfExportPageDirectoryPath);
                    statusMessage = "Pdf-Export: Verzeichnis " + pdfExportPageDirectoryPath + " wurde angelegt.";
                    GlobalFunctions.LogInfo(statusMessage);
                }
                catch (SystemException ex)
                {
                    statusMessage = "Fehler beim Löschen/Anlegen des Pdf-Exportseitenverzeichnisses";
                    GlobalFunctions.LogError(statusMessage, ex);
                }
            }
            try
            {
                statusMessage = "Pdf-Export: Pdf-Export wurde gestartet.";
                GlobalFunctions.LogInfo(statusMessage);

                pdfMergeDoc = new AsposePdf.Document();

                // Stammlesezeichen für das Dokument anlegen
                AsposePdf.OutlineItemCollection bmRoot = new AsposePdf.OutlineItemCollection(pdfMergeDoc.Outlines);
                bmRoot.Title = GlobalVars.EAktName;
                pdfMergeDoc.Outlines.Add(bmRoot);

            
                // Alle Pdf-Dateien des Klammerdokuments zusammenfassen
                foreach(EAktDocumentViewModel eaktDoc  in documentsVm.Documents)
                {
                    // Nur gültige Dokumente exportieren
                    if (eaktDoc.IsValid)
                    {
                        docIndex++;
                        try
                        {
                            statusMessage = String.Format("Pdf-Export: {0} von {1} Dokumenten wird zusammengefasst.", docIndex, documentsVm.Documents.Count);
                            GlobalFunctions.LogInfo(statusMessage);

                            // Neues Pdf-Dokument für die aktuelle Pdf-Datei anlegen
                            AsposePdf.Document pdfDoc = new AsposePdf.Document(eaktDoc.PdfPath);

                            // Seiten des aktuellen Dokuments zum Gesamtdokument hinzufügen
                            pdfMergeDoc.Pages.Add(pdfDoc.Pages);
                            pageCount += pdfDoc.Pages.Count;

                            // Dokumentname als Lesemarke hinzufügen
                            AsposePdf.OutlineItemCollection bmDoc = new AsposePdf.OutlineItemCollection(pdfMergeDoc.Outlines);
                            bmDoc.Title = eaktDoc.Name;
                            bmDoc.Action = new AsposePdfInteractive.GoToAction(pdfMergeDoc.Pages[pageIndex]);

                            // Ersteller hinzufügen
                            AsposePdf.OutlineItemCollection bmErsteller = new AsposePdf.OutlineItemCollection(pdfMergeDoc.Outlines);
                            bmErsteller.Title = eaktDoc.Ersteller;
                            bmErsteller.Action = new AsposePdfInteractive.GoToAction(pdfMergeDoc.Pages[pageIndex]);
                            bmDoc.Add(bmErsteller);

                            // Eingang hinzufügen
                            AsposePdf.OutlineItemCollection bmEingang = new AsposePdf.OutlineItemCollection(pdfMergeDoc.Outlines);
                            bmEingang.Title = eaktDoc.Eingang;
                            bmEingang.Action = new AsposePdfInteractive.GoToAction(pdfMergeDoc.Pages[pageIndex]);
                            bmDoc.Add(bmEingang);

                            // Kommentare als Bookmark anhängen
                            AsposePdf.OutlineItemCollection bmKommentare = new AsposePdf.OutlineItemCollection(pdfMergeDoc.Outlines);
                            string allComments = String.Join(",", (from k in  eaktDoc.CommentsVm.Comments select k.Comment).ToArray());
                            if (allComments == "")
                                allComments = "Keine Kommentare";
                            bmKommentare.Title = allComments;
                            bmKommentare.Action = new AsposePdfInteractive.GoToAction(pdfMergeDoc.Pages[pageIndex]);
                            bmDoc.Add(bmKommentare);
                            pdfMergeDoc.Outlines.First.Add(bmDoc);

                            pageIndex += pdfDoc.Pages.Count;
                        }
                        catch (AsposePdfEx.InvalidFileFormatException ex)
                        {
                            statusMessage = "Aspose-Fehler: Ungültiges Dateiformat bei " + eaktDoc.PdfPath;
                            GlobalFunctions.LogError(statusMessage, new SystemException("Fehler beim Pdf-Export", ex));
                        }
                        catch (AsposePdfEx.PdfException ex)
                        {
                            statusMessage = "Aspose-Fehler: Allgemeiner Pdf-Fehler bei " + eaktDoc.PdfPath;
                            GlobalFunctions.LogError(statusMessage, new SystemException("Fehler beim Pdf-Export", ex));
                        }
                        catch (SystemException ex)
                        {
                            statusMessage = "Pdf-Export: Fehler beim Export von " + eaktDoc.PdfPath;
                            GlobalFunctions.LogError(statusMessage, ex);
                        }
                    }
                }

                // Sollen die Seiten einzeln gespeichert werden?
                if (record.PageMode)
                {
                    pageIndex = 1;
                    foreach (AsposePdf.Page pdfPage in pdfMergeDoc.Pages)
                    {
                        string seitePfad = Path.Combine(pdfExportPageDirectoryPath, String.Format("{0}_{1:000}.pdf",
                            GlobalVars.EAktName, pageIndex));
                        pageIndex++;
                        using (AsposePdf.Document pdfSeiteDoc = new AsposePdf.Document())
                        {
                            pdfSeiteDoc.Pages.Add(pdfPage);
                            pdfSeiteDoc.Save(seitePfad);
                            statusMessage = "Pdf-Export: " + seitePfad + " wurde gespeichert.";
                            GlobalFunctions.LogInfo(statusMessage);
                        }
                    }
                }
            }
            catch (SystemException ex)
            {
                statusMessage = "Pdf-Export: Allgemeiner Fehler.";
                GlobalFunctions.LogError(statusMessage, ex);
            }

            // Die gesamt Pdf-Datei im temporären Ausgabeverzeichnis speichern
            try
            {
                // Passwort für die exportierte Pdf-Datei setzen und Dokument verschlüsseln
                pdfMergeDoc.Encrypt(record.Password, record.Password, 0, AsposePdf.CryptoAlgorithm.AESx256);
                pdfMergeDoc.Save(pdfExportFilePath);
                pdfMergeDoc.Dispose();
                pdfMergeDoc = null;
                // Garbage Collector starten, um Arbeitsspeicher aufzuräumen
                GC.Collect();
                statusMessage = String.Format("Pdf-Export: {0} mit {1} Seiten wurde angelegt.", pdfExportFilePath, pageCount);
                GlobalFunctions.LogInfo(statusMessage);
            }
            catch (SystemException ex)
            {
                statusMessage = "Fehler beim Pdf-Merge (" + ex.Message + ")";
                GlobalFunctions.LogError(statusMessage, ex);
            }
            return record;
        }

        /// <summary>
        /// Text aus allen Pdf-Dateien extrahieren und separat speichern
        /// </summary>
        /// <param name="documentsVm"></param>
        /// <param name=""></param>
        public static void ExtractText(EAktDocumentsViewModel documentsVm)
        {
            try
            {
                string extractPath = Path.Combine(GlobalVars.TempPath, "TextExtract");
                // Verzeichnisinhalt löschen falls vorhanden
                if (Directory.Exists(extractPath))
                {
                    Array.ForEach(Directory.GetFiles(extractPath), File.Delete);
                }
                else
                {
                    Directory.CreateDirectory(extractPath);
                }
                foreach (EAktDocumentViewModel EAktDoc in documentsVm.Documents)
                {
                    // Pdf-Datei per Aspose-Bibliothek extrahieren
                    AsposeKit.PdfExtractor pdfExtractor = new AsposeKit.PdfExtractor();
                    pdfExtractor.BindPdf(EAktDoc.PdfPath);
                    pdfExtractor.ExtractText();
                    MemoryStream tmpMs = new MemoryStream();
                    pdfExtractor.GetText(tmpMs);
                    string pdfText;
                    using (StreamReader streamReader = new StreamReader(tmpMs, Encoding.UTF8))
                    {
                        streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                        pdfText = streamReader.ReadToEnd();
                    }
                    // Erweiterung der Originaldatei in den Dateinamen einbauen, damit mehrfach vorkommende
                    // Dateinamen kein Problem sind
                    string txtPath = Path.Combine(extractPath, Path.GetFileNameWithoutExtension(EAktDoc.Name) +
                         "_" + Path.GetExtension(EAktDoc.Name) +  ".txt");
                    File.WriteAllText(txtPath, pdfText);
                }
            }
            catch (AsposePdfEx.PdfException pdfEx)
            {
                throw pdfEx;
            }
            catch (SystemException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Images aus allen Pdf-Dateien extrahieren und separat speichern
        /// </summary>
        /// <param name="documentsVm"></param>
        public static void ExtractImages(EAktDocumentsViewModel documentsVm)
        {
            try
            {
                string extractPath = Path.Combine(GlobalVars.TempPath, "ImageExtract");
                // Verzeichnisinhalt löschen falls vorhanden
                if (Directory.Exists(extractPath))
                {
                    Array.ForEach(Directory.GetFiles(extractPath), File.Delete);
                }
                else
                {
                    Directory.CreateDirectory(extractPath);
                }
                // Alle Dokumente durchgehen
                foreach (EAktDocumentViewModel EAktDoc in documentsVm.Documents)
                {
                    AsposeKit.PdfExtractor pdfExtractor = new AsposeKit.PdfExtractor();
                    pdfExtractor.BindPdf(EAktDoc.PdfPath);
                    pdfExtractor.ExtractImage();
                    while (pdfExtractor.HasNextImage())
                    {
                        // Erweiterung der Originaldatei in den Dateinamen einbauen, damit mehrfach vorkommende
                        // Dateinamen kein Problem sind
                        string jpgPath = Path.Combine(extractPath, Path.GetFileNameWithoutExtension(EAktDoc.Name) +
                             "_" + Path.GetExtension(EAktDoc.Name) + ".jpg");
                        MemoryStream msStream = new MemoryStream();
                        pdfExtractor.GetNextImage(msStream);
                        using (FileStream fs = new FileStream(jpgPath, FileMode.Create))
                        {
                            msStream.WriteTo(fs);
                        }
                    }
                }
            }
            catch (AsposePdfEx.PdfException pdfEx)
            {
                throw pdfEx;
            }
            catch (SystemException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Speichert die erste Seite als eigenes Pdf-Dokument im Temp-Verzeichnis
        /// </summary>
        /// <param name="PdfPath"></param>
        /// <returns></returns>
        public static string SaveFirstPage(string PdfPath)
        {
            string firstPagePath = Path.Combine(GlobalVars.TempPath, GlobalVars.EAktName);
            firstPagePath = Path.Combine(firstPagePath, 
                Path.GetFileNameWithoutExtension(PdfPath) + "_FirstPage.pdf");
            try
            {
                AsposePdf.Document pdfDoc = new AsposePdf.Document(PdfPath);
                // Index beginnt bei 1
                AsposePdf.Page firstPage = pdfDoc.Pages[1];
                AsposePdf.Document firstPageDoc = new AsposePdf.Document();
                firstPageDoc.Pages.Add(firstPage);
                firstPageDoc.Save(firstPagePath);
            }
            catch (AsposePdfEx.PdfException pdfEx)
            {
                throw new SystemException("Aspose-Fehler in SaveFirstPage", pdfEx);
            }
            catch (SystemException ex)
            {
                throw ex;
            }
            return firstPagePath;
        }

    }
}
