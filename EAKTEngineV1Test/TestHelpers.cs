// ====================================================================
// File: TestHelpers.cs
// ====================================================================
using System;
using System.IO;
using System.Xml.Linq;

using EAKTEngineV1.Helper;

namespace EAKTEngineV1Test
{
    /// <summary>
    /// Verschiedene Hilfsfunktionen für Tests
    /// </summary>
    public static class TestHelpers
    {
        /// <summary>
        /// Anlegen eines Test-Verzeichnisses
        /// </summary>
        public static void CreateTestDirectory()
        {
            string testPath = Path.Combine(Path.GetTempPath(), GlobalVars.AppName);
            if (!Directory.Exists(testPath))
            {
                Directory.CreateDirectory(testPath);
            }
        }

        /// <summary>
        /// EAkt-Datei in Temp-Verzeichnis kopieren und Pfade anpassen
        /// </summary>
        /// <param name="EAktPath"></param>
        /// <returns></returns>
        public static string UpdateEAktFile(string EAktPath)
        {
            CreateTestDirectory();
            XName xnDok = XName.Get("Dokument", "urn:eakte");
            XDocument xEAkt = XDocument.Load(EAktPath);
            string eaktName = Path.GetFileNameWithoutExtension(EAktPath);
            foreach (XElement xDoc in xEAkt.Descendants(xnDok))
            {
                string newPath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestDocuments", xDoc.Attribute("Pfad").Value);
                xDoc.Attribute("Pfad").Value = newPath;
            }
            EAktPath = Path.Combine(Path.GetTempPath(), "EAKTEngineV1");
            EAktPath = Path.Combine(EAktPath, eaktName + "_Tests.eakt");
            xEAkt.Save(EAktPath);
            return EAktPath;
        }

        /// <summary>
        /// Kommmentardatei löschen
        /// </summary>
        /// <param name="EAktName"></param>
        public static void DeleteCommentFile(string EAktName)
        {
            // Kommentardatei löschen falls vorhanden
            string commentPfad = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                EAktName + ".EAktKommentar");
            if (File.Exists(commentPfad))
            {
                File.Delete(commentPfad);
            }
        }
    }

}
