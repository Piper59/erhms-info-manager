using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Communication;
using ERHMS.DataAccess;
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
using Project = ERHMS.EpiInfo.Project;
using Template = ERHMS.EpiInfo.Template;

namespace ERHMS.Presentation.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ICollection<ViewModelBase> cachedDocuments;

        private DataContext dataSource;
        public DataContext DataSource
        {
            get { return dataSource; }
            private set { Set(() => DataSource, ref dataSource, value); }
        }

        public ObservableCollection<ViewModelBase> Documents { get; private set; }

        private ViewModelBase activeDocument;
        public ViewModelBase ActiveDocument
        {
            get { return activeDocument; }
            set { Set(() => ActiveDocument, ref activeDocument, value); }
        }

        public RelayCommand DataSourcesCommand { get; private set; }
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
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "DataSource")
                {
                    ShowRespondersCommand.RaiseCanExecuteChanged();
                    CreateResponderCommand.RaiseCanExecuteChanged();
                    ShowIncidentsCommand.RaiseCanExecuteChanged();
                    CreateIncidentCommand.RaiseCanExecuteChanged();
                    FormsCommand.RaiseCanExecuteChanged();
                    TemplatesCommand.RaiseCanExecuteChanged();
                    AssignmentsCommand.RaiseCanExecuteChanged();
                    AnalysesCommand.RaiseCanExecuteChanged();
                    DashboardsCommand.RaiseCanExecuteChanged();
                }
                else if (e.PropertyName == "ActiveDocument" && ActiveDocument != null)
                {
                    Log.Current.DebugFormat("Activating tab: {0}", ActiveDocument.GetType().Name);
                }
            };
            Documents = new ObservableCollection<ViewModelBase>();
            Documents.CollectionChanged += Documents_CollectionChanged;
            cachedDocuments = new List<ViewModelBase>(Documents);
            DataSourcesCommand = new RelayCommand(OpenDataSourceListView);
            ShowRespondersCommand = new RelayCommand(OpenResponderListView, HasDataSource);
            CreateResponderCommand = new RelayCommand(() => { OpenResponderDetailView(DataContext.Responders.Create()); }, HasDataSource);
            ShowIncidentsCommand = new RelayCommand(OpenIncidentListView, HasDataSource);
            CreateIncidentCommand = new RelayCommand(() => { OpenIncidentView(DataContext.Incidents.Create()); }, HasDataSource);
            FormsCommand = new RelayCommand(OpenViewListView, HasDataSource);
            TemplatesCommand = new RelayCommand(OpenTemplateListView, HasDataSource);
            AssignmentsCommand = new RelayCommand(OpenAssignmentListView, HasDataSource);
            AnalysesCommand = new RelayCommand(OpenPgmListView, HasDataSource);
            DashboardsCommand = new RelayCommand(OpenCanvasListView, HasDataSource);
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
            App.Current.Invoke(() =>
            {
                ViewModelBase document = (ViewModelBase)sender;
                switch (e.PropertyName)
                {
                    case "Closed":
                        if (document.Closed)
                        {
                            Log.Current.DebugFormat("Closing tab: {0}", document.GetType().Name);
                            Documents.Remove(document);
                        }
                        else
                        {
                            Documents.Add(document);
                        }
                        break;
                }
            });
        }

        public bool HasDataSource()
        {
            return DataSource != null;
        }

        private void OpenDataSourceInternal(FileInfo file)
        {
            App.Current.Invoke(() =>
            {
                Documents.Clear();
                DataSource = new DataContext(new Project(file));
                Title = string.Format("{0} - {1}", App.Title, DataSource.Project.Name);
            });
        }

        public void OpenDataSource(FileInfo file)
        {
            if (HasDataSource())
            {
                if (DataSource.Project.FilePath.Equals(file.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    CloseDataSourceListView();
                    return;
                }
                ConfirmMessage msg = new ConfirmMessage(
                    "Open?",
                    "Open data source? This will close the currently active data source.",
                    "Open",
                    "Don't Open");
                msg.Confirmed += (sender, e) =>
                {
                    OpenDataSourceInternal(file);
                };
                Messenger.Default.Send(msg);
            }
            else
            {
                OpenDataSourceInternal(file);
            }
        }

        private TViewModel GetDocument<TViewModel>(Predicate<TViewModel> predicate = null) where TViewModel : ViewModelBase
        {
            foreach (ViewModelBase document in Documents)
            {
                TViewModel typedDocument = document as TViewModel;
                if (typedDocument != null)
                {
                    if (predicate == null || predicate(typedDocument))
                    {
                        return typedDocument;
                    }
                }
            }
            return null;
        }

        private bool TryActivateDocument<TViewModel>(Predicate<TViewModel> predicate = null) where TViewModel : ViewModelBase
        {
            TViewModel document = GetDocument(predicate);
            if (document == null)
            {
                return false;
            }
            else
            {
                ActiveDocument = document;
                return true;
            }
        }

        private void OpenDocument(ViewModelBase document)
        {
            App.Current.Invoke(() =>
            {
                Log.Current.DebugFormat("Opening tab: {0}", document.GetType().Name);
                Documents.Add(document);
                ActiveDocument = document;
            });
        }

        public void OpenDataSourceListView()
        {
            if (!TryActivateDocument<DataSourceListViewModel>())
            {
                OpenDocument(new DataSourceListViewModel());
            }
        }

        public void CloseDataSourceListView()
        {
            GetDocument<DataSourceListViewModel>()?.Close();
        }

        public void OpenResponderListView()
        {
            if (!TryActivateDocument<ResponderListViewModel>())
            {
                OpenDocument(new ResponderListViewModel());
            }
        }

        public void OpenResponderDetailView(Responder responder)
        {
            if (!TryActivateDocument<ResponderDetailViewModel>(
                document => document.Responder.ResponderId.Equals(responder.ResponderId, StringComparison.OrdinalIgnoreCase)))
            {
                OpenDocument(new ResponderDetailViewModel(responder));
            }
        }

        public void OpenIncidentListView()
        {
            if (!TryActivateDocument<IncidentListViewModel>())
            {
                OpenDocument(new IncidentListViewModel());
            }
        }

        public void OpenIncidentView(Incident incident)
        {
            if (!TryActivateDocument<IncidentViewModel>(
                document => document.Incident.IncidentId.Equals(incident.IncidentId, StringComparison.OrdinalIgnoreCase)))
            {
                OpenDocument(new IncidentViewModel(incident));
            }
        }

        public void OpenLocationDetailView(Location location)
        {
            if (!TryActivateDocument<LocationDetailViewModel>(
                document => document.Location.LocationId.Equals(location.LocationId, StringComparison.OrdinalIgnoreCase)))
            {
                OpenDocument(new LocationDetailViewModel(location));
            }
        }

        public void OpenViewListView()
        {
            if (!TryActivateDocument<ViewListViewModel>())
            {
                OpenDocument(new ViewListViewModel(null));
            }
        }

        public void OpenRecordListView(View view)
        {
            if (!TryActivateDocument<RecordListViewModel>(document => document.View.Id == view.Id))
            {
                OpenDocument(new RecordListViewModel(view));
            }
        }

        public void OpenTemplateListView()
        {
            if (!TryActivateDocument<TemplateListViewModel>())
            {
                OpenDocument(new TemplateListViewModel(null));
            }
        }

        public void OpenAssignmentListView()
        {
            if (!TryActivateDocument<AssignmentListViewModel>())
            {
                OpenDocument(new AssignmentListViewModel(null));
            }
        }

        public void OpenPgmListView()
        {
            if (!TryActivateDocument<PgmListViewModel>())
            {
                OpenDocument(new PgmListViewModel(null));
            }
        }

        public void OpenCanvasListView()
        {
            if (!TryActivateDocument<CanvasListViewModel>())
            {
                OpenDocument(new CanvasListViewModel(null));
            }
        }

        public void OpenSettingsView()
        {
            // TODO: Implement
        }

        public void OpenLogListView()
        {
            if (!TryActivateDocument<LogListViewModel>())
            {
                OpenDocument(new LogListViewModel());
            }
        }

        public void OpenHelpView()
        {
            if (!TryActivateDocument<HelpViewModel>())
            {
                OpenDocument(new HelpViewModel());
            }
        }

        public void OpenAboutView()
        {
            if (!TryActivateDocument<AboutViewModel>())
            {
                OpenDocument(new AboutViewModel());
            }
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
