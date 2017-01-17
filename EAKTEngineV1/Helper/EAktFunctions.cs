// ========================================================
// File: EAktFunctions.cs
// ========================================================

using System;
using System.Linq;
using System.Xml.Linq;
using System.IO;
using System.Text;

using EAKTEngineV1;
using EAKTEngineV1.Messages;
using EAKTEngineV1.Model;
using EAKTEngineV1.ViewModel;

using Microsoft.Practices.Unity;

using DevExpress.Mvvm;
using System.Collections.Generic;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Functions für den Umgang mit EAkt-Mappendateien
    /// </summary>
    public static class EAktFunctions
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private static string statusMessage;
        private static SetProgressbarMessage msgProgress;

        /// <summary>
        /// Nur provisorisch, da ein EAktDocumentsViewModel-Objekt offiziell für den DI Container instanziert wird
        /// </summary>
        /// <param name="EAktPath"></param>
        /// <returns></returns>
        [Obsolete("Sollte nicht mehr verwendet werden, da EAktDocumentsViewModel über DI Container geholt wird.")]
        public static List<EAktDocumentViewModel> LoadEAkt2(string EAktPath)
        {
            List<EAktDocumentViewModel> eaktDocuments = new List<EAktDocumentViewModel>();
            return eaktDocuments;
        }

        /// <summary>
        /// Laden einer EAkt-Datei
        /// </summary>
        /// <param name="EAktPath"></param>
        /// <returns></returns>
        [Obsolete("Sollte nicht mehr verwendet werden, da EAktDocumentsViewModel über DI Container geholt wird.")]
        public static EAktDocumentsViewModel LoadEAkt(string EAktPath)
        {
            // Lokale Variablen
            int documentIndex = 0;
            int errorCount = 0;
            int documentCount = 0;

            // Namen der EAkt-Mappe speichern
            GlobalVars.EAktName = Path.GetFileNameWithoutExtension(EAktPath);
            // Namen der Kommentardatei festlegen
            GlobalVars.CommentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Path.ChangeExtension(GlobalVars.EAktName, ".EAktKommentar"));
            // ViewModel für die ganze Mappe anlegen
            // TODO: Über DI Container mit einem parameterlosen Konstruktor - Alternative wäre ein Pfad als Argument
            // In V2 kann dies auch eine URL auf einen Azure Storage-Bereich sein
            EAktDocumentsViewModel tmpVm = new EAktDocumentsViewModel();
            try
            {
                // Die EAKT-XML-Struktur einlesen
                XElement xEAKT;
                using (StreamReader xmlSr = new StreamReader(EAktPath, Encoding.UTF8))
                {
                    xEAKT = XElement.Load(xmlSr);
                }
                XName xnDoc = XName.Get("Dokument", "urn:eakte");
                // Anzahl der Dokumente feststellen
                documentCount = xEAKT.Descendants(xnDoc).Count();

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

                        // Zuerst ein DokumentModel-Objekt anlegen
                        EAktDocumentViewModel document = new EAktDocumentViewModel();
                        document.Id = documentIndex;
                        document.Path = docFilepath;
                        document.Name = docName;
                        document.Ersteller = ersteller;
                        document.Eingang = eingang;
                        document.Schriftsatz = xDoc.Attribute("Schriftsatz") != null ? xDoc.Attribute("Schriftsatz").Value : "";
                        document.Indicator = xDoc.Attribute("Ampel") != null ? (DocumentIndicator) Enum.Parse(typeof(DocumentIndicator), xDoc.Attribute("Ampel").Value) : (DocumentIndicator)Enum.Parse(typeof(DocumentIndicator), "Green");
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

                        document.IsValid = true;

                        // Gibt es die Datei nicht, wird sie ausgelassen
                        if (!File.Exists(docFilepath))
                        {
                            errorCount++;
                            statusMessage = docFilepath + " existiert nicht und wird ausgelassen.";
                            AddStatusMessage msgStatus = new AddStatusMessage
                            {
                                StatusMessage = new StatusMessageViewModel
                                {
                                    Message = statusMessage,
                                    IsError = false,
                                    TimeStamp = DateTime.Now
                                }
                            };
                            Messenger.Default.Send<AddStatusMessage>(msgStatus);
                            // GlobalFunctions.LogError(statusMessage, null, true);

                            document.IsValid = false;
                            // dokument.Schriftsatz = "Nicht vorhanden";
                        }
                        // DokumentViewModel zur Liste hinzufügen
                        tmpVm.Documents.Add(document);
                    }
                    catch (SystemException ex)
                    {
                        string docName = xDoc.Element("Name") != null ? xDoc.Element("Name").Value : "Kein DokName";
                        string nodeName = xDoc.Name.LocalName != null ? xDoc.Name.LocalName : "Kein Knotenname";
                        statusMessage = String.Format("Blöder Fehler in EAktLaden beim Knoten {0} in Dokument {1} ({2})", nodeName, docName, ex.Message);
                        GlobalFunctions.LogError(statusMessage, ex);
                    }
                }
            }
            catch (SystemException ex)
            {
                statusMessage = "Allgemeiner Fehler in EAktLaden";
                GlobalFunctions.LogError(statusMessage, ex);
            }

            // Fortschrittsbalken zurücksetzen
            msgProgress = new SetProgressbarMessage { Value = 0 };
            Messenger.Default.Send<SetProgressbarMessage>(msgProgress);

            return tmpVm;
        }

        /// <summary>
        /// Liest ein Verzeichnis rekursiv ein und gibt das resultierende Xml als XElement zurück
        /// </summary>
        /// <param name="DirectoryPath"></param>
        /// <param name="DocumentIndex"></param>
        /// <param name="xRoot"></param>
        /// <returns></returns>
        private static XElement ReadDirectory(string DirectoryPath, int DocumentIndex, XElement xRoot)
        {
            // Name für XML-aDatei
            XNamespace eaktName = "urn:eakte";
            // Das aktuelle Verzeichnis durchgehen
            FileInfo[] eaktFiles = new DirectoryInfo(DirectoryPath).GetFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo fi in eaktFiles)
            {
                // Temporäre Dateien auslassen
                if (fi.Name.StartsWith("~") || fi.Name.StartsWith("$"))
                {
                    continue;
                }
                // Besitzt die Datei eine erlaubte Erweiterung?
                if (GlobalVars.AllowedExtensionsList.Contains(fi.Extension.ToLower()))
                {
                    try
                    {
                        string DocName = fi.Name;
                        string DocPath = Path.GetDirectoryName(fi.FullName);
                        string GroupName = fi.Directory.Name;
                        // Schriftsatz wird aus dem Dateinamen abgeleitet
                        var schriftsatzArt = GlobalVars.SchriftsatzArten.ToList().Find(SchrArt => fi.Name.Contains(SchrArt));
                        string Schriftsatz = schriftsatzArt != null ? schriftsatzArt : "Nicht festgelegt";
                        string Ersteller = "Kein Ersteller";
                        string Eingang = fi.CreationTime.ToShortDateString();
                        // NEU: Der Verzeichnisname wird als GroupName-Attribut gespeichert
                        XElement XDok = new XElement(eaktName + "Dokument",
                          new XAttribute("Pfad", DocPath),
                          new XAttribute("Name", DocName),
                          new XAttribute("Gruppe", GroupName),
                          new XAttribute("DokNr", DocumentIndex),
                          new XAttribute("Schriftsatz", Schriftsatz),
                          new XAttribute("Eingang", Eingang),
                          new XAttribute("Ersteller", Ersteller),
                          new XAttribute("Ampel", "Grün"));
                        xRoot.Add(XDok);
                        DocumentIndex++;
                    }
                    catch (SystemException ex)
                    {
                        statusMessage = "Fehler beim Verarbeiten der Datei " + fi.Name;
                        GlobalFunctions.LogError(statusMessage, ex);
                    }
                }
                // Datei besitzt nicht erlaubte Dateierweiterung
                else
                {
                    statusMessage = "Die Datei " + fi.Name + " wird ausgelassen. Dateien vom Typ '"
                        + fi.Extension.Trim('.').ToLower() + "' werden nicht geladen.";
                    GlobalFunctions.LogInfo(statusMessage);
                }
            }

            // Jetzt alle Unterverzeichnisse durchen
            foreach (DirectoryInfo dir in new DirectoryInfo(DirectoryPath).GetDirectories())
            {
                xRoot = ReadDirectory(dir.FullName, DocumentIndex, xRoot);
            }

            return xRoot;
        }

        /// <summary>
        /// Liest alle Dokumente in einem Verzeichnis ein
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public static EAktDocumentsViewModel ReadEAktDirectory(string DirectoryPath, string EAktPath)
        {
            // Pfad der anzulegenden EAkt-Datei bilden
            // string EAktPath = Path.Combine(DirectoryPath, Path.GetFileName(DirectoryPath + ".Eakt"));
            int documentIndex = 1;
            // XName für Stammelement bilden
            XNamespace eakt = "urn:eakte";
            XElement eaktRoot = new XElement(eakt + "EAkte", new XAttribute(XNamespace.Xmlns + "eakt", eakt));
            // Alle Dateien in dem Verzeichnis rekursiv durchgehen
            eaktRoot = ReadDirectory(DirectoryPath, documentIndex,  eaktRoot);
            // EAkt-Datei speichern
            eaktRoot.Save(EAktPath);
            // Pfad und Name der EAkt-Datei merken
            GlobalVars.EAktPath = EAktPath;
            GlobalVars.EAktName = Path.GetFileNameWithoutExtension(EAktPath);
            // Pfad für Kommentardatei festlegen
            GlobalVars.CommentsPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Path.GetFileNameWithoutExtension(GlobalVars.EAktName) + ".EAktKommentar");

            GlobalFunctions.LogInfo(EAktPath + " wurde gespeichert.");
            // EAkt-Datei einlesen und als EAktDocumentsViewModel zurückgeben
            return LoadEAkt(EAktPath);

        }

        /// <summary>
        /// Speichert die aktuelle EAkt-Mappe in einer Xml-Datei
        /// </summary>
        /// <param name="EAktPath"></param>
        public static void SaveEAktDocument(EAktDocumentsViewModel documentsVm, string EAktPath)
        {
            try
            {
                // XName für Stammelement bilden
                XNamespace eakt = "urn:eakte";
                try
                {
                    XElement root = new XElement(eakt + "EAkte", new XAttribute(XNamespace.Xmlns + "eakt", eakt));
                    // Alle Dateien in der Liste durchgehen
                    foreach (EAktDocumentViewModel EAktDoc in documentsVm.Documents)
                    {
                        // Dokument-Knoten anlegen
                        XElement XDok = new XElement(eakt + "Dokument",
                            new XAttribute("Pfad", EAktDoc.Path),
                            new XAttribute("Name", EAktDoc.Name),
                            new XAttribute("DokNr", EAktDoc.Id),
                            new XAttribute("Schriftsatz", EAktDoc.Schriftsatz),
                            new XAttribute("Eingang", EAktDoc.Eingang),
                            new XAttribute("Ersteller", EAktDoc.Ersteller),
                            new XAttribute("Ampel", EAktDoc.Indicator));
                        root.Add(XDok);
                    }
                    // Xml-Datei speichern
                    root.Save(EAktPath);
                }
                catch (SystemException ex)
                {
                    statusMessage = "Fehler beim Dokument-Knoten.";
                    GlobalFunctions.LogError(statusMessage, ex);
                }
            }
            catch (SystemException ex)
            {
                statusMessage = "Fehler beim Speichern der EAkt-Datei.";
                GlobalFunctions.LogError(statusMessage, ex);
            }

        }

        /// <summary>
        /// List die in der Kommentardatei gespeicherte Dokumenteauswahl ein und markiert die
        /// Dokumente als selektiert
        /// </summary>
        /// <param name="documentsVm"></param>
        public static void ReadDocumentSelection(EAktDocumentsViewModel documentsVm)
        {
            // Gibt es eine Kommentardatei?
            if (File.Exists(GlobalVars.CommentsPath))
            {
                try
                {
                    // Gibt es das Selektion-Element?
                    XElement xDok = XElement.Load(GlobalVars.CommentsPath);
                    XElement xSelektion = xDok.Element("Selektion");
                    if (xSelektion != null)
                    {
                        int anzahl = 0;
                        // Alle Dokument-Elemente einlesen
                        foreach (XElement xSelektiert in xSelektion.Descendants("Dokument"))
                        {
                            // Zuordnung zu einem Dokument der Dokumentmappe
                           EAktDocumentViewModel EAktDoc = (from d in documentsVm.Documents
                                                            where d.Name == xSelektiert.Value
                                                            select d).SingleOrDefault();
                            // Gibt es ein Dokument in der Dokumentmappe?
                            if (EAktDoc != null)
                            {
                                anzahl++;
                                // Dokument als selektiert markieren
                                EAktDoc.IsSelected = true;
                            }
                            else
                            {
                                statusMessage = "Keine Dokumentzuordnung beim Laden der Aktenauswahl möglich.";
                                GlobalFunctions.LogError(statusMessage, null);
                            }
                        }
                        statusMessage = String.Format("{0} selektierte Dokumente eingelesen.", anzahl);
                        GlobalFunctions.LogInfo(statusMessage);
                    }
                }
                catch (SystemException ex)
                {
                    statusMessage = "Fehler beim Laden der Aktenauswahl.";
                    GlobalFunctions.LogError(statusMessage, ex);
                }
            }
        }

        /// <summary>
        /// Speichern der Dokumenteauswahl in Gestalt selektierter Dokumente
        /// in der Kommentardatei
        /// </summary>
        /// <param name="documentsVm"></param>
        public static void SaveDocumentSelection(EAktDocumentsViewModel documentsVm)
        {
            try
            {
                XDocument xDok = null;
                XElement xSelektion = null;
                // Alle selektierten Dokumente in eine Liste
                List<EAktDocumentViewModel> documentsSelected = (from d in documentsVm.Documents
                                                                 where d.IsSelected select d).ToList();
                // Gibt es selektierte Dokumente?
                if (documentsSelected.Count > 0)
                {
                    try
                    {
                        // Kommentardatei und gegebenenfalls Selektion-Element anlegen
                        if (!File.Exists(GlobalVars.CommentsPath))
                        {
                            xDok = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
                            xDok.Add(new XElement("EAktKommentare", new XElement("Selektion")));
                            xDok.Save(GlobalVars.CommentsPath);
                            xSelektion = xDok.Element("EAktKommentare").Element("Selektion");
                        }
                        else
                        {
                            // Vorhandene Kommentardatei laden
                            xDok = XDocument.Load(GlobalVars.CommentsPath);
                            xSelektion = xDok.Element("EAktKommentare").Element("Selektion");
                            // Gibt es ein Seletion-Element?
                            if (xSelektion == null)
                            {
                                xDok.Element("EAktKommentare").Add(new XElement("Selektion"));
                                xSelektion = xDok.Element("EAktKommentare").Element("Selektion");
                            }
                            // Alle vorhandenen Einträge entfernen
                            else
                            {
                                xSelektion.RemoveAll();
                            }
                        }
                        int anzahl = 0;
                        // Alle als selektiert markierten Dokumente durchgehen
                        foreach (EAktDocumentViewModel EAktDoc in documentsSelected)
                        {
                            try
                            {
                                xSelektion.Add(new XElement("Dokument", EAktDoc.Name));
                                anzahl++;
                            }
                            catch (SystemException ex)
                            {
                                statusMessage = "Fehler während des Speichern einer EAkt-Selektion.";
                                GlobalFunctions.LogError(statusMessage, ex);
                            }
                        }
                        // Die Kommentardatei speichern
                        xDok.Save(GlobalVars.CommentsPath);
                        statusMessage = String.Format("Die Dokumenteauswahl wurde mit {0} Dokumenten gespeichert.", anzahl);
                        GlobalFunctions.LogInfo(statusMessage);
                    }
                    catch (SystemException ex)
                    {
                        statusMessage = "Allgemeiner Fehler beim Speichern der Dateiauswahl.";
                        GlobalFunctions.LogError(statusMessage, ex);
                    }
                }
            }
            catch (SystemException ex)
            {
                statusMessage = "Allgemeiner Fehler beim Speichern der Dateiauswahl.";
                GlobalFunctions.LogError(statusMessage, ex);
            }
        }
    }
}
