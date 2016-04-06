using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ERHMS.WPF.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public RelayCommand NewResponderCommand { get; private set; }
        public RelayCommand NewIncidentCommand { get; private set; }
        public RelayCommand SearchRespondersCommand { get; private set; }
        public RelayCommand ViewFormListCommand { get; private set; }
        public RelayCommand ViewTemplateListCommand { get; private set; }
        public RelayCommand ViewHelpCommand { get; private set; }
        public RelayCommand ViewAboutCommand { get; private set; }
        public RelayCommand ExitApplicationCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            NewResponderCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("ShowNewResponder"));
            });
            NewIncidentCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("ShowNewIncident"));
            });
            SearchRespondersCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("ShowResponderSearch"));
            });
            ViewFormListCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("ShowFormList"));
            });
            ViewTemplateListCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("ShowTemplateList"));
            });
            ViewHelpCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("ShowHelp"));
            });
            ViewAboutCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("ShowAbout"));
            });
            ExitApplicationCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("ExitApplication"));
            });
        }
    }
}