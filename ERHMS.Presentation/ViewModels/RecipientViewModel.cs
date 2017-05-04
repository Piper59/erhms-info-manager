using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class RecipientViewModel : ViewModelBase
    {
        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(nameof(Active), ref active, value); }
        }

        private ICollection<Responder> responders;
        public ICollection<Responder> Responders
        {
            get { return responders; }
            set { Set(nameof(Responders), ref responders, value); }
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

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public RecipientViewModel()
        {
            Responders = DataContext.Responders.SelectUndeleted()
                .OrderBy(responder => responder.FullName)
                .ToList();
            IsResponder = true;
            AddCommand = new RelayCommand(Add);
            CancelCommand = new RelayCommand(Cancel);
        }

        public RecipientViewModel(Responder responder)
        {
            IsResponder = true;
            Responder = responder;
        }

        public event EventHandler Adding;
        private void OnAdding(EventArgs e)
        {
            Adding?.Invoke(this, e);
        }
        private void OnAdding()
        {
            OnAdding(EventArgs.Empty);
        }

        public string GetEmailAddress()
        {
            if (IsResponder)
            {
                return Responder?.EmailAddress;
            }
            else
            {
                return EmailAddress;
            }
        }

        private bool Validate()
        {
            if (Responder == null && string.IsNullOrWhiteSpace(EmailAddress))
            {
                Messenger.Default.Send(new AlertMessage
                {
                    Message = "Please select a responder or enter an email address."
                });
                return false;
            }
            else if (!IsResponder && !MailExtensions.IsValidAddress(EmailAddress))
            {
                Messenger.Default.Send(new AlertMessage
                {
                    Message = "Please enter a valid email address."
                });
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Add()
        {
            if (!Validate())
            {
                return;
            }
            OnAdding();
            Active = false;
        }

        public void Cancel()
        {
            Active = false;
        }

        public override string ToString()
        {
            if (IsResponder)
            {
                return Responder?.FullName;
            }
            else
            {
                return EmailAddress;
            }
        }
    }
}
