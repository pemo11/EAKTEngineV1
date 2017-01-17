// ========================================================
// File: EAktConverter.cs
// ========================================================

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using AsposeWords = Aspose.Words;
using AspGen = Aspose.Pdf.Generator;

using DevExpress.Mvvm;

using EAKTEngineV1.Messages;
using EAKTEngineV1.ViewModel;
using EAKTEngineV1.Model;
using EAKTEngineV1.Helper;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Stellt Konvertierungsfunktionen zur Verfügung
    /// </summary>
    public static class EAktConverter
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private static string statusMessage;
        private static AddStatusMessage msgStatus;
        private static SetProgressbarMessage msgProgress;

        /// <summary>
        /// Konvertiert alle Dokumente der EAKt-Datei
        /// </summary>
        /// <param name="Dokumente"></param>
        /// <returns></returns>
        public static DocumentConvertRecord ConvertDocuments(EAktDocumentsViewModel DocumentsVm)
        {
            DocumentConvertRecord record = new DocumentConvertRecord();
            // Temporäres Verzeichnis mit EAkt-Namen anlegen
            string tmpPath = Path.Combine(GlobalVars.TempPath, GlobalVars.EAktName);
            if (Directory.Exists(tmpPath))
            {
                // Zuerst das Verzeichnis samt aller Dateien löschen
                DeleteDirectory(tmpPath);
            }
            // Verzeichnis neu anlegen
            Directory.CreateDirectory(tmpPath);
            try
            {
                // Alle Dokumente durchgehen
                foreach (EAktDocumentViewModel EAktDoc in DocumentsVm.Documents)
                {
                    // Fortschrittsbalken aktualisieren
                    double progressValue = (double)EAktDoc.Id / DocumentsVm.Documents.Count * 100;
                    msgProgress = new SetProgressbarMessage { Value = Convert.ToInt32(progressValue) };
                    Messenger.Default.Send<SetProgressbarMessage>(msgProgress);

                    // Ist die Datei gültig bzw. vorhanden?
                    if (EAktDoc.IsValid)
                    {
                        // Pdf-Dateien werden nicht konvertiert
                        if (EAktDoc.Type == DocumentType.Pdf)
                        {
                            EAktDoc.PdfPath = EAktDoc.Path;
                            EAktDoc.PageCount = PdfFunctions.GetPageCount(EAktDoc.Path);
                            // Gültige Pdf-Datei?
                            if (EAktDoc.PageCount == -1)
                            {
                                // Dokument als ungültig markieren, konnte aus irgendeinem Grund nicht gelesen werden
                                EAktDoc.IsValid = false;
                                continue;
                            }
                            // Umfasst das Dokument mehr als eine Seite?
                            if (EAktDoc.PageCount > 1)
                            {
                                // Erste Seite separat speichern
                                try
                                {
                                    EAktDoc.FirstPagePath = PdfFunctions.SaveFirstPage(EAktDoc.PdfPath);
                                }
                                catch (SystemException ex)
                                {
                                    // Etwas ging schief, es gibt keine erste Seite
                                    statusMessage = "Fehler beim Erstellen der ersten Pdf-Seite (" + ex.ToString() + ")";
                                    SendStatusMessage(statusMessage, true);
                                }
                            }
                            else
                            {
                                // Pfad der ersten Seite ist der Pfad der gesamten Datei
                                EAktDoc.FirstPagePath = EAktDoc.PdfPath;
                            }
                            // Anzahl der konvertierten Dokumente mitzählen
                            record.DocumentConvertedCount++;
                            record.PdfDocumentCount++;
                            statusMessage = String.Format("{0} mit {1} Seiten erfasst.", EAktDoc.Name, EAktDoc.PageCount);
                            SendStatusMessage(statusMessage);
                        }
                        else
                        {
                            // Anzahl der konvertierten Dokumente mitzählen
                            record.DocumentConvertedCount++;
                            // Erweiterung soll in den Pdf-Pfad eingebaut werden, damit bei Dateien mit
                            // gleichem Namen, aber unterschiedlichen Erweiterungen die Namen eindeutig bleiben
                            string ext = Path.GetExtension(EAktDoc.Name);
                            // Erweiterung bitte ohne Punkt falls vorhanden
                            ext = ext.IndexOf(".") >= 0 ? ext.Substring(1) : ext;
                            string pdfPath = Path.Combine(tmpPath, Path.GetFileNameWithoutExtension(EAktDoc.Name) + "_" + ext + ".pdf");
                            // Dokument wird in Abhängigkeit des Dokumenttyps konvertiert
                            switch (EAktDoc.Type)
                            {
                                // Ist es ein Word-Dokument?
                                case DocumentType.Word:
                                    // Word-Dokument nach Pdf konvertieren und Seiten zählen
                                    EAktDoc.PageCount = ConvertWordDocument(EAktDoc, pdfPath);
                                    statusMessage = String.Format("{0} mit {1} Seiten nach {2} konvertiert.",
                                        EAktDoc.Name, EAktDoc.PageCount, EAktDoc.PdfPath);
                                    SendStatusMessage(statusMessage);
                                    break;
                                // Ist es ein Bitmap-Dokument?
                                case DocumentType.Bitmap:
                                    // Bitmap-Dokument nach Pdf konvertieren und Seiten zählen
                                    EAktDoc.PageCount = ConvertBitmapDocument(EAktDoc, pdfPath);
                                    statusMessage = String.Format("{0} mit {1} Seiten nach {2} konvertiert.",
                                        EAktDoc.Name, EAktDoc.PageCount, EAktDoc.PdfPath);
                                    SendStatusMessage(statusMessage);
                                    break;
                                // Ist es ein Tiff-Dokument?
                                case DocumentType.Tiff:
                                    // Tiff-Dokument nach Pdf konvertieren und Seiten zählen
                                    EAktDoc.PageCount = ConvertTiffDocument(EAktDoc, pdfPath);
                                    statusMessage = String.Format("{0} mit {1} Seiten nach {2} konvertiert.",
                                        EAktDoc.Name, EAktDoc.PageCount, EAktDoc.PdfPath);
                                    SendStatusMessage(statusMessage);
                                    break;
                                // Ist es ein Html-Dokument?
                                case DocumentType.Html:
                                    // Html-Dokument nach Pdf konvertieren und Seiten zählen
                                    EAktDoc.PageCount = ConvertHtmlDocument(EAktDoc, pdfPath);
                                    statusMessage = String.Format("{0} mit {1} Seiten nach {2} konvertiert.",
                                        EAktDoc.Name, EAktDoc.PageCount, EAktDoc.PdfPath);
                                    SendStatusMessage(statusMessage);
                                    break;
                                // Ist es ein Text-Dokument?
                                case DocumentType.Text:
                                    // Text-Dokument nach Pdf konvertieren und Seiten zählen
                                    EAktDoc.PageCount = ConvertTextDocument(EAktDoc, pdfPath);
                                    statusMessage = String.Format("{0} mit {1} Seiten nach {2} konvertiert.",
                                        EAktDoc.Name, EAktDoc.PageCount, EAktDoc.PdfPath);
                                    SendStatusMessage(statusMessage);
                                    break;
                                // Ist es ein Rtf-Dokument?
                                case DocumentType.Rtf:
                                    // Rft-Dokument nach Pdf konvertieren und Seiten zählen
                                    EAktDoc.PageCount = ConvertRtfDocument(EAktDoc, pdfPath);
                                    msgStatus = new AddStatusMessage();
                                    statusMessage = String.Format("{0} mit {1} Seiten nach {2} konvertiert.",
                                        EAktDoc.Name, EAktDoc.PageCount, EAktDoc.PdfPath);
                                    SendStatusMessage(statusMessage);
                                    break;
                                // Unbekannter Dateityp
                                case DocumentType.Unknown:
                                    msgStatus = new AddStatusMessage();
                                    statusMessage = "Unbekannter Dokumenttyp: " + Path.GetExtension(EAktDoc.Path);
                                    SendStatusMessage(statusMessage);
                                    EAktDoc.IsValid = false;
                                    break;
                                // Dieser Zweig sollte nie aktiv werden
                                default:
                                    msgStatus = new AddStatusMessage();
                                    statusMessage = "Unbekannter Dokumenttyp: " + Path.GetExtension(EAktDoc.Path);
                                    SendStatusMessage(statusMessage);
                                    EAktDoc.IsValid = false;
                                    break;
                            }
                            // Erste Seite separat speichern  - gibt es mehr als eine Seite?
                            if (EAktDoc.PageCount > 1)
                            {
                                try
                                {
                                    // Pfad der ersten Seitendatei speichern
                                    EAktDoc.FirstPagePath = PdfFunctions.SaveFirstPage(EAktDoc.PdfPath);
                                }
                                catch (SystemException ex)
                                {
                                    // Etwas ging schief, es gibt keine erste Seite
                                    statusMessage = "Fehler beim Erstellen der ersten Pdf-Seite (" + ex.ToString() + ")";
                                    SendStatusMessage(statusMessage, true);
                                }
                            }
                            else
                            {
                                // Pfad der ersten Seite ist Pfad der gesamten Pdf-Datei
                                EAktDoc.FirstPagePath = EAktDoc.PdfPath;
                            }
                        }
                        // Das Dokument ist fertig
                        PdfDocumentReadyMessage msgPdfReady = new PdfDocumentReadyMessage
                        {
                            PdfPath = EAktDoc.PdfPath,
                            FirstPagePath = EAktDoc.FirstPagePath
                        };
                        Messenger.Default.Send<PdfDocumentReadyMessage>(msgPdfReady);
                        // Datenbindung aktualisieren
                        DocumentsVm.CurrentDocument = EAktDoc;
                    }
                    else
                    {
                        EAktDoc.IsValid = false;
                        msgStatus = new AddStatusMessage();
                        statusMessage = String.Format("Fehler - ungültiges Dokument: {0}", EAktDoc.Name);
                        SendStatusMessage(statusMessage, true);
                    }
                }
            }
            catch (SystemException ex)
            {
                statusMessage = "Allgemeiner Fehler in ConvertDocuments";
                GlobalFunctions.LogError(statusMessage, ex);
            }

            // Fortschrittsbalken zurücksetzen
            msgProgress = new SetProgressbarMessage { Value = 0 };
            Messenger.Default.Send<SetProgressbarMessage>(msgProgress);

            return record;
        }

        /// <summary>
        /// Hilfsfunktion für das zuverlässige Löschen eines Verzeichnisses
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteDirectory(string Path)
        {
            // Rekursives Löschen aller Unterverzeichnisse
            foreach (string directory in Directory.GetDirectories(Path))
            {
                DeleteDirectory(directory);
            }
            // Das Verzeichnis selber löschen
            try
            {
                Directory.Delete(Path, true);
            }
            catch (IOException)
            {
                // Einfach noch einmal probieren
                Directory.Delete(Path, true);
            }
            catch (UnauthorizedAccessException)
            {
                // Einfach noch einmal probieren
                Directory.Delete(Path, true);
            }
        }

        /// <summary>
        /// Konvertiert eine Word-Datei in eine Pdf-Datei
        /// </summary>
        /// <param name="EAktDocument"></param>
        /// <param name="PdfPath"></param>
        /// <returns></returns>
        private static int ConvertWordDocument(EAktDocumentViewModel EAktDocument, string PdfPath)
        {
            int pageCount = 0;
            object objLock = new object();
            try
            {
                // Während der Konvertierung darf die Methode nicht erneut aufrufbar sein
                lock (objLock)
                {
                    AsposeWords.Document wdDoc = new AsposeWords.Document(EAktDocument.Path);
                    wdDoc.Save(PdfPath, AsposeWords.SaveFormat.Pdf);
                    EAktDocument.PdfPath = PdfPath;
                    statusMessage = "ConvertWordDocument: " + EAktDocument.Path + " wurde in " + EAktDocument.PdfPath + " konvertiert.";
                    GlobalFunctions.LogInfo(statusMessage);
                    pageCount = wdDoc.PageCount;
                }
            }
            catch (SystemException ex)
            {
                statusMessage = "Allgemeiner Fehler in ConvertWordDocument";
                GlobalFunctions.LogError(statusMessage, ex);
            }
            return pageCount;
        }

        /// <summary>
        /// Konvertiert eine Html-Datei in eine Pdf-Datei
        /// </summary>
        /// <param name="EAktDocument"></param>
        /// <param name="PdfPath"></param>
        /// <returns></returns>
        private static int ConvertHtmlDocument(EAktDocumentViewModel EAktDocument, string PdfPath)
        {
            int pageCount = 0;
            object objLock = new object();
            try
            {
                // Während der Konvertierung darf die Methode nicht erneut aufrufbar sein
                lock (objLock)
                {
                    AsposeWords.Document wdDoc = new Aspose.Words.Document(EAktDocument.Path);
                    wdDoc.Save(PdfPath, Aspose.Words.SaveFormat.Pdf);
                    EAktDocument.PdfPath = PdfPath;
                    statusMessage = "ConvertHtmlDocument: " + EAktDocument.Path + " wurde in " + EAktDocument.PdfPath + " konvertiert.";
                    GlobalFunctions.LogInfo(statusMessage);
                    pageCount = wdDoc.PageCount;
                }
            }
            catch (SystemException ex)
            {
                statusMessage = "ConvertHtmlDocument: Allgemeiner Fehler bei der Html nach Pdf-Konvertierung.";
                GlobalFunctions.LogError(statusMessage, ex);
            }
            return pageCount;
        }

        /// <summary>
        /// Konvertiert ein Rtf-Dokument nach Pdf
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="PdfPath"></param>
        /// <returns></returns>
        private static int ConvertRtfDocument(EAktDocumentViewModel EAktDocument, string PdfPath)
        {
            int pageCount = 0;
            object objLock = new object();
            try
            {
                // Während der Konvertierung darf die Methode nicht erneut aufrufbar sein
                lock (objLock)
                {
                    AsposeWords.Document wdDoc = new AsposeWords.Document(EAktDocument.Path);
                    // Bei der alten Aspose-Words ist ein Zwischenschritt erforderlich - abspeichern als Docx-Datei
                    string docxPfad = Path.Combine(GlobalVars.TempPath, Path.ChangeExtension(EAktDocument.Name, ".docx"));
                    wdDoc.Save(docxPfad, Aspose.Words.SaveFormat.Docx);
                    wdDoc = new AsposeWords.Document(docxPfad);
                    wdDoc.Save(PdfPath, Aspose.Words.SaveFormat.Pdf);
                    EAktDocument.PdfPath = PdfPath;
                    statusMessage = "ConvertRtfDocument: " + EAktDocument.Path + " wurde in " + EAktDocument.PdfPath + " konvertiert.";
                    GlobalFunctions.LogInfo(statusMessage);
                }
            }
            catch (AsposeWords.FileCorruptedException ex)
            {
                statusMessage = "ConvertRtfDocument: AsposeWords-Fehler bei der Rtf nach Pdf-Konvertierung.";
                GlobalFunctions.LogError(statusMessage, new SystemException("", ex));
            }
            catch (SystemException ex)
            {
                statusMessage = "ConvertRtfDocument: Allgemeiner Fehler bei der Rtf nach Pdf-Konvertierung.";
                GlobalFunctions.LogError(statusMessage, ex);
            }
            return pageCount;
        }


        /// <summary>
        /// Konvertiert eine Bitmap (außer Tiff) in eine Pdf-Datei
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="PdfPath"></param>
        /// <returns></returns>
        private static int ConvertBitmapDocument(EAktDocumentViewModel EAktDocument, string PdfPath)
        {
            int pageCount = 0;
            object objLock = new object();
            // Während der Konvertierung darf die Methode nicht erneut aufrufbar sein
            lock (objLock)
            {
                try
                {
                    // Neues Pdf-Dokument anlegen
                    AspGen.Pdf pdfGenerator = new AspGen.Pdf();
                    AspGen.Section sec1 = pdfGenerator.Sections.Add();
                    sec1.PageInfo.Margin.Top = 5;
                    sec1.PageInfo.Margin.Bottom = 5;
                    sec1.PageInfo.Margin.Left = 5;
                    sec1.PageInfo.Margin.Right = 5;
                    AspGen.Image imgPdf = new AspGen.Image(sec1);
                    sec1.Paragraphs.Add(imgPdf);
                    imgPdf.ImageInfo.File = EAktDocument.Path;
                    string bitmapExt = Path.GetExtension(EAktDocument.Path);
                    switch (bitmapExt)
                    {
                        case ".bmp":
                            imgPdf.ImageInfo.ImageFileType = AspGen.ImageFileType.Bmp;
                            imgPdf.ImageInfo.Title = "Png-Datei";
                            break;
                        case ".png":
                            imgPdf.ImageInfo.ImageFileType = AspGen.ImageFileType.Png;
                            imgPdf.ImageInfo.Title = "Png-Datei";
                            break;
                        case ".jpg":
                        case ".jpeg":
                            imgPdf.ImageInfo.ImageFileType = AspGen.ImageFileType.Jpeg;
                            imgPdf.ImageInfo.Title = "Jpeg-Datei";
                            break;
                        case ".gif":
                            imgPdf.ImageInfo.ImageFileType = AspGen.ImageFileType.Gif;
                            imgPdf.ImageInfo.Title = "Gif-Datei";
                            break;
                        default:
                            statusMessage = "Unbekantes Bitmapformat: " + bitmapExt;
                            GlobalFunctions.LogError(statusMessage, null);
                            break;
                    }
                    pdfGenerator.Save(PdfPath);
                    EAktDocument.PdfPath = PdfPath;
                    pageCount = pdfGenerator.PageCount;
                }
                catch (SystemException ex)
                {
                    statusMessage = "ConvertBitmapDocument: Allgemeiner Fehler bei der Bitmap nach Pdf-Konvertierung.";
                    GlobalFunctions.LogError(statusMessage, ex);
                }
            }
            return pageCount;
        }

        /// <summary>
        /// Konvertiert eine Tiff-Datei in eine Pdf-Datei
        /// </summary>
        /// <param name="Document"></param>
        /// <returns></returns>
        private static int ConvertTiffDocument(EAktDocumentViewModel EAktDocument, string PdfPath)
        {
            int pageCount = 0;
            object objLock = new object();
            // Während der Konvertierung darf die Methode nicht erneut aufrufbar sein
            lock (objLock)
            {
                try
                {
                    // Byte-Array anlegen
                    byte[] tmpBytes;
                    // Tiff-Datei byteweise einlesen
                    using (FileStream fs = new FileStream(EAktDocument.Path, FileMode.Open, FileAccess.Read))
                    {
                        tmpBytes = new byte[fs.Length];
                        fs.Read(tmpBytes, 0, Convert.ToInt32(fs.Length));
                    }
                    // MemoryStream mit den Bytes anlegen
                    using (MemoryStream ms = new MemoryStream(tmpBytes))
                    {
                        Bitmap b = new Bitmap(ms);

                        // Neues Pdf-Dokument anlegen
                        AspGen.Pdf pdfGenerator = new AspGen.Pdf();
                        AspGen.Section sec1 = new AspGen.Section(pdfGenerator);
                        sec1.PageInfo.Margin.Top = 5;
                        sec1.PageInfo.Margin.Bottom = 5;
                        sec1.PageInfo.Margin.Left = 5;
                        sec1.PageInfo.Margin.Right = 5;
                        sec1.PageInfo.PageWidth = (b.Width / b.HorizontalResolution) * 72;
                        sec1.PageInfo.PageHeight = (b.Height / b.VerticalResolution) * 72;
                        pdfGenerator.Sections.Add(sec1);

                        // Image-Bereich hinzufügen
                        AspGen.Image imgPdf = new AspGen.Image(sec1);
                        sec1.Paragraphs.Add(imgPdf);
                        imgPdf.ImageInfo.ImageFileType = Aspose.Pdf.Generator.ImageFileType.Tiff;
                        //imgPdf.ImageInfo.IsBlackWhite = true;
                        // Inhalt festlegen
                        imgPdf.ImageInfo.ImageStream = ms;
                        imgPdf.ImageScale = 0.95F;
                        // Pdf-Dokument speichern
                        pdfGenerator.Save(PdfPath);
                        EAktDocument.PdfPath = PdfPath;
                        statusMessage = EAktDocument.Path + " wurde nach " + EAktDocument.PdfPath + " konvertiert.";
                        GlobalFunctions.LogInfo(statusMessage);
                        // Anzahl Seiten holen
                        pageCount = b.GetFrameCount(FrameDimension.Page);
                        b.Dispose();
                    }
                }
                catch (SystemException ex)
                {
                    statusMessage = "ConvertTiffDocument: Allgemeiner Fehler bei der Tiff nach Pdf-Konvertierung.";
                    GlobalFunctions.LogError(statusMessage, ex);
                }
            }
            return pageCount;
        }

        /// <summary>
        /// Konvertiert eine Textdatei in eine Pdf-Datei
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="PdfPath"></param>
        /// <returns></returns>
        private static int ConvertTextDocument(EAktDocumentViewModel EAktDocument, string PdfPath)
        {
            int pageCount = 0;
            object objLock = new object();
            // Während der Konvertierung darf die Methode nicht erneut aufrufbar sein
            lock (objLock)
            {
                try
                {
                    // Ein Aspose-Dokument anlegen
                    AsposeWords.Document wdDoc = new AsposeWords.Document(EAktDocument.Path);
                    // Dokument als Pdf-Datei speichern
                    wdDoc.Save(PdfPath, AsposeWords.SaveFormat.Pdf);
                    EAktDocument.PdfPath = PdfPath;
                    statusMessage = "ConvertTxtDocument: " + EAktDocument.Path + " wurde in " + EAktDocument.PdfPath + " konvertiert.";
                    GlobalFunctions.LogInfo(statusMessage);
                    pageCount = wdDoc.PageCount;
                }
                catch (SystemException ex)
                {
                    statusMessage = "ConvertTxtDocument: Allgemeiner Fehler bei der Txt nach Pdf-Konvertierung.";
                    GlobalFunctions.LogError(statusMessage, ex);
                }
            }
            return pageCount;
        }

        /// <summary>
        /// Sendet eine Meldung als StatusMessageViewModel-Objekt
        /// </summary>
        /// <param name="Message"></param>
        private static void SendStatusMessage(string Message, bool IsError = false)
        {
            msgStatus = new AddStatusMessage
            {
                StatusMessage = new StatusMessageViewModel
                {
                    Message = Message,
                    IsError = IsError,
                    TimeStamp = DateTime.Now
                }
            };
            Messenger.Default.Send<AddStatusMessage>(msgStatus);
        }

    }

}
