// ============================================================
// File: LRUDocumentsViewModel.cs
// ============================================================

using System;
using System.Linq;
using System.Collections.Generic;

namespace EAKTEngineV1.ViewModel
{
    /// <summary>
    /// Repräsentiert die zuletzt geöffneten EAkt-Dateien
    /// </summary>
    public class LRUDocumentsViewModel
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private int documentMaxCount;
        private Queue<string> documents;

        /// <summary>
        /// Konstruktor ohne Parameter
        /// </summary>
        public LRUDocumentsViewModel(int MaxCount)
        {
            // Maximale Anzahl an Einträgen festlegen
            documentMaxCount = MaxCount;
            documents = new Queue<string>();
        }

        /// <summary>
        /// Hinzufügen eines Dokuments
        /// </summary>
        /// <param name="Path"></param>
        public void AddDocument(string Path)
        {
            // Ist Pfad nicht bereits in der Liste enthalten?
            if (!documents.Contains(Path))
            {
                if (documents.Count == documentMaxCount)
                {
                    documents.Dequeue();
                }
                documents.Enqueue(Path);
            }
        }

        /// <summary>
        /// Entfernen des zuerst hinzugefügten Dokuments
        /// </summary>
        public void RemoveDocument()
        {
            // Gibt es einen Eintrag?
            if (documents.Count > 0)
            {
                documents.Dequeue();
            }
        }

        /// <summary>
        /// Alle Dokumente aus der Liste entfernen
        /// </summary>
        public void ClearDocuments()
        {
            documents.Clear();
        }

        /// <summary>
        /// Stelle alle Dokumente als Liste zur Verfügung
        /// </summary>
        public List<string> Documents
        {
            get { return documents.ToList(); }
        }
    }
}
