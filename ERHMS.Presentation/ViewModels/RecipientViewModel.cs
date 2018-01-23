using ERHMS.Domain;
using ERHMS.Presentation.Commands;
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
            public ResponderListChildViewModel(IServiceManager services)
                : base(services)
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

        public RecipientViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Add a Recipient";
            IsResponder = true;
            Responders = new ResponderListChildViewModel(services);
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
                await Services.Dialog.AlertAsync("Please select a responder or enter an email address.");
                return false;
            }
            if (!IsResponder && !MailExtensions.IsValidAddress(EmailAddress))
            {
                await Services.Dialog.AlertAsync("Please enter a valid email address.");
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

        public override void Dispose()
        {
            Responders.Dispose();
            base.Dispose();
        }
    }
}
