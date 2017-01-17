// ========================================================
// File: PdfExportRecord.cs
// ========================================================

using System;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Repräsentiert die Parameter für den Pdf-Export einer EAkt-Mappe
    /// </summary>
    public class PdfExportRecord
    {
        /// <summary>
        /// Legt fest, ob die Seiten einzeln gespeichert werden
        /// </summary>
        public bool PageMode { get; set; }

        /// <summary>
        /// Das obligatorische Kennwort unverschlüsselt
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gibt die Gesamtzahl der exportierten Seiten an
        /// </summary>
        public int TotalPageCount { get; set; }

        /// <summary>
        /// Optionaler Verzeichnispfad für die exportierte Pdf-Datei
        /// </summary>
        public string ExportDirectoryPath { get; set; }
    }
}
