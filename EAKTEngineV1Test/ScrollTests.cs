// ====================================================================
// File: ScrollTests.cs
// ====================================================================

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EAKTEngineV1.Helper;
using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class ScrollTests
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
        /// Testet das Blättern in der gesamten Mappe
        /// </summary>
        [TestMethod]
        public void ScrollNextPage1()
        {
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            mainVm.DocumentsVm.DocumentScrollMode = ScrollMode.AllDocuments;
            // Blättern in der Mappe und damit zum nächsten Dokument
            if (mainVm.DocumentsVm.CurrentDocument.NextPageCommand.CanExecute(false))
            {
                // Eine Seite blättern, da das erste Dokument 1 Seite umfasst
                mainVm.DocumentsVm.CurrentDocument.NextPageCommand.Execute(false);
            }
            // Ist das zweite Dokument zum aktuellen Dokument geworden?
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 2);
        }

        /// <summary>
        /// Testet das Vorwärtsblättern im selben Dokument
        /// </summary>
        [TestMethod]
        public void ScrollNextPage2()
        {
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            mainVm.DocumentsVm.DocumentScrollMode = ScrollMode.CurrentDocument;
            if (mainVm.DocumentsVm.CurrentDocument.NextPageCommand.CanExecute(false))
            {
                mainVm.DocumentsVm.CurrentDocument.NextPageCommand.Execute(false);
            }
            // Das Dokument sollte sich nicht geändert haben
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 1);
        }

        /// <summary>
        /// Rückwärtsblättern im selben Dokument
        /// </summary>
        [TestMethod]
        public void ScrollPrevPage1()
        {
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Blättern innerhalb des Dokuments
            mainVm.DocumentsVm.DocumentScrollMode = ScrollMode.CurrentDocument;
            if (mainVm.DocumentsVm.CurrentDocument.PrevPageCommand.CanExecute(false))
            {
                mainVm.DocumentsVm.CurrentDocument.PrevPageCommand.Execute(false);
            }
            // Das Dokument sollte sich nicht geändert haben
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 1);
        }

        /// <summary>
        /// Rückwärtsblättern innerhalb der Mappe
        /// </summary>
        [TestMethod]
        public void ScrollPrevPage2()
        {
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Blättern in der gesamten Mappe
            mainVm.DocumentsVm.DocumentScrollMode = ScrollMode.AllDocuments;
            if (mainVm.DocumentsVm.CurrentDocument.PrevPageCommand.CanExecute(false))
            {
                mainVm.DocumentsVm.CurrentDocument.PrevPageCommand.Execute(false);
            }
            // Das letzte Dokument sollte das aktuelle Dokument sein
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 6);
        }
    
        /// <summary>
        /// Blättern auf das nächste Dokument
        /// </summary>
        [TestMethod]
        public void ScrollNextDocument1()
        {
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Blättern im selben Dokument
            mainVm.DocumentsVm.DocumentScrollMode = ScrollMode.CurrentDocument;
            if (mainVm.DocumentsVm.NextDocumentCommand.CanExecute(false))
            {
                mainVm.DocumentsVm.NextDocumentCommand.Execute(false);
            }
            // Das nächste Dokument sollte das aktuelle Dokument sein
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 2);
        }

        /// <summary>
        /// Blättern auf das zurückliegende Dokument
        /// </summary>
        [TestMethod]
        public void ScrollNextDocument2()
        {
            mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Blättern in der gesamten Mappe
            mainVm.DocumentsVm.DocumentScrollMode = ScrollMode.AllDocuments;
            if (mainVm.DocumentsVm.PrevDocumentCommand.CanExecute(false))
            {
                mainVm.DocumentsVm.PrevDocumentCommand.Execute(false);
            }
            // Das letzte Dokument sollte das aktuelle Dokument sein
            Assert.IsTrue(mainVm.DocumentsVm.CurrentDocument.Id == 6);
        }


    }
}
