using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ICollection<DocumentViewModel> cachedDocuments;

        public string Title
        {
            get { return App.Title; }
        }

        public ObservableCollection<DocumentViewModel> Documents { get; private set; }
        public ICommand ExitCommand { get; private set; }

        public MainViewModel()
        {
            Documents = new ObservableCollection<DocumentViewModel>();
            Documents.CollectionChanged += Documents_CollectionChanged;
            cachedDocuments = new List<DocumentViewModel>(Documents);
            ExitCommand = new RelayCommand(Exit);
        }

        private void Documents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (DocumentViewModel document in e.NewItems)
                {
                    document.PropertyChanged += Document_PropertyChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (DocumentViewModel document in e.OldItems)
                {
                    document.PropertyChanged -= Document_PropertyChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (DocumentViewModel document in cachedDocuments)
                {
                    document.PropertyChanged -= Document_PropertyChanged;
                }
                foreach (DocumentViewModel document in Documents)
                {
                    document.PropertyChanged += Document_PropertyChanged;
                }
            }
            cachedDocuments = new List<DocumentViewModel>(Documents);
        }

        private void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DocumentViewModel document = (DocumentViewModel)sender;
            switch (e.PropertyName)
            {
                case "Closed":
                    if (document.Closed)
                    {
                        Documents.Remove(document);
                    }
                    else
                    {
                        Documents.Add(document);
                    }
                    break;
            }
        }

        public void Exit()
        {
            Messenger.Default.Send(new ExitMessage());
        }
    }
}
