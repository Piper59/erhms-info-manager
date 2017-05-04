using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

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
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasOneSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasOneSelectedItem);
            EmailCommand = new RelayCommand(Email, HasAnySelectedItems);
            RefreshCommand = new RelayCommand(Refresh);
            SelectedItemChanged += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            };
            Messenger.Default.Register<RefreshMessage<Responder>>(this, msg => Refresh());
        }

        protected override IEnumerable<Responder> GetItems()
        {
            return DataContext.Responders.SelectUndeleted()
                .OrderBy(item => item.LastName)
                .ThenBy(item => item.FirstName);
        }

        protected override IEnumerable<string> GetFilteredValues(Responder item)
        {
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
            Main.OpenResponderDetailView(DataContext.Responders.Create());
        }

        public void Edit()
        {
            Main.OpenResponderDetailView((Responder)SelectedItem.Clone());
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected responder?"
            };
            msg.Confirmed += (sender, e) =>
            {
                DataContext.Responders.Delete(SelectedItem);
                Messenger.Default.Send(new RefreshMessage<Responder>());
            };
            Messenger.Default.Send(msg);
        }

        public void Email()
        {
            Main.OpenEmailView(new EmailViewModel(TypedSelectedItems));
        }
    }
}
