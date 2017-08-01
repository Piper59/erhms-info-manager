using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class RecipientViewModel : DialogViewModel
    {
        private bool isResponder;
        public bool IsResponder
        {
            get
            {
                return isResponder;
            }
            set
            {
                if (Set(nameof(IsResponder), ref isResponder, value))
                {
                    if (value)
                    {
                        EmailAddress = null;
                    }
                    else
                    {
                        Responder = null;
                    }
                }
            }
        }

        public ICollection<Responder> Responders { get; private set; }

        private Responder responder;
        public Responder Responder
        {
            get { return responder; }
            set { Set(nameof(Responder), ref responder, value); }
        }

        private string emailAddress;
        public string EmailAddress
        {
            get { return emailAddress; }
            set { Set(nameof(EmailAddress), ref emailAddress, value); }
        }

        private RelayCommand addCommand;
        public ICommand AddCommand
        {
            get { return addCommand ?? (addCommand = new RelayCommand(Add)); }
        }

        public RecipientViewModel(IServiceManager services, bool editable)
            : base(services)
        {
            Title = "Add a Recipient";
            IsResponder = true;
            if (editable)
            {
                Responders = Context.Responders.SelectUndeleted().ToList();
            }
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

        public string GetEmailAddress()
        {
            return IsResponder ? Responder?.EmailAddress : EmailAddress;
        }

        private bool Validate()
        {
            if (Responder == null && string.IsNullOrWhiteSpace(EmailAddress))
            {
                MessengerInstance.Send(new AlertMessage
                {
                    Message = "Please select a responder or enter an email address."
                });
                return false;
            }
            if (!IsResponder && !MailExtensions.IsValidAddress(EmailAddress))
            {
                MessengerInstance.Send(new AlertMessage
                {
                    Message = "Please enter a valid email address."
                });
                return false;
            }
            return true;
        }

        public void Add()
        {
            if (!Validate())
            {
                return;
            }
            OnAdded();
            Close();
        }

        public override string ToString()
        {
            return IsResponder ? Responder?.FullName : EmailAddress;
        }
    }
}
