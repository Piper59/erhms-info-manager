using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using ERHMS.Presentation.Infrastructure;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using Settings = ERHMS.Utility.Settings;
using View = Epi.View;

namespace ERHMS.Presentation.ViewModels
{
    public class EmailViewModel : ViewModelBase
    {
        public class ViewListViewModel : ListViewModelBase<View>
        {
            public ViewListViewModel()
            {
                Refresh();
                Messenger.Default.Register<RefreshMessage<View>>(this, msg => Refresh());
            }

            protected override IEnumerable<View> GetItems()
            {
                return DataContext.Project.GetViews()
                    .Where(view => view.IsWebSurvey())
                    .OrderBy(view => view.Name);
            }
        }

        public static void RequestConfiguration(string message)
        {
            AlertMessage msg = new AlertMessage
            {
                Message = message
            };
            msg.Dismissed += (sender, e) =>
            {
                MainViewModel.Instance.OpenSettingsView();
            };
            Messenger.Default.Send(msg);
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

        private bool appendWebSurveyUrl;
        public bool AppendWebSurveyUrl
        {
            get
            {
                return appendWebSurveyUrl;
            }
            set
            {
                if (Set(nameof(AppendWebSurveyUrl), ref appendWebSurveyUrl, value))
                {
                    if (!AppendWebSurveyUrl)
                    {
                        Views.SelectedItem = null;
                    }
                }
            }
        }

        private bool canAppendWebSurveyUrl;
        public bool CanAppendWebSurveyUrl
        {
            get
            {
                return canAppendWebSurveyUrl;
            }
            private set
            {
                if (Set(nameof(CanAppendWebSurveyUrl), ref canAppendWebSurveyUrl, value))
                {
                    if (!CanAppendWebSurveyUrl)
                    {
                        AppendWebSurveyUrl = false;
                    }
                }
            }
        }

        public ViewListViewModel Views { get; private set; }

        private bool prepopulateResponderId;
        public bool PrepopulateResponderId
        {
            get { return prepopulateResponderId; }
            set { Set(nameof(PrepopulateResponderId), ref prepopulateResponderId, value); }
        }

        private bool canPrepopulateResponderId;
        public bool CanPrepopulateResponderId
        {
            get
            {
                return canPrepopulateResponderId;
            }
            private set
            {
                if (Set(nameof(CanPrepopulateResponderId), ref canPrepopulateResponderId, value))
                {
                    if (!CanPrepopulateResponderId)
                    {
                        PrepopulateResponderId = false;
                    }
                }
            }
        }

        public RelayCommand AddRecipientCommand { get; private set; }
        public RelayCommand<RecipientViewModel> RemoveRecipientCommand { get; private set; }
        public RelayCommand AddAttachmentCommand { get; private set; }
        public RelayCommand<string> RemoveAttachmentCommand { get; private set; }
        public RelayCommand SendCommand { get; private set; }

        public EmailViewModel(IEnumerable<Responder> responders)
        {
            Title = "Email";
            Recipients = new ObservableCollection<RecipientViewModel>();
            foreach (Responder responder  in responders)
            {
                Recipients.Add(new RecipientViewModel(responder));
            }
            Attachments = new ObservableCollection<string>();
            Views = new ViewListViewModel();
            AddRecipientCommand = new RelayCommand(AddRecipient);
            RemoveRecipientCommand = new RelayCommand<RecipientViewModel>(RemoveRecipient);
            AddAttachmentCommand = new RelayCommand(AddAttachment);
            RemoveAttachmentCommand = new RelayCommand<string>(RemoveAttachment);
            SendCommand = new RelayCommand(Send);
            Views.Refreshed += (sender, e) =>
            {
                CanAppendWebSurveyUrl = !Views.Items.IsEmpty;
            };
            Views.SelectedItemChanged += (sender, e) =>
            {
                CanPrepopulateResponderId = Views.SelectedItem != null && DataContext.IsResponderLinkedView(Views.SelectedItem);
            };
        }

        public void AddRecipient()
        {
            RecipientViewModel recipient = new RecipientViewModel()
            {
                Active = true
            };
            recipient.Adding += (sender, e) =>
            {
                Recipients.Add(recipient);
            };
            Messenger.Default.Send(new ShowMessage
            {
                ViewModel = recipient
            });
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
                if (dialog.ShowDialog(App.Current.MainWin32Window) == DialogResult.OK)
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
                ShowRequiredMessage(fields);
                return false;
            }
            else if (AppendWebSurveyUrl && Views.SelectedItem == null)
            {
                Messenger.Default.Send(new AlertMessage
                {
                    Message = "Please select a form to append a web survey URL."
                });
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Send()
        {
            if (!Validate())
            {
                return;
            }
            if (!Settings.Default.IsEmailConfigured())
            {
                RequestConfiguration("Please configure email settings.");
                return;
            }
            ConfigurationError error = ConfigurationError.None;
            Survey survey = null;
            ICollection<RecipientViewModel> failures = new List<RecipientViewModel>();
            bool success = false;
            BlockMessage msg = new BlockMessage
            {
                Message = "Sending email \u2026"
            };
            msg.Executing += (sender, e) =>
            {
                if (PrepopulateResponderId)
                {
                    if (!Service.IsConfigured(out error))
                    {
                        return;
                    }
                    survey = Service.GetSurvey(Views.SelectedItem);
                    if (survey == null)
                    {
                        return;
                    }
                }
                try
                {
                    MailMessage message = Settings.Default.GetMailMessage();
                    message.Subject = Subject;
                    foreach (string path in Attachments)
                    {
                        message.Attachments.Add(new Attachment(path));
                    }
                    using (SmtpClient client = Settings.Default.GetSmtpClient())
                    {
                        foreach (RecipientViewModel recipient in Recipients)
                        {
                            try
                            {
                                message.To.Clear();
                                message.To.Add(new MailAddress(recipient.GetEmailAddress()));
                                if (AppendWebSurveyUrl)
                                {
                                    StringBuilder body = new StringBuilder();
                                    body.Append(Body.TrimEnd());
                                    body.AppendLine();
                                    body.AppendLine();
                                    if (recipient.IsResponder && PrepopulateResponderId)
                                    {
                                        Record record = new Record(new
                                        {
                                            ResponderId = recipient.Responder.ResponderId
                                        });
                                        if (!Service.TryAddRecord(Views.SelectedItem, survey, record))
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
                                        body.AppendFormat("URL: {0}", Views.SelectedItem.GetWebSurveyUrl());
                                    }
                                    message.Body = body.ToString();
                                }
                                else
                                {
                                    message.Body = Body;
                                }
                                client.Send(message);
                            }
                            catch
                            {
                                failures.Add(recipient);
                            }
                        }
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    Log.Logger.Warn("Failed to send email", ex);
                }
            };
            msg.Executed += (sender, e) =>
            {
                if (error != ConfigurationError.None)
                {
                    SurveyViewModel.RequestConfiguration(error);
                }
                else if (PrepopulateResponderId && survey == null)
                {
                    SurveyViewModel.RequestConfiguration("Failed to retrieve web survey details.");
                }
                else if (failures.Count > 0)
                {
                    StringBuilder message = new StringBuilder();
                    message.AppendLine("Delivery to the following recipients failed:");
                    message.AppendLine();
                    message.Append(string.Join("; ", failures));
                    Messenger.Default.Send(new AlertMessage
                    {
                        Message = message.ToString()
                    });
                }
                else if (!success)
                {
                    RequestConfiguration("Failed to send email. Please verify email settings.");
                }
                else
                {
                    Dirty = false;
                    Messenger.Default.Send(new ToastMessage
                    {
                        Message = "Email has been sent."
                    });
                    Close();
                }
            };
            Messenger.Default.Send(msg);
        }
    }
}
