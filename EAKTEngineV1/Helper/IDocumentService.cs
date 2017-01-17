// ============================================================
// File: IDocumentService.cs
// ============================================================

using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using EAKTEngineV1.ViewModel;

namespace EAKTEngineV1.Helper
{
    /// <summary>
    /// Schnittstelle für Klassen, die EAktDocumentViewModel-Objekte zur Verfügung stellen
    /// </summary>
    public interface IDocumentService
    {
        /// <summary>
        /// Einlesen der Dokumente über einen Pfad oder einer URI
        /// </summary>
        /// <param name="EAktPath"></param>
        /// <returns></returns>
        ObservableCollection<EAktDocumentViewModel> GetDocuments(string EAktPath);
    }
}
