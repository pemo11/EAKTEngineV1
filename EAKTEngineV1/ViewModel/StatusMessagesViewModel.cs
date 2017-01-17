// ========================================================
// File: StatusMessagesViewModel.cs
// ========================================================

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using DevExpress.Mvvm;

namespace EAKTEngineV1.ViewModel
{
    /// <summary>
    /// Das ViewModel für alle Statusmeldungen
    /// </summary>
    public class StatusMessagesViewModel : ViewModelBase
    {
        /// <summary>
        /// Private Variablen
        /// </summary>
        private ObservableCollection<StatusMessageViewModel> messages;

        /// <summary>
        /// Konstruktor ohne Parameter
        /// </summary>
        public StatusMessagesViewModel()
        {
            messages = new ObservableCollection<ViewModel.StatusMessageViewModel>();
        }

        /// <summary>
        /// Alle Nachrichten als MessageViewModel-Objekte
        /// </summary>
        public ObservableCollection<StatusMessageViewModel> Messages
        {
            get { return messages; }
        }

        /// <summary>
        /// Hinfügen einer weiteren Nachricht
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="IsError"></param>
        public void AddMessage(string Message, bool IsError)
        {
            messages.Add(new StatusMessageViewModel
            {
                Message = Message,
                IsError = IsError,
                Id = messages.Count + 1,
                TimeStamp = DateTime.Now
            });
            RaisePropertiesChanged("Messages");
        }

        /// <summary>
        /// Entfernt alle Nachrichten
        /// </summary>
        public void Clear()
        {
            messages.Clear();
        }

    }
}
