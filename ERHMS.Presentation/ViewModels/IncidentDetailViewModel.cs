using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Mantin.Controls.Wpf.Notification;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentDetailViewModel : ViewModelBase
    {
        public Incident Incident { get; private set; }
        public ICollection<Phase> Phases { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        public IncidentDetailViewModel(Incident incident)
        {
            Title = incident.Name;
            Incident = incident;
            Phases = EnumExtensions.GetValues<Phase>().ToList();
            SaveCommand = new RelayCommand(Save);
        }

        public void Save()
        {
            // TODO: Validate fields
            DataContext.Incidents.Save(Incident);
            Messenger.Default.Send(new ToastMessage(NotificationType.Information, "Incident has been saved."));
            Messenger.Default.Send(new RefreshListMessage<Incident>());
        }
    }
}
