using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ERHMS.Presentation.ViewModels
{
    public class WorkflowViewModel : ViewModelBase
    {
        public RelayCommand RespondersCommand { get; private set; }
        public RelayCommand IncidentsCommand { get; private set; }
        public RelayCommand FormsCommand { get; private set; }
        public RelayCommand TemplatesCommand { get; private set; }
        public RelayCommand AnalysesCommand { get; private set; }
        public RelayCommand DashboardsCommand { get; private set; }

        public WorkflowViewModel()
        {
            Title = "Workflow";
            RespondersCommand = new RelayCommand(OpenResponderListView);
            IncidentsCommand = new RelayCommand(OpenIncidentListView);
            FormsCommand = new RelayCommand(OpenViewListView);
            TemplatesCommand = new RelayCommand(OpenTemplateListView);
            AnalysesCommand = new RelayCommand(OpenPgmListView);
            DashboardsCommand = new RelayCommand(OpenCanvasListView);
        }

        private bool CheckDataContext()
        {
            if (DataContext == null)
            {
                NotifyMessage msg = new NotifyMessage("Please open a data source.");
                msg.Dismissed += (sender, e) =>
                {
                    Locator.Main.OpenDataSourceListView();
                };
                Messenger.Default.Send(msg);
                return false;
            }
            else
            {
                return true;
            }
        }

        public void OpenResponderListView()
        {
            if (CheckDataContext())
            {
                Locator.Main.OpenResponderListView();
            }
        }

        public void OpenIncidentListView()
        {
            if (CheckDataContext())
            {
                Locator.Main.OpenIncidentListView();
            }
        }

        public void OpenViewListView()
        {
            if (CheckDataContext())
            {
                Locator.Main.OpenViewListView();
            }
        }

        public void OpenTemplateListView()
        {
            if (CheckDataContext())
            {
                Locator.Main.OpenTemplateListView();
            }
        }

        public void OpenPgmListView()
        {
            if (CheckDataContext())
            {
                Locator.Main.OpenPgmListView();
            }
        }

        public void OpenCanvasListView()
        {
            if (CheckDataContext())
            {
                Locator.Main.OpenCanvasListView();
            }
        }
    }
}
