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
    public class ResponderViewModel : DocumentViewModel
    {
        public Responder Responder { get; private set; }
        public ICollection<string> Prefixes { get; private set; }
        public ICollection<string> Suffixes { get; private set; }
        public ICollection<string> Genders { get; private set; }
        public ICollection<string> States { get; private set; }
        public ResponderReportViewModel Report { get; private set; }

        public ICommand SaveCommand { get; private set; }

        public ResponderViewModel(IServiceManager services, Responder responder)
            : base(services)
        {
            Title = responder.New ? "New Responder" : responder.FullName;
            Responder = responder;
            AddDirtyCheck(responder);
            Prefixes = Context.Prefixes.ToList();
            Suffixes = Context.Suffixes.ToList();
            Genders = Context.Genders.ToList();
            States = Context.States.ToList();
            Report = new ResponderReportViewModel(services, responder);
            SaveCommand = new AsyncCommand(SaveAsync);
        }

        private async Task<bool> ValidateAsync()
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
                await Services.Dialog.AlertAsync(ValidationError.Required, fields);
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
                await Services.Dialog.AlertAsync(ValidationError.Invalid, fields);
                return false;
            }
            return true;
        }

        public async Task SaveAsync()
        {
            if (!await ValidateAsync())
            {
                return;
            }
            Context.Responders.Save(Responder);
            Services.Dialog.Notify("Responder has been saved.");
            Services.Data.Refresh(typeof(Responder));
            Title = Responder.FullName;
            Dirty = false;
        }

        public override void Dispose()
        {
            Report.Dispose();
            base.Dispose();
        }
    }
}
