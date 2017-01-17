// ========================================================
// File: DocumentConvertRecord.cs
// ========================================================

using System;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Fasst die Daten einer Konvertierung zusammen
    /// </summary>
    public class DocumentConvertRecord
    {
        /// <summary>
        /// Anzahl der konvertierten Dokumente
        /// </summary>
        public int DocumentConvertedCount { get; set; }
        /// <summary>
        /// Anzahl der aufgetretenen Fehler
        /// </summary>
        public int ConvertErrorCount { get; set; }
        /// <summary>
        /// Anzahl der konvertierten Pdf-Dokumene
        /// </summary>
        public int PdfDocumentCount { get; set; }
    }
}
