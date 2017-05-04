using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmListViewModel : ListViewModelBase<DeepLink<Pgm>>
    {
        public Incident Incident { get; private set; }

        public RelayCommand OpenCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand LinkCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public PgmListViewModel(Incident incident)
        {
            Title = "Analyses";
            Incident = incident;
            Refresh();
            OpenCommand = new RelayCommand(Open, HasOneSelectedItem);
            DeleteCommand = new RelayCommand(Delete, HasOneSelectedItem);
            LinkCommand = new RelayCommand(Link, HasOneSelectedItem);
            RefreshCommand = new RelayCommand(Refresh);
            SelectedItemChanged += (sender, e) =>
            {
                OpenCommand.RaiseCanExecuteChanged();
                LinkCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            };
            Messenger.Default.Register<RefreshMessage<Pgm>>(this, msg => Refresh());
            Messenger.Default.Register<RefreshMessage<Incident>>(this, msg => Refresh());
        }

        protected override IEnumerable<DeepLink<Pgm>> GetItems()
        {
            IEnumerable<DeepLink<Pgm>> items;
            if (Incident == null)
            {
                items = DataContext.PgmLinks.SelectDeepLinks();
            }
            else
            {
                items = DataContext.PgmLinks.SelectDeepLinksByIncidentId(Incident.IncidentId);
            }
            return items.OrderBy(item => item.Item.Name);
        }

        protected override IEnumerable<string> GetFilteredValues(DeepLink<Pgm> item)
        {
            yield return item.Item.Name;
            yield return item.Item.Comment;
            yield return item.Item.Author;
            if (Incident == null)
            {
                yield return item.Incident?.Name;
            }
        }

        public void Open()
        {
            Pgm pgm = DataContext.Project.GetPgmById(SelectedItem.Item.PgmId);
            Wrapper wrapper = Analysis.OpenPgm.Create(DataContext.Project.FilePath, pgm.Name, pgm.Content, false);
            wrapper.Event += (sender, e) =>
            {
                if (e.Type == WrapperEventType.PgmSaved)
                {
                    Messenger.Default.Send(new RefreshMessage<Pgm>());
                }
            };
            wrapper.Invoke();
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected analysis?"
            };
            msg.Confirmed += (sender, e) =>
            {
                DataContext.PgmLinks.DeleteByPgmId(SelectedItem.Item.PgmId);
                DataContext.Project.DeletePgm(SelectedItem.Item);
                Messenger.Default.Send(new RefreshMessage<Pgm>());
            };
            Messenger.Default.Send(msg);
        }

        public void Link()
        {
            Messenger.Default.Send(new ShowMessage
            {
                ViewModel = new PgmLinkViewModel(SelectedItem)
                {
                    Active = true
                }
            });
        }
    }
}
