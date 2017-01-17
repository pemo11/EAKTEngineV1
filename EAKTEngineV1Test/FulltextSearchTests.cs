// ========================================================
// File: FulltextSearchTests.cs
// ========================================================

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EAKTEngineV1.Helper;
using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class FulltextSearchTests
    {
        /// <summary>
        /// Suche nach einem einzelnen Begriff
        /// </summary>
        [TestMethod]
        public void TextSearch1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test6.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            string eaktName = Path.GetFileNameWithoutExtension(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            string searchWord = "Mainz";
            List<SearchResultViewModel> results = LuceneFunctions.StartSearch(searchWord);
            // Suchbegriff ist in zwei Dokumenten enthalten
            Assert.IsTrue(results.Count == 2);
        }

        /// <summary>
        /// Suche mit dem AND-Operator
        /// </summary>
        [TestMethod]
        public void TextSearch2()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test6.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            string eaktName = Path.GetFileNameWithoutExtension(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // string searchWord = "\"OVG Rheinland-Pfalz\" AND \"Aldi\"";
            string searchWord = "\"12.08.2003\" AND \"Aldi\"";
            List<SearchResultViewModel> results = LuceneFunctions.StartSearch(searchWord);
            // Suchbegriff darf nur einmal vorkommen
            Assert.IsTrue(results.Count == 1);
        }

        /// <summary>
        /// Suche mit Platzhaltern und einer E-Mail-Adresse
        /// </summary>
        [TestMethod]
        public void TextSearch3()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test6.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            string eaktName = Path.GetFileNameWithoutExtension(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.LoadEAktCommand.Execute(eaktPath);
            string searchWord = "*@mainz-bingen.de*";
            List<SearchResultViewModel> results = LuceneFunctions.StartSearch(searchWord);
            // E-Mail-Adresse muss 1 Mal enthalten sein
            Assert.IsTrue(results.Count == 1);
        }

        /// <summary>
        /// Suche mit dem Zeilenindex
        /// TODO: Test ist noch nicht optimal was die Suchergebnisse angeht
        /// </summary>
        [TestMethod]
        public void TextSearch4()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test6.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            string eaktName = Path.GetFileNameWithoutExtension(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            mainVm.UseLineIndex = true;
            mainVm.LoadEAktCommand.Execute(eaktPath);
            string searchWord = "Mainz";
            List<SearchResultViewModel> results = LuceneFunctions.StartSearch(searchWord);
            // Suchwort muss mehrmals enthalten sein
            Assert.IsTrue(results.Count > 2);

        }
    }
}
