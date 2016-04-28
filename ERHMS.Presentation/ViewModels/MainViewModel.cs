using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ERHMS.Presentation.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ICollection<DocumentViewModel> cachedDocuments;

        public string Title { get; private set; }
        public ObservableCollection<DocumentViewModel> Documents { get; private set; }

        private DocumentViewModel activeDocument;
        public DocumentViewModel ActiveDocument
        {
            get { return activeDocument; }
            set { Set(() => ActiveDocument, ref activeDocument, value); }
        }

        public RelayCommand OpenAboutCommand { get; private set; }
        public RelayCommand OpenResponderListCommand { get; private set; }
        public RelayCommand OpenResponderDetailCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        public MainViewModel()
        {
            Title = App.Title;
            Documents = new ObservableCollection<DocumentViewModel>();
            Documents.CollectionChanged += Documents_CollectionChanged;
            cachedDocuments = new List<DocumentViewModel>(Documents);
            OpenResponderListCommand = new RelayCommand(OpenResponderList);
            OpenResponderDetailCommand = new RelayCommand(() => { OpenResponderDetail(DataContext.Responders.Create()); });
            OpenAboutCommand = new RelayCommand(OpenAbout);
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

        // TODO: Use existing document if applicable

        private void OpenDocument(DocumentViewModel document)
        {
            Documents.Add(document);
            ActiveDocument = document;
        }

        public void OpenResponderList()
        {
            OpenDocument(new ResponderListViewModel());
        }

        public void OpenResponderDetail(Responder responder)
        {
            OpenDocument(new ResponderDetailViewModel(responder));
        }

        public void OpenAbout()
        {
            OpenDocument(new AboutViewModel());
        }

        public void Exit()
        {
            Messenger.Default.Send(new ExitMessage());
        }
    }
}
