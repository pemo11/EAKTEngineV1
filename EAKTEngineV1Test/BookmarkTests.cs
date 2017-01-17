// ============================================================
// File: BookmarkTests.cs
// ============================================================

using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DevExpress.Mvvm;

using EAKTEngineV1.ViewModel;


namespace EAKTEngineV1Test
{
    [TestClass]
    public class BookmarkTests
    {
        private MainViewModel mainVm;
        private string eaktPath;

        /// <summary>
        /// Anzahl der Lesemarken abfragen
        /// </summary>
        [TestMethod]
        public void BookmarksTest1()
        {
            // EAkt-Mappe mit Pdf-Dateien mit Lesemarken öffnen
            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test12.eakt");

            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);

            // Stimmt die Anzahl der Lesemarken?
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.BookmarksVm.Bookmarks.Count == 41);
        }
    }
}
