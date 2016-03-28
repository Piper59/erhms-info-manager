using GalaSoft.MvvmLight;
using ERHMS.WPF.Model;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.CommandWpf;

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
        private readonly IDataService _dataService;

        public RelayCommand NewResponderCommand { get; private set; }
        public RelayCommand NewIncidentCommand { get; private set; }
        public RelayCommand SearchRespondersCommand { get; private set; }
        public RelayCommand ViewFormListCommand { get; private set; }
        public RelayCommand ViewTemplateListCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                });

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
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}