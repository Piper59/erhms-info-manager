using Epi;
using ERHMS.Common.Logging;
using ERHMS.Common.Naming;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Naming;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateViewViewModels
    {
        partial class State
        {
            public View SourceView { get; set; }
        }

        public static class FromExisting
        {
            public class SetSourceViewViewModel : StepViewModel<State>
            {
                public static async Task<SetSourceViewViewModel> CreateAsync(State state)
                {
                    SetSourceViewViewModel result = new SetSourceViewViewModel(state);
                    await result.InitializeAsync();
                    return result;
                }

                private readonly IFileDialogService fileDialog;

                public override string Title => Strings.CreateView_Lead_SetSourceView;

                private ViewListCollectionView views;
                public ViewListCollectionView Views
                {
                    get { return views; }
                    private set { SetProperty(ref views, value); }
                }

                public ICommand BrowseCommand { get; }

                private SetSourceViewViewModel(State state)
                    : base(state)
                {
                    fileDialog = ServiceLocator.Resolve<IFileDialogService>();
                    fileDialog.Filter = Strings.FileDialog_Filter_Projects;
                    BrowseCommand = new AsyncCommand(BrowseAsync);
                }

                private async Task InitializeAsync()
                {
                    Views = await ViewListCollectionView.CreateAsync(State.Project);
                    fileDialog.FileName = State.Project.FilePath;
                }

                public async Task BrowseAsync()
                {
                    if (fileDialog.Open() != true)
                    {
                        return;
                    }
                    IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                    progress.Lead = Strings.Lead_LoadingProject;
                    Views = await progress.Run(async () =>
                    {
                        Project project = await Task.Run(() =>
                        {
                            return ProjectExtensions.Open(fileDialog.FileName);
                        });
                        return await ViewListCollectionView.CreateAsync(project);
                    });
                }

                public override bool CanContinue()
                {
                    return Views.HasCurrent();
                }

                public override async Task ContinueAsync()
                {
                    State.SourceView = Views.CurrentItem;
                    IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                    Wizard.GoForward(await progress.Run(() =>
                    {
                        return SetViewNameViewModel.CreateAsync(State);
                    }));
                }
            }

            public class SetViewNameViewModel : CreateViewViewModels.SetViewNameViewModel
            {
                public static async Task<SetViewNameViewModel> CreateAsync(State state)
                {
                    SetViewNameViewModel result = new SetViewNameViewModel(state);
                    await result.InitializeAsync();
                    return result;
                }

                private SetViewNameViewModel(State state)
                    : base(state) { }

                private async Task InitializeAsync()
                {
                    ViewName = await Task.Run(() =>
                    {
                        ViewNameUniquifier viewNames = new ViewNameUniquifier(State.Project);
                        return viewNames.UniquifyIfExists(State.SourceView.Name);
                    });
                }

                protected override StepViewModel GetSubsequent()
                {
                    return new CommitViewModel(State);
                }
            }

            public class CommitViewModel : CreateViewViewModels.CommitViewModel
            {
                public CommitViewModel(State state)
                    : base(state)
                {
                    Details = new DetailsViewModel
                    {
                        { Strings.Label_Project, state.SourceView.Project.FilePath },
                        { Strings.Label_View, state.SourceView.Name },
                        { Strings.Label_Name, state.ViewName }
                    };
                }

                protected override View ContinueCore()
                {
                    ViewTemplateCreator creator = new ViewTemplateCreator(State.SourceView)
                    {
                        Progress = Log.Progress
                    };
                    XTemplate xTemplate = creator.Create();
                    xTemplate.XProject.XView.Name = State.ViewName;
                    ViewTemplateInstantiator instantiator = new ViewTemplateInstantiator(xTemplate, State.Project)
                    {
                        Progress = Log.Progress
                    };
                    instantiator.Instantiate();
                    return instantiator.View;
                }
            }
        }
    }
}
