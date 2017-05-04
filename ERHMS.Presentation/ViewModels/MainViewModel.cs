using Epi;
using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.Presentation.Infrastructure;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Presentation.ViewModels
{
    public class MainViewModel : GalaSoft.MvvmLight.ViewModelBase
    {
        private static MainViewModel instance = new MainViewModel();

        public static MainViewModel Instance
        {
            get { return instance; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            private set { Set(nameof(Title), ref title, value); }
        }

        private DataContext dataContext;
        public DataContext DataContext
        {
            get
            {
                return dataContext;
            }
            private set
            {
                if (Set(nameof(DataContext), ref dataContext, value))
                {
                    OpenResponderListViewCommand.RaiseCanExecuteChanged();
                    CreateResponderCommand.RaiseCanExecuteChanged();
                    OpenIncidentListViewCommand.RaiseCanExecuteChanged();
                    CreateIncidentCommand.RaiseCanExecuteChanged();
                    OpenViewListViewCommand.RaiseCanExecuteChanged();
                    OpenTemplateListViewCommand.RaiseCanExecuteChanged();
                    OpenAssignmentListViewCommand.RaiseCanExecuteChanged();
                    OpenPgmListViewCommand.RaiseCanExecuteChanged();
                    OpenCanvasListViewCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<ViewModelBase> Documents { get; private set; }

        private ViewModelBase activeDocument;
        public ViewModelBase ActiveDocument
        {
            get
            {
                return activeDocument;
            }
            set
            {
                if (Set(nameof(ActiveDocument), ref activeDocument, value))
                {
                    if (value != null)
                    {
                        Log.Logger.DebugFormat("Activating tab: {0}", value.GetType().Name);
                    }
                    CloseActiveDocumentCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand CloseActiveDocumentCommand { get; private set; }
        public RelayCommand OpenDataSourceListViewCommand { get; private set; }
        public RelayCommand OpenResponderListViewCommand { get; private set; }
        public RelayCommand CreateResponderCommand { get; private set; }
        public RelayCommand OpenIncidentListViewCommand { get; private set; }
        public RelayCommand CreateIncidentCommand { get; private set; }
        public RelayCommand OpenViewListViewCommand { get; private set; }
        public RelayCommand OpenTemplateListViewCommand { get; private set; }
        public RelayCommand OpenAssignmentListViewCommand { get; private set; }
        public RelayCommand OpenPgmListViewCommand { get; private set; }
        public RelayCommand OpenCanvasListViewCommand { get; private set; }
        public RelayCommand OpenSettingsViewCommand { get; private set; }
        public RelayCommand OpenLogListViewCommand { get; private set; }
        public RelayCommand OpenHelpViewCommand { get; private set; }
        public RelayCommand OpenAboutViewCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        private MainViewModel()
        {
            Title = App.Title;
            Documents = new ObservableCollection<ViewModelBase>();
            CloseActiveDocumentCommand = new RelayCommand(CloseActiveDocument, HasActiveDocument);
            OpenDataSourceListViewCommand = new RelayCommand(OpenDataSourceListView);
            OpenResponderListViewCommand = new RelayCommand(OpenResponderListView, HasDataContext);
            CreateResponderCommand = new RelayCommand(CreateResponder, HasDataContext);
            OpenIncidentListViewCommand = new RelayCommand(OpenIncidentListView, HasDataContext);
            CreateIncidentCommand = new RelayCommand(CreateIncident, HasDataContext);
            OpenViewListViewCommand = new RelayCommand(OpenViewListView, HasDataContext);
            OpenTemplateListViewCommand = new RelayCommand(OpenTemplateListView, HasDataContext);
            OpenAssignmentListViewCommand = new RelayCommand(OpenAssignmentListView, HasDataContext);
            OpenPgmListViewCommand = new RelayCommand(OpenPgmListView, HasDataContext);
            OpenCanvasListViewCommand = new RelayCommand(OpenCanvasListView, HasDataContext);
            OpenSettingsViewCommand = new RelayCommand(OpenSettingsView);
            OpenLogListViewCommand = new RelayCommand(OpenLogListView);
            OpenHelpViewCommand = new RelayCommand(OpenHelpView);
            OpenAboutViewCommand = new RelayCommand(OpenAboutView);
            ExitCommand = new RelayCommand(Exit);
        }

        public bool HasActiveDocument()
        {
            return ActiveDocument != null;
        }

        public bool HasDataContext()
        {
            return DataContext != null;
        }

        private T GetDocument<T>(Predicate<T> predicate = null) where T : ViewModelBase
        {
            foreach (T document in Documents.OfType<T>())
            {
                if (predicate == null || predicate(document))
                {
                    return document;
                }
            }
            return null;
        }

        private bool ActivateDocument<T>(Predicate<T> predicate = null) where T : ViewModelBase
        {
            T document = GetDocument(predicate);
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
            Log.Logger.DebugFormat("Opening tab: {0}", document.GetType().Name);
            document.Closing += (sender, e) =>
            {
                CloseDocument(document);
            };
            Documents.Add(document);
            ActiveDocument = document;
        }

        private void CloseDocument(ViewModelBase document)
        {
            Log.Logger.DebugFormat("Closing tab: {0}", document.GetType().Name);
            Documents.Remove(document);
            if (Documents.Count == 0)
            {
                ActiveDocument = null;
            }
        }

        public void CloseActiveDocument()
        {
            ActiveDocument.Close();
        }

        private void OpenDataSourceInternal(string path)
        {
            try
            {
                foreach (ViewModelBase document in Documents.ToList())
                {
                    document.Close();
                }
                using (new WaitCursor())
                {
                    DataContext = new DataContext(new Project(path));
                }
                Title = string.Format("{0} - {1}", App.Title, DataContext.Project.Name);
                OpenHelpView();
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to open data source", ex);
                Messenger.Default.Send(new AlertMessage
                {
                    Message = "Failed to open data source."
                });
                OpenDataSourceListView();
            }
        }

        public void OpenDataSource(string path)
        {
            if (HasDataContext())
            {
                if (DataContext.Project.FilePath.EqualsIgnoreCase(path))
                {
                    GetDocument<DataSourceListViewModel>()?.Close();
                    return;
                }
                ConfirmMessage msg = new ConfirmMessage
                {
                    Verb = "Open",
                    Message = "Open data source? This will close the current data source."
                };
                msg.Confirmed += (sender, e) =>
                {
                    OpenDataSourceInternal(path);
                };
                Messenger.Default.Send(msg);
            }
            else
            {
                OpenDataSourceInternal(path);
            }
        }

        public void OpenDataSourceListView()
        {
            if (!ActivateDocument<DataSourceListViewModel>())
            {
                OpenDocument(new DataSourceListViewModel());
            }
        }

        public void OpenResponderListView()
        {
            if (!ActivateDocument<ResponderListViewModel>())
            {
                using (new WaitCursor())
                {
                    OpenDocument(new ResponderListViewModel());
                }
            }
        }

        public void OpenResponderDetailView(Responder responder)
        {
            if (!ActivateDocument<ResponderDetailViewModel>(document => document.Responder.ResponderId.EqualsIgnoreCase(responder.ResponderId)))
            {
                using (new WaitCursor())
                {
                    OpenDocument(new ResponderDetailViewModel(responder));
                }
            }
        }

        public void CreateResponder()
        {
            using (new WaitCursor())
            {
                OpenDocument(new ResponderDetailViewModel(DataContext.Responders.Create()));
            }
        }

        public void OpenIncidentListView()
        {
            if (!ActivateDocument<IncidentListViewModel>())
            {
                using (new WaitCursor())
                {
                    OpenDocument(new IncidentListViewModel());
                }
            }
        }

        public void OpenIncidentView(Incident incident)
        {
            if (!ActivateDocument<IncidentViewModel>(document => document.Incident.IncidentId.EqualsIgnoreCase(incident.IncidentId)))
            {
                using (new WaitCursor())
                {
                    OpenDocument(new IncidentViewModel(incident));
                }
            }
        }

        public void CreateIncident()
        {
            using (new WaitCursor())
            {
                OpenDocument(new IncidentViewModel(DataContext.Incidents.Create()));
            }
        }

        public void OpenLocationDetailView(Location location)
        {
            if (!ActivateDocument<LocationDetailViewModel>(document => document.Location.LocationId.EqualsIgnoreCase(location.LocationId)))
            {
                OpenDocument(new LocationDetailViewModel(location));
            }
        }

        public void OpenViewListView()
        {
            if (!ActivateDocument<ViewListViewModel>())
            {
                using (new WaitCursor())
                {
                    OpenDocument(new ViewListViewModel(null));
                }
            }
        }

        public void OpenRecordListView(View view)
        {
            if (!ActivateDocument<RecordListViewModel>(document => document.View.Id == view.Id))
            {
                using (new WaitCursor())
                {
                    OpenDocument(new RecordListViewModel(view));
                }
            }
        }

        public void OpenTemplateListView()
        {
            if (!ActivateDocument<TemplateListViewModel>())
            {
                OpenDocument(new TemplateListViewModel(null));
            }
        }

        public void OpenAssignmentListView()
        {
            if (!ActivateDocument<AssignmentListViewModel>())
            {
                using (new WaitCursor())
                {
                    OpenDocument(new AssignmentListViewModel(null));
                }
            }
        }

        public void OpenPgmListView()
        {
            if (!ActivateDocument<PgmListViewModel>())
            {
                using (new WaitCursor())
                {
                    OpenDocument(new PgmListViewModel(null));
                }
            }
        }

        public void OpenCanvasListView()
        {
            if (!ActivateDocument<CanvasListViewModel>())
            {
                using (new WaitCursor())
                {
                    OpenDocument(new CanvasListViewModel(null));
                }
            }
        }

        public void OpenEmailView(EmailViewModel email)
        {
            OpenDocument(email);
        }

        public void OpenSettingsView()
        {
            if (!ActivateDocument<SettingsViewModel>())
            {
                OpenDocument(new SettingsViewModel());
            }
        }

        public void OpenLogListView()
        {
            if (!ActivateDocument<LogListViewModel>())
            {
                OpenDocument(new LogListViewModel());
            }
        }

        public void OpenHelpView()
        {
            if (!ActivateDocument<HelpViewModel>())
            {
                OpenDocument(new HelpViewModel());
            }
        }

        public void OpenAboutView()
        {
            if (!ActivateDocument<AboutViewModel>())
            {
                OpenDocument(new AboutViewModel());
            }
        }

        public void Exit()
        {
            Messenger.Default.Send(new ExitMessage());
        }
    }
}
