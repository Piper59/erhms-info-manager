using Epi;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateProjectViewModel
    {
        public static class FromExisting
        {
            public class SetSourceProjectViewModel : StepViewModel<CreateProjectViewModel>
            {
                private readonly IFileDialogService fileDialog;

                public override string Title => Strings.CreateProject_Lead_SetSourceProject;

                private Project project;
                public Project Project
                {
                    get { return project; }
                    private set { SetProperty(ref project, value); }
                }

                public ICommand BrowseCommand { get; }

                public SetSourceProjectViewModel(CreateProjectViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent)
                {
                    fileDialog = ServiceLocator.Resolve<IFileDialogService>();
                    fileDialog.InitialDirectory = Configuration.Instance.GetProjectsDirectory();
                    fileDialog.Filter = Strings.FileDialog_Filter_Projects;
                    BrowseCommand = new AsyncCommand(BrowseAsync);
                }

                public async Task BrowseAsync()
                {
                    if (fileDialog.Open() != true)
                    {
                        return;
                    }
                    IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                    progress.Lead = Strings.Lead_LoadingProject;
                    Project = await progress.Run(() =>
                    {
                        return ProjectExtensions.Open(fileDialog.FileName);
                    });
                    Command.OnCanExecuteChanged();
                }

                public override bool CanContinue()
                {
                    return Project != null;
                }

                public override Task ContinueAsync()
                {
                    Wizard.SourceProject = Project;
                    GoToStep(new SetProjectCreationInfoViewModel(Wizard, this));
                    return Task.CompletedTask;
                }
            }

            public class SetProjectCreationInfoViewModel : CreateProjectViewModel.SetProjectCreationInfoViewModel
            {
                public SetProjectCreationInfoViewModel(CreateProjectViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent) { }

                protected override void GoToNextStep()
                {
                    GoToStep(new CommitViewModel(Wizard, this));
                }
            }

            public class CommitViewModel : CreateProjectViewModel.CommitViewModel
            {
                public CommitViewModel(CreateProjectViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent)
                {
                    Details.Insert(0, Strings.Label_Project, Wizard.SourceProject.FilePath);
                }

                protected override void CreateViews(Project project, IProgress<string> progress)
                {
                    progress.Report(Strings.Body_CreatingTemplate);
                    ProjectTemplateCreator creator = new ProjectTemplateCreator(Wizard.SourceProject)
                    {
                        Progress = Log.Progress
                    };
                    XTemplate xTemplate = creator.Create();
                    ProjectTemplateInstantiator instantiator = new ProjectTemplateInstantiator(xTemplate, project)
                    {
                        Progress = Log.Progress
                    };
                    instantiator.Instantiating += (sender, e) =>
                    {
                        if (e.Level == TemplateLevel.View)
                        {
                            progress.Report(string.Format(Strings.Body_CreatingView, e.Name));
                        }
                    };
                    instantiator.Instantiate();
                }
            }
        }

        private Project SourceProject { get; set; }
    }
}
