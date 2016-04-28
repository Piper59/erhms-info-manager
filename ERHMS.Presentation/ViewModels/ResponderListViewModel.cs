using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderListViewModel : DocumentViewModel
    {
        private static readonly ICollection<Func<Responder, string>> FilterPropertyAccessors = new Func<Responder, string>[]
        {
            responder => responder.ResponderId,
            responder => responder.LastName,
            responder => responder.FirstName,
            responder => responder.EmailAddress,
            responder => responder.City,
            responder => responder.State,
            responder => responder.OrganizationName,
            responder => responder.Occupation
        };

        private string filter;
        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                if (!Set(() => Filter, ref filter, value))
                {
                    return;
                }
                Responders.Refresh();
            }
        }

        private ICollectionView responders;
        public ICollectionView Responders
        {
            get { return responders; }
            set { Set(() => Responders, ref responders, value); }
        }

        private IList selectedResponders;
        public IList SelectedResponders
        {
            get
            {
                return selectedResponders;
            }
            set
            {
                if (!Set(() => SelectedResponders, ref selectedResponders, value))
                {
                    return;
                }
                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            }
        }

        public Responder SelectedResponder
        {
            get
            {
                if (SelectedResponders != null && SelectedResponders.Count == 1)
                {
                    return (Responder)SelectedResponders[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public ResponderListViewModel()
        {
            Title = "Responders";
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasOneSelectedResponder);
            DeleteCommand = new RelayCommand(Delete, HasSelectedResponders);
            EmailCommand = new RelayCommand(Email, HasSelectedResponders);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Responder>>(this, OnRefreshMessage);
        }

        public bool HasSelectedResponders()
        {
            return SelectedResponders != null && SelectedResponders.Count > 0;
        }

        public bool HasOneSelectedResponder()
        {
            return SelectedResponders != null && SelectedResponders.Count == 1;
        }

        public void Create()
        {
            Locator.Main.OpenResponderDetail(DataContext.Responders.Create());
        }

        public void Edit()
        {
            Locator.Main.OpenResponderDetail((Responder)SelectedResponder.Clone());
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage(
                "Delete?",
                string.Format("Are you sure you want to delete {0}?", SelectedResponders.Count == 1 ? "this responder" : "these responders"),
                "Delete",
                "Don't Delete");
            msg.Confirmed += (sender, e) =>
            {
                foreach (Responder responder in SelectedResponders)
                {
                    DataContext.Responders.Delete(responder);
                }
                Messenger.Default.Send(new RefreshMessage<Responder>());
            };
            Messenger.Default.Send(msg);
        }

        public void Email()
        {
            // TODO: Locator.Main.OpenEmail
        }

        public void Refresh()
        {
            Responders = CollectionViewSource.GetDefaultView(DataContext.Responders.SelectByDeleted(false));
            Responders.Filter = FilterResponder;
        }

        private bool FilterResponder(object item)
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                return true;
            }
            Responder responder = (Responder)item;
            foreach (Func<Responder, string> accessor in FilterPropertyAccessors)
            {
                string property = accessor(responder);
                if (property != null && property.ContainsIgnoreCase(Filter))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnRefreshMessage(RefreshMessage<Responder> msg)
        {
            Refresh();
        }
    }
}
