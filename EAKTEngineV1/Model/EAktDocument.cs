// ========================================================
// File: EAktDokument.cs
// ========================================================

using System;

namespace EAKTEngineV1.Model
{
    /// <summary>
    /// Repräsentiert alle Dokumenttypen
    /// </summary>
    public enum DocumentType
    {
        Word,
        Pdf,
        Text,
        Rtf,
        Bitmap,
        Tiff,
        Html,
        Unknown
    }

    /// <summary>
    /// Repräsentiert die Einstellungen der Dokumentampel
    /// </summary>
    public enum DocumentIndicator
    {
        Grün,
        Gelb,
        Rot
    }

    /// <summary>
    /// Repräsentiert ein EAkt-Dokument
    /// </summary>
    public class EAktDocument
    {
        /// <summary>
        /// Die Id des Dokuments - wird fortlaufend vergeben
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Der Pfad der Dokumentdatei
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Der Pfad der Pdf-Datei
        /// </summary>
        public string PdfPath { get; set; }

        /// <summary>
        /// Der Name des Dokuments (Dateiname)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Der Name der Gruppe (Verzeichnisname)
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Der Dokumenttyp in Abhängigkeit der Erweiterung
        /// </summary>
        public DocumentType Type { get; set; }

        /// <summary>
        /// Die Dokumentampel
        /// </summary>
        public DocumentIndicator Indicator { get; set; }

        /// <summary>
        /// Gibt an, ob das Dokument gültig ist
        /// </summary>
        public bool IsValid { get; set; }


        /// <summary>
        /// Gibt an, ob das Dokument vorhanden ist
        /// </summary>
        public bool IsPresent { get; set; }

        /// <summary>
        /// Anzahl der Seiten
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Meta-Attribut Schriftssatz
        /// </summary>
        public string Schriftsatz { get; set; }

        /// <summary>
        /// Meta-Attribut Ersteller
        /// </summary>
        public string Ersteller { get; set; }

        /// <summary>
        /// Meta-Attribut Eingangsdatum
        /// </summary>
        public string Eingang { get; set; }

    }
}
