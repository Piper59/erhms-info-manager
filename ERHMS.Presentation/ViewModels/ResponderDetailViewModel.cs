using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderDetailViewModel : DocumentViewModel
    {
        public Responder Responder { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        public ResponderDetailViewModel(Responder responder)
        {
            Responder = responder;
            UpdateTitle();
            SaveCommand = new RelayCommand(Save);
        }

        private void UpdateTitle()
        {
            if (Responder.New)
            {
                Title = "New Responder";
            }
            else
            {
                Title = string.Format("{0}, {1}", Responder.LastName, Responder.FirstName);
            }
        }

        public void Save()
        {
            // TODO: Check required fields
            DataContext.Responders.Save(Responder);
            UpdateTitle();
            Messenger.Default.Send(new RefreshMessage<Responder>());
        }
    }
}
