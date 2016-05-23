using ERHMS.Domain;
using ERHMS.Presentation.Converters;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;

namespace ERHMS.Presentation.ViewModels
{
    public class EmailViewModel : ViewModelBase
    {
        public class RecipientViewModel : ViewModelBase
        {
            public EmailViewModel Parent { get; private set; }

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

            public RecipientViewModel(EmailViewModel parent)
            {
                Parent = parent;
                IsResponder = true;
                PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "IsResponder")
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

            public RecipientViewModel(EmailViewModel parent, Responder responder)
                : this(parent)
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

            public void Add()
            {
                // TODO: Validate fields
                Parent.Recipients.Add(this);
                Active = false;
            }

            public void Cancel()
            {
                Active = false;
            }

            public void Remove()
            {
                Parent.Recipients.Remove(this);
            }
        }

        public class AttachmentViewModel : ViewModelBase
        {
            public EmailViewModel Parent { get; private set; }
            public FileInfo File { get; private set; }

            public RelayCommand RemoveCommand { get; private set; }

            public AttachmentViewModel(EmailViewModel parent, FileInfo file)
            {
                Parent = parent;
                File = file;
                RemoveCommand = new RelayCommand(Remove);
            }

            public void Remove()
            {
                Parent.Attachments.Remove(this);
            }
        }

        private static readonly RecipientToStringConverter RecipientToStringConverter = new RecipientToStringConverter();

        private ICollection<Responder> responders;
        public ICollection<Responder> Responders
        {
            get { return responders; }
            set { Set(() => Responders, ref responders, value); }
        }

        public ObservableCollection<RecipientViewModel> Recipients { get; private set; }

        private RecipientViewModel recipient;
        public RecipientViewModel Recipient
        {
            get { return recipient; }
            set { Set(() => Recipient, ref recipient, value); }
        }

        private string subject;
        public string Subject
        {
            get { return subject; }
            set { Set(() => Subject, ref subject, value); }
        }

        public ObservableCollection<AttachmentViewModel> Attachments { get; private set; }

        private string body;
        public string Body
        {
            get { return body; }
            set { Set(() => Body, ref body, value); }
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand AttachCommand { get; private set; }
        public RelayCommand SendCommand { get; private set; }

        public EmailViewModel()
        {
            Title = "Email";
            Responders = DataContext.Responders.SelectByDeleted(false)
                .OrderBy(responder => responder.LastName)
                .ThenBy(responder => responder.FirstName)
                .ThenBy(responder => responder.EmailAddress)
                .ToList();
            Recipients = new ObservableCollection<RecipientViewModel>();
            Attachments = new ObservableCollection<AttachmentViewModel>();
            AddCommand = new RelayCommand(Add);
            AttachCommand = new RelayCommand(Attach);
            SendCommand = new RelayCommand(Send);
            // TODO: Handle RefreshListMessage<Responder>
        }

        public EmailViewModel(IEnumerable<Responder> responders)
            : this()
        {
            foreach (Responder responder in responders)
            {
                Recipients.Add(new RecipientViewModel(this, responder));
            }
        }

        private static void RequestConfiguration(string message)
        {
            NotifyMessage msg = new NotifyMessage(message);
            msg.Dismissed += (sender, e) =>
            {
                Locator.Main.OpenSettingsView();
            };
            Messenger.Default.Send(msg);
        }

        public void Add()
        {
            Recipient = new RecipientViewModel(this)
            {
                Active = true
            };
        }

        public void Attach()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Title = "Attach a File";
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string path in dialog.FileNames)
                    {
                        Attachments.Add(new AttachmentViewModel(this, new FileInfo(path)));
                    }
                }
            }
        }

        public void Send()
        {
            // TODO: Validate fields
            if (!Email.IsConfigured())
            {
                RequestConfiguration("Please configure email settings.");
                return;
            }
            ICollection<RecipientViewModel> invalidRecipients = new List<RecipientViewModel>();
            MailMessage message = Email.GetMessage();
            foreach (RecipientViewModel recipient in Recipients)
            {
                try
                {
                    message.Bcc.Add(new MailAddress(recipient.GetEmailAddress()));
                }
                catch
                {
                    invalidRecipients.Add(recipient);
                }
            }
            if (invalidRecipients.Count > 0)
            {
                Messenger.Default.Send(new NotifyMessage(string.Format(
                    "The following recipients have invalid email addresses:{0}{0}{1}",
                    Environment.NewLine,
                    string.Join("; ", invalidRecipients.Select(recipient => RecipientToStringConverter.Convert(recipient))))));
                return;
            }
            message.Subject = Subject;
            message.Body = Body;
            Exception ex = null;
            BlockMessage msg = new BlockMessage("Sending email \u2026");
            msg.Executing += (sender, e) =>
            {
                try
                {
                    foreach (AttachmentViewModel attachment in Attachments)
                    {
                        message.Attachments.Add(new Attachment(attachment.File.FullName));
                    }
                    Email.GetClient().Send(message);
                }
                catch (Exception _ex)
                {
                    ex = _ex;
                }
            };
            msg.Executed += (sender, e) =>
            {
                if (ex == null)
                {
                    Messenger.Default.Send(new ToastMessage("Email has been sent."));
                    Close();
                }
                else
                {
                    SmtpFailedRecipientsException recipientsEx = ex as SmtpFailedRecipientsException;
                    if (recipientsEx == null)
                    {
                        RequestConfiguration("Failed to send email. Please verify email settings.");
                    }
                    else
                    {
                        Messenger.Default.Send(new NotifyMessage(string.Format(
                            "Delivery to the following recipients failed:{0}{0}{1}",
                            Environment.NewLine,
                            string.Join("; ", recipientsEx.InnerExceptions.Select(recipientEx => recipientEx.FailedRecipient)))));
                    }
                }
            };
            Messenger.Default.Send(msg);
        }
    }
}
