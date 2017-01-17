// ========================================================
// File: DocumentNavigationTests.cs
// ========================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using EAKTEngineV1.Helper;
using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class DocumentNavigationTests
    {
        /// <summary>
        /// Zum nächsten Dokument in einer Mappe navigieren
        /// </summary>
        [TestMethod]
        public void Move2NextDocument()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            // EAkt-Datei mit 6 Dokumenten laden
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Auf das nächste Dokument wechseln
            if (mainVm.DocumentsVm.NextDocumentCommand.CanExecute(false))
            {
                mainVm.DocumentsVm.NextDocumentCommand.Execute(false);
            }
            // Aktuelles Dokument muss DokumentNr = 2 besitzen
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 2);
        }

        /// <summary>
        /// Zum vorherigen Dokument in einer Mappe navigieren
        /// </summary>
        [TestMethod]
        public void Move2PrevDocument1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            // EAkt-Datei mit 6 Dokumenten laden
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Auf das nächste Dokument wechseln
            if (mainVm.DocumentsVm.NextDocumentCommand.CanExecute(false))
            {
                mainVm.DocumentsVm.NextDocumentCommand.Execute(false);
            }
            // Auf das vorherige Dokument wechseln
            if (mainVm.DocumentsVm.PrevDocumentCommand.CanExecute(false))
            {
                mainVm.DocumentsVm.PrevDocumentCommand.Execute(false);
            }
            // Aktuelles Dokument muss wieder DokumentNr = 1 besitzen
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 1);
        }

        /// <summary>
        /// Zum vorherigen Dokument in einer Mappe navigieren
        /// </summary>
        [TestMethod]
        public void Move2PrevDocument2()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            // EAkt-Datei mit 6 Dokumenten laden
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Auf das letzte Dokument wechseln
            if (mainVm.DocumentsVm.PrevDocumentCommand.CanExecute(false))
            {
                mainVm.DocumentsVm.PrevDocumentCommand.Execute(false);
            }
            // Aktuelles Dokument muss DokumentNr = 6 besitzen
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 6);
        }

        /// <summary>
        /// An das Ende einer Mappe navigieren
        /// </summary>
        [TestMethod]
        public void Move2EndOfDocuments()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            // EAkt-Datei mit 6 Dokumenten laden
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Auf das nächste Dokument wechseln
            if (mainVm.DocumentsVm.NextDocumentCommand.CanExecute(false))
            {
                mainVm.DocumentsVm.NextDocumentCommand.Execute(false);
                mainVm.DocumentsVm.NextDocumentCommand.Execute(false);
                mainVm.DocumentsVm.NextDocumentCommand.Execute(false);
                mainVm.DocumentsVm.NextDocumentCommand.Execute(false);
                mainVm.DocumentsVm.NextDocumentCommand.Execute(false);
                mainVm.DocumentsVm.NextDocumentCommand.Execute(false);
            }
            // Aktuelles Dokument muss wieder DokumentNr = 1 besitzen
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 1);
        }

        /// <summary>
        /// Zu einem bestimmten Dokument navigieren
        /// </summary>
        [TestMethod]
        public void GotoDocument()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            // EAkt-Datei mit 6 Dokumenten laden
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Auf ein Dokument wechseln
            if (mainVm.DocumentsVm.GotoDocumentCommand.CanExecute(0))
            {
                mainVm.DocumentsVm.GotoDocumentCommand.Execute(4);
            }
            // Aktuelles Dokument muss DokumentNr = 4 besitzen
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 4);
        }

    }
}
