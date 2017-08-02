using Epi.Collections;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using Settings = ERHMS.Utility.Settings;
using View = ERHMS.Domain.View;

namespace ERHMS.Presentation.ViewModels
{
    public class EmailViewModel : ViewModelBase
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
                    .OrderBy(view => view.Name);
            }

            public void Select(int viewId)
            {
                SelectedItem = TypedItems.SingleOrDefault(view => view.ViewId == viewId);
            }
        }

        public ObservableCollection<RecipientViewModel> Recipients { get; private set; }

        private string subject;
        [DirtyCheck]
        public string Subject
        {
            get { return subject; }
            set { Set(nameof(Subject), ref subject, value); }
        }

        private string body;
        [DirtyCheck]
        public string Body
        {
            get { return body; }
            set { Set(nameof(Body), ref body, value); }
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
                if (Set(nameof(AppendUrl), ref appendUrl, value))
                {
                    if (!value)
                    {
                        Views.SelectedItem = null;
                        CanPrepopulate = false;
                    }
                }
            }
        }

        private bool canAppendUrl;
        public bool CanAppendUrl
        {
            get
            {
                return canAppendUrl;
            }
            private set
            {
                if (Set(nameof(CanAppendUrl), ref canAppendUrl, value))
                {
                    if (!value)
                    {
                        AppendUrl = false;
                    }
                }
            }
        }

        public ViewListChildViewModel Views { get; private set; }

        private bool prepopulate;
        public bool Prepopulate
        {
            get { return prepopulate; }
            set { Set(nameof(Prepopulate), ref prepopulate, value); }
        }

        private bool canPrepopulate;
        public bool CanPrepopulate
        {
            get
            {
                return canPrepopulate;
            }
            private set
            {
                if (Set(nameof(CanPrepopulate), ref canPrepopulate, value))
                {
                    if (!value)
                    {
                        Prepopulate = false;
                    }
                }
            }
        }

        public RelayCommand AddRecipientCommand { get; private set; }
        public RelayCommand<RecipientViewModel> RemoveRecipientCommand { get; private set; }
        public RelayCommand AddAttachmentCommand { get; private set; }
        public RelayCommand<string> RemoveAttachmentCommand { get; private set; }
        public RelayCommand SendCommand { get; private set; }

        public EmailViewModel(IServiceManager services, IEnumerable<Responder> recipients)
            : base(services)
        {
            Title = "Email";
            Recipients = new ObservableCollection<RecipientViewModel>();
            foreach (Responder recipient in recipients)
            {
                Recipients.Add(new RecipientViewModel(services, false)
                {
                    Responder = recipient
                });
            }
            Attachments = new ObservableCollection<string>();
            Views = new ViewListChildViewModel(services);
            SetCanAppendUrl();
            SetCanPrepopulate();
            AddRecipientCommand = new RelayCommand(AddRecipient);
            RemoveRecipientCommand = new RelayCommand<RecipientViewModel>(RemoveRecipient);
            AddAttachmentCommand = new RelayCommand(AddAttachment);
            RemoveAttachmentCommand = new RelayCommand<string>(RemoveAttachment);
            SendCommand = new RelayCommand(Send);
            Views.SelectionChanged += (sender, e) =>
            {
                SetCanAppendUrl();
                SetCanPrepopulate();
            };
        }

        private void SetCanAppendUrl()
        {
            CanAppendUrl = !Views.Items.IsEmpty;
        }

        private void SetCanPrepopulate()
        {
            CanPrepopulate = AppendUrl && Views.SelectedItem != null && Views.SelectedItem.HasResponderIdField;
        }

        public void AddRecipient()
        {
            RecipientViewModel recipient = new RecipientViewModel(Services, true);
            recipient.Added += (sender, e) =>
            {
                Recipients.Add(recipient);
            };
            Dialogs.ShowAsync(recipient);
        }

        public void RemoveRecipient(RecipientViewModel recipient)
        {
            Recipients.Remove(recipient);
        }

        public void AddAttachment()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Attach a File";
                dialog.Multiselect = true;
                if (dialog.ShowDialog(Dialogs.Win32Window) == DialogResult.OK)
                {
                    foreach (string path in dialog.FileNames)
                    {
                        Attachments.Add(path);
                    }
                }
            }
        }

        public void RemoveAttachment(string path)
        {
            Attachments.Remove(path);
        }

        private bool Validate()
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
                ShowValidationMessage(ValidationError.Required, fields);
                return false;
            }
            if (AppendUrl && Views.SelectedItem == null)
            {
                MessengerInstance.Send(new AlertMessage
                {
                    Message = "Please select a form to append a web survey URL."
                });
                return false;
            }
            return true;
        }

        private Record GetRecord(Responder responder, FieldCollectionMaster fields)
        {
            Record record = new Record();
            record[fields["ResponderID"].Name] = responder.ResponderId;
            return record;
        }

        public void Send()
        {
            if (!Validate())
            {
                return;
            }
            if (!Settings.Default.IsEmailConfigured())
            {
                Documents.ShowSettings("Please configure email settings.");
                return;
            }
            ConfigurationError error = ConfigurationError.None;
            if (Prepopulate && !Service.IsConfigured(out error, true))
            {
                Documents.ShowSettings(SurveyViewModel.GetErrorMessage(error));
                return;
            }
            Survey survey = null;
            ICollection<RecipientViewModel> failures = new List<RecipientViewModel>();
            Exception exception = null;
            bool success = false;
            BlockMessage msg = new BlockMessage
            {
                Message = "Sending email \u2026"
            };
            msg.Executing += (sender, e) =>
            {
                FieldCollectionMaster fields = null;
                if (Prepopulate)
                {
                    if (!Service.IsConfigured(out error))
                    {
                        return;
                    }
                    survey = Service.GetSurvey(Views.SelectedItem.WebSurveyId);
                    if (survey == null)
                    {
                        return;
                    }
                    fields = Context.Project.GetViewById(Views.SelectedItem.ViewId).Fields;
                }
                try
                {
                    MailMessage message = Settings.Default.GetMailMessage();
                    message.Subject = Subject;
                    foreach (string path in Attachments)
                    {
                        message.Attachments.Add(new Attachment(path));
                    }
                    using (SmtpClient client = Settings.Default.GetSmtpClient(ConfigurationExtensions.DecryptSafe))
                    {
                        foreach (RecipientViewModel recipient in Recipients)
                        {
                            try
                            {
                                message.To.Clear();
                                message.To.Add(new MailAddress(recipient.GetEmailAddress()));
                                if (AppendUrl)
                                {
                                    StringBuilder body = new StringBuilder();
                                    body.Append(Body.Trim());
                                    body.AppendLine();
                                    body.AppendLine();
                                    if (recipient.IsResponder && Prepopulate)
                                    {
                                        Record record = GetRecord(recipient.Responder, fields);
                                        if (!Service.TryAddRecord(survey, record))
                                        {
                                            failures.Add(recipient);
                                            continue;
                                        }
                                        body.AppendFormat("URL: {0}", record.GetUrl());
                                        body.AppendLine();
                                        body.AppendFormat("Passcode: {0}", record.Passcode);
                                        message.Body = body.ToString();
                                    }
                                    else
                                    {
                                        body.AppendFormat(ViewExtensions.GetWebSurveyUrl(Views.SelectedItem.WebSurveyId).ToString());
                                    }
                                    message.Body = body.ToString();
                                }
                                else
                                {
                                    message.Body = Body;
                                }
                                client.Send(message);
                            }
                            catch (Exception ex)
                            {
                                Log.Logger.Warn("Failed to send email", ex);
                                failures.Add(recipient);
                            }
                        }
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    Log.Logger.Warn("Failed to send email", ex);
                    exception = ex;
                }
            };
            msg.Executed += (sender, e) =>
            {
                if (error != ConfigurationError.None)
                {
                    Documents.ShowSettings(SurveyViewModel.GetErrorMessage(error));
                }
                else if (Prepopulate && survey == null)
                {
                    Documents.ShowSettings(SurveyViewModel.GetErrorMessage("Failed to retrieve web survey details."));
                }
                else if (failures.Count > 0)
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine("Delivery to the following recipients failed:");
                    message.AppendLine();
                    foreach (RecipientViewModel failure in failures)
                    {
                        message.AppendLine(failure.ToString());
                    }
                    MessengerInstance.Send(new AlertMessage
                    {
                        Message = message.ToString().Trim()
                    });
                }
                else if (!success)
                {
                    Documents.ShowSettings("Failed to send email. Please verify settings.", exception);
                }
                else
                {
                    MessengerInstance.Send(new ToastMessage
                    {
                        Message = "Email has been sent."
                    });
                    Dirty = false;
                    Close();
                }
            };
            MessengerInstance.Send(msg);
        }
    }
}
