using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Presentation.ViewModels
{
    public class MainViewModel : GalaSoft.MvvmLight.ViewModelBase, IDocumentManager
    {
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
                    closeCommand.RaiseCanExecuteChanged();
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
                showRespondersCommand.RaiseCanExecuteChanged();
                showNewResponderCommand.RaiseCanExecuteChanged();
                showIncidentsCommand.RaiseCanExecuteChanged();
                showNewIncidentCommand.RaiseCanExecuteChanged();
                showViewsCommand.RaiseCanExecuteChanged();
                showTemplatesCommand.RaiseCanExecuteChanged();
                showAssignmentsCommand.RaiseCanExecuteChanged();
                showPgmsCommand.RaiseCanExecuteChanged();
                showCanvasesCommand.RaiseCanExecuteChanged();
            }
        }

        private RelayCommand showDataSourcesCommand;
        public ICommand ShowDataSourcesCommand
        {
            get { return showDataSourcesCommand ?? (showDataSourcesCommand = new RelayCommand(ShowDataSources)); }
        }

        private RelayCommand showRespondersCommand;
        public ICommand ShowRespondersCommand
        {
            get { return showRespondersCommand ?? (showRespondersCommand = new RelayCommand(ShowResponders, HasContext)); }
        }

        private RelayCommand showNewResponderCommand;
        public ICommand ShowNewResponderCommand
        {
            get { return showNewResponderCommand ?? (showNewResponderCommand = new RelayCommand(ShowNewResponder, HasContext)); }
        }

        private RelayCommand showIncidentsCommand;
        public ICommand ShowIncidentsCommand
        {
            get { return showIncidentsCommand ?? (showIncidentsCommand = new RelayCommand(ShowIncidents, HasContext)); }
        }

        private RelayCommand showNewIncidentCommand;
        public ICommand ShowNewIncidentCommand
        {
            get { return showNewIncidentCommand ?? (showNewIncidentCommand = new RelayCommand(ShowNewIncident, HasContext)); }
        }

        private RelayCommand showViewsCommand;
        public ICommand ShowViewsCommand
        {
            get { return showViewsCommand ?? (showViewsCommand = new RelayCommand(ShowViews, HasContext)); }
        }

        private RelayCommand showTemplatesCommand;
        public ICommand ShowTemplatesCommand
        {
            get { return showTemplatesCommand ?? (showTemplatesCommand = new RelayCommand(ShowTemplates, HasContext)); }
        }

        private RelayCommand showAssignmentsCommand;
        public ICommand ShowAssignmentsCommand
        {
            get { return showAssignmentsCommand ?? (showAssignmentsCommand = new RelayCommand(ShowAssignments, HasContext)); }
        }

        private RelayCommand showPgmsCommand;
        public ICommand ShowPgmsCommand
        {
            get { return showPgmsCommand ?? (showPgmsCommand = new RelayCommand(ShowPgms, HasContext)); }
        }

        private RelayCommand showCanvasesCommand;
        public ICommand ShowCanvasesCommand
        {
            get { return showCanvasesCommand ?? (showCanvasesCommand = new RelayCommand(ShowCanvases, HasContext)); }
        }

        private RelayCommand showSettingsCommand;
        public ICommand ShowSettingsCommand
        {
            get { return showSettingsCommand ?? (showSettingsCommand = new RelayCommand(ShowSettings)); }
        }

        private RelayCommand showLogsCommand;
        public ICommand ShowLogsCommand
        {
            get { return showLogsCommand ?? (showLogsCommand = new RelayCommand(ShowLogs)); }
        }

        private RelayCommand showHelpCommand;
        public ICommand ShowHelpCommand
        {
            get { return showHelpCommand ?? (showHelpCommand = new RelayCommand(ShowHelp)); }
        }

        private RelayCommand showAboutCommand;
        public ICommand ShowAboutCommand
        {
            get { return showAboutCommand ?? (showAboutCommand = new RelayCommand(ShowAbout)); }
        }

        private RelayCommand closeCommand;
        public ICommand CloseCommand
        {
            get { return closeCommand ?? (closeCommand = new RelayCommand(Close, HasActiveDocument)); }
        }

        private RelayCommand exitCommand;
        public ICommand ExitCommand
        {
            get { return exitCommand ?? (exitCommand = new RelayCommand(Exit)); }
        }

        public MainViewModel(IServiceManager services)
        {
            Services = services;
            Title = App.Title;
            Documents = new ObservableCollection<ViewModelBase>();
        }

        public bool HasActiveDocument()
        {
            return ActiveDocument != null;
        }

        public bool HasContext()
        {
            return Context != null;
        }

        private void OpenDataSourceInternal(string path)
        {
            try
            {
                foreach (ViewModelBase document in Documents.ToList())
                {
                    document.Close(false);
                }
                using (new WaitCursor())
                {
                    Context = new DataContext(new Project(path));
                }
                Title = string.Format("{0} - {1}", App.Title, Context.Project.Name);
                ShowHelp();
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to open data source", ex);
                Services.Dialogs.ShowErrorAsync("Failed to open data source.", ex);
                ShowDataSources();
            }
        }

        public void OpenDataSource(string path)
        {
            if (HasContext())
            {
                if (Context.Project.FilePath.EqualsIgnoreCase(path))
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
                    OpenDataSourceInternal(path);
                };
                MessengerInstance.Send(msg);
            }
            else
            {
                OpenDataSourceInternal(path);
            }
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

        private bool Activate<TViewModel>(Func<TViewModel, bool> predicate = null)
            where TViewModel : ViewModelBase
        {
            TViewModel document = Get(predicate);
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

        public void Show<TViewModel>(Func<TViewModel> constructor, Func<TViewModel, bool> predicate = null)
            where TViewModel : ViewModelBase
        {
            if (!Activate(predicate))
            {
                using (new WaitCursor())
                {
                    Open(constructor());
                }
            }
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
            ShowResponder(new Responder());
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
            ShowIncident(new Incident());
        }

        public void ShowLocation(Location location)
        {
            Show(
                () => new LocationDetailViewModel(Services, location),
                document => document.Location.LocationId.EqualsIgnoreCase(location.LocationId));
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
