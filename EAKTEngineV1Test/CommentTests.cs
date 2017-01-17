// ========================================================
// File: ConvertTests.cs
// ========================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DevExpress.Mvvm;

using EAKTEngineV1.ViewModel;
using EAKTEngineV1.Helper;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class CommentTests
    {
        /// <summary>
        /// Einlesen nicht vorhandener Kommentare
        /// </summary>
        [TestMethod]
        public void ReadComments1()
        {
            string eaktPfad = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPfad = TestHelpers.UpdateEAktFile(eaktPfad);
            string eaktName = Path.GetFileNameWithoutExtension(eaktPfad);
            MainViewModel mainVm = new MainViewModel();
            // Kommentardatei löschen falls vorhanden
            TestHelpers.DeleteCommentFile(eaktName);
            // EAktDatei laden
            mainVm.LoadEAktCommand.Execute(eaktPfad);
            // Im Moment gibt es noch keine Kommentare
            Assert.IsTrue(mainVm.DocumentsVm.AllComments.Count == 0);
        }

        /// <summary>
        /// Hinzufügen eines Kommentars
        /// </summary>
        [TestMethod]
        public void AddComment1()
        {
            string eaktPfad = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPfad = TestHelpers.UpdateEAktFile(eaktPfad);
            string eaktName = Path.GetFileNameWithoutExtension(eaktPfad);
            MainViewModel mainVm = new MainViewModel();
            // Kommentardatei löschen falls vorhanden
            TestHelpers.DeleteCommentFile(eaktName);
            // EAktDatei laden
            mainVm.LoadEAktCommand.Execute(eaktPfad);
            // Hinzufügen eines Kommentars
            EAktDocumentViewModel eactDoc = mainVm.DocumentsVm.AddEAktDocument();
            eactDoc.CommentsVm.AddComment("3 K 764-09.pdf", "blaba", "", "Grün");
            Assert.IsTrue(mainVm.DocumentsVm.AllComments.Count == 1);
        }

        /// <summary>
        /// Entfernen eines hinzugefügten Kommentars
        /// </summary>
        [TestMethod]
        public void RemoveComment1()
        {
            string eaktPfad = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPfad = TestHelpers.UpdateEAktFile(eaktPfad);
            string eaktName = Path.GetFileNameWithoutExtension(eaktPfad);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPfad);
            // Kommentardatei löschen falls vorhanden
            TestHelpers.DeleteCommentFile(eaktName);
            // EAktDatei laden
            mainVm.LoadEAktCommand.Execute(eaktPfad);
            // Hinzufügen eines Kommentars
            EAktDocumentViewModel eactDoc = mainVm.DocumentsVm.AddEAktDocument();
            EAktCommentViewModel newComment = eactDoc.CommentsVm.AddComment("3 K 764-09.pdf", "blaba", "", "Grün");
            // Entfernen eines Kommentars
            eactDoc.CommentsVm.RemoveComment(newComment.Id);
            Assert.IsTrue(mainVm.DocumentsVm.AllComments.Count == 0);
        }

        /// <summary>
        /// Speichern von Kommentaren in einer externen Datei
        /// </summary>
        [TestMethod]
        public void SaveComments1()
        {
            string eaktPfad = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPfad = TestHelpers.UpdateEAktFile(eaktPfad);
            string eaktName = Path.GetFileNameWithoutExtension(eaktPfad);
            MainViewModel mainVm = new MainViewModel();
            // Kommentardatei löschen falls vorhanden
            TestHelpers.DeleteCommentFile(eaktName);
            // EAktDatei laden
            mainVm.LoadEAktCommand.Execute(eaktPfad);
            // Hinzufügen eines Kommentars
            EAktDocumentViewModel eactDoc = mainVm.DocumentsVm.AddEAktDocument();
            EAktCommentViewModel newComment = eactDoc.CommentsVm.AddComment("3 K 764-09.pdf", "blaba", "", "Grün");
            // Speichern der Kommentare
            string tmpPath = Path.GetTempFileName();
            mainVm.DocumentsVm.SaveComments(tmpPath);
            Assert.IsTrue(File.Exists(tmpPath));
        }

        [TestMethod]
        public void SaveComments2()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test2.eakt");
            string eaktBasePath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Akte2");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            string eaktName = Path.GetFileNameWithoutExtension(eaktPath);
            // Kommentardatei löschen falls vorhanden
            TestHelpers.DeleteCommentFile(eaktName);
            // EAktDatei laden
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Hinzufügen eines Kommentars
            EAktDocumentViewModel eactDoc = mainVm.DocumentsVm.AddEAktDocument();
            string docPath = Path.Combine(eaktBasePath, "3 K 764 - 09.pdf");
            EAktCommentViewModel newComment = eactDoc.CommentsVm.AddComment(docPath, "blaba", "", "Grün");
            // Speichern der Kommentare
            string tmpPath = Path.GetTempFileName();
            mainVm.DocumentsVm.SaveComments(tmpPath);
            // Anzahl gespeicherte Kommentare zählen
            int commentCount = XDocument.Load(tmpPath).Descendants("EAktKommentar").Count();
            Assert.IsTrue(commentCount == 1);
        }

        /// <summary>
        /// Laden von Kommentaren aus einer zuvor gespeicherten Kommentardatei
        /// </summary>
        [TestMethod]
        public void LoadComments1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            string eaktBasePath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Akte1");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            string eaktName = Path.GetFileNameWithoutExtension(eaktPath);
            // Kommentardatei löschen falls vorhanden
            TestHelpers.DeleteCommentFile(eaktName);
            // EAktDatei laden
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Hinzufügen von drei Kommentaren
            EAktDocumentViewModel eactDoc = mainVm.DocumentsVm.AddEAktDocument();
            string docPath = Path.Combine(eaktBasePath, "3 K 764-09.pdf");
            EAktCommentViewModel newComment = eactDoc.CommentsVm.AddComment(docPath, "Das ist mein erster Kommentar", "", "Grün");
            newComment = eactDoc.CommentsVm.AddComment(docPath, "Ein weiterer Kommentar", "", "Gelb");
            newComment = eactDoc.CommentsVm.AddComment(docPath, "Alles Gute, mir reichts", "", "Magenta");
            // Speichern der Kommentare
            string commentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                GlobalVars.EAktName + ".EAktKommentar");
            mainVm.DocumentsVm.SaveComments(commentPath);
            // Dokument erneut laden
            mainVm.LoadEAktCommand.Execute(eaktPath);
            int commentCount = mainVm.DocumentsVm.AllComments.Count;
            Assert.IsTrue(commentCount == 3);
        }

        /// <summary>
        /// Suche in den Kommentaren mit einem Suchwort
        /// </summary>
        [TestMethod]
        public void SearchComments1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            string eaktBasePath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Akte1");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            string eaktName = Path.GetFileNameWithoutExtension(eaktPath);
            // Kommentardatei löschen falls vorhanden
            TestHelpers.DeleteCommentFile(eaktName);
            // EAktDatei laden
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Hinzufügen von drei Kommentaren
            EAktDocumentViewModel eactDoc = mainVm.DocumentsVm.AddEAktDocument();
            string docPath = Path.Combine(eaktBasePath, "3 K 764-09.pdf");
            EAktCommentViewModel newComment = eactDoc.CommentsVm.AddComment(docPath, "Ein Kommentar zum BGB", "", "Grün");
            newComment = eactDoc.CommentsVm.AddComment(docPath, "Ein Kommentar zum BVB", "", "Gelb");
            newComment = eactDoc.CommentsVm.AddComment(docPath, "Ob BGB oder BVB ist mir total egal", "", "Magenta");
            // Durchsuchen der Kommentare
            CommentSearchRecord record = new CommentSearchRecord { SearchWord = "BVB" };
            List<EAktCommentViewModel> comments = mainVm.DocumentsVm.SearchComments(record);
            Assert.IsTrue(comments.Count == 2);
        }

    }
}
