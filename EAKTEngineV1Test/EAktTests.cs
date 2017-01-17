// ========================================================
// File: EAktTest.cs
// ========================================================

using System;
using System.Linq;
using System.Xml.Linq;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using DevExpress.Mvvm;

using EAKTEngineV1.Helper;
using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class EAktTests
    {
        private MainViewModel mainVm;
        private string eaktPath;

        [TestInitialize]
        public void InitEAktFile()
        {
            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
        }

        /// <summary>
        /// Wird EAkt-Datei geladen?
        /// </summary>
        [TestMethod]
        public void LoadEAktFile1()
        {
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            int docCount =  mainVm.DocumentsVm.Documents.Count;
            Assert.IsTrue(docCount == 6);
        }

        /// <summary>
        /// Sind alle geladenen Dokumente gültig?
        /// </summary>
        [TestMethod]
        public void LoadEAktFile2()
        {
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            int docCount = mainVm.DocumentsVm.Documents.Where((d) => d.IsValid).Count();
            Assert.IsTrue(docCount == 6);
        }

        /// <summary>
        /// Laden mehrerer Dokumente nacheinander
        /// </summary>
        [TestMethod]
        public void LoadEAktFile3()
        {
            // 1. Eakt-Mappe laden
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            int docCount = mainVm.DocumentsVm.Documents.Where((d) => d.IsValid).Count();
            
            // 2. Eakt-Mappe laden
            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test2.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            docCount += mainVm.DocumentsVm.Documents.Where((d) => d.IsValid).Count();

            // 3. Eakt-Mappe laden
            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test3.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            docCount += mainVm.DocumentsVm.Documents.Where((d) => d.IsValid).Count();

            // 4. Eakt-Mappe laden
            eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test4.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            docCount += mainVm.DocumentsVm.Documents.Where((d) => d.IsValid).Count();

            Assert.IsTrue(docCount == 13);
        }

        /// <summary>
        /// Feststellen, ob das erste Dokument die Id 1 besitzt
        /// </summary>
        [TestMethod]
        public void TestFirstDocument()
        {
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 1);

        }

        /// <summary>
        /// Konvertieren der Dokumente einer EAKt-Mappe
        /// </summary>
        [TestMethod]
        public void DocumentConvertTest1()
        {
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Wurden alle Word-Dokumente konvertiert?
            int pdfCount = new DirectoryInfo(Path.Combine(mainVm.AppInfo.TempPath, "Test1_Tests")).GetFiles("*_doc.pdf").Length;
            Assert.IsTrue(pdfCount == 1);
        }

        /// <summary>
        /// Anlegen einer neuen EAKt-Mappe
        /// </summary>
        [TestMethod]
        public void NewEAktTest1()
        {
            MainViewModel mainVm = new MainViewModel();
            mainVm.NewEAktCommand.Execute("EAktTest1");
            EAktDocumentViewModel eaktDoc = mainVm.DocumentsVm.AddEAktDocument();
            eaktDoc.Eingang = "1.9.2016";
            eaktDoc.Ersteller = "Dr. Möller";
            eaktDoc.Indicator = EAKTEngineV1.Model.DocumentIndicator.Gelb;
            eaktDoc.Schriftsatz = "Schwere Klage";
            eaktDoc.Path = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Akte1\1_A_10873_10_OVG_Eingangsbestätigung_201007290838051.doc");
            string eaktPfad = Path.Combine(mainVm.AppInfo.TempPath, "EAktTest1");
            if (!Directory.Exists(eaktPfad))
            {
                Directory.CreateDirectory(eaktPfad);
            }
            eaktPfad = Path.Combine(eaktPfad, "EAktTest1.eakt");
            EAktFunctions.SaveEAktDocument(mainVm.DocumentsVm, eaktPfad);
            Assert.IsTrue(File.Exists(eaktPfad));
        }
    }
}
