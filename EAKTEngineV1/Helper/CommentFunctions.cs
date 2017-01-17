// ========================================================
// File: CommentFunctions.cs
// ========================================================

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using EAKTEngineV1.ViewModel;
using EAKTEngineV1.Messages;

using DevExpress.Mvvm;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Enthält Functions für den Umgang mit EAkt-Kommentaren
    /// </summary>
    public static class CommentFunctions
    {
        // Private Variablen
        private static string statusMessage;
        
        /// <summary>
        /// Alle Kommentare laden und den Dokumenten zuordnen
        /// </summary>
        /// <param name="documentsVm"></param>
        public static void LoadEAktComments(EAktDocumentsViewModel documentsVm)
        {
            int commentCount = 0;

            // Gibt es eine Kommentardatei?
            if (File.Exists(GlobalVars.CommentsPath))
            {
                // Neues ViewModel für alle Kommentare anlegen
                EAktCommentsViewModel commentsVm = new EAktCommentsViewModel();
                try
                {
                    // Inhalt der Xml-Datei laden
                    XDocument XDocComments = XDocument.Load(GlobalVars.CommentsPath);
                    // Alle Kommentare durchgehen
                    foreach (XElement XlKommentar in XDocComments.Descendants("EAktKommentar"))
                    {
                        try
                        {
                            double xPos = XlKommentar.Attribute("XPosition") != null ? Double.Parse(XlKommentar.Attribute("XPosition").Value, System.Globalization.NumberStyles.Number) : 0;
                            double yPos = XlKommentar.Attribute("YPosition") != null ? Double.Parse(XlKommentar.Attribute("YPosition").Value, System.Globalization.NumberStyles.Number) : 0;
                            double winHeight = XlKommentar.Attribute("Hoehe") != null ? (int)double.Parse(XlKommentar.Attribute("Hoehe").Value) : 160;
                            double winWidth = XlKommentar.Attribute("Breite") != null ? (int)double.Parse(XlKommentar.Attribute("Breite").Value) : 320;
                            string color = XlKommentar.Attribute("Farbe") != null ? XlKommentar.Attribute("Farbe").Value : "Gelb";
                            string commentText = XlKommentar.Element("Text") != null ? XlKommentar.Element("Text").Value : "Kein Text";
                            string commentLink = XlKommentar.Element("Link") != null ? XlKommentar.Element("Link").Value : "";
                            EAktCommentViewModel eaktComment = new EAktCommentViewModel
                            {
                                DocumentPath = XlKommentar.Attribute("DokPfad").Value,
                                Id = Int32.Parse(XlKommentar.Attribute("KommentarNr").Value),
                                PageNr = Int32.Parse(XlKommentar.Attribute("SeitenNr").Value),
                                XPos = xPos,
                                YPos = yPos,
                                WinHeight = winHeight,
                                WinWidth = winWidth,
                                Color = color,
                                Comment = commentText,
                                Link = commentText
                            };
                            commentCount++;
                            commentsVm.Comments.Add(eaktComment);

                        }
                        catch (SystemException ex)
                        {
                            statusMessage = "Fehler beim Einlesen eines Kommentars";
                            GlobalFunctions.LogError(statusMessage, ex);
                        }
                    }

                    // Alle Aktenkommentare den Dokumenten zuordnen
                    foreach (EAktCommentViewModel eaktComment in commentsVm.Comments)
                    {
                        // Kommentar über DocumentId-Eigenschaft zuordnen
                        EAktDocumentViewModel eaktDoc = (from d in documentsVm.Documents where d.Path == eaktComment.DocumentPath select d).SingleOrDefault();
                        // Wurde ein Kommentar gefunden?
                        if (eaktDoc != null)
                        {
                            eaktDoc.CommentsVm.Comments.Add(eaktComment);
                        }
                        else
                        {
                            statusMessage = "Ein Kommentar kann keinem Dokument zugeordnet werden.";
                            GlobalFunctions.LogError(statusMessage, null);
                        }
                    }
                }
                catch (SystemException ex)
                {
                    statusMessage = "Allgemeiner Fehler beim Aktenkommentare laden.";
                    GlobalFunctions.LogError(statusMessage, ex);
                }
            }
            else
            {
                statusMessage = "Keine Kommentardatei gefunden.";
                GlobalFunctions.LogInfo(statusMessage);
            }
        }

        /// <summary>
        /// Speichern aller Kommentare der EAkt-Mappe
        /// </summary>
        /// <param name="documentsVm"></param>
        public static void SaveEAktComments(EAktDocumentsViewModel documentsVm, string commentsPath)
        {
            int commentsSavedCount = 0;

            // Alle Aktenkommentare der Mappe zusammenfassen 
            List<EAktCommentViewModel> comments = documentsVm.Documents.SelectMany(d => d.CommentsVm.Comments).ToList();

            // Gibt es Kommentare?
            if (comments.Count > 0)
            {
                // Alle Kommentare in die Kommentardatei speichern
                XElement xKommentarDok = null;
                try
                {
                    // Kommmentardatei wird neu angelegt
                    xKommentarDok = new XElement("EAktKommentare", new XAttribute("EAktPfad", GlobalVars.CommentsPath));
                    foreach (EAktCommentViewModel eaktComment in comments)
                    {
                        try
                        {
                            xKommentarDok.Add(new XElement("EAktKommentar",
                                new XAttribute("DokPfad", eaktComment.DocumentPath),
                                new XAttribute("XPosition", eaktComment.XPos),
                                new XAttribute("YPosition", eaktComment.YPos),
                                new XAttribute("Breite", eaktComment.WinWidth),
                                new XAttribute("Hoehe", eaktComment.WinHeight),
                                new XAttribute("SeitenNr", eaktComment.PageNr),
                                new XAttribute("KommentarNr", eaktComment.Id),
                                new XAttribute("Farbe", eaktComment.Color),
                                new XElement("Link", eaktComment.Link),
                                new XElement("Text", eaktComment.Comment)
                            ));
                            commentsSavedCount++;
                        }
                        catch (SystemException ex)
                        {
                            statusMessage = "Fehler beim Anlegen eines EAktKommentar-Knoten.";
                            GlobalFunctions.LogError(statusMessage, ex);
                        }
                    }

                    // Gibt es das Selektion-Element in der Kommmentaredatei?
                    XElement xSelektion = xKommentarDok.Descendants("Selektion").SingleOrDefault();
                    if (xSelektion == null)
                    {
                        // Selection-Element anlegen
                        xSelektion = new XElement("Selektion");
                        xKommentarDok.Add(xSelektion);
                    }
                    // Das Ganze wird in einer Datei gespeichert
                    xKommentarDok.Save(commentsPath);
                    if (comments.Count == 1)
                    {
                        statusMessage = "Der Kommentar wurde in der Kommentardatei gespeichert.";
                    }
                    else
                    {
                        statusMessage = String.Format("{0} Kommentare wurden in der Kommentardatei gespeichert.", comments.Count);
                    }
                    GlobalFunctions.LogInfo(statusMessage);
                }
                catch (SystemException ex)
                {
                    statusMessage = "Allgemeiner Fehler beim Speichern der Aktenkommentare.";
                    GlobalFunctions.LogError(statusMessage, ex);
                }
            }
        }

        /// <summary>
        /// Abspeichern aller Aktenkommentare unter der Standarddatei
        /// </summary>
        /// <param name="documentsVm"></param>
        /// <param name="validationMode"></param>
        public static void SaveEAktComments(EAktDocumentsViewModel documentsVm, bool validationMode = false)
        {
            SaveEAktComments(documentsVm, validationMode);
        }

    }
}
