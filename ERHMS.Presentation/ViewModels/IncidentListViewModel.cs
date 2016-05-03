using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

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
            Selecting += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
            Refresh();
            CreateCommand = new RelayCommand(Create);
            OpenCommand = new RelayCommand(Open, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshListMessage<Incident>>(this, OnRefreshListMessage);
        }

        protected override ICollectionView GetItems()
        {
            return CollectionViewSource.GetDefaultView(DataContext.Incidents
                .SelectByDeleted(false)
                .OrderBy(incident => incident.Name));
        }

        protected override IEnumerable<string> GetFilteredValues(Incident item)
        {
            yield return item.Name;
            yield return item.Description;
            yield return EnumExtensions.ToDescription(item.Phase);
            yield return item.StartDate.HasValue ? item.StartDate.Value.ToShortDateString() : null;
        }

        public void Create()
        {
            Locator.Main.OpenIncidentView(DataContext.Incidents.Create());
        }

        public void Open()
        {
            Locator.Main.OpenIncidentView((Incident)SelectedItem.Clone());
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage(
                "Delete?",
                "Are you sure you want to delete this incident?",
                "Delete",
                "Don't Delete");
            msg.Confirmed += (sender, e) =>
            {
                SelectedItem.Deleted = true;
                DataContext.Incidents.Save(SelectedItem);
                Messenger.Default.Send(new RefreshListMessage<Incident>());
            };
            Messenger.Default.Send(msg);
        }

        private void OnRefreshListMessage(RefreshListMessage<Incident> msg)
        {
            Refresh();
        }
    }
}
