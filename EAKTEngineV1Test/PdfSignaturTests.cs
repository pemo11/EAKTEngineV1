// ====================================================================
// File: PdfSignaturTests.cs
// ====================================================================

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EAKTEngineV1.Helper;
using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test.TestDocuments
{
    [TestClass]
    public class PdfSignaturTests
    {
        /// <summary>
        /// Testet, ob Signatur erkannt wird
        /// </summary>
        [TestMethod]
        public void SignaturTest1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test7.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
                MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            string pdfPath = mainVm.DocumentsVm.Documents[0].PdfPath;
            bool hasSignature = PdfFunctions.CheckSignature(pdfPath);
            Assert.IsTrue(hasSignature);
        }

        /// <summary>
        /// Testet das Entfernen einer Signatur aus einem Dokument
        /// </summary>
        [TestMethod]
        public void RemoveSignatur1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test7.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            EAktDocumentViewModel eaktDoc = mainVm.DocumentsVm.Documents[0];
            bool sucess1 = PdfFunctions.RemoveSignature(eaktDoc);
            // Prüfen, ob die Signatur entfernt wurde
            bool sucess2 = !PdfFunctions.CheckSignature(eaktDoc.PdfPath);
            Assert.IsTrue(sucess1 && sucess2);
        }

    }
}
