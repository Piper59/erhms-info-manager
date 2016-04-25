using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo.Domain;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace ERHMS.Presentation.ViewModel
{
    public class ResponderSearchViewModel : ViewModelBase
    {
        private IList selectedResponders;
        public IList SelectedResponders
        {
            get { return selectedResponders; }
            set
            { 
                Set(() => SelectedResponders, ref selectedResponders, value);

                EditCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged(); 
            }
        }

        private ICollectionView responderList;
        public ICollectionView ResponderList
        {
            get { return responderList; }
            private set { Set(() => ResponderList, ref responderList, value); }
        }

        private string filter;
        public string Filter
        {
            get { return filter; }
            set
            {
                Set(() => Filter, ref filter, value);
                ResponderList.Filter = ListFilterFunc;
            }
        }

        private bool ListFilterFunc(object item)
        {
            Responder r = item as Responder;

            return r.Deleted == false && (filter == null ||
                Filter.Equals("") ||
                (r.ResponderId != null && r.ResponderId.ToLower().Contains(Filter.ToLower())) ||
                (r.FirstName != null && r.FirstName.ToLower().Contains(Filter.ToLower())) ||
                (r.LastName != null && r.LastName.ToLower().Contains(Filter.ToLower())) ||
                (r.City != null && r.City.ToLower().Contains(Filter.ToLower())) ||
                (r.State != null && r.State.ToLower().Contains(Filter.ToLower())) ||
                (r.EmailAddress != null && r.EmailAddress.ToLower().Contains(Filter.ToLower())) ||
                (r.OrganizationName != null && r.OrganizationName.ToLower().Contains(Filter.ToLower())) ||
                (r.Occupation != null && r.Occupation.ToLower().Contains(Filter.ToLower())));
        }

        public RelayCommand AddCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }

        private void RefreshData()
        {
            ResponderList = CollectionViewSource.GetDefaultView(App.Current.DataContext.Responders.SelectByDeleted(false));
            SelectedResponders = new ObservableCollection<Responder>();
        }

        public ResponderSearchViewModel()
        {
            AddCommand = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new NotificationMessage("ShowNewResponder"));
                });
            EditCommand = new RelayCommand(() => 
                {
                    Responder selectedResponder = ((Responder)((Responder)SelectedResponders[0]).Clone());
                    selectedResponder.New = false;
                    Messenger.Default.Send(new NotificationMessage<Responder>(selectedResponder, "ShowEditResponder"));
                }, HasSelectedSingleResponder);
            DeleteCommand = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new NotificationMessage<Action>(() =>
                        {
                            for(int i = SelectedResponders.Count-1; i >= 0; i--)
                            {
                                App.Current.DataContext.Responders.Delete((Responder)SelectedResponders[i]);
                            }
                        }, "ConfirmDeleteResponder"));
                }, HasSelectedResponders);
            EmailCommand = new RelayCommand(() =>
                {
                    Messenger.Default.Send(new NotificationMessage<Tuple<IList, string, string>>(new Tuple<IList, string, string>(SelectedResponders, "", ""), "ComposeEmail"));
                }, HasSelectedResponders);

            Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            {
                if (msg.Notification == "RefreshResponders")
                {
                    RefreshData();
                }
            });

            RefreshData();
        }

        public bool HasSelectedSingleResponder()
        {
            if (SelectedResponders == null)
                return false;
            return SelectedResponders.Count == 1;
        }

        public bool HasSelectedResponders()
        {
            if (SelectedResponders == null)
                return false;
            return SelectedResponders.Count > 0;
        }
    }
}
