// ====================================================================
// File: PdfPreviewTests.cs
// ====================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EAKTEngineV1.Helper;
using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class PdfPreviewTests
    {
        private MainViewModel mainVm;

        [TestInitialize()]
        public void Init()
        {
            // MainVm anlegen für Lizenzdatei
            mainVm = new MainViewModel();
        }

        /// <summary>
        /// Anlegen von Vorschauseiten für ein 100-Seiten Dokument
        /// </summary>
        /// <returns></returns>
        [TestMethod, Ignore()]
        public async Task Pdf100PagesPreviewTest1()
        {
            Init();
            string pdfPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Akte12\ebook.pdf");
            GlobalVars.EAktName = "Akte11";
            GlobalVars.TempPath = Path.Combine(Path.GetTempPath(), "EAKTEngineV1");
            await PreviewFunctions.CreatePdfPagePreviewAsync(pdfPath);
            int pageCount = 0;
            // Wurde die erwartete Seitenzahl zurückgemeldet?
            Assert.IsTrue(pageCount == 92);
        }

    /// <summary>
    /// Anlegen von Vorschauseiten für ein 100-Seiten Dokument
    /// </summary>
    /// <returns></returns>
        [TestMethod]
        public async Task Pdf100PagesPreviewTest2()
        {
            Init();
            string pdfPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Akte12\ebook.pdf");
            GlobalVars.EAktName = "Akte12";
            GlobalVars.TempPath = Path.Combine(Path.GetTempPath(), "EAKTEngineV1");
            // Rückgabewert kann nicht direkt abgefragt werden ?
            await PreviewFunctions.CreatePdfPagePreviewAsync(pdfPath);
            // Wurde das Verzeichnis mit der erwarteten Anzahl an Jpg-Dateien angelegt?
            string previewPath = Path.Combine(GlobalVars.TempPath, GlobalVars.EAktName);
            previewPath = Path.Combine(previewPath, "PreviewImagePath");
            Assert.IsTrue((new DirectoryInfo(previewPath).GetFiles("*.jpg").Length == 92));
        }

        /// <summary>
        /// Test dauert fast 3 Minuten
        /// </summary>
        /// <returns></returns>
        [TestMethod, Ignore()]
        public async Task Pdf300PagesPreview1()
        {
            Init();
            string pdfPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Akte11\VFHGDM1.pdf");
            GlobalVars.EAktName = "Akte11";
            GlobalVars.TempPath = Path.Combine(Path.GetTempPath(), "EAKTEngineV1");
            await PreviewFunctions.CreatePdfPagePreviewAsync(pdfPath);
            // Wurde das Verzeichnis mit der erwarteten Anzahl an Jpg-Dateien angelegt?
            string previewPath = Path.Combine(GlobalVars.TempPath, GlobalVars.EAktName);
            previewPath = Path.Combine(previewPath, "PreviewImagePath");
            Assert.IsTrue((new DirectoryInfo(previewPath).GetFiles("*.jpg").Length == 293));
        }

    }
}
