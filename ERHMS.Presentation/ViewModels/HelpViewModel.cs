using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class HelpViewModel : ViewModelBase
    {
        private RelayCommand showRespondersCommand;
        public ICommand ShowRespondersCommand
        {
            get { return showRespondersCommand ?? (showRespondersCommand = new RelayCommand(ShowResponders)); }
        }

        private RelayCommand showIncidentsCommand;
        public ICommand ShowIncidentsCommand
        {
            get { return showIncidentsCommand ?? (showIncidentsCommand = new RelayCommand(ShowIncidents)); }
        }

        private RelayCommand showViewsCommand;
        public ICommand ShowViewsCommand
        {
            get { return showViewsCommand ?? (showViewsCommand = new RelayCommand(ShowViews)); }
        }

        private RelayCommand showTemplatesCommand;
        public ICommand ShowTemplatesCommand
        {
            get { return showTemplatesCommand ?? (showTemplatesCommand = new RelayCommand(ShowTemplates)); }
        }

        private RelayCommand showPgmsCommand;
        public ICommand ShowPgmsCommand
        {
            get { return showPgmsCommand ?? (showPgmsCommand = new RelayCommand(ShowPgms)); }
        }

        private RelayCommand showCanvasesCommand;
        public ICommand ShowCanvasesCommand
        {
            get { return showCanvasesCommand ?? (showCanvasesCommand = new RelayCommand(ShowCanvases)); }
        }

        public HelpViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Help";
        }

        private bool Validate()
        {
            if (Context == null)
            {
                AlertMessage msg = new AlertMessage
                {
                    Message = "Please open a data source."
                };
                msg.Dismissed += (sender, e) =>
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
