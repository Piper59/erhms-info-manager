using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentListViewModel : ListViewModelBase<Incident>
    {
        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public IncidentListViewModel()
        {
            Title = "Incidents";
            Refresh();
            CreateCommand = new RelayCommand(Create);
            OpenCommand = new RelayCommand(Open, HasOneSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasOneSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            SelectedItemChanged += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
            Messenger.Default.Register<RefreshMessage<Incident>>(this, msg => Refresh());
        }

        protected override IEnumerable<Incident> GetItems()
        {
            return DataContext.Incidents.SelectUndeleted().OrderBy(incident => incident.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(Incident item)
        {
            yield return item.Name;
            yield return item.Description;
            yield return EnumExtensions.ToDescription(item.Phase);
            if (item.StartDate.HasValue)
            {
                yield return item.StartDate.Value.ToShortDateString();
            }
        }

        public void Create()
        {
            Main.OpenIncidentView(DataContext.Incidents.Create());
        }

        public void Open()
        {
            Main.OpenIncidentView((Incident)SelectedItem.Clone());
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected incident?"
            };
            msg.Confirmed += (sender, e) =>
            {
                SelectedItem.Deleted = true;
                DataContext.Incidents.Save(SelectedItem);
                Messenger.Default.Send(new RefreshMessage<Incident>());
            };
            Messenger.Default.Send(msg);
        }
    }
}
