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
        private static readonly RecipientToStringConverter RecipientToStringConverter = new RecipientToStringConverter();

        private static void RequestConfiguration(string message)
        {
            NotifyMessage msg = new NotifyMessage(message);
            msg.Dismissed += (sender, e) =>
            {
                Locator.Main.OpenSettingsView();
            };
            Messenger.Default.Send(msg);
        }

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
            RefreshResponders();
            Recipients = new ObservableCollection<RecipientViewModel>();
            Attachments = new ObservableCollection<AttachmentViewModel>();
            AddCommand = new RelayCommand(Add);
            AttachCommand = new RelayCommand(Attach);
            SendCommand = new RelayCommand(Send);
            Messenger.Default.Register<RefreshListMessage<Responder>>(this, OnRefreshResponderList);
        }

        public EmailViewModel(IEnumerable<Responder> responders)
            : this()
        {
            foreach (Responder responder in responders)
            {
                Recipients.Add(new RecipientViewModel(Recipients, responder));
            }
        }

        private void RefreshResponders()
        {
            Responders = DataContext.Responders.SelectByDeleted(false)
                .OrderBy(responder => responder.LastName)
                .ThenBy(responder => responder.FirstName)
                .ThenBy(responder => responder.EmailAddress)
                .ToList();
        }

        public void Add()
        {
            Recipient = new RecipientViewModel(Recipients)
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
                        Attachments.Add(new AttachmentViewModel(Attachments, new FileInfo(path)));
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
                string address = recipient.GetEmailAddress();
                if (Email.IsValidAddress(address))
                {
                    message.Bcc.Add(new MailAddress(address));
                }
                else
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

        private void OnRefreshResponderList(RefreshListMessage<Responder> msg)
        {
            RefreshResponders();
        }
    }
}
