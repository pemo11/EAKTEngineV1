// ========================================================
// File: PreviewPagesTests.cs
// ========================================================

using System;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using EAKTEngineV1.Helper;
using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class PreviewPagesTests
    {
        [TestInitialize()]
        public void Init()
        {
            // Im Moment noch leer
        }

        /// <summary>
        /// Testet das Anlegen von Vorschauseiten mit einer Pdf-Datei
        /// </summary>
        [TestMethod]
        public void CreatePreviewPages1()
        {
            string eaktPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Test1.eakt");
            // Pfade in EAkt-Datei anpassen
            eaktPath = TestHelpers.UpdateEAktFile(eaktPath);
            MainViewModel mainVm = new MainViewModel();
            // Es sollen Vorschauseiten angelegt werden
            mainVm.CreatePreviewPages = true;
            mainVm.LoadEAktCommand.Execute(eaktPath);
            // Anzahl der Vorschaudateien zählen
            string previewPath = Path.Combine(Path.Combine(Path.GetDirectoryName(eaktPath), Path.GetFileNameWithoutExtension(eaktPath)), "PreviewPages");
            bool DirExists = Directory.Exists(previewPath);
            int pageCount = 0;
            if (DirExists)
            {
                pageCount = new DirectoryInfo(previewPath).GetFiles("*.jpg").Length;
            }
            Assert.IsTrue(DirExists && pageCount == 60);

        }
    }
}
