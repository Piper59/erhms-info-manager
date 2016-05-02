using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderDetailViewModel : DocumentViewModel
    {
        public Responder Responder { get; private set; }
        public ICollection<string> Prefixes { get; private set; }
        public ICollection<string> Suffixes { get; private set; }
        public ICollection<string> Genders { get; private set; }
        public ICollection<string> States { get; private set; }

        public RelayCommand SaveCommand { get; private set; }

        public ResponderDetailViewModel(Responder responder)
        {
            Responder = responder;
            UpdateTitle();
            Prefixes = DataContext.Prefixes
                .Select()
                .Prepend("")
                .ToList();
            Suffixes = DataContext.Suffixes
                .Select()
                .Prepend("")
                .ToList();
            Genders = DataContext.Genders
                .Select()
                .Prepend("")
                .ToList();
            States = DataContext.States
                .Select()
                .Prepend("")
                .ToList();
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
            // TODO: Validate fields
            DataContext.Responders.Save(Responder);
            Messenger.Default.Send(new RefreshMessage<Responder>());
            UpdateTitle();
        }
    }
}
