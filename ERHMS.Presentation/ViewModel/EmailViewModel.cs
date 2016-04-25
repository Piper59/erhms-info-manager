using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ERHMS.EpiInfo.Domain;
using ERHMS.Domain;
using ERHMS.Utility;

namespace ERHMS.Presentation.ViewModel
{
    public class EmailViewModel : ViewModelBase
    {
        private string title;
        public string Title
        {
            get { return title; }
            set { Set(() => title, ref title, value); }
        }

        private ObservableCollection<ViewEntity> recipients;
        public ObservableCollection<ViewEntity> Recipients
        {
            get { return recipients; }
            private set { Set(() => recipients, ref recipients, value); }
        }

        private ObservableCollection<Assignment> assignments;
        public ObservableCollection<Assignment> Assignments
        {
            get { return assignments; }
            private set { Set(() => Assignments, ref assignments, value); }
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

        public RelayCommand SendCommand { get; private set; }

        public EmailViewModel(IList responders, string subject, string body)
        {
            Recipients = new ObservableCollection<ViewEntity>();
            foreach (ViewEntity r in responders)
                Recipients.Add(r);

            Subject = subject;

            Body = body;

            SendCommand = new RelayCommand(() =>
                {
                    try
                    {
                        var emails = from q in Recipients
                                     select q.GetProperty("EmailAddress").ToString();

                        //Email.Send(Properties.Settings.Default.SmtpHost,
                        //    Properties.Settings.Default.SmtpPort,
                        //    emails.ToList(),
                        //    Properties.Settings.Default.EmailSender,
                        //    Subject,
                        //    Body);

                        Messenger.Default.Send(new NotificationMessage<string>("Email(s) were successfully sent.", "ShowSuccessMessage"));
                    }
                    catch (Exception)
                    {
                        Messenger.Default.Send(new NotificationMessage<string>("An error occurred while trying to send the email(s). Ensure email settings are accurate.", "ShowErrorMessage"));
                    }
                });
        }
    }
}
