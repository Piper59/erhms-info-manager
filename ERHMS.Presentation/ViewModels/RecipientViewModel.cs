using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;

namespace ERHMS.Presentation.ViewModels
{
    public class RecipientViewModel : ViewModelBase
    {
        public ICollection<RecipientViewModel> Container { get; private set; }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(() => Active, ref active, value); }
        }

        private bool isResponder;
        public bool IsResponder
        {
            get { return isResponder; }
            set { Set(() => IsResponder, ref isResponder, value); }
        }

        private Responder responder;
        public Responder Responder
        {
            get { return responder; }
            set { Set(() => Responder, ref responder, value); }
        }

        private string emailAddress;
        public string EmailAddress
        {
            get { return emailAddress; }
            set { Set(() => EmailAddress, ref emailAddress, value); }
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand RemoveCommand { get; private set; }

        public RecipientViewModel(ICollection<RecipientViewModel> container)
        {
            Container = container;
            IsResponder = true;
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(IsResponder))
                {
                    if (IsResponder)
                    {
                        EmailAddress = null;
                    }
                    else
                    {
                        Responder = null;
                    }
                }
            };
            AddCommand = new RelayCommand(Add);
            CancelCommand = new RelayCommand(Cancel);
            RemoveCommand = new RelayCommand(Remove);
        }

        public RecipientViewModel(ICollection<RecipientViewModel> container, Responder responder)
            : this(container)
        {
            Responder = responder;
        }

        public string GetEmailAddress()
        {
            if (IsResponder)
            {
                if (Responder == null)
                {
                    return null;
                }
                else
                {
                    return Responder.EmailAddress;
                }
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
                Messenger.Default.Send(new NotifyMessage("Please select a Responder or enter an Email Address."));
                return false;
            }
            else if (!IsResponder && !Email.IsValidAddress(EmailAddress))
            {
                Messenger.Default.Send(new NotifyMessage("Please enter a valid Email Address."));
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
            Container.Add(this);
            Active = false;
        }

        public void Cancel()
        {
            Active = false;
        }

        public void Remove()
        {
            Container.Remove(this);
        }
    }
}
