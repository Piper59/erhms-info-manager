using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Presentation.ViewModels
{
    public class MainViewModel : GalaSoft.MvvmLight.ViewModelBase, IDocumentManager
    {
        private static ICollection<Type> ContextSafeDocumentTypes = new Type[]
        {
            typeof(AboutViewModel),
            typeof(HelpViewModel),
            typeof(LogListViewModel),
            typeof(SettingsViewModel),
            typeof(StartViewModel)
        };

        public IServiceManager Services { get; private set; }

        private string title;
        public string Title
        {
            get { return title; }
            private set { Set(nameof(Title), ref title, value); }
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
                        Log.Logger.DebugFormat("Activating tab: {0}", value);
                    }
                }
            }
        }

        public DataContext Context
        {
            get
            {
                return Services.Context;
            }
            private set
            {
                Services.Context = value;
                RaisePropertyChanged(nameof(Context));
            }
        }

        public RelayCommand ShowDataSourcesCommand { get; private set; }
        public RelayCommand ShowRespondersCommand { get; private set; }
        public RelayCommand ShowNewResponderCommand { get; private set; }
        public RelayCommand ShowIncidentsCommand { get; private set; }
        public RelayCommand ShowNewIncidentCommand { get; private set; }
        public RelayCommand ShowViewsCommand { get; private set; }
        public RelayCommand ShowTemplatesCommand { get; private set; }
        public RelayCommand ShowAssignmentsCommand { get; private set; }
        public RelayCommand ShowPgmsCommand { get; private set; }
        public RelayCommand ShowCanvasesCommand { get; private set; }
        public RelayCommand ShowStartCommand { get; private set; }
        public RelayCommand ShowSettingsCommand { get; private set; }
        public RelayCommand ShowLogsCommand { get; private set; }
        public RelayCommand ShowHelpCommand { get; private set; }
        public RelayCommand ShowAboutCommand { get; private set; }
        public RelayCommand CloseCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }

        public MainViewModel(IServiceManager services)
        {
            Services = services;
            Title = App.Title;
            Documents = new ObservableCollection<ViewModelBase>();
            ShowDataSourcesCommand = new RelayCommand(ShowDataSources);
            ShowRespondersCommand = new RelayCommand(ShowResponders, HasContext);
            ShowNewResponderCommand = new RelayCommand(ShowNewResponder, HasContext);
            ShowIncidentsCommand = new RelayCommand(ShowIncidents, HasContext);
            ShowNewIncidentCommand = new RelayCommand(ShowNewIncident, HasContext);
            ShowViewsCommand = new RelayCommand(ShowViews, HasContext);
            ShowTemplatesCommand = new RelayCommand(ShowTemplates, HasContext);
            ShowAssignmentsCommand = new RelayCommand(ShowAssignments, HasContext);
            ShowPgmsCommand = new RelayCommand(ShowPgms, HasContext);
            ShowCanvasesCommand = new RelayCommand(ShowCanvases, HasContext);
            ShowStartCommand = new RelayCommand(ShowStart);
            ShowSettingsCommand = new RelayCommand(ShowSettings);
            ShowLogsCommand = new RelayCommand(ShowLogs);
            ShowHelpCommand = new RelayCommand(ShowHelp);
            ShowAboutCommand = new RelayCommand(ShowAbout);
            CloseCommand = new RelayCommand(Close, HasActiveDocument);
            ExitCommand = new RelayCommand(Exit);
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ActiveDocument))
                {
                    CloseCommand.RaiseCanExecuteChanged();
                }
                else if (e.PropertyName == nameof(Context))
                {
                    ShowRespondersCommand.RaiseCanExecuteChanged();
                    ShowNewResponderCommand.RaiseCanExecuteChanged();
                    ShowIncidentsCommand.RaiseCanExecuteChanged();
                    ShowNewIncidentCommand.RaiseCanExecuteChanged();
                    ShowViewsCommand.RaiseCanExecuteChanged();
                    ShowTemplatesCommand.RaiseCanExecuteChanged();
                    ShowAssignmentsCommand.RaiseCanExecuteChanged();
                    ShowPgmsCommand.RaiseCanExecuteChanged();
                    ShowCanvasesCommand.RaiseCanExecuteChanged();
                    ShowStartCommand.RaiseCanExecuteChanged();
                }
            };
        }

        public bool HasActiveDocument()
        {
            return ActiveDocument != null;
        }

        public bool HasContext()
        {
            return Context != null;
        }

        public void OpenDataSource(ProjectInfo dataSource)
        {
            if (dataSource.Version > Assembly.GetExecutingAssembly().GetName().Version)
            {
                ICollection<string> message = new List<string>();
                message.Add(string.Format("The selected data source was created in a newer version of {0}.", App.Title));
                message.Add(string.Format("Please upgrade to the latest version of {0} to open this data source.", App.Title));
                MessengerInstance.Send(new AlertMessage
                {
                    Title = "Help",
                    Message = string.Join(" ", message)
                });
            }
            else if (HasContext())
            {
                if (Context.Project.FilePath.EqualsIgnoreCase(dataSource.FilePath))
                {
                    Get<DataSourceListViewModel>()?.Close();
                    return;
                }
                ConfirmMessage msg = new ConfirmMessage
                {
                    Verb = "Open",
                    Message = "Open data source? This will close the current data source."
                };
                msg.Confirmed += (sender, e) =>
                {
                    OnDataSourceOpening(dataSource);
                };
                MessengerInstance.Send(msg);
            }
            else
            {
                OnDataSourceOpening(dataSource);
            }
        }

        private void OnDataSourceOpening(ProjectInfo dataSource)
        {
            try
            {
                foreach (ViewModelBase document in Documents.Where(document => !ContextSafeDocumentTypes.Contains(document.GetType())).ToList())
                {
                    document.Close(false);
                }
                using (new WaitCursor())
                {
                    Context = new DataContext(new Project(dataSource.FilePath));
                    Settings.Default.LastDataSourcePath = dataSource.FilePath;
                    Settings.Default.Save();
                }
                if (Context.NeedsUpgrade())
                {
                    ICollection<string> message = new List<string>();
                    message.Add(string.Format("The selected data source was created in an older version of {0}.", App.Title));
                    message.Add("Upgrade this data source?");
                    message.Add(string.Format("This may make the data source inaccessible to other users with older versions of {0}.", App.Title));
                    ConfirmMessage msg = new ConfirmMessage
                    {
                        Verb = "Upgrade",
                        Message = string.Join(" ", message)
                    };
                    msg.Confirmed += (sender, e) =>
                    {
                        Context.Upgrade();
                        MessengerInstance.Send(new ToastMessage
                        {
                            Message = "Data source has been upgraded."
                        });
                        OnDataSourceOpened();
                    };
                    msg.Canceled += (sender, e) =>
                    {
                        Context = null;
                        ShowDataSources();
                    };
                    MessengerInstance.Send(msg);
                }
                else
                {
                    OnDataSourceOpened();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to open data source", ex);
                Services.Dialogs.ShowErrorAsync("Failed to open data source.", ex);
                ShowDataSources();
            }
        }

        private void OnDataSourceOpened()
        {
            Title = string.Format("{0} - {1}", App.Title, Context.Project.Name);
            ShowStart();
        }

        private TViewModel Get<TViewModel>(Func<TViewModel, bool> predicate = null)
            where TViewModel : ViewModelBase
        {
            foreach (TViewModel document in Documents.OfType<TViewModel>())
            {
                if (predicate == null || predicate(document))
                {
                    return document;
                }
            }
            return null;
        }

        private TViewModel Activate<TViewModel>(Func<TViewModel, bool> predicate = null)
            where TViewModel : ViewModelBase
        {
            TViewModel document = Get(predicate);
            if (document != null)
            {
                ActiveDocument = document;
            }
            return document;
        }

        private void Open(ViewModelBase document)
        {
            Log.Logger.DebugFormat("Opening tab: {0}", document);
            document.Closed += (sender, e) =>
            {
                Close(document);
            };
            Documents.Add(document);
            ActiveDocument = document;
        }

        public TViewModel Show<TViewModel>(Func<TViewModel> constructor, Func<TViewModel, bool> predicate = null)
            where TViewModel : ViewModelBase
        {
            TViewModel document = Activate(predicate);
            if (document == null)
            {
                using (new WaitCursor())
                {
                    document = constructor();
                    Open(document);
                }
            }
            return document;
        }

        public void ShowDataSources()
        {
            Show(() => new DataSourceListViewModel(Services));
        }

        public void ShowResponders()
        {
            Show(() => new ResponderListViewModel(Services));
        }

        public void ShowResponder(Responder responder)
        {
            Show(
                () => new ResponderDetailViewModel(Services, responder),
                document => document.Responder.ResponderId.EqualsIgnoreCase(responder.ResponderId));
        }

        public void ShowNewResponder()
        {
            ShowResponder(new Responder(true));
        }

        public void ShowIncidents()
        {
            Show(() => new IncidentListViewModel(Services));
        }

        public void ShowIncident(Incident incident)
        {
            Show(
                () => new IncidentViewModel(Services, incident),
                document => document.Incident.IncidentId.EqualsIgnoreCase(incident.IncidentId));
        }

        public void ShowNewIncident()
        {
            ShowIncident(new Incident(true));
        }

        public void ShowTeam(Team team)
        {
            Show(
                () => new TeamViewModel(Services, team),
                document => document.Team.TeamId.EqualsIgnoreCase(team.TeamId));
        }

        public void ShowLocation(Location location)
        {
            Show(
                () => new LocationDetailViewModel(Services, location),
                document => document.Location.LocationId.EqualsIgnoreCase(location.LocationId));
        }

        public void ShowJob(Job job)
        {
            Show(
                () => new JobViewModel(Services, job),
                document => document.Job.JobId.EqualsIgnoreCase(job.JobId));
        }

        public void ShowViews()
        {
            Show(() => new ViewListViewModel(Services, null));
        }

        public void ShowRecords(Epi.View view)
        {
            Show(
                () => new RecordListViewModel(Services, view),
                document => document.Entities.View.Id == view.Id);
        }

        public void ShowTemplates()
        {
            Show(() => new TemplateListViewModel(Services, null));
        }

        public void ShowAssignments()
        {
            Show(() => new AssignmentListViewModel(Services, null));
        }

        public void ShowPgms()
        {
            Show(() => new PgmListViewModel(Services, null));
        }

        public void ShowCanvases()
        {
            Show(() => new CanvasListViewModel(Services, null));
        }

        public void ShowStart()
        {
            Show(() => new StartViewModel(Services));
        }

        public void ShowSettings()
        {
            Show(() => new SettingsViewModel(Services));
        }

        public async void ShowSettings(string message, Exception exception = null)
        {
            if (exception == null)
            {
                AlertMessage msg = new AlertMessage
                {
                    Message = message
                };
                msg.Dismissed += (sender, e) =>
                {
                    ShowSettings();
                };
                MessengerInstance.Send(msg);
            }
            else
            {
                await Services.Dialogs.ShowErrorAsync(message, exception);
                ShowSettings();
            }
        }

        public void ShowLogs()
        {
            Show(() => new LogListViewModel(Services));
        }

        public void ShowHelp()
        {
            Show(() => new HelpViewModel(Services));
        }

        public void ShowAbout()
        {
            Show(() => new AboutViewModel(Services));
        }

        private void Close(ViewModelBase document)
        {
            Log.Logger.DebugFormat("Closing tab: {0}", document);
            Documents.Remove(document);
            if (Documents.Count == 0)
            {
                ActiveDocument = null;
            }
        }

        public void Close()
        {
            ActiveDocument.Close();
        }

        public void Exit()
        {
            MessengerInstance.Send(new ExitMessage());
        }
    }
}
