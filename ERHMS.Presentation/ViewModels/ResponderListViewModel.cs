using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderListViewModel : ListViewModel<Responder>
    {
        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }

        public ResponderListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Responders";
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasSingleSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasSingleSelectedItem);
            EmailCommand = new RelayCommand(Email, HasSelectedItem);
            SelectionChanged += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<Responder> GetItems()
        {
            return Context.Responders.SelectUndeleted()
                .OrderBy(responder => responder.FullName)
                .ThenBy(responder => responder.EmailAddress);
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
            Documents.ShowNewResponder();
        }

        public void Edit()
        {
            Documents.ShowResponder((Responder)SelectedItem.Clone());
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
                SelectedItem.Deleted = true;
                Context.Responders.Save(SelectedItem);
                MessengerInstance.Send(new RefreshMessage(typeof(Responder)));
            };
            MessengerInstance.Send(msg);
        }

        public void Email()
        {
            Documents.Show(
                () => new EmailViewModel(Services, TypedSelectedItems),
                document => false);
        }
    }
}
