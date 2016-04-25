using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;

namespace ERHMS.Presentation.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the StartViewModel class.
        /// </summary>
        private string smtpHost;
        public string SmtpHost
        {
            get { return smtpHost; }
            set { Set(() => SmtpHost, ref smtpHost, value); }
        }
        private int smtpPort;
        public int SmtpPort
        {
            get { return smtpPort; }
            set { Set(() => SmtpPort, ref smtpPort, value); }
        }
        private string emailSender;
        public string EmailSender
        {
            get { return emailSender; }
            set { Set(() => EmailSender, ref emailSender, value); }
        }
        private string bingMapsLicenseKey;
        public string BingMapsLicenseKey
        {
            get { return bingMapsLicenseKey; }
            set { Set(() => BingMapsLicenseKey, ref bingMapsLicenseKey, value); }
        }
        private string organizationName;
        public string OrganizationName
        {
            get { return organizationName; }
            set { Set(() => OrganizationName, ref organizationName, value); }
        }
        private string organizationWebSurveyKey;
        public string OrganizationWebSurveyKey
        {
            get { return organizationWebSurveyKey; }
            set { Set(() => OrganizationWebSurveyKey, ref organizationWebSurveyKey, value); }
        }
        public RelayCommand SaveCommand { get; private set; }

        public SettingsViewModel()
        {
            SmtpHost = Properties.Settings.Default.SmtpHost;
            smtpPort = Properties.Settings.Default.SmtpPort;
            EmailSender = Properties.Settings.Default.EmailSender;
            BingMapsLicenseKey = Properties.Settings.Default.BingMapsLicenseKey;
            OrganizationName = Properties.Settings.Default.OrganizationName;
            OrganizationWebSurveyKey = Properties.Settings.Default.OrganizationWebSurveyKey;

            SaveCommand = new RelayCommand(() =>
            {
                try
                {
                    Properties.Settings.Default.SmtpHost = SmtpHost;
                    Properties.Settings.Default.SmtpPort = SmtpPort;
                    Properties.Settings.Default.EmailSender = EmailSender;
                    Properties.Settings.Default.BingMapsLicenseKey = BingMapsLicenseKey;
                    Properties.Settings.Default.OrganizationName = OrganizationName;
                    Properties.Settings.Default.OrganizationWebSurveyKey = OrganizationWebSurveyKey;

                    Properties.Settings.Default.Save();

                    Messenger.Default.Send(new NotificationMessage<string>("Application settings were successfully saved.", "ShowSuccessMessage"));
                }
                catch (Exception)
                {
                    Messenger.Default.Send(new NotificationMessage<string>("An error occurred while trying to save the settings.", "ShowErrorMessage"));
                }
            });
        }
    }
}