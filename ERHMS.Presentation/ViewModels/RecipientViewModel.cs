using ERHMS.Domain;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class RecipientViewModel : DialogViewModel
    {
        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public ResponderListChildViewModel()
            {
                Refresh();
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.SelectUndeleted().OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase);
            }
        }

        private bool isResponder;
        public bool IsResponder
        {
            get
            {
                return isResponder;
            }
            set
            {
                SetProperty(nameof(IsResponder), ref isResponder, value);
                if (value)
                {
                    EmailAddress = null;
                }
                else
                {
                    Responders.Unselect();
                }
            }
        }

        public ResponderListChildViewModel Responders { get; private set; }

        private string emailAddress;
        public string EmailAddress
        {
            get { return emailAddress; }
            set { SetProperty(nameof(EmailAddress), ref emailAddress, value); }
        }

        public ICommand AddCommand { get; private set; }

        public RecipientViewModel()
        {
            Title = "Add a Recipient";
            IsResponder = true;
            Responders = new ResponderListChildViewModel();
            AddCommand = new AsyncCommand(AddAsync);
        }

        public event EventHandler Added;
        private void OnAdded(EventArgs e)
        {
            Added?.Invoke(this, e);
        }
        private void OnAdded()
        {
            OnAdded(EventArgs.Empty);
        }

        private async Task<bool> ValidateAsync()
        {
            if (!Responders.HasSelectedItem() && string.IsNullOrWhiteSpace(EmailAddress))
            {
                await ServiceLocator.Dialog.AlertAsync(Resources.EmailRecipientNotSpecified);
                return false;
            }
            if (!IsResponder && !MailExtensions.IsValidAddress(EmailAddress))
            {
                await ServiceLocator.Dialog.AlertAsync(Resources.EmailRecipientAddressInvalid);
                return false;
            }
            return true;
        }

        public async Task AddAsync()
        {
            if (!await ValidateAsync())
            {
                return;
            }
            OnAdded();
            Close();
        }
    }
}
