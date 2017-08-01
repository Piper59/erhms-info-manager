using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderListViewModel : ListViewModel<Responder>
    {
        private RelayCommand createCommand;
        public ICommand CreateCommand
        {
            get { return createCommand ?? (createCommand = new RelayCommand(Create)); }
        }

        private RelayCommand editCommand;
        public ICommand EditCommand
        {
            get { return editCommand ?? (editCommand = new RelayCommand(Edit, HasSingleSelectedItem)); }
        }

        private RelayCommand deleteCommand;
        public ICommand DeleteCommand
        {
            get { return deleteCommand ?? (deleteCommand = new RelayCommand(Delete, HasSingleSelectedItem)); }
        }

        private RelayCommand emailCommand;
        public ICommand EmailCommand
        {
            get { return emailCommand ?? (emailCommand = new RelayCommand(Email, HasSelectedItem)); }
        }

        public ResponderListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Responders";
            SelectionChanged += (sender, e) =>
            {
                editCommand.RaiseCanExecuteChanged();
                deleteCommand.RaiseCanExecuteChanged();
                emailCommand.RaiseCanExecuteChanged();
            };
            Refresh();
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
