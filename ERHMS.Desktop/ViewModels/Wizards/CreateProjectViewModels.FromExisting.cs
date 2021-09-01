using Epi;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateProjectViewModels
    {
        partial class State
        {
            public Project SourceProject { get; set; }
        }

        public static class FromExisting
        {
            public class SetSourceProjectViewModel : StepViewModel<State>
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

                public SetSourceProjectViewModel(State state)
                    : base(state)
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
                    State.SourceProject = Project;
                    Wizard.GoForward(new CommitViewModel(State));
                    return Task.CompletedTask;
                }
            }

            public class CommitViewModel : CreateProjectViewModels.CommitViewModel
            {
                public CommitViewModel(State state)
                    : base(state)
                {
                    Details.Add(Strings.Label_Strategy_CreateProject, Strings.Strategy_FromExisting);
                    Details.Add(Strings.Label_Source, state.SourceProject);
                }

                protected override void ContinueCore(Project project)
                {
                    Progress.Report(Strings.Body_CreatingTemplate);
                    ProjectTemplateCreator creator = new ProjectTemplateCreator(State.SourceProject)
                    {
                        Progress = Log.Progress
                    };
                    XTemplate xTemplate = creator.Create();
                    ProjectTemplateInstantiator instantiator = new ProjectTemplateInstantiator(xTemplate, project)
                    {
                        Progress = Log.Progress
                    };
                    instantiator.Instantiating += Instantiator_Instantiating;
                    instantiator.Instantiate();
                }

                private void Instantiator_Instantiating(object sender, InstantiatingEventArgs e)
                {
                    if (e.Level == TemplateLevel.View)
                    {
                        Progress.Report(string.Format(Strings.Body_CreatingView, e.Name));
                    }
                }
            }
        }
    }
}
