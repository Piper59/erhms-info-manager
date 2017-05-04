using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ERHMS.Presentation.ViewModels
{
    public class HelpViewModel : ViewModelBase
    {
        public RelayCommand OpenResponderListViewCommand { get; private set; }
        public RelayCommand OpenIncidentListViewCommand { get; private set; }
        public RelayCommand OpenViewListViewCommand { get; private set; }
        public RelayCommand OpenTemplateListViewCommand { get; private set; }
        public RelayCommand OpenPgmListViewCommand { get; private set; }
        public RelayCommand OpenCanvasListViewCommand { get; private set; }

        public HelpViewModel()
        {
            Title = "Help";
            OpenResponderListViewCommand = new RelayCommand(OpenResponderListView);
            OpenIncidentListViewCommand = new RelayCommand(OpenIncidentListView);
            OpenViewListViewCommand = new RelayCommand(OpenViewListView);
            OpenTemplateListViewCommand = new RelayCommand(OpenTemplateListView);
            OpenPgmListViewCommand = new RelayCommand(OpenPgmListView);
            OpenCanvasListViewCommand = new RelayCommand(OpenCanvasListView);
        }

        private bool Validate()
        {
            if (DataContext == null)
            {
                AlertMessage msg = new AlertMessage
                {
                    Message = "Please open a data source."
                };
                msg.Dismissed += (sender, e) =>
                {
                    Main.OpenDataSourceListView();
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
            if (Validate())
            {
                Main.OpenResponderListView();
            }
        }

        public void OpenIncidentListView()
        {
            if (Validate())
            {
                Main.OpenIncidentListView();
            }
        }

        public void OpenViewListView()
        {
            if (Validate())
            {
                Main.OpenViewListView();
            }
        }

        public void OpenTemplateListView()
        {
            if (Validate())
            {
                Main.OpenTemplateListView();
            }
        }

        public void OpenPgmListView()
        {
            if (Validate())
            {
                Main.OpenPgmListView();
            }
        }

        public void OpenCanvasListView()
        {
            if (Validate())
            {
                Main.OpenCanvasListView();
            }
        }
    }
}
