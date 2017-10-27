using GalaSoft.MvvmLight.Command;

namespace ERHMS.Presentation.ViewModels
{
    public class StartViewModel : ViewModelBase
    {
        public RelayCommand ShowRespondersCommand { get; private set; }
        public RelayCommand ShowIncidentsCommand { get; private set; }
        public RelayCommand ShowViewsCommand { get; private set; }
        public RelayCommand ShowTemplatesCommand { get; private set; }
        public RelayCommand ShowPgmsCommand { get; private set; }
        public RelayCommand ShowCanvasesCommand { get; private set; }

        public StartViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Start";
            ShowRespondersCommand = new RelayCommand(ShowResponders);
            ShowIncidentsCommand = new RelayCommand(ShowIncidents);
            ShowViewsCommand = new RelayCommand(ShowViews);
            ShowTemplatesCommand = new RelayCommand(ShowTemplates);
            ShowPgmsCommand = new RelayCommand(ShowPgms);
            ShowCanvasesCommand = new RelayCommand(ShowCanvases);
        }

        public void ShowResponders()
        {
            Documents.ShowResponders();
        }

        public void ShowIncidents()
        {
            Documents.ShowIncidents();
        }

        public void ShowViews()
        {
            Documents.ShowViews();
        }

        public void ShowTemplates()
        {
            Documents.ShowTemplates();
        }

        public void ShowPgms()
        {
            Documents.ShowPgms();
        }

        public void ShowCanvases()
        {
            Documents.ShowCanvases();
        }
    }
}
