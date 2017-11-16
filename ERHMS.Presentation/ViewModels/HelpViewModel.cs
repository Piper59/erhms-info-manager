using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;

namespace ERHMS.Presentation.ViewModels
{
    public class HelpViewModel : ViewModelBase
    {
        public RelayCommand ShowRespondersCommand { get; private set; }
        public RelayCommand ShowIncidentsCommand { get; private set; }
        public RelayCommand ShowViewsCommand { get; private set; }
        public RelayCommand ShowTemplatesCommand { get; private set; }
        public RelayCommand ShowPgmsCommand { get; private set; }
        public RelayCommand ShowCanvasesCommand { get; private set; }

        public HelpViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Help";
            ShowRespondersCommand = new RelayCommand(ShowResponders);
            ShowIncidentsCommand = new RelayCommand(ShowIncidents);
            ShowViewsCommand = new RelayCommand(ShowViews);
            ShowTemplatesCommand = new RelayCommand(ShowTemplates);
            ShowPgmsCommand = new RelayCommand(ShowPgms);
            ShowCanvasesCommand = new RelayCommand(ShowCanvases);
        }

        private bool Validate()
        {
            if (Context == null)
            {
                ConfirmMessage msg = new ConfirmMessage
                {
                    Title = "Help",
                    Message = "To get started, please create or open a data source."
                };
                msg.Confirmed += (sender, e) =>
                {
                    Documents.ShowDataSources();
                };
                MessengerInstance.Send(msg);
                return false;
            }
            return true;
        }

        public void ShowResponders()
        {
            if (!Validate())
            {
                return;
            }
            Documents.ShowResponders();
        }

        public void ShowIncidents()
        {
            if (!Validate())
            {
                return;
            }
            Documents.ShowIncidents();
        }

        public void ShowViews()
        {
            if (!Validate())
            {
                return;
            }
            Documents.ShowViews();
        }

        public void ShowTemplates()
        {
            if (!Validate())
            {
                return;
            }
            Documents.ShowTemplates();
        }

        public void ShowPgms()
        {
            if (!Validate())
            {
                return;
            }
            Documents.ShowPgms();
        }

        public void ShowCanvases()
        {
            if (!Validate())
            {
                return;
            }
            Documents.ShowCanvases();
        }
    }
}
