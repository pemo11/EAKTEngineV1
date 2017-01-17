// ============================================================
// File: PdfBookmark.cs
// ============================================================

using System;

namespace EAKTEngineV1.Model
{
    /// <summary>
    /// Repräsentiert eine Lesemarke
    /// </summary>
    public class PdfBookmark
    {   
        /// <summary>
        /// Die Seitennummer der Lesemarke
        /// </summary>
        public int PageNr { get; set; }

        /// <summary>
        /// Der Titel der Lesemarke
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Die Aktion, die über die Lesemarke ausgeführt wird (in der Regel Aufruf der Seite)
        /// </summary>
        public string Action { get; set; }
    }
}
