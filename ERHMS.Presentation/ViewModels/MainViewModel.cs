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

        public RelayCommand ShowRespondersCommand { get; private set; }
        public RelayCommand CreateResponderCommand { get; private set; }
        public RelayCommand ShowIncidentsCommand { get; private set; }
        public RelayCommand CreateIncidentCommand { get; private set; }
        public RelayCommand LogCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        public MainViewModel()
        {
            Title = App.Title;
            Documents = new ObservableCollection<DocumentViewModel>();
            Documents.CollectionChanged += Documents_CollectionChanged;
            cachedDocuments = new List<DocumentViewModel>(Documents);
            ShowRespondersCommand = new RelayCommand(OpenResponderListView);
            CreateResponderCommand = new RelayCommand(() => { OpenResponderDetailView(DataContext.Responders.Create()); });
            ShowIncidentsCommand = new RelayCommand(OpenIncidentListView);
            CreateIncidentCommand = new RelayCommand(() => { OpenIncidentView(DataContext.Incidents.Create()); });
            LogCommand = new RelayCommand(OpenLogView);
            AboutCommand = new RelayCommand(OpenAboutView);
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

        public void OpenResponderListView()
        {
            OpenDocument(new ResponderListViewModel());
        }

        public void OpenResponderDetailView(Responder responder)
        {
            OpenDocument(new ResponderDetailViewModel(responder));
        }

        public void OpenIncidentListView()
        {
            OpenDocument(new IncidentListViewModel());
        }

        public void OpenIncidentView(Incident incident)
        {
            OpenDocument(new IncidentViewModel(incident));
        }

        public void OpenLocationDetailView(Location location)
        {
            OpenDocument(new LocationDetailViewModel(location));
        }

        public void OpenLogView()
        {
            OpenDocument(new LogViewModel());
        }

        public void OpenAboutView()
        {
            OpenDocument(new AboutViewModel());
        }

        public void Exit()
        {
            Messenger.Default.Send(new ExitMessage());
        }
    }
}
