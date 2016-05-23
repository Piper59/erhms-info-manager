using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
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
using View = Epi.View;

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

        private string body;
        public string Body
        {
            get { return body; }
            set { Set(() => Body, ref body, value); }
        }

        public ObservableCollection<AttachmentViewModel> Attachments { get; private set; }

        private ICollection<View> views;
        public ICollection<View> Views
        {
            get { return views; }
            set { Set(() => Views, ref views, value); }
        }

        private View selectedView;
        public View SelectedView
        {
            get { return selectedView; }
            set { Set(() => SelectedView, ref selectedView, value); }
        }

        private bool appendUrl;
        public bool AppendUrl
        {
            get { return appendUrl; }
            set { Set(() => AppendUrl, ref appendUrl, value); }
        }

        private bool canAppendUrl;
        public bool CanAppendUrl
        {
            get { return canAppendUrl; }
            set { Set(() => CanAppendUrl, ref canAppendUrl, value); }
        }

        private bool prepopulate;
        public bool Prepopulate
        {
            get { return prepopulate; }
            set { Set(() => Prepopulate, ref prepopulate, value); }
        }

        private bool canPrepopulate;
        public bool CanPrepopulate
        {
            get { return canPrepopulate; }
            set { Set(() => CanPrepopulate, ref canPrepopulate, value); }
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
            RefreshViews();
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Views))
                {
                    CanAppendUrl = Views.Count > 0;
                }
                else if (e.PropertyName == nameof(SelectedView))
                {
                    CanPrepopulate = SelectedView != null && DataContext.IsResponderLinkedView(SelectedView);
                }
                else if (e.PropertyName == nameof(AppendUrl) && !AppendUrl)
                {
                    SelectedView = null;
                }
                else if (e.PropertyName == nameof(CanAppendUrl) && !CanAppendUrl)
                {
                    AppendUrl = false;
                }
                else if (e.PropertyName == nameof(CanPrepopulate) && !CanPrepopulate)
                {
                    Prepopulate = false;
                }
            };
            AddCommand = new RelayCommand(Add);
            AttachCommand = new RelayCommand(Attach);
            SendCommand = new RelayCommand(Send);
            Messenger.Default.Register<RefreshListMessage<Responder>>(this, OnRefreshResponderListMessage);
            Messenger.Default.Register<RefreshListMessage<View>>(this, OnRefreshViewListMessage);
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

        private void RefreshViews()
        {
            Views = DataContext.GetViews()
                .Where(view => view.IsPublished())
                .OrderBy(view => view.Name)
                .ToList();
        }

        public void SetSelectedView(string viewName)
        {
            SelectedView = Views.SingleOrDefault(view => view.Name.EqualsIgnoreCase(viewName));
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
            // TODO: Handle errors
            if (!Email.IsConfigured())
            {
                RequestConfiguration("Please configure email settings.");
                return;
            }
            bool success = false;
            ICollection<RecipientViewModel> failures = new List<RecipientViewModel>();
            BlockMessage msg = new BlockMessage("Sending email \u2026");
            msg.Executing += (sender, e) =>
            {
                Service service = null;
                Survey survey = null;
                if (Prepopulate)
                {
                    service = new Service();
                    // TODO: Check configuration
                    survey = service.GetSurvey(SelectedView);
                }
                MailMessage message = Email.GetMessage();
                message.Subject = Subject;
                foreach (AttachmentViewModel attachment in Attachments)
                {
                    message.Attachments.Add(new Attachment(attachment.File.FullName));
                }
                using (SmtpClient client = Email.GetClient())
                {
                    foreach (RecipientViewModel recipient in Recipients)
                    {
                        try
                        {
                            message.To.Clear();
                            message.To.Add(new MailAddress(recipient.GetEmailAddress()));
                            if (AppendUrl)
                            {
                                if (recipient.IsResponder && Prepopulate)
                                {
                                    Record record = service.AddRecord(SelectedView, survey, new
                                    {
                                        ResponderId = recipient.Responder.ResponderId
                                    });
                                    message.Body = string.Format(
                                        "{0}{1}{1}URL: {2}{1}Passcode: {3}",
                                        Body.TrimEnd(),
                                        Environment.NewLine,
                                        record.GetUrl(),
                                        record.Passcode);
                                }
                                else
                                {
                                    message.Body = string.Format(
                                        "{0}{1}{1}URL: {2}",
                                        Body.TrimEnd(),
                                        Environment.NewLine,
                                        SelectedView.GetUrl());
                                }
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
            };
            msg.Executed += (sender, e) =>
            {
                if (success)
                {
                    if (failures.Count > 0)
                    {
                        Messenger.Default.Send(new NotifyMessage(string.Format(
                            "Delivery to the following recipients failed:{0}{0}{1}",
                            Environment.NewLine,
                            string.Join("; ", failures.Select(recipient => RecipientToStringConverter.Convert(recipient))))));
                    }
                    else
                    {
                        Messenger.Default.Send(new ToastMessage("Email has been sent."));
                        Close();
                    }
                }
                else
                {
                    RequestConfiguration("Failed to send email. Please verify email settings.");
                }
            };
            Messenger.Default.Send(msg);
        }

        private void OnRefreshResponderListMessage(RefreshListMessage<Responder> msg)
        {
            RefreshResponders();
        }

        private void OnRefreshViewListMessage(RefreshListMessage<View> msg)
        {
            RefreshViews();
        }
    }
}
