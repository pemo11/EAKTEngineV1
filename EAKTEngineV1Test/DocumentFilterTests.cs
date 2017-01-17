// ========================================================
// File: DocumentFilterTests.cs
// ========================================================

using System;
using System.IO;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EAKTEngineV1.Helper;
using EAKTEngineV1.Model;
using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class DocumentFilterTests
    {
        /// <summary>
        /// Filter mit dem Namen als Kriterium
        /// </summary>
        [TestMethod]
        [TestCategory("DocumentFilter")]
        public void SetDocumentFilter1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            EAktDocumentFilter filter = new EAktDocumentFilter();
            filter.Ersteller = "Dr. Meier";
            mainVm.ApplyFilterCommand.Execute(filter);
            Assert.IsTrue(mainVm.DocumentsVm.Documents.Count == 3);
        }

        /// <summary>
        /// Filter mit Name und Schriftsatz als Kriterium
        /// </summary>
        [TestMethod]
        [TestCategory("DocumentFilter")]
        public void SetDocumentFilter2()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            EAktDocumentFilter filter = new EAktDocumentFilter();
            filter.Ersteller = "Dr. Möller";
            filter.Schriftsatz = "Klage";
            mainVm.ApplyFilterCommand.Execute(filter);
            Assert.IsTrue(mainVm.DocumentsVm.Documents.Count == 1);
        }

        /// <summary>
        /// Filter mit einem Zeitrahmen als Kriterium - funktioniert nicht bei VSOnline?
        /// </summary>
        [TestMethod]
        [TestCategory("DocumentFilter")]
        public void SetDocumentFilter3()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            EAktDocumentFilter filter = new EAktDocumentFilter();
            // Für Ausführung unter VS-Online ?
            if (Thread.CurrentThread.CurrentCulture.Name == "de-DE")
            {
                filter.EingangVon = "15.09.2016";
                filter.EingangBis = "25.09.2016";
            }
            else
            {
                filter.EingangVon = "01/15/2016";
                filter.EingangBis = "09/25/2016";
            }
            mainVm.ApplyFilterCommand.Execute(filter);
            Assert.IsTrue(mainVm.DocumentsVm.Documents.Count == 3);
        }

        /// <summary>
        /// Filter mit einem Zeitrahmen und Datumsangaben, die sowohl bei de-de als auch bei en-us funktioeren
        /// </summary>
        [TestMethod]
        [TestCategory("DocumentFilter")]
        public void SetDocumentFilter4()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            EAktDocumentFilter filter = new EAktDocumentFilter();
            filter.EingangVon = "1.1.2016";
            filter.EingangBis = "1.4.2016";
            mainVm.ApplyFilterCommand.Execute(filter);
            Assert.IsTrue(mainVm.DocumentsVm.Documents.Count == 2);
        }


        /// <summary>
        /// Abspeichern der Dokumente, die das Ergebnis eines Filters sind, in einer EAkt-Datei
        /// </summary>
        [TestMethod]
        [TestCategory("DocumentFilter")]
        public void SaveDocumentFilter()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            string filterPath = Path.Combine(mainVm.AppInfo.TempPath, mainVm.AppInfo.EAktName + "_FilterAuswahl.eakt");
            GlobalVars.FilterPath  = filterPath;
            EAktDocumentFilter filter = new EAktDocumentFilter();
            filter.Ersteller = "Dr. Meier";
            mainVm.ApplyFilterCommand.Execute(filter);
            mainVm.SaveFilterCommand.Execute(null);
            Assert.IsTrue(File.Exists(filterPath));
        }

        /// <summary>
        /// Zurücksetzen eines Dokumentefilters
        /// </summary>
        [TestMethod]
        [TestCategory("DocumentFilter")]
        public void ClearDocumentFilter1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            EAktDocumentFilter filter = new EAktDocumentFilter();
            filter.Ampel = DocumentIndicator.Grün;
            mainVm.ApplyFilterCommand.Execute(filter);
            mainVm.ClearFilterCommand.Execute(null);
            Assert.IsTrue(mainVm.DocumentsVm.Documents.Count == 6);
        }

    }
}
