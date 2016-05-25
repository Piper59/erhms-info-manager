using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Converters;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderDetailViewModel : ViewModelBase
    {
        private static readonly ResponderToNameConverter ResponderToNameConverter = new ResponderToNameConverter();

        public Responder Responder { get; private set; }
        public ICollection<string> Prefixes { get; private set; }
        public ICollection<string> Suffixes { get; private set; }
        public ICollection<string> Genders { get; private set; }
        public ICollection<string> States { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        public ResponderDetailViewModel(Responder responder)
        {
            Responder = responder;
            UpdateTitle();
            Prefixes = GetCodes(DataContext.Prefixes);
            Suffixes = GetCodes(DataContext.Suffixes);
            Genders = GetCodes(DataContext.Genders);
            States = GetCodes(DataContext.States);
            SaveCommand = new RelayCommand(Save);
        }

        private void UpdateTitle()
        {
            Title = Responder.New ? "New Responder" : ResponderToNameConverter.Convert(Responder);
        }

        private ICollection<string> GetCodes(CodeRepository codes)
        {
            return codes.Select()
                .Prepend("")
                .ToList();
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
                NotifyRequired(fields);
                return false;
            }
            else
            {
                if (!Email.IsValidAddress(Responder.EmailAddress))
                {
                    fields.Add("Email Address");
                }
                if (!string.IsNullOrWhiteSpace(Responder.ContactEmailAddress) && !Email.IsValidAddress(Responder.ContactEmailAddress))
                {
                    fields.Add("Emergency Contact Email Address");
                }
                if (!string.IsNullOrWhiteSpace(Responder.OrganizationEmailAddress) && !Email.IsValidAddress(Responder.OrganizationEmailAddress))
                {
                    fields.Add("Organization Email Address");
                }
                if (fields.Count > 0)
                {
                    NotifyInvalid(fields);
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void Save()
        {
            if (!Validate())
            {
                return;
            }
            DataContext.Responders.Save(Responder);
            Messenger.Default.Send(new ToastMessage("Responder has been saved."));
            Messenger.Default.Send(new RefreshListMessage<Responder>());
            UpdateTitle();
        }
    }
}
