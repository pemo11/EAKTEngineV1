// ============================================================
// File: CommentSearchRecord.cs
// ============================================================

using System;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Repräsentiert die Parameter der Kommentarsuche
    /// </summary>
    public class CommentSearchRecord
    {
        /// <summary>
        /// Das Suchwort
        /// </summary>
        public string SearchWord { get; set; }

        /// <summary>
        /// Die Suchfarbe
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Der Zeitpunkt der Erstellung
        /// </summary>
        public DateTime CreationDate { get; set; }
    }
}
