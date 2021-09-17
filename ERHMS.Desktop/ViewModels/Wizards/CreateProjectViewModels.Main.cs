using Epi;
using ERHMS.Data;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Naming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public static partial class CreateProjectViewModels
    {
        public partial class State
        {
            public CoreProject CoreProject { get; }
            public ProjectInfo ProjectInfo { get; set; }
            public IDatabase Database { get; set; }
            public DatabaseStatus DatabaseStatus { get; set; }
            public Project Project { get; set; }

            public State(CoreProject coreProject)
            {
                CoreProject = coreProject;
            }
        }

        public class SetProjectInfoViewModel : StepViewModel<State>
        {
            private readonly IDirectoryDialogService directoryDialog;
            private readonly IDictionary<DatabaseProvider, ConnectionInfoViewModel> connectionInfosByDatabaseProvider;

            public override string Title => Strings.CreateProject_Lead_SetProjectInfo;

            private string name;
            public string Name
            {
                get { return name; }
                set { SetProperty(ref name, value); }
            }

            private string locationRoot = EpiInfo.Configuration.Instance.Directories.Projects;
            public string LocationRoot
            {
                get { return locationRoot; }
                private set { SetProperty(ref locationRoot, value); }
            }

            public ListCollectionView<DatabaseProvider> DatabaseProviders { get; }
            public ConnectionInfoViewModel ConnectionInfo =>
                connectionInfosByDatabaseProvider[DatabaseProviders.CurrentItem];

            public ICommand BrowseCommand { get; }

            public SetProjectInfoViewModel(State state)
                : base(state)
            {
                Directory.CreateDirectory(EpiInfo.Configuration.Instance.Directories.Projects);
                directoryDialog = ServiceLocator.Resolve<IDirectoryDialogService>();
                directoryDialog.Directory = LocationRoot;
                DatabaseProviders = new ListCollectionView<DatabaseProvider>(
                    DatabaseProvider.Access2003,
                    DatabaseProvider.SqlServer);
                DatabaseProviders.CurrentChanged += DatabaseProviders_CurrentChanged;
                connectionInfosByDatabaseProvider = new Dictionary<DatabaseProvider, ConnectionInfoViewModel>
                {
                    { DatabaseProvider.Access2003, new ConnectionInfoViewModel.Access2003() },
                    { DatabaseProvider.SqlServer, new ConnectionInfoViewModel.SqlServer() }
                };
                BrowseCommand = new SyncCommand(Browse);
            }

            private void DatabaseProviders_CurrentChanged(object sender, EventArgs e)
            {
                OnPropertyChanged(nameof(ConnectionInfo));
            }

            public void Browse()
            {
                if (!directoryDialog.Open().GetValueOrDefault())
                {
                    return;
                }
                LocationRoot = directoryDialog.Directory;
            }

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                ProjectNameValidator validator = new ProjectNameValidator(LocationRoot);
                bool valid = validator.IsValid(Name, out InvalidNameReason reason);
                if (reason != InvalidNameReason.None)
                {
                    IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                    dialog.Severity = DialogSeverity.Warning;
                    dialog.Lead = reason.GetLead();
                    dialog.Body = reason.GetProjectBody();
                    dialog.Buttons = DialogButtonCollection.Close;
                    dialog.Show();
                }
                if (!valid || !ConnectionInfo.Validate())
                {
                    return;
                }
                ProjectInfo projectInfo = new ProjectInfo
                {
                    Name = Name,
                    LocationRoot = LocationRoot
                };
                string connectionString = ConnectionInfo.GetConnectionString(projectInfo.FilePath);
                IDatabase database = DatabaseProviders.CurrentItem.ToDatabase(connectionString);
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                DatabaseStatus databaseStatus = await progress.Run(() =>
                {
                    return database.GetStatus();
                });
                State.ProjectInfo = projectInfo;
                State.Database = database;
                State.DatabaseStatus = databaseStatus;
                if (databaseStatus == DatabaseStatus.Initialized)
                {
                    Wizard.GoForward(new Blank.CommitViewModel(State));
                }
                else
                {
                    Wizard.GoForward(new SetStrategyViewModel(State));
                }
            }
        }

        public class SetStrategyViewModel : StepViewModel<State>
        {
            public override string Title => Strings.CreateProject_Lead_SetStrategy;
            public IEnumerable<CoreView> CoreViews => CoreView.GetInstances(State.CoreProject);

            private bool expanded;
            public bool Expanded
            {
                get { return expanded; }
                set { SetProperty(ref expanded, value); }
            }

            public ICommand CreateStandardCommand { get; }
            public ICommand CreateBlankCommand { get; }
            public ICommand CreateFromTemplateCommand { get; }
            public ICommand CreateFromExistingCommand { get; }

            public SetStrategyViewModel(State state)
                : base(state)
            {
                CreateStandardCommand = new SyncCommand(CreateStandard);
                CreateBlankCommand = new SyncCommand(CreateBlank);
                CreateFromTemplateCommand = new SyncCommand(CreateFromTemplate);
                CreateFromExistingCommand = new SyncCommand(CreateFromExisting);
            }

            public void CreateStandard()
            {
                Wizard.GoForward(new Standard.CommitViewModel(State));
            }

            public void CreateBlank()
            {
                Wizard.GoForward(new Blank.CommitViewModel(State));
            }

            public void CreateFromTemplate()
            {
                Wizard.GoForward(new FromTemplate.SetXTemplateViewModel(State));
            }

            public void CreateFromExisting()
            {
                Wizard.GoForward(new FromExisting.SetSourceProjectViewModel(State));
            }
        }

        public abstract class CommitViewModel : StepViewModel<State>
        {
            public override string Title => Strings.Lead_Commit;
            public override string ContinueAction => Strings.AccessText_Finish;
            public DetailsViewModel Details { get; }
            protected IProgress<string> Progress { get; private set; }

            public CommitViewModel(State state)
                : base(state)
            {
                Details = new DetailsViewModel
                {
                    { Strings.Label_Name, state.ProjectInfo.Name },
                    { Strings.Label_LocationRoot, state.ProjectInfo.LocationRoot },
                    { Strings.Label_DatabaseProvider, state.Database.Provider },
                    { Strings.Label_ConnectionInfo, state.Database.GetConnectionStringBuilder() },
                    { Strings.Label_DatabaseStatus, state.DatabaseStatus }
                };
            }

            protected abstract void ContinueCore(Project project);

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Lead = Strings.Lead_CreatingProject;
                State.Project = await progress.Run(() =>
                {
                    if (!State.Database.Exists())
                    {
                        State.Database.Create();
                    }
                    Project project = ProjectExtensions.Create(State.ProjectInfo, State.Database);
                    if (!project.IsInitialized())
                    {
                        progress.Report(Strings.Body_Initializing);
                        project.Initialize();
                    }
                    Progress = progress;
                    try
                    {
                        ContinueCore(project);
                    }
                    finally
                    {
                        Progress = null;
                    }
                    return project;
                });
                Configuration.Instance.SetProjectPath(State.CoreProject, State.Project.FilePath);
                Configuration.Instance.Save();
                Wizard.Result = true;
                Wizard.Committed = true;
                Wizard.GoForward(new CloseViewModel(State));
            }
        }

        public class CloseViewModel : StepViewModel<State>
        {
            public override string Title => Strings.CreateProject_Lead_Close;
            public override string ContinueAction => Strings.AccessText_Close;

            public CloseViewModel(State state)
                : base(state) { }

            public override bool CanContinue()
            {
                return true;
            }

            public override Task ContinueAsync()
            {
                Wizard.Close();
                return Task.CompletedTask;
            }
        }

        public static WizardViewModel GetWizard(CoreProject coreProject)
        {
            State state = new State(coreProject);
            StepViewModel step = new SetProjectInfoViewModel(state);
            return new WizardViewModel(step);
        }
    }
}
