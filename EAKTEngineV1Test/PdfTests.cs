// ========================================================
// File: PdfTests.cs
// ========================================================

using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DevExpress.Mvvm;

using EAKTEngineV1.ViewModel;
using EAKTEngineV1.Helper;

namespace EAKTEngineV1Test.TestDocuments
{
    [TestClass]
    public class PdfTests
    {
        /// <summary>
        /// Testet das Exportieren einer Mappe als Pdf-Datei
        /// </summary>
        [TestMethod]
        public void PdfMerge1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Alle Pdf-Dokumente in einer Pdf-Datei zusammenfassen
            PdfExportRecord pdfRecord = new PdfExportRecord();
            string pdfExportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            pdfRecord.ExportDirectoryPath = pdfExportPath;
            pdfRecord.Password = "pw";
            PdfFunctions.EAktPdfMerge(mainVm.DocumentsVm, pdfRecord);
            // Testen, ob Datei angelegt wurde
            string pdfExportFilePath = Path.Combine(pdfExportPath, "Test1_Tests_Merge.pdf");
            Assert.IsTrue(File.Exists(pdfExportFilePath));
        }

        /// <summary>
        /// Testet das Exportieren einer Mappe als Pdf-Datei
        /// </summary>
        [TestMethod]
        public void PdfMerge2()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            int pageCountShouldBe = 60;
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Alle Pdf-Dokumente in einer Pdf-Datei zusammenfassen
            PdfExportRecord pdfRecord = new PdfExportRecord();
            string pdfExportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            pdfRecord.ExportDirectoryPath = pdfExportPath;
            pdfRecord.Password = "pw";
            PdfFunctions.EAktPdfMerge(mainVm.DocumentsVm, pdfRecord);
            // Testen, ob alle Seiten exportiert wurden
            string pdfExportFilePath = Path.Combine(pdfExportPath, "Test1_Tests_Merge.pdf");
            int pageCount = PdfFunctions.GetPageCount(pdfExportFilePath, "pw");
            // Stimmt die Seitenzahl mit der Vorgabe überein?
            Assert.IsTrue(pageCount == pageCountShouldBe);
        }

        /// <summary>
        /// Testet das Extrahieren von Text aus einer Pdf-Datei
        /// </summary>
        [TestMethod]
        public void ExtractText1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            int txtCountShouldBe = 6;
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Texte aus Pdf-Dateien extrahieren
            PdfFunctions.ExtractText(mainVm.DocumentsVm);
            // Testen, ob die erwartete Anzahl an Txt-Dateien entstanden ist
            string txtPath = Path.Combine(mainVm.AppInfo.TempPath, "TextExtract");
            int txtCount = (new DirectoryInfo(txtPath)).GetFiles("*.txt").Length;
            Assert.IsTrue(txtCount == txtCountShouldBe);
        }

        /// <summary>
        /// Testet das Exportieren von Bitmaps aus einer Pdf-Datei
        /// </summary>
        [TestMethod]
        public void ExtractImages1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test5.eakt");
            int jpgCountShouldBe = 1;
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Texte aus Pdf-Dateien extrahieren
            PdfFunctions.ExtractImages(mainVm.DocumentsVm);
            // Testen, ob die erwartete Anzahl an Txt-Dateien entstanden ist
            string txtPath = Path.Combine(mainVm.AppInfo.TempPath, "ImageExtract");
            int jpgCount = (new DirectoryInfo(txtPath)).GetFiles("*.jpg").Length;
            Assert.IsTrue(jpgCount == jpgCountShouldBe);
        }

    }
}
