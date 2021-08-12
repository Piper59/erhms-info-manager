using Epi;
using ERHMS.Data;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.Desktop.Wizards;
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
    public partial class CreateProjectViewModel : WizardViewModel
    {
        public class SetStrategyViewModel : StepViewModel<CreateProjectViewModel>
        {
            public override string Title => Strings.CreateProject_Lead_SetStrategy;
            public IEnumerable<CoreView> CoreViews => CoreView.GetInstances(Wizard.CoreProject);

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

            public SetStrategyViewModel(CreateProjectViewModel wizard)
                : base(wizard)
            {
                CreateStandardCommand = new SyncCommand(CreateStandard);
                CreateBlankCommand = new SyncCommand(CreateBlank);
                CreateFromTemplateCommand = new SyncCommand(CreateFromTemplate);
                CreateFromExistingCommand = new SyncCommand(CreateFromExisting);
            }

            public void CreateStandard()
            {
                GoToStep(new Standard.SetProjectCreationInfoViewModel(Wizard, this));
            }

            public void CreateBlank()
            {
                GoToStep(new Blank.SetProjectCreationInfoViewModel(Wizard, this));
            }

            public void CreateFromTemplate()
            {
                GoToStep(new FromTemplate.SetXTemplateViewModel(Wizard, this));
            }

            public void CreateFromExisting()
            {
                GoToStep(new FromExisting.SetSourceProjectViewModel(Wizard, this));
            }
        }

        public abstract class SetProjectCreationInfoViewModel : StepViewModel<CreateProjectViewModel>
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

            protected SetProjectCreationInfoViewModel(CreateProjectViewModel wizard, IStep antecedent)
                : base(wizard, antecedent)
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

            protected abstract void GoToNextStep();

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
                if (reason == InvalidNameReason.Identical)
                {
                    // TODO: Offer to open
                    // TODO: Check for core views
                }
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
                Wizard.ProjectCreationInfo = projectCreationInfo;
                GoToNextStep();
            }
        }

        public abstract class CommitViewModel : StepViewModel<CreateProjectViewModel>
        {
            public override string Title => Strings.Lead_Commit;
            public override string ContinueAction => Strings.AccessText_Finish;
            public DetailsViewModel Details { get; }

            public CommitViewModel(CreateProjectViewModel wizard, IStep antecedent)
                : base(wizard, antecedent)
            {
                Details = new DetailsViewModel
                {
                    { Strings.Label_Name, wizard.ProjectCreationInfo.Name },
                    { Strings.Label_Description, wizard.ProjectCreationInfo.Description },
                    { Strings.Label_LocationRoot, wizard.ProjectCreationInfo.LocationRoot },
                    { Strings.Label_DatabaseProvider, wizard.ProjectCreationInfo.Database.Provider },
                    {
                        Strings.Label_ConnectionInfo,
                        wizard.ProjectCreationInfo.Database.GetConnectionStringBuilder()
                    }
                };
            }

            protected abstract void ContinueCore(Project project, IProgress<string> progress);

            public override bool CanContinue()
            {
                return true;
            }

            public override async Task ContinueAsync()
            {
                IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                progress.Lead = Strings.Lead_CreatingProject;
                Wizard.Project = await progress.Run(() =>
                {
                    if (Wizard.ProjectCreationInfo.Database.Exists())
                    {
                        if (Wizard.ProjectCreationInfo.Database.IsInitialized())
                        {
                            // TODO: Offer to open
                            // TODO: Check for core views
                        }
                        else
                        {
                            // TODO: Offer to initialize and open
                            // TODO: Check for core views
                        }
                    }
                    else
                    {
                        Wizard.ProjectCreationInfo.Database.Create();
                    }
                    Project project = ProjectExtensions.Create(Wizard.ProjectCreationInfo);
                    progress.Report(Strings.Body_Initializing);
                    project.Initialize();
                    ContinueCore(project, progress);
                    return project;
                });
                Settings.Default.SetProjectPath(Wizard.CoreProject, Wizard.Project.FilePath);
                Settings.Default.Save();
                Commit(true);
                GoToStep(new CloseViewModel(Wizard, this));
            }
        }

        public class CloseViewModel : StepViewModel<CreateProjectViewModel>
        {
            public override string Title => Strings.CreateProject_Lead_Close;
            public override string ContinueAction => Strings.AccessText_Close;

            public CloseViewModel(CreateProjectViewModel wizard, IStep antecedent)
                : base(wizard, antecedent) { }

            public override bool CanContinue()
            {
                return true;
            }

            public override Task ContinueAsync()
            {
                Close();
                return Task.CompletedTask;
            }
        }

        public CoreProject CoreProject { get; }
        public Project Project { get; private set; }
        private ProjectCreationInfo ProjectCreationInfo { get; set; }

        public CreateProjectViewModel(CoreProject coreProject)
        {
            CoreProject = coreProject;
            Step = new SetStrategyViewModel(this);
        }
    }
}
