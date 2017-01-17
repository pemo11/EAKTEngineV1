// ========================================================
// File: EAktCommentViewModel.cs
// ========================================================
using System;

using DevExpress.Mvvm;
using EAKTEngineV1.Model;

namespace EAKTEngineV1.ViewModel
{
    /// <summary>
    /// Das ViewModel für einen einzelnen Kommentar
    /// </summary>
    public class EAktCommentViewModel : ViewModelBase
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private EAktComment comment;
        private double xPos { get; set; }
        private double yPos { get; set; }
        private double winHeight { get; set; }
        private double winWidth { get; set; }

        /// <summary>
        /// Konstruktor ohne Parameter
        /// </summary>
        public EAktCommentViewModel()
        {
            comment = new EAktComment();
        }

        /// <summary>
        /// Die Id des Kommentars
        /// </summary>
        public int Id
        {
            get { return comment.Id; }
            set {
                comment.Id = value;
                RaisePropertyChanged("Id");
            }
        }

        /// <summary>
        /// Der Autor des Kommentars
        /// </summary>
        public string Author
        {
            get { return comment.Author; }
            set
            {
                comment.Author = value;
                RaisePropertyChanged("Author");
            }
        }

        /// <summary>
        /// Zeitpunkt an dem der Kommentar angelegt wurde
        /// </summary>
        public DateTime CreationDate
        {
            get { return comment.CreationDate; }
            set
            {
                comment.CreationDate = value;
                RaisePropertyChanged("CreationDate");
            }
        }

        /// <summary>
        /// Der Kommentartext
        /// </summary>
        public string Comment
        {
            get { return comment.Comment; }
            set
            {
                comment.Comment = value;
                RaisePropertyChanged("Comment");
            }
        }

        /// <summary>
        /// Die Verknüpfung des Kommentars
        /// </summary>
        public string Link
        {
            get { return comment.Link; }
            set
            {
                comment.Link = value;
                RaisePropertyChanged("Link");
            }
        }

        /// <summary>
        /// Die Farbe des Kommentarfensters
        /// </summary>
        public string Color
        {
            get { return comment.Color; }
            set
            {
                comment.Color = value;
                RaisePropertyChanged("Color");
            }
        }

        /// <summary>
        /// Der Pfad des Dokuments zu dem der Kommentar gehört
        /// </summary>
        public string DocumentPath
        {
            get { return comment.DocumentPath; }
            set
            {
                comment.DocumentPath = value;
                RaisePropertyChanged("DocumentPath");
            }
        }

        /// <summary>
        /// Die Seitennummer, zu der der Kommentar gehört
        /// </summary>
        public int PageNr
        {
            get { return comment.PageNr; }
            set
            {
                comment.PageNr = value;
                RaisePropertyChanged("PageNr");
            }
        }

        /// <summary>
        /// X-Position des Kommentarfensters
        /// </summary>
        public double XPos
        {
            get { return xPos; }
            set
            {
                xPos = value;
                RaisePropertyChanged("XPos");
            }
        }

        /// <summary>
        /// Y-Position des Kommentarfensters
        /// </summary>
        public double YPos
        {
            get { return yPos; }
            set
            {
                yPos = value;
                RaisePropertyChanged("YPos");
            }
        }

        /// <summary>
        /// Höhe des Kommentarfensters
        /// </summary>
        public double WinHeight
        {
            get { return winHeight; }
            set
            {
                winHeight = value;
                RaisePropertyChanged("WinHeight");
            }
        }

        /// <summary>
        /// Breite des Kommentarfensters
        /// </summary>
        public double WinWidth
        {
            get { return winWidth; }
            set
            {
                winWidth = value;
                RaisePropertyChanged("WinWidth");
            }
        }
    }
}
