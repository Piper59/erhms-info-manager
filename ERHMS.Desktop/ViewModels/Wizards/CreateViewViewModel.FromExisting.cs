using Epi;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Naming;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateViewViewModel
    {
        public static class FromExisting
        {
            public class SetSourceViewViewModel : StepViewModel<CreateViewViewModel>
            {
                public static async Task<SetSourceViewViewModel> CreateAsync(
                    CreateViewViewModel wizard,
                    IStep antecedent)
                {
                    SetSourceViewViewModel result = new SetSourceViewViewModel(wizard, antecedent);
                    await result.InitializeAsync();
                    return result;
                }

                private readonly IFileDialogService fileDialog;

                public override string Title => Strings.CreateView_Lead_SetSourceView;

                private ViewCollectionView views;
                public ViewCollectionView Views
                {
                    get { return views; }
                    private set { SetProperty(ref views, value); }
                }

                public ICommand BrowseCommand { get; }

                private SetSourceViewViewModel(CreateViewViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent)
                {
                    fileDialog = ServiceLocator.Resolve<IFileDialogService>();
                    fileDialog.Filter = Strings.FileDialog_Filter_Projects;
                    BrowseCommand = new AsyncCommand(BrowseAsync);
                }

                private async Task InitializeAsync()
                {
                    fileDialog.FileName = Wizard.Project.FilePath;
                    Views = await ViewCollectionView.CreateAsync(Wizard.Project);
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
                        return await ViewCollectionView.CreateAsync(project);
                    });
                }

                public override bool CanContinue()
                {
                    return Views.HasCurrentItem();
                }

                public override async Task ContinueAsync()
                {
                    Wizard.SourceView = Views.CurrentItem;
                    IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                    IStep step = await progress.Run(async () =>
                    {
                        return await SetViewNameViewModel.CreateAsync(Wizard, this);
                    });
                    GoToStep(step);
                }
            }

            public class SetViewNameViewModel : CreateViewViewModel.SetViewNameViewModel
            {
                public static async Task<SetViewNameViewModel> CreateAsync(CreateViewViewModel wizard, IStep antecedent)
                {
                    SetViewNameViewModel result = new SetViewNameViewModel(wizard, antecedent);
                    await result.InitializeAsync();
                    return result;
                }

                private SetViewNameViewModel(CreateViewViewModel wizard, IStep step)
                    : base(wizard, step) { }

                private async Task InitializeAsync()
                {
                    await Task.Run(() =>
                    {
                        ViewName = Wizard.SourceView.Name;
                        ViewNameUniquifier viewNames = new ViewNameUniquifier(Wizard.Project);
                        if (viewNames.Exists(ViewName))
                        {
                            ViewName = viewNames.Uniquify(ViewName);
                        }
                    });
                }

                protected override void GoToNextStep()
                {
                    GoToStep(new CommitViewModel(Wizard, this));
                }
            }

            public class CommitViewModel : StepViewModel<CreateViewViewModel>
            {
                public override string Title => Strings.CreateView_Lead_Commit;
                public override string ContinueAction => Strings.AccessText_Finish;
                public DetailsViewModel Details { get; }

                public CommitViewModel(CreateViewViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent)
                {
                    Details = new DetailsViewModel
                    {
                        { Strings.Label_SourceView, wizard.SourceView },
                        { Strings.Label_TargetView, wizard.ViewName }
                    };
                }

                public override bool CanContinue()
                {
                    return true;
                }

                public override async Task ContinueAsync()
                {
                    IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                    progress.Lead = Strings.Lead_CreatingView;
                    Wizard.View = await progress.Run(() =>
                    {
                        ViewTemplateCreator creator = new ViewTemplateCreator(Wizard.SourceView)
                        {
                            Progress = progress
                        };
                        XTemplate xTemplate = creator.Create();
                        xTemplate.XProject.XView.Name = Wizard.ViewName;
                        ViewTemplateInstantiator instantiator = new ViewTemplateInstantiator(xTemplate, Wizard.Project)
                        {
                            Progress = progress
                        };
                        instantiator.Instantiate();
                        return instantiator.View;
                    });
                    Commit(true);
                    GoToStep(new CloseViewModel(Wizard, this));
                }
            }
        }

        public View SourceView { get; private set; }
    }
}
