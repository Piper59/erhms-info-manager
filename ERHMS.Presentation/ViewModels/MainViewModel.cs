using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Communication;
using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
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
        private ICollection<ViewModelBase> cachedDocuments;

        public ObservableCollection<ViewModelBase> Documents { get; private set; }

        private ViewModelBase activeDocument;
        public ViewModelBase ActiveDocument
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
        public RelayCommand AnalysesCommand { get; private set; }
        public RelayCommand DashboardsCommand { get; private set; }
        public RelayCommand SettingsCommand { get; private set; }
        public RelayCommand LogsCommand { get; private set; }
        public RelayCommand HelpCommand { get; private set; }
        public RelayCommand AboutCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        public MainViewModel()
        {
            Title = App.Title;
            Documents = new ObservableCollection<ViewModelBase>();
            Documents.CollectionChanged += Documents_CollectionChanged;
            cachedDocuments = new List<ViewModelBase>(Documents);
            OpenDataSourceCommand = new RelayCommand(OpenDataSource);
            CloseDataSourceCommand = new RelayCommand(CloseDataSource);
            ShowRespondersCommand = new RelayCommand(OpenResponderListView);
            CreateResponderCommand = new RelayCommand(() => { OpenResponderDetailView(DataContext.Responders.Create()); });
            ShowIncidentsCommand = new RelayCommand(OpenIncidentListView);
            CreateIncidentCommand = new RelayCommand(() => { OpenIncidentView(DataContext.Incidents.Create()); });
            FormsCommand = new RelayCommand(OpenViewListView);
            TemplatesCommand = new RelayCommand(OpenTemplateListView);
            AssignmentsCommand = new RelayCommand(OpenAssignmentListView);
            AnalysesCommand = new RelayCommand(OpenPgmListView);
            DashboardsCommand = new RelayCommand(OpenCanvasListView);
            SettingsCommand = new RelayCommand(OpenSettingsView);
            LogsCommand = new RelayCommand(OpenLogListView);
            HelpCommand = new RelayCommand(OpenHelpView);
            AboutCommand = new RelayCommand(OpenAboutView);
            ExitCommand = new RelayCommand(Exit);
            App.Current.Service.ViewAdded += Service_ViewAdded;
            App.Current.Service.ViewDataImported += Service_ViewDataImported;
            App.Current.Service.RecordSaved += Service_RecordSaved;
            App.Current.Service.TemplateAdded += Service_TemplateAdded;
            App.Current.Service.CanvasClosed += Service_CanvasClosed;
        }

        private void Documents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (ViewModelBase document in e.NewItems)
                {
                    document.PropertyChanged += Document_PropertyChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (ViewModelBase document in e.OldItems)
                {
                    document.PropertyChanged -= Document_PropertyChanged;
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (ViewModelBase document in cachedDocuments)
                {
                    document.PropertyChanged -= Document_PropertyChanged;
                }
                foreach (ViewModelBase document in Documents)
                {
                    document.PropertyChanged += Document_PropertyChanged;
                }
            }
            cachedDocuments = new List<ViewModelBase>(Documents);
        }

        private void Document_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ViewModelBase document = (ViewModelBase)sender;
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

        public void OpenDataSource()
        {
            // TODO: Implement
        }

        public void CloseDataSource()
        {
            // TODO: Implement
        }

        private void OpenDocument(ViewModelBase document)
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

        public void OpenViewListView()
        {
            OpenDocument(new ViewListViewModel(null));
        }

        public void OpenRecordListView(View view)
        {
            OpenDocument(new RecordListViewModel(view));
        }

        public void OpenTemplateListView()
        {
            OpenDocument(new TemplateListViewModel(null));
        }

        public void OpenAssignmentListView()
        {
            OpenDocument(new AssignmentListViewModel(null));
        }

        public void OpenPgmListView()
        {
            OpenDocument(new PgmListViewModel(null));
        }

        public void OpenCanvasListView()
        {
            OpenDocument(new CanvasListViewModel(null));
        }

        public void OpenSettingsView()
        {
            // TODO: Implement
        }

        public void OpenLogListView()
        {
            OpenDocument(new LogListViewModel());
        }

        public void OpenHelpView()
        {
            OpenDocument(new HelpViewModel());
        }

        public void OpenAboutView()
        {
            OpenDocument(new AboutViewModel());
        }

        public void Exit()
        {
            Messenger.Default.Send(new ExitMessage());
        }

        private void Service_ViewAdded(object sender, ViewEventArgs e)
        {
            string incidentId = e.Tag;
            if (e.ProjectPath.Equals(DataContext.Project.FilePath, StringComparison.OrdinalIgnoreCase) && incidentId != null)
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
            Messenger.Default.Send(new RefreshListMessage<View>(incidentId));
        }

        private void Service_ViewDataImported(object sender, ViewEventArgs e)
        {
            Messenger.Default.Send(new ServiceMessage<ViewEventArgs>("ViewDataImported", e));
        }

        private void Service_RecordSaved(object sender, RecordEventArgs e)
        {
            Messenger.Default.Send(new ServiceMessage<RecordEventArgs>("RecordSaved", e));
        }

        private void Service_TemplateAdded(object sender, TemplateEventArgs e)
        {
            Messenger.Default.Send(new RefreshListMessage<Template>());
        }

        // TODO: Make this more robust
        private void Service_CanvasClosed(object sender, CanvasEventArgs e)
        {
            string incidentId = e.Tag;
            if (DataContext.Project.FilePath.Equals(e.ProjectPath, StringComparison.OrdinalIgnoreCase))
            {
                Canvas canvas = DataContext.Project.GetCanvasById(e.CanvasId);
                canvas.Content = File.ReadAllText(e.CanvasPath);
                DataContext.Project.UpdateCanvas(canvas);
            }
            Messenger.Default.Send(new RefreshListMessage<Canvas>(incidentId));
        }
    }
}
