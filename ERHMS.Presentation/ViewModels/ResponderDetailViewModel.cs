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
    public class ResponderDetailViewModel : ViewModelBase
    {
        public Responder Responder { get; private set; }
        public ICollection<string> Prefixes { get; private set; }
        public ICollection<string> Suffixes { get; private set; }
        public ICollection<string> Genders { get; private set; }
        public ICollection<string> States { get; private set; }

        private RelayCommand saveCommand;
        public ICommand SaveCommand
        {
            get { return saveCommand ?? (saveCommand = new RelayCommand(Save)); }
        }

        public ResponderDetailViewModel(IServiceManager services, Responder responder)
            : base(services)
        {
            Title = responder.New ? "New Responder" : responder.FullName;
            Responder = responder;
            AddDirtyCheck(responder);
            Prefixes = Context.Prefixes.ToList();
            Suffixes = Context.Suffixes.ToList();
            Genders = Context.Genders.ToList();
            States = Context.States.ToList();
        }

        private bool Validate()
        {
            ICollection<string> fields = new List<string>();
            if (string.IsNullOrWhiteSpace(Responder.FirstName))
            {
                fields.Add("First Name");
            }
            if (string.IsNullOrWhiteSpace(Responder.LastName))
            {
                fields.Add("Last Name");
            }
            if (string.IsNullOrWhiteSpace(Responder.EmailAddress))
            {
                fields.Add("Email Address");
            }
            if (fields.Count > 0)
            {
                ShowValidationMessage(ValidationError.Required, fields);
                return false;
            }
            if (Responder.BirthDate.HasValue && Responder.BirthDate.Value.Date > DateTime.Today)
            {
                fields.Add("Birth Date");
            }
            if (!MailExtensions.IsValidAddress(Responder.EmailAddress))
            {
                fields.Add("Email Address");
            }
            if (!string.IsNullOrWhiteSpace(Responder.ContactEmailAddress) && !MailExtensions.IsValidAddress(Responder.ContactEmailAddress))
            {
                fields.Add("Emergency Contact Email Address");
            }
            if (!string.IsNullOrWhiteSpace(Responder.OrganizationEmailAddress) && !MailExtensions.IsValidAddress(Responder.OrganizationEmailAddress))
            {
                fields.Add("Organization Email Address");
            }
            if (fields.Count > 0)
            {
                ShowValidationMessage(ValidationError.Invalid, fields);
                return false;
            }
            return true;
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            Context.Responders.Save(Responder);
            MessengerInstance.Send(new ToastMessage
            {
                Message = "Responder has been saved."
            });
            MessengerInstance.Send(new RefreshMessage(typeof(Responder)));
            Title = Responder.FullName;
            Dirty = false;
        }
    }
}
