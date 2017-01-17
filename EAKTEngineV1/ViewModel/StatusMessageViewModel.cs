// ========================================================
// File: StatusMessageViewModel.cs
// ========================================================

using System;
using DevExpress.Mvvm;

namespace EAKTEngineV1.ViewModel
{
    /// <summary>
    /// Das ViewModel für eine Statusmeldung
    /// </summary>
    public class StatusMessageViewModel : ViewModelBase
    {
        /// <summary>
        /// Die interne Id der Meldung - wird fortlaufend nummeriert
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Zeitpunkt, an dem die Meldung angelegt wurde
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Die Meldung selber
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///  Gibt an, ob die Meldung eine Fehlermeldung ist
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// Gibt alle Details zur Message zurück
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Message {0:000} um {1:HH:mm:ss}: {2}", this.Id, this.TimeStamp, this.Message);
        }

    }
}
