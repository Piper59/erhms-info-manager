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
using System.Threading.Tasks;
using System.Windows.Input;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public static partial class CreateProjectViewModels
    {
        public partial class State
        {
            public CoreProject CoreProject { get; }
            public Project Project { get; set; }
            public ProjectCreationInfo ProjectCreationInfo { get; set; }

            public State(CoreProject coreProject)
            {
                CoreProject = coreProject;
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
                Wizard.GoForward(new Standard.SetProjectCreationInfoViewModel(State));
            }

            public void CreateBlank()
            {
                Wizard.GoForward(new Blank.SetProjectCreationInfoViewModel(State));
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

        public abstract class SetProjectCreationInfoViewModel : StepViewModel<State>
        {
            private readonly IDirectoryDialogService directoryDialog;
            private readonly IDictionary<DatabaseProvider, ConnectionInfoViewModel> connectionInfosByDatabaseProvider;

            public override string Title => Strings.CreateProject_Lead_SetProjectCreationInfo;

            private string name;
            public string Name
            {
                get { return name; }
                set { SetProperty(ref name, value); }
            }

            private string description;
            public string Description
            {
                get { return description; }
                set { SetProperty(ref description, value); }
            }

            private string locationRoot = Configuration.Instance.GetProjectsDirectory();
            public string LocationRoot
            {
                get { return locationRoot; }
                private set { SetProperty(ref locationRoot, value); }
            }

            public ListCollectionView<DatabaseProvider> DatabaseProviders { get; }
            public ConnectionInfoViewModel ConnectionInfo =>
                connectionInfosByDatabaseProvider[DatabaseProviders.CurrentItem];

            public ICommand BrowseCommand { get; }

            protected SetProjectCreationInfoViewModel(State state)
                : base(state)
            {
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
                if (directoryDialog.Open() != true)
                {
                    return;
                }
                LocationRoot = directoryDialog.Directory;
            }

            protected abstract StepViewModel GetSubsequent();

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Lead = Strings.Lead_ValidatingName;
                InvalidNameReason reason = InvalidNameReason.None;
                bool valid = await progress.Run(() =>
                {
                    ProjectNameValidator validator = new ProjectNameValidator(LocationRoot);
                    return validator.IsValid(Name, out reason);
                });
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
                ProjectCreationInfo projectCreationInfo = new ProjectCreationInfo
                {
                    Name = Name,
                    Description = Description ?? "",
                    LocationRoot = LocationRoot
                };
                string connectionString = ConnectionInfo.GetConnectionString(projectCreationInfo.FilePath);
                projectCreationInfo.Database = DatabaseProviders.CurrentItem.ToDatabase(connectionString);
                State.ProjectCreationInfo = projectCreationInfo;
                Wizard.GoForward(GetSubsequent());
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
                    { Strings.Label_Name, state.ProjectCreationInfo.Name },
                    { Strings.Label_Description, state.ProjectCreationInfo.Description },
                    { Strings.Label_LocationRoot, state.ProjectCreationInfo.LocationRoot },
                    { Strings.Label_DatabaseProvider, state.ProjectCreationInfo.Database.Provider },
                    {
                        Strings.Label_ConnectionInfo,
                        state.ProjectCreationInfo.Database.GetConnectionStringBuilder()
                    }
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
                    if (!State.ProjectCreationInfo.Database.Exists())
                    {
                        State.ProjectCreationInfo.Database.Create();
                    }
                    Project project = ProjectExtensions.Create(State.ProjectCreationInfo);
                    progress.Report(Strings.Body_Initializing);
                    if (project.IsInitialized())
                    {
                        // TODO: Confirm
                    }
                    else
                    {
                        project.Initialize();
                    }
                    Progress = progress;
                    ContinueCore(project);
                    return project;
                });
                Settings.Default.SetProjectPath(State.CoreProject, State.Project.FilePath);
                Settings.Default.Save();
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
            StepViewModel step = new SetStrategyViewModel(state);
            return new WizardViewModel(step);
        }
    }
}
