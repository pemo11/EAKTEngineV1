// ========================================================
// File: PreviewPagesFunctions.cs
// ========================================================

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

using AsposePdf = Aspose.Pdf;

using EAKTEngineV1.Helper;
using EAKTEngineV1.Messages;
using EAKTEngineV1.ViewModel;

using DevExpress.Mvvm;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Enthält Functions für das Anlegen von Vorschauseiten
    /// </summary>
    public static class PreviewFunctions
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private static string statusMessage;
        private static SetProgressbarMessage msgProgress;

        /// <summary>
        /// Legt eine Seitenvorschau an
        /// </summary>
        /// <param name="Documents"></param>
        /// <returns></returns>
        public static int CreateDocumentsPreview(ObservableCollection<EAktDocumentViewModel> Documents)
        {
            int anzahl = 0;
            // Verzeichnis für Vorschaudateien anlegen
            string previewPath = Path.Combine(GlobalVars.TempPath, GlobalVars.EAktName);
            previewPath = Path.Combine(previewPath, "PreviewPages");
            if (!Directory.Exists(previewPath))
            {
                Directory.CreateDirectory(previewPath);
            }
            foreach (EAktDocumentViewModel Document in Documents)
            {
                // Fortschrittsbalken aktualisieren
                double progressValue = (double)Document.Id / Documents.Count * 100;
                msgProgress = new SetProgressbarMessage { Value = Convert.ToInt32(progressValue) };
                Messenger.Default.Send<SetProgressbarMessage>(msgProgress);

                anzahl += CreateDocumentPreview(Document, previewPath);
            }

            // Fortschrittsbalken zurücksetzen
            msgProgress = new SetProgressbarMessage { Value = 0 };
            Messenger.Default.Send<SetProgressbarMessage>(msgProgress);

            return anzahl;
        }

        /// <summary>
        /// Legt die Seitenvorschau für ein Dokument an
        /// </summary>
        /// <param name="Document"></param>
        /// <param name="PreviewPath"></param>
        /// <returns></returns>
        private static int CreateDocumentPreview(EAktDocumentViewModel Document, string PreviewPath)
        {
            int anzahl = 0;
            try
            {
                // Nur für gültige Dokumente eine Seitenvorschau anlegen
                if (Document.IsValid)
                {
                    try
                    {
                        AsposePdf.Document pdfDoc = new Aspose.Pdf.Document(Document.PdfPath);
                        for (int i = 1; i <= pdfDoc.Pages.Count; i++)
                        {
                            string pfadJpgSeite = Path.Combine(PreviewPath, String.Format("{0}_Seite{1:000}.jpg",
                                Path.GetFileName(Document.PdfPath), i));
                            // Nur anlegen, wenn Datei noch nicht existiert
                            if (!File.Exists(pfadJpgSeite))
                            {
                                using (FileStream fs = new System.IO.FileStream(pfadJpgSeite, FileMode.Create))
                                {
                                    AsposePdf.Devices.Resolution jpgResolution = new AsposePdf.Devices.Resolution(96, 64);
                                    AsposePdf.Devices.JpegDevice jpgDevice = new AsposePdf.Devices.JpegDevice(jpgResolution);
                                    jpgDevice.Process(pdfDoc.Pages[i], fs);
                                }
                            }
                            anzahl++;
                        }
                    }
                    catch (Aspose.Pdf.Exceptions.PdfException)
                    {
                        statusMessage = "Aspose: Fehler in CreateSeitenvorschau beim Laden einer Pdf-Datei.";
                        GlobalFunctions.LogError(statusMessage, null);
                    }
                    catch (SystemException ex)
                    {
                        statusMessage = "Aspose: Fehler in CreateSeitenvorschau beim Laden einer Pdf-Datei.";
                        GlobalFunctions.LogError(statusMessage, ex);
                    }

                }
            }
            catch (SystemException ex)
            {
                statusMessage = "Allgemeiner Fehler in CreateDocumentPreview";
                GlobalFunctions.LogError(statusMessage, ex);
            }
            return anzahl;
        }

        /// <summary>
        /// Legt für eine Pdf-Datei Vorschauseiten als Jpg-Dateien an
        /// </summary>
        /// <param name="PdfPath"></param>
        public static Task CreatePdfPagePreviewAsync(string PdfPath)
        {
            return Task.Run<int>(() =>
            {
                int anzahl = 0;
                // Verzeichnispfad für die Vorschaudateien bilden
                string PdfImagePath = Path.Combine(GlobalVars.TempPath, GlobalVars.EAktName);
                PdfImagePath = Path.Combine(PdfImagePath, "PreviewImagePath");
                // Verzeichnispfad für die Vorschaudateien anlegen
                if (!Directory.Exists(PdfImagePath))
                {
                    Directory.CreateDirectory(PdfImagePath);
                }
                try
                {
                    AsposePdf.Document pdfDoc = new Aspose.Pdf.Document(PdfPath);
                    for (int i = 1; i <= pdfDoc.Pages.Count; i++)
                    {
                        string pfadJpgSeite = Path.Combine(PdfImagePath, String.Format("{0}_Seite{1:000}.jpg",
                            Path.GetFileName(PdfPath), i));
                        // Nur anlegen, wenn Datei noch nicht existiert
                        if (!File.Exists(pfadJpgSeite))
                        {
                            using (FileStream fs = new System.IO.FileStream(pfadJpgSeite, FileMode.Create))
                            {
                                AsposePdf.Devices.Resolution jpgResolution = new AsposePdf.Devices.Resolution(96, 64);
                                AsposePdf.Devices.JpegDevice jpgDevice = new AsposePdf.Devices.JpegDevice(jpgResolution);
                                jpgDevice.Process(pdfDoc.Pages[i], fs);
                            }
                        }
                        anzahl++;
                    }
                }
                catch (Aspose.Pdf.Exceptions.PdfException)
                {
                    statusMessage = "Aspose-Fehler in CreatePdfPagePreview.";
                    GlobalFunctions.LogError(statusMessage, null);
                }
                catch (SystemException ex)
                {
                    statusMessage = "Fehler in CreatePdfPagePreview.";
                    GlobalFunctions.LogError(statusMessage, ex);
                }
                return anzahl;

            });
        }
    }
}
