// ========================================================
// SearchResultViewModel.cs
// ========================================================

using System;

using DevExpress.Mvvm;

namespace EAKTEngineV1.ViewModel
{
    /// <summary>
    /// Das ViewModel für das Ergebnis der Volltextsuche
    /// </summary>
    public class SearchResultViewModel : ViewModelBase
    {
        /// <summary>
        /// Interne ID des Treffers
        /// </summary>
        public int HitId { get; set; }

        /// <summary>
        /// Pfad des Dokuments, in dem der Treffer auftrat
        /// </summary>
        public string DocumentPath { get; set; }

        /// <summary>
        /// Name des Dokuments, in dem der Treffer auftrat
        /// </summary>
        public string DocumentName { get; set; }

        /// <summary>
        /// Nr des Dokuments, in dem der Treffer auftrat
        /// </summary>
        public int DocumentId { get; set; }

        /// <summary>
        /// Inhalt des Schriftart-Feldes
        /// </summary>
        public string Schriftsatz { get; set; }

        /// <summary>
        /// Der gefundene Text
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// Interne Bewertung des Treffers
        /// </summary>
        public double ResultScore { get; set; }

        /// <summary>
        /// Die gefundene Textstelle im HTML-Format mit Hervorhebung
        /// </summary>
        public string ResultHTML { get; set; }

    }
}
