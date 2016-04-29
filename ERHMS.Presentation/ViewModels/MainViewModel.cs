using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Communication;
using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using Template = ERHMS.EpiInfo.Template;

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

        public RelayCommand OpenDataSourceCommand { get; private set; }
        public RelayCommand CloseDataSourceCommand { get; private set; }
        public RelayCommand ShowRespondersCommand { get; private set; }
        public RelayCommand CreateResponderCommand { get; private set; }
        public RelayCommand ShowIncidentsCommand { get; private set; }
        public RelayCommand CreateIncidentCommand { get; private set; }
        public RelayCommand FormsCommand { get; private set; }
        public RelayCommand TemplatesCommand { get; private set; }
        public RelayCommand AssignmentsCommand { get; private set; }
        public RelayCommand AnalysisCommand { get; private set; }
        public RelayCommand DashboardsCommand { get; private set; }
        public RelayCommand SettingsCommand { get; private set; }
        public RelayCommand LogCommand { get; private set; }
        public RelayCommand HelpCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        public MainViewModel()
        {
            Title = App.Title;
            Documents = new ObservableCollection<DocumentViewModel>();
            Documents.CollectionChanged += Documents_CollectionChanged;
            cachedDocuments = new List<DocumentViewModel>(Documents);
            OpenDataSourceCommand = new RelayCommand(OpenDataSource);
            CloseDataSourceCommand = new RelayCommand(CloseDataSource);
            ShowRespondersCommand = new RelayCommand(OpenResponderListView);
            CreateResponderCommand = new RelayCommand(() => { OpenResponderDetailView(DataContext.Responders.Create()); });
            ShowIncidentsCommand = new RelayCommand(OpenIncidentListView);
            CreateIncidentCommand = new RelayCommand(() => { OpenIncidentView(DataContext.Incidents.Create()); });
            FormsCommand = new RelayCommand(OpenFormListView);
            TemplatesCommand = new RelayCommand(OpenTemplateListView);
            AssignmentsCommand = new RelayCommand(OpenAssignmentListView);
            AnalysisCommand = new RelayCommand(OpenAnalysisListView);
            DashboardsCommand = new RelayCommand(OpenDashboardListView);
            SettingsCommand = new RelayCommand(OpenSettingsView);
            LogCommand = new RelayCommand(OpenLogView);
            HelpCommand = new RelayCommand(OpenHelpView);
            AboutCommand = new RelayCommand(OpenAboutView);
            ExitCommand = new RelayCommand(Exit);
            App.Current.Service.ViewAdded += Service_ViewAdded;
            App.Current.Service.ViewDataImported += (sender, e) => { Messenger.Default.Send(new ServiceMessage<ViewEventArgs>("ViewDataImported", e)); };
            App.Current.Service.RecordSaved += (sender, e) => { Messenger.Default.Send(new ServiceMessage<RecordEventArgs>("RecordSaved", e)); };
            App.Current.Service.TemplateAdded += Service_TemplateAdded;
            App.Current.Service.CanvasClosed += Service_CanvasClosed;
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

        private void Service_ViewAdded(object sender, ViewEventArgs e)
        {
            string incidentId = e.Tag;
            if (e.ProjectPath == DataContext.Project.FilePath && incidentId != null)
            {
                View view = DataContext.Project.GetViewByName(e.ViewName);
                if (view == null)
                {
                    Log.Current.WarnFormat("View not found: {0}", e);
                }
                else
                {
                    ViewLink viewLink = DataContext.ViewLinks.Create();
                    viewLink.ViewId = view.Id;
                    viewLink.IncidentId = incidentId;
                    DataContext.ViewLinks.Save(viewLink);
                }
            }
            Messenger.Default.Send(new RefreshMessage<View>(incidentId));
        }

        private void Service_TemplateAdded(object sender, TemplateEventArgs e)
        {
            Messenger.Default.Send(new RefreshMessage<Template>());
        }

        // TODO: Make this more robust
        private void Service_CanvasClosed(object sender, CanvasEventArgs e)
        {
            string incidentId = e.Tag;
            if (DataContext.Project.FilePath == e.ProjectPath)
            {
                Canvas canvas = DataContext.Project.GetCanvasById(e.CanvasId);
                canvas.Content = File.ReadAllText(e.CanvasPath);
                DataContext.Project.UpdateCanvas(canvas);
            }
            Messenger.Default.Send(new RefreshMessage<Canvas>(incidentId));
        }

        public void OpenDataSource()
        {
            // TODO: Implement
        }

        public void CloseDataSource()
        {
            // TODO: Implement
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

        public void OpenFormListView()
        {
            OpenDocument(new FormListViewModel(null));
        }

        public void OpenTemplateListView()
        {
            OpenDocument(new TemplateListViewModel(null));
        }

        public void OpenAssignmentListView()
        {
            // TODO: Implement
        }

        public void OpenAnalysisListView()
        {
            OpenDocument(new PgmListViewModel(null));
        }

        public void OpenDashboardListView()
        {
            OpenDocument(new CanvasListViewModel(null));
        }

        public void OpenSettingsView()
        {
            // TODO: Implement
        }

        public void OpenLogView()
        {
            OpenDocument(new LogViewModel());
        }

        public void OpenHelpView()
        {
            // TODO: Implement
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
