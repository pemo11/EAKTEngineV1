// ========================================================
// File: EAktKommentar.cs
// ========================================================

using System;

namespace EAKTEngineV1.Model
{
    /// <summary>
    /// Repräsentiert einen Kommentar
    /// </summary>
    public class EAktComment
    {
        public int Id { get; set; }
        public string Author { get; set; }
        public DateTime CreationDate { get; set; }
        public string Comment { get; set; }
        public string Link { get; set; }
        public string Color { get; set; }

        public string DocumentPath { get; set; }
        public int PageNr { get; set; }

    }
}
