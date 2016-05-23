using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Converters;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderDetailViewModel : ViewModelBase
    {
        private static readonly ResponderToNameConverter ResponderToNameConverter = new ResponderToNameConverter();

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
            Prefixes = GetCodes(DataContext.Prefixes);
            Suffixes = GetCodes(DataContext.Suffixes);
            Genders = GetCodes(DataContext.Genders);
            States = GetCodes(DataContext.States);
            SaveCommand = new RelayCommand(Save);
        }

        private void UpdateTitle()
        {
            Title = Responder.New ? "New Responder" : ResponderToNameConverter.Convert(Responder);
        }

        private ICollection<string> GetCodes(CodeRepository codes)
        {
            return codes.Select()
                .Prepend("")
                .ToList();
        }

        public void Save()
        {
            // TODO: Validate fields
            DataContext.Responders.Save(Responder);
            Messenger.Default.Send(new ToastMessage("Responder has been saved."));
            Messenger.Default.Send(new RefreshListMessage<Responder>());
            UpdateTitle();
        }
    }
}
