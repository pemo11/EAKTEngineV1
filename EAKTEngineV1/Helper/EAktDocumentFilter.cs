// ============================================================
// EAktDocumentFilter.cs
// ============================================================

using System;
using EAKTEngineV1.Model;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Repräsentiert einen Filter für die Dokumente der EAkt-Mappe
    /// </summary>
    public class EAktDocumentFilter
    {
        /// <summary>
        /// Filter nach dem Ersteller
        /// </summary>
        public string Ersteller { get; set; }

        /// <summary>
        /// Filtern nach der Schriftsatz-Eigenschaft
        /// </summary>
        public string Schriftsatz { get; set; }

        /// <summary>
        /// Filtern nach dem Eingang Von-Datum
        /// </summary>
        public string EingangVon { get; set; }

        /// <summary>
        /// Filtern nach dem Eingang Bis-Datum
        /// </summary>
        public string EingangBis { get; set; }

        /// <summary>
        /// Filtern nach der Dokument-Ampel
        /// </summary>
        public DocumentIndicator? Ampel { get; set; }

    }
}
