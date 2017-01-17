// ========================================================
// File: EAktCommentsViewModel.cs
// ========================================================

using System;
using System.Collections.ObjectModel;
using System.Linq;

using DevExpress.Mvvm;

namespace EAKTEngineV1.ViewModel
{
    /// <summary>
    /// Das ViewModel für alle Kommentare eines EAkt-Dokuments
    /// </summary>
    public class EAktCommentsViewModel : ViewModelBase
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private ObservableCollection<EAktCommentViewModel> comments;

        /// <summary>
        /// Konstruktor ohne Parameter
        /// </summary>
        public EAktCommentsViewModel()
        {
            comments = new ObservableCollection<EAktCommentViewModel>();
        }

        /// <summary>
        /// Stelle alle Kommentare als EAktCommentViewModel-Objekte zur Verfügung
        /// </summary>
        public ObservableCollection<EAktCommentViewModel> Comments
        {
            get { return comments; }
        }

        /// <summary>
        /// Hinzufügen eines Kommentars
        /// </summary>
        /// <param name="Comment"></param>
        /// <param name="Link"></param>
        /// <param name="Color"></param>
        public EAktCommentViewModel AddComment(string DocumentPath, string Comment, string Link, string Color)
        {
            // ViewModel mit den Eckdaten des Kommentars anlegen
            EAktCommentViewModel newComment = new EAktCommentViewModel
            {
                Id = comments.Count + 1,
                DocumentPath = DocumentPath,
                Comment = Comment,
                Link = Link,
                Color = Color
            };
            // Kommentar zur Liste der Kommentare hinzufügen
            comments.Add(newComment);
            RaisePropertiesChanged("Comments");
            return newComment;
        }

        /// <summary>
        /// Entfernen eines Kommentars
        /// </summary>
        /// <param name="Id"></param>
        public void RemoveComment(int Id)
        {
            // ViewModel für den Kommentar aus der Liste der Kommentare holen
            EAktCommentViewModel comment = (from c in comments where c.Id == Id select c).SingleOrDefault();
            // Gab es einen Kommentar?
            if (comment != null)
            {
                comments.Remove(comment);
            }
            RaisePropertiesChanged("Comments");
        }
    }
}
