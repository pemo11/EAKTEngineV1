// ============================================================
// File: LRUTests.cs
// ============================================================

using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DevExpress.Mvvm;

using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class LRUTests
    {
        private MainViewModel mainVm;
        private string eaktPath;



        /// <summary>
        /// Testet das Laden eines Dokuments und die LRU-Liste
        /// </summary>
        [TestMethod]
        public void LRUTestLoadDocument()
        {
            // 1. Dokument öffnen
            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");

            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);

            // 2. Testen, ob es in der LRU-Liste erscheint
            Assert.IsTrue(mainVm.LRUDocuments.Documents.Count == 1);
        }

        /// <summary>
        /// Testet das Entfernen eines Dokuments aus der LRU-Liste
        /// </summary>
        [TestMethod]
        public void LRUTestRemoveDocument()
        {
            // 1. Dokument öffnen
            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");

            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);

            // Dokument aus LRU-Liste enfernen
            mainVm.LRUDocuments.RemoveDocument();

            // 2. Testen, ob es in der LRU-Liste erscheint
            Assert.IsTrue(mainVm.LRUDocuments.Documents.Count == 0);
        }

        /// <summary>
        /// Testet die Auswirkung zu vieler Dokumente auf die LRU-Liste
        /// </summary>
        [TestMethod]
        public void LRUTestLoadManyDocuments()
        {
            string eaktPath2;

            // 1. Mehrere Dokumente öffnen - Limit wurde auf 3 gesetzt
            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");

            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);

            eaktPath2 = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test2.eakt");

            // Pfade in EAkt-Datei anpassen
            eaktPath2 = TestHelpers.UpdateEAktFile(eaktPath2);
            mainVm.LoadEAktCommand.Execute(eaktPath2);

            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test3.eakt");

            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            mainVm.LoadEAktCommand.Execute(eaktPath);

            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test4.eakt");

            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            mainVm.LoadEAktCommand.Execute(eaktPath);

            // Testen, ob das erste Element aus der Liste entfernt wurde
            Assert.IsTrue(mainVm.LRUDocuments.Documents[0] == eaktPath2);
        }

        /// <summary>
        /// Testet das Löschen der LRU-Liste
        /// </summary>
        [TestMethod]
        public void LRUClearList()
        {
            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");

            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);

            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test2.eakt");

            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            mainVm.LoadEAktCommand.Execute(eaktPath);

            mainVm.LRUDocuments.ClearDocuments();

            Assert.IsTrue(mainVm.LRUDocuments.Documents.Count == 0);
        }
    }
}
