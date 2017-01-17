// ============================================================
// File: FileDocumentService.cs
// ============================================================

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using EAKTEngineV1.Helper;
using EAKTEngineV1.Model;
using EAKTEngineV1.ViewModel;
using EAKTEngineV1.Messages;

using DevExpress.Mvvm;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Wird für das Laden der Dokumente über das Dateisystem verwendet
    /// </summary>
    public class FileDocumentService : IDocumentService
    {
        // Private Variablen
        private string statusMessage;
        private int documentCount;
        private int documentIndex;
        private int errorCount;
        private SetProgressbarMessage msgProgress;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EAktPath"></param>
        /// <returns></returns>
        ObservableCollection<EAktDocumentViewModel> IDocumentService.GetDocuments(string EAktPath)
        {
            ObservableCollection<EAktDocumentViewModel> eaktDocuments = new ObservableCollection<EAktDocumentViewModel>();

            // Namen der EAkt-Mappe speichern
            GlobalVars.EAktName = Path.GetFileNameWithoutExtension(EAktPath);
            // Namen der Kommentardatei festlegen
            GlobalVars.CommentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Path.ChangeExtension(GlobalVars.EAktName, ".EAktKommentar"));

            try
            {
                // Die EAKT-XML-Struktur einlesen
                XElement xEAKT;
                // XML-Datei über StreamReader einlesen
                using (StreamReader xmlSr = new StreamReader(EAktPath, Encoding.UTF8))
                {
                    // XML-Datei einlesen
                    xEAKT = XElement.Load(xmlSr);
                }
                // XName für den Dokument-Knoten holen
                XName xnDoc = XName.Get("Dokument", "urn:eakte");
                // Anzahl der Dokumente feststellen
                documentCount = xEAKT.Descendants(xnDoc).Count();

                // Index explizit auf 0 setzen
                documentIndex = 0;

                // Alle Dokument-Einträge durchgehen
                foreach (XElement xDoc in xEAKT.Descendants(xnDoc))
                {
                    documentIndex++;

                    // Fortschrittsbalken aktualisieren
                    double progressValue = (double)documentIndex / documentCount * 100;
                    msgProgress = new SetProgressbarMessage { Value = Convert.ToInt32(progressValue) };
                    Messenger.Default.Send<SetProgressbarMessage>(msgProgress);

                    try
                    {
                        string docPath = xDoc.Attribute("Pfad") != null ? xDoc.Attribute("Pfad").Value : "";
                        string docName = xDoc.Attribute("Name") != null ? xDoc.Attribute("Name").Value : "";
                        string docFilepath = Path.Combine(docPath, docName);
                        string docExt = Path.GetExtension(docFilepath);
                        string ersteller = xDoc.Attribute("Ersteller") != null ? xDoc.Attribute("Ersteller").Value : "";
                        string eingang = xDoc.Attribute("Eingang") != null ? xDoc.Attribute("Eingang").Value : "";

                        // Zuerst ein DokumentViewModel-Objekt anlegen
                        EAktDocumentViewModel document = new EAktDocumentViewModel();
                        document.Id = documentIndex;
                        document.Path = docFilepath;
                        document.Name = docName;
                        document.Ersteller = ersteller;
                        document.Eingang = eingang;
                        document.Schriftsatz = xDoc.Attribute("Schriftsatz") != null ? xDoc.Attribute("Schriftsatz").Value : "";
                        document.Indicator = xDoc.Attribute("Ampel") != null ? (DocumentIndicator)Enum.Parse(typeof(DocumentIndicator), xDoc.Attribute("Ampel").Value) : (DocumentIndicator)Enum.Parse(typeof(DocumentIndicator), "Green");
                        switch (docExt.ToLower())
                        {
                            case ".pdf":
                                document.Type = DocumentType.Pdf;
                                break;
                            case ".doc":
                            case ".docx":
                                document.Type = DocumentType.Word;
                                break;
                            case ".htm":
                            case ".html":
                                document.Type = DocumentType.Html;
                                break;
                            case ".txt":
                            case ".rtf":
                                document.Type = DocumentType.Text;
                                break;
                            case ".bmp":
                            case ".png":
                            case ".jpg":
                            case ".jpeg":
                                document.Type = DocumentType.Bitmap;
                                break;
                            case ".tif":
                            case ".tiff":
                                document.Type = DocumentType.Tiff;
                                break;
                            default:
                                document.Type = DocumentType.Unknown;
                                break;
                        }

                        // Dokument am Anfang als gültig markieren - ob es lesbar ist, wird erst beim
                        // konvertieren festgestellt
                        document.IsValid = true;
                        // Dokument ist doch hoffentlich vorhanden
                        document.IsPresent = true;

                        // Gibt es die Datei nicht, wird sie ausgelassen
                        if (!File.Exists(docFilepath))
                        {
                            // Dokument ist doch nicht vorhanden daher markieren
                            document.IsPresent = false;
                            // Ein nicht vorhandenes Dokument kann eigentlich nicht gültig sein
                            document.IsValid = false;
                            errorCount++;
                            statusMessage = docFilepath + " existiert nicht und wird ausgelassen.";
                            AddStatusMessage msgStatus = new AddStatusMessage
                            {
                                StatusMessage = new StatusMessageViewModel
                                {
                                    Message = statusMessage,
                                    IsError = true,
                                    TimeStamp = DateTime.Now
                                }
                            };
                            Messenger.Default.Send<AddStatusMessage>(msgStatus);
                            // GlobalFunctions.LogError(statusMessage, null, true);
                        }

                        // dokument.Schriftsatz = "Nicht vorhanden";
                        // DokumentViewModel zur Liste hinzufügen
                        eaktDocuments.Add(document);
                    }
                    catch (SystemException ex)
                    {
                        string docName = xDoc.Element("Name") != null ? xDoc.Element("Name").Value : "Kein DokName";
                        string nodeName = xDoc.Name.LocalName != null ? xDoc.Name.LocalName : "Kein Knotenname";
                        statusMessage = String.Format("Blöder Fehler in EAktLaden beim Knoten {0} in Dokument {1} ({2)", nodeName, docName, ex.Message);
                        GlobalFunctions.LogError(statusMessage, ex);
                    }
                }
            }
            catch (SystemException ex)
            {
                statusMessage = "Allgemeiner Fehler in GetDocuments()";
                GlobalFunctions.LogError(statusMessage, ex);
            }
            return eaktDocuments;
        }
    }
}
