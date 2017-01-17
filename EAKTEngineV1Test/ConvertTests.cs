// ========================================================
// File: ConvertTests.cs
// ========================================================

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DevExpress.Mvvm;

using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class ConvertTests
    {
        /// <summary>
        /// Konvertieren einer Html-Datei nach Pdf
        /// </summary>
        [TestMethod]
        public void ConvertHtmlDoc()
        {
            string eaktPfad = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test2.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPfad = TestHelpers.UpdateEAktFile(eaktPfad);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPfad);
            // Wurd das Html-Dokument konvertiert?
            int pdfCount = new DirectoryInfo(Path.Combine(mainVm.AppInfo.TempPath, "Test2_Tests")).GetFiles("*.pdf").Length;
            Assert.IsTrue(pdfCount == 1);
        }

        /// <summary>
        /// Konvertieren einer Bitmap-Datei nach Pdf
        /// </summary>
        [TestMethod]
        public void ConvertBitmapDoc()
        {
            string eaktPfad = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test3.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPfad = TestHelpers.UpdateEAktFile(eaktPfad);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPfad);
            // Wurden alle Bitmaps in Pdf-Dateien konvertiert?
            int pdfCount = new DirectoryInfo(Path.Combine(mainVm.AppInfo.TempPath, "Test3_Tests")).GetFiles("*.pdf").Length;
            Assert.IsTrue(pdfCount == 4);
        }

        /// <summary>
        /// Konvertieren einer Txt- und Rtf-Datei nach Pdf
        /// </summary>
        [TestMethod]
        public void ConvertTextDoc()
        {
            string eaktPfad = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test4.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPfad = TestHelpers.UpdateEAktFile(eaktPfad);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPfad);
            // Wurden alle Bitmaps in Pdf-Dateien konvertiert?
            int pdfCount = new DirectoryInfo(Path.Combine(mainVm.AppInfo.TempPath, "Test4_Tests")).GetFiles("*.pdf").Length;
            // Aktuell sind es vier Seiten, da eine FirstPage-Datei angelegt wird bei Dateien mit
            // mehr als einer Seite
            Assert.IsTrue(pdfCount == 4);
        }

    }


}
