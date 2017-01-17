// ========================================================
// File: DocumentTests.cs
// ========================================================

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DevExpress.Mvvm;

using EAKTEngineV1.ViewModel;
using EAKTEngineV1.Helper;

namespace EAKTEngineV1Test
{
    /// <summary>
    /// Tests für den Umgang mit EAkt-Dateien
    /// </summary>
    [TestClass]
    public class DocumentTests
    {

        /// <summary>
        /// Anlegen einer EAkt-Datei
        /// </summary>
        [TestMethod]
        public void NewEActDocument()
        {
            string EAktName = "TestDokument123";
            MainViewModel mainVm = new MainViewModel();
            if (mainVm.NewEAktCommand.CanExecute(""))
            {
                mainVm.NewEAktCommand.Execute(EAktName);
            }
            Assert.IsTrue(GlobalVars.EAktName == EAktName);
        }

        /// <summary>
        /// Dokument zu einer Mappe  hinzufügen
        /// </summary>
        [TestMethod]
        public void AddDocument()
        {
            MainViewModel mainVm = new MainViewModel();
            if (mainVm.NewEAktCommand.CanExecute(""))
            {
                mainVm.NewEAktCommand.Execute("TestDokument123");
            }
            EAktDocumentViewModel eaktDoc = mainVm.DocumentsVm.AddEAktDocument();
            Assert.IsNotNull(eaktDoc);
        }

        /// <summary>
        /// Dokument aus einer Mappe entfernen
        /// </summary>
        [TestMethod]
        public void RemoveDocument()
        {
            MainViewModel mainVm = new MainViewModel();
            if (mainVm.NewEAktCommand.CanExecute(""))
            {
                mainVm.NewEAktCommand.Execute("TestDokument123");
            }
            EAktDocumentViewModel eaktDoc = mainVm.DocumentsVm.AddEAktDocument();
            mainVm.DocumentsVm.RemoveEAktDocument(eaktDoc.Id);
            Assert.IsTrue(mainVm.DocumentsVm.Documents.Count == 0);
        }

    }
}
