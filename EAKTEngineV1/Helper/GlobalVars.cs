// ========================================================
// File: GlobalVars.cs
// ========================================================

using System;
using System.IO;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Definiert globale Variablen
    /// </summary>
    public static class GlobalVars
    {
        public static string AppName;
        public static string AppVersion;
        public static string EAktName;
        public static string EAktPath;
        public static string TempPath;
        public static string IndexPath;
        public static string FilterPath;
        public static string LastUpdate;
        public static string CommentsPath;
        public static string[] AllowedExtensionsList = { ".doc", ".docx", ".pdf", ".html", ".htm", ".txt", ".tiff", ".tif", ".rtf" };
        public static string[] SchriftsatzArten = { "Antrag", "Berufung", "Beschluss", "Bestätigung", "Drucksache", "Klage", "Niederschrift", "Schriftsatz", "Stammdaten", "Übersendung", "Urteil", "Verfügung", "Vollmacht", "Zuleitung" };

        static GlobalVars()
        {
            TempPath = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.TempPath.TrimEnd());
            TempPath = Path.Combine(TempPath, "EAKTEngineV1");
            AppName = "EAKTEngineV1";
            AppVersion = "0.45";
            LastUpdate = "01.11.2016";
        }

    }
}
