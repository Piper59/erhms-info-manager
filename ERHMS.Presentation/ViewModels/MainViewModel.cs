using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Presentation.ViewModels
{
    public class MainViewModel : ViewModelBase, IDocumentService
    {
        public ObservableCollection<DocumentViewModel> Documents { get; private set; }

        private DocumentViewModel activeDocument;
        public DocumentViewModel ActiveDocument
        {
            get
            {
                return activeDocument;
            }
            set
            {
                SetProperty(nameof(ActiveDocument), ref activeDocument, value);
                if (value != null)
                {
                    Log.Logger.DebugFormat("Activating document: {0}", value);
                }
            }
        }

        public ICommand ShowDataSourcesCommand { get; private set; }
        public ICommand ShowRespondersCommand { get; private set; }
        public ICommand CreateResponderCommand { get; private set; }
        public ICommand ShowIncidentsCommand { get; private set; }
        public ICommand CreateIncidentCommand { get; private set; }
        public ICommand ShowViewsCommand { get; private set; }
        public ICommand ShowTemplatesCommand { get; private set; }
        public ICommand ShowAssignmentsCommand { get; private set; }
        public ICommand ShowPgmsCommand { get; private set; }
        public ICommand ShowCanvasesCommand { get; private set; }
        public ICommand ShowStartCommand { get; private set; }
        public ICommand ShowSettingsCommand { get; private set; }
        public ICommand ShowLogsCommand { get; private set; }
        public ICommand ShowHelpCommand { get; private set; }
        public ICommand ShowAboutCommand { get; private set; }
        public ICommand CloseCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        public MainViewModel()
        {
            Title = Resources.AppTitle;
            Documents = new ObservableCollection<DocumentViewModel>();
            ShowDataSourcesCommand = new Command(ShowDataSources);
            ShowRespondersCommand = new Command(ShowResponders, HasContext);
            CreateResponderCommand = new Command(CreateResponder, HasContext);
            ShowIncidentsCommand = new Command(ShowIncidents, HasContext);
            CreateIncidentCommand = new Command(CreateIncident, HasContext);
            ShowViewsCommand = new Command(ShowViews, HasContext);
            ShowTemplatesCommand = new Command(ShowTemplates, HasContext);
            ShowAssignmentsCommand = new Command(ShowAssignments, HasContext);
            ShowPgmsCommand = new Command(ShowPgms, HasContext);
            ShowCanvasesCommand = new Command(ShowCanvases, HasContext);
            ShowStartCommand = new Command(ShowStart);
            ShowSettingsCommand = new Command(ShowSettings);
            ShowLogsCommand = new Command(ShowLogs);
            ShowHelpCommand = new Command(ShowHelp);
            ShowAboutCommand = new Command(ShowAbout);
            CloseCommand = new AsyncCommand(CloseAsync, CanClose);
            ExitCommand = new Command(Exit);
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ActiveDocument))
                {
                    CloseCommand.OnCanExecuteChanged();
                }
                else if (e.PropertyName == nameof(Context))
                {
                    ShowRespondersCommand.OnCanExecuteChanged();
                    CreateResponderCommand.OnCanExecuteChanged();
                    ShowIncidentsCommand.OnCanExecuteChanged();
                    CreateIncidentCommand.OnCanExecuteChanged();
                    ShowViewsCommand.OnCanExecuteChanged();
                    ShowTemplatesCommand.OnCanExecuteChanged();
                    ShowAssignmentsCommand.OnCanExecuteChanged();
                    ShowPgmsCommand.OnCanExecuteChanged();
                    ShowCanvasesCommand.OnCanExecuteChanged();
                }
            };
        }

        public async Task SetContextAsync(ProjectInfo projectInfo)
        {
            if (projectInfo.Version > Assembly.GetExecutingAssembly().GetName().Version)
            {
                await ServiceLocator.Dialog.AlertAsync(Resources.DataSourceNewerVersion, "Help");
                return;
            }
            if (Context == null)
            {
                await SetContextInternalAsync(projectInfo);
            }
            else
            {
                if (Context.Project.FilePath.Equals(projectInfo.FilePath, StringComparison.OrdinalIgnoreCase))
                {
                    await OnContextSetAsync();
                }
                else
                {
                    if (await ServiceLocator.Dialog.ConfirmAsync(Resources.DataSourceConfirmCloseAndOpen, "Open"))
                    {
                        await SetContextInternalAsync(projectInfo);
                    }
                }
            }
        }

        private async Task SetContextInternalAsync(ProjectInfo projectInfo)
        {
            try
            {
                foreach (DocumentViewModel model in Documents.ToList())
                {
                    if (!model.GetType().HasCustomAttribute<ContextSafeAttribute>())
                    {
                        await model.CloseAsync(false);
                    }
                }
                Context = new DataContext(new Project(projectInfo.FilePath));
                if (Context.NeedsUpgrade())
                {
                    if (await ServiceLocator.Dialog.ConfirmAsync(Resources.DataSourceConfirmUpgrade, "Upgrade"))
                    {
                        await ServiceLocator.Dialog.BlockAsync(Resources.DataSourceUpgrading, () =>
                        {
                            Context.Upgrade();
                        });
                        ServiceLocator.Dialog.Notify(Resources.DataSourceUpgraded);
                    }
                    else
                    {
                        Context = null;
                        ShowDataSources();
                    }
                }
                if (Context != null)
                {
                    Settings.Default.LastDataSourcePath = projectInfo.FilePath;
                    Settings.Default.Save();
                    Title = string.Format("{0} - {1}", Resources.AppTitle, Context.Project.Name);
                    await OnContextSetAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Warn("Failed to open data source", ex);
                await ServiceLocator.Dialog.ShowErrorAsync(Resources.DataSourceOpenFailed, ex);
                ShowDataSources();
            }
        }

        private async Task OnContextSetAsync()
        {
            ServiceLocator.Dialog.Notify(Resources.DataSourceOpened);
            DataSourceListViewModel model = FindByType<DataSourceListViewModel>();
            if (model != null)
            {
                await model.CloseAsync();
            }
            ShowStart();
        }

        private TModel Find<TModel>(Func<TModel, bool> predicate)
            where TModel : DocumentViewModel
        {
            foreach (TModel model in Documents.OfType<TModel>())
            {
                if (predicate(model))
                {
                    return model;
                }
            }
            return null;
        }

        private TModel FindByType<TModel>()
            where TModel : DocumentViewModel
        {
            return Find<TModel>(model => true);
        }

        public TModel Show<TModel>(Func<TModel> constructor)
            where TModel : DocumentViewModel
        {
            return Show(model => false, constructor);
        }

        public TModel Show<TModel>(Func<TModel, bool> predicate, Func<TModel> constructor)
            where TModel : DocumentViewModel
        {
            TModel model = Find(predicate);
            if (model == null)
            {
                using (ServiceLocator.Busy.Begin())
                {
                    model = constructor();
                    Log.Logger.DebugFormat("Opening document: {0}", model);
                    model.Closed += (sender, e) =>
                    {
                        Log.Logger.DebugFormat("Closing document: {0}", model);
                        Documents.Remove(model);
                        if (Documents.Count == 0)
                        {
                            ActiveDocument = null;
                        }
                    };
                    Documents.Add(model);
                }
            }
            ActiveDocument = model;
            return model;
        }

        public TModel ShowByType<TModel>(Func<TModel> constructor)
            where TModel : DocumentViewModel
        {
            return Show(model => true, constructor);
        }

        public bool HasContext()
        {
            return Context != null;
        }

        public void ShowDataSources()
        {
            ShowByType(() => new DataSourceListViewModel());
        }

        public void ShowResponders()
        {
            ShowByType(() => new ResponderListViewModel());
        }

        public void CreateResponder()
        {
            Show(() => new ResponderViewModel(new Responder(true)));
        }

        public void ShowIncidents()
        {
            ShowByType(() => new IncidentListViewModel());
        }

        public void CreateIncident()
        {
            Show(() => new IncidentViewModel(new Incident(true)));
        }

        public void ShowViews()
        {
            ShowByType(() => new ViewListViewModel(null));
        }

        public void ShowTemplates()
        {
            ShowByType(() => new TemplateListViewModel(null));
        }

        public void ShowAssignments()
        {
            ShowByType(() => new AssignmentListViewModel(null));
        }

        public void ShowPgms()
        {
            ShowByType(() => new PgmListViewModel(null));
        }

        public void ShowCanvases()
        {
            ShowByType(() => new CanvasListViewModel(null));
        }

        public void ShowStart()
        {
            ShowByType(() => new StartViewModel());
        }

        public void ShowSettings()
        {
            ShowByType(() => new SettingsViewModel());
        }

        public void ShowLogs()
        {
            ShowByType(() => new LogListViewModel());
        }

        public void ShowHelp()
        {
            ShowByType(() => new HelpViewModel());
        }

        public void ShowAbout()
        {
            ShowByType(() => new AboutViewModel());
        }

        public bool CanClose()
        {
            return ActiveDocument != null;
        }

        public async Task CloseAsync()
        {
            await ActiveDocument.CloseAsync();
        }

        public void Exit()
        {
            ServiceLocator.App.Exit();
        }
    }
}
