// ========================================================
// File: ErrorTests.cs
// ========================================================

using System;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using EAKTEngineV1.Helper;
using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{ 

    /// <summary>
    /// Tests mit fehlerhaften Dokumenten
    /// </summary>
    [TestClass]
    public class ErrorTests
    {

        /// <summary>
        /// Dokument mit Signatur laden
        /// </summary>
          [TestMethod]
        public void LoadDocumentsWithSignature1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test8.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Gibt es gültige Dokumente
            int docCount = mainVm.DocumentsVm.Documents.Where((d) => d.IsValid).Count();
            // Es sollte drei gültige Dokumente geben
            Assert.IsTrue(docCount == 3);
        }

        /// <summary>
        /// Mappe mit ungültigem Dokument laden
        /// </summary>
        [TestMethod]
        public void LoadInvalidDocuments1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test9.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Gibt es gültige Dokumente?
            int docCount = mainVm.DocumentsVm.Documents.Where((d) => d.IsValid).Count();
            // Es sollte nur ein gültiges Dokument geben
            Assert.IsTrue(docCount == 1);
        }

    }
}
