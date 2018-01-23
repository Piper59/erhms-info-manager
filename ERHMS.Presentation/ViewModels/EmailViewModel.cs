using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Settings = ERHMS.Utility.Settings;
using View = ERHMS.Domain.View;

namespace ERHMS.Presentation.ViewModels
{
    public class EmailViewModel : DocumentViewModel
    {
        public class ViewListChildViewModel : ListViewModel<View>
        {
            public ViewListChildViewModel(IServiceManager services)
                : base(services)
            {
                Refresh();
            }

            protected override IEnumerable<View> GetItems()
            {
                return Context.Views.SelectUndeleted()
                    .Where(view => ViewExtensions.IsWebSurvey(view.WebSurveyId))
                    .OrderBy(view => view.Name, StringComparer.OrdinalIgnoreCase);
            }
        }

        public class RecipientChildViewModel
        {
            public Responder Responder { get; set; }

            private string emailAddress;
            public string EmailAddress
            {
                get { return emailAddress ?? Responder?.EmailAddress; }
                set { emailAddress = value; }
            }

            public RecipientChildViewModel(Responder responder)
            {
                Responder = responder;
            }

            public RecipientChildViewModel(RecipientViewModel model)
            {
                Responder = model.Responders.SelectedItem;
                EmailAddress = model.EmailAddress;
            }

            public override string ToString()
            {
                return Responder?.FullName ?? EmailAddress;
            }
        }

        public ObservableCollection<RecipientChildViewModel> Recipients { get; private set; }

        private string subject;
        [DirtyCheck]
        public string Subject
        {
            get { return subject; }
            set { SetProperty(nameof(Subject), ref subject, value); }
        }

        private string body;
        [DirtyCheck]
        public string Body
        {
            get { return body; }
            set { SetProperty(nameof(Body), ref body, value); }
        }

        public ObservableCollection<string> Attachments { get; private set; }

        private bool appendUrl;
        public bool AppendUrl
        {
            get
            {
                return appendUrl;
            }
            set
            {
                SetProperty(nameof(AppendUrl), ref appendUrl, value);
                if (!value)
                {
                    Views.Unselect();
                }
            }
        }

        public ViewListChildViewModel Views { get; private set; }

        public ICommand AddRecipientCommand { get; private set; }
        public ICommand RemoveRecipientCommand { get; private set; }
        public ICommand AddAttachmentCommand { get; private set; }
        public ICommand RemoveAttachmentCommand { get; private set; }
        public ICommand SendCommand { get; private set; }

        public EmailViewModel(IServiceManager services, IEnumerable<Responder> responders)
            : base(services)
        {
            Title = "Email";
            Recipients = new ObservableCollection<RecipientChildViewModel>();
            foreach (Responder responder in responders.OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase))
            {
                Recipients.Add(new RecipientChildViewModel(responder));
            }
            Attachments = new ObservableCollection<string>();
            Views = new ViewListChildViewModel(services);
            AddRecipientCommand = new AsyncCommand(AddRecipientAsync);
            RemoveRecipientCommand = new Command<RecipientChildViewModel>(RemoveRecipient);
            AddAttachmentCommand = new Command(AddAttachment);
            RemoveAttachmentCommand = new Command<string>(RemoveAttachment);
            SendCommand = new AsyncCommand(SendAsync);
        }

        public async Task AddRecipientAsync()
        {
            using (RecipientViewModel model = new RecipientViewModel(Services))
            {
                model.Added += (sender, e) =>
                {
                    Recipients.Add(new RecipientChildViewModel(model));
                };
                await Services.Dialog.ShowAsync(model);
            }
        }

        public void RemoveRecipient(RecipientChildViewModel recipient)
        {
            Recipients.Remove(recipient);
        }

        public void AddAttachment()
        {
            foreach (string path in Services.Dialog.OpenFiles("Attach a File"))
            {
                Attachments.Add(path);
            }
        }

        public void RemoveAttachment(string path)
        {
            Attachments.Remove(path);
        }

        private async Task<bool> ValidateAsync()
        {
            ICollection<string> fields = new List<string>();
            if (Recipients.Count == 0)
            {
                fields.Add("Recipients");
            }
            if (string.IsNullOrWhiteSpace(Subject))
            {
                fields.Add("Subject");
            }
            if (string.IsNullOrWhiteSpace(Body))
            {
                fields.Add("Body");
            }
            if (fields.Count > 0)
            {
                await Services.Dialog.AlertAsync(ValidationError.Required, fields);
                return false;
            }
            if (AppendUrl && !Views.HasSelectedItem())
            {
                await Services.Dialog.AlertAsync("Please select a form to append a web survey URL.");
                return false;
            }
            return true;
        }

        public async Task SendAsync()
        {
            if (!await ValidateAsync())
            {
                return;
            }
            if (!Settings.Default.IsEmailConfigured())
            {
                await Services.Dialog.AlertAsync("Please configure email settings.");
                Services.Document.ShowByType(() => new SettingsViewModel(Services));
                return;
            }
            ICollection<RecipientChildViewModel> failures = new List<RecipientChildViewModel>();
            try
            {
                await Services.Dialog.BlockAsync("Sending email \u2026", () =>
                {
                    MailMessage message = Settings.Default.GetMailMessage();
                    message.Subject = Subject;
                    foreach (string path in Attachments)
                    {
                        message.Attachments.Add(new Attachment(path));
                    }
                    StringBuilder body = new StringBuilder();
                    body.Append(Body.Trim());
                    if (AppendUrl)
                    {
                        body.AppendLine();
                        body.AppendLine();
                        body.AppendFormat(ViewExtensions.GetWebSurveyUrl(Views.SelectedItem.WebSurveyId).ToString());
                    }
                    message.Body = body.ToString();
                    using (SmtpClient client = Settings.Default.GetSmtpClient(ConfigurationExtensions.DecryptSafe))
                    {
                        foreach (RecipientChildViewModel recipient in Recipients)
                        {
                            try
                            {
                                message.To.Clear();
                                message.To.Add(new MailAddress(recipient.EmailAddress));
                                client.Send(message);
                            }
                            catch (Exception ex)
                            {
                                Log.Logger.Warn("Failed to send email", ex);
                                failures.Add(recipient);
                            }
                        }
                    }
                });
                if (failures.Count > 0)
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine("Delivery to the following recipients failed:");
                    message.AppendLine();
                    foreach (RecipientChildViewModel failure in failures)
                    {
                        message.AppendLine(failure.ToString());
                    }
                    await Services.Dialog.AlertAsync(message.ToString().Trim());
                }
                else
                {
                    Services.Dialog.Notify("Email has been sent.");
                    Dirty = false;
                    await CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to send email", ex);
                await Services.Dialog.AlertAsync("Failed to send email. Please verify email settings.", ex);
                Services.Document.ShowByType(() => new SettingsViewModel(Services));
            }
        }

        public override void Dispose()
        {
            Views.Dispose();
            base.Dispose();
        }
    }
}
