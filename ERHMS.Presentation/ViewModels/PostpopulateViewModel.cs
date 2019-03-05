using ERHMS.Domain;
using ERHMS.Presentation.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class PostpopulateViewModel : DialogViewModel
    {
        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public ResponderListChildViewModel()
            {
                Refresh();
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.Select().OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase);
            }

            public void SelectById(string responderId)
            {
                SelectedObject = Items.FirstOrDefault(responder => responder.ResponderId.Equals(responderId, StringComparison.OrdinalIgnoreCase));
            }
        }

        public class SavedEventArgs : EventArgs
        {
            public string ResponderId { get; private set; }

            public SavedEventArgs(string responderId)
            {
                ResponderId = responderId;
            }
        }

        public ResponderListChildViewModel Responders { get; private set; }

        public ICommand LinkCommand { get; private set; }
        public ICommand UnlinkCommand { get; private set; }

        public PostpopulateViewModel(string responderId)
        {
            Title = "Link to Responder";
            Responders = new ResponderListChildViewModel();
            Responders.SelectById(responderId);
            LinkCommand = new Command(Link, Responders.HasSelectedItem);
            UnlinkCommand = new Command(Unlink);
        }

        public event EventHandler<SavedEventArgs> Saved;
        private void OnSaved(SavedEventArgs e)
        {
            Saved?.Invoke(this, e);
        }
        private void OnSaved(string responderId)
        {
            OnSaved(new SavedEventArgs(responderId));
        }

        public void Link()
        {
            OnSaved(Responders.SelectedItem.ResponderId);
            Close();
        }

        public void Unlink()
        {
            OnSaved((string)null);
            Close();
        }
    }
}
