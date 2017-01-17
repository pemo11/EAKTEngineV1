// ========================================================
// File: FirstPageTests.cs
// ========================================================

using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class FirstPageTests
    {
        /// <summary>
        /// Testet, ob für Dokumente mit mehr als einer Seite ein Erste-Seite-Dokument angelegt wird
        /// </summary>
        [TestMethod]
        public void FirstPageCreatedTest()
        {
            string eaktPfad = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPfad = TestHelpers.UpdateEAktFile(eaktPfad);
            MainViewModel mainVm = new MainViewModel();
            mainVm.CreatePreviewPages = false;
            mainVm.UseLineIndex = false;
            mainVm.LoadEAktCommand.Execute(eaktPfad);
            // Wurden ErsteSeite-Pdf-Dateien für alle Dokumente mit mehr als einer Seite angelegt?
            int docCount = new DirectoryInfo(Path.Combine(mainVm.AppInfo.TempPath, "Test1_Tests")).GetFiles("*FirstPage.pdf").Length;
            Assert.IsTrue(docCount == 3);
        }
    }
}
