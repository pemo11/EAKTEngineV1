// ============================================================
// File: DirectoryReadTests.cs
// ============================================================

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DevExpress.Mvvm;

using EAKTEngineV1.ViewModel;
using EAKTEngineV1.Helper;

namespace EAKTEngineV1Test
{
    [TestClass]
    public class DirectoryReadTests
    {
        /// <summary>
        /// Testet, ob eine EAkt-Datei angelegt wird
        /// </summary>
        /// <returns></returns
        /// >
        [TestMethod]
        public async Task ReadDirectory1()
        {
            string directoryPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Akte10");
            MainViewModel mainVm = new MainViewModel();
            // An Temp-Verzeichnis anpassen
            // directoryPath = Path.GetFullPath(directoryPath).TrimEnd(Path.DirectorySeparatorChar);
            // string eaktName = directoryPath.Split(Path.DirectorySeparatorChar).Last();
            // Oder ohne LINQ-Erweiterugsmethoden
            string eaktName = new DirectoryInfo(directoryPath).Name;
            string eaktPath = Path.Combine(mainVm.AppInfo.TempPath, eaktName);
            if (!Directory.Exists(eaktPath))
            {
                Directory.CreateDirectory(eaktPath);
            }
            eaktPath = Path.Combine(eaktPath, eaktName + ".eakt");
            await mainVm.ReadDirectoryAsync(directoryPath, eaktPath);
            // Testen, ob EAkt-Datei angelegt wurde
            Assert.IsTrue(File.Exists(eaktPath));
        }

        /// <summary>
        /// Testet, ob die geschriebene EAkt-Datei die richtige Anzahl an Document-Elementen enthält
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ReadDirectory2()
        {
            string directoryPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments\Akte10");
            MainViewModel mainVm = new MainViewModel();
            // An Temp-Verzeichnis anpassen
            string eaktName = new DirectoryInfo(directoryPath).Name;
            string eaktPath = Path.Combine(mainVm.AppInfo.TempPath, eaktName);
            if (!Directory.Exists(eaktPath))
            {
                Directory.CreateDirectory(eaktPath);
            }
            eaktPath = Path.Combine(eaktPath, eaktName + ".eakt");
            await mainVm.ReadDirectoryAsync(directoryPath, eaktPath);
            XName xnDoc = XName.Get("Dokument", "urn:eakte");
            XDocument xDoc = XDocument.Load(eaktPath);
            int docCount = xDoc.Descendants(xnDoc).Count();
            // EAkt-Datei sollte 6 Dokumente enthalten
            Assert.IsTrue(docCount == 6);
        }

    }
}
