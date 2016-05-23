using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderListViewModel : ListViewModelBase<Responder>
    {
        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public ResponderListViewModel()
        {
            Title = "Responders";
            Refresh();
            Selecting += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            };
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSelectedItem);
            EmailCommand = new RelayCommand(Email, HasSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshListMessage<Responder>>(this, OnRefreshResponderListMessage);
            Messenger.Default.Register<RefreshDataMessage>(this, OnRefreshDataMessage);
        }

        protected override ICollectionView GetItems()
        {
            return CollectionViewSource.GetDefaultView(DataContext.Responders.SelectByDeleted(false)
                .OrderBy(responder => responder.LastName)
                .ThenBy(responder => responder.FirstName));
        }

        protected override IEnumerable<string> GetFilteredValues(Responder item)
        {
            yield return item.ResponderId;
            yield return item.LastName;
            yield return item.FirstName;
            yield return item.EmailAddress;
            yield return item.City;
            yield return item.State;
            yield return item.OrganizationName;
            yield return item.Occupation;
        }

        public void Create()
        {
            Locator.Main.OpenResponderDetailView(DataContext.Responders.Create());
        }

        public void Edit()
        {
            Locator.Main.OpenResponderDetailView((Responder)SelectedItem.Clone());
        }

        public void Delete()
        {
            Responder responder = SelectedItem;
            SelectedItems.Clear();
            SelectedItem = responder;
            ConfirmMessage msg = new ConfirmMessage("Delete", "Delete the selected responder?");
            msg.Confirmed += (sender, e) =>
            {
                DataContext.Responders.Delete(SelectedItem);
                Messenger.Default.Send(new RefreshListMessage<Responder>());
            };
            Messenger.Default.Send(msg);
        }

        public void Email()
        {
            Locator.Main.OpenEmailView(new EmailViewModel(SelectedItems.Cast<Responder>()));
        }

        private void OnRefreshResponderListMessage(RefreshListMessage<Responder> msg)
        {
            Refresh();
        }

        private void OnRefreshDataMessage(RefreshDataMessage msg)
        {
            if (msg.ProjectPath.EqualsIgnoreCase(DataContext.Project.FilePath) && msg.ViewName.EqualsIgnoreCase(DataContext.Responders.View.Name))
            {
                Refresh();
            }
        }
    }
}
