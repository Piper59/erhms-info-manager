using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo.Naming;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateViewViewModel
    {
        public static class FromTemplate
        {
            public class SetXTemplateViewModel : StepViewModel<CreateViewViewModel>
            {
                private readonly IFileDialogService fileDialog;

                public override string Title => Strings.CreateView_Lead_SetXTemplate;

                private string templatePath;
                public string TemplatePath
                {
                    get { return templatePath; }
                    private set { SetProperty(ref templatePath, value); }
                }

                private XTemplate XTemplate { get; set; }

                public ICommand BrowseCommand { get; }

                public SetXTemplateViewModel(CreateViewViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent)
                {
                    fileDialog = ServiceLocator.Resolve<IFileDialogService>();
                    fileDialog.InitialDirectory = Configuration.Instance.GetTemplatesDirectory();
                    fileDialog.Filter = Strings.FileDialog_Filter_Templates;
                    BrowseCommand = new AsyncCommand(BrowseAsync);
                }

                public async Task BrowseAsync()
                {
                    if (fileDialog.Open() != true)
                    {
                        return;
                    }
                    IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                    progress.Lead = Strings.Lead_LoadingTemplate;
                    XTemplate xTemplate = await progress.Run(() =>
                    {
                        try
                        {
                            return XTemplate.Load(fileDialog.FileName);
                        }
                        catch
                        {
                            return null;
                        }
                    });
                    if (xTemplate == null || xTemplate.Level != TemplateLevel.View)
                    {
                        IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                        dialog.Severity = DialogSeverity.Warning;
                        dialog.Lead = Strings.Lead_InvalidTemplatePath;
                        dialog.Body = string.Format(Strings.Body_InvalidViewTemplatePath, fileDialog.FileName);
                        dialog.Buttons = DialogButtonCollection.Close;
                        dialog.Show();
                        return;
                    }
                    TemplatePath = fileDialog.FileName;
                    XTemplate = xTemplate;
                }

                public override bool CanContinue()
                {
                    return TemplatePath != null;
                }

                public override async Task ContinueAsync()
                {
                    Wizard.TemplatePath = TemplatePath;
                    Wizard.XTemplate = XTemplate;
                    IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                    IStep step = await progress.Run(() =>
                    {
                        return SetViewNameViewModel.CreateAsync(Wizard, this);
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

                private SetViewNameViewModel(CreateViewViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent) { }

                private async Task InitializeAsync()
                {
                    ViewName = await Task.Run(() =>
                    {
                        string viewName = Wizard.XTemplate.XProject.XView.Name;
                        ViewNameUniquifier viewNames = new ViewNameUniquifier(Wizard.Project);
                        if (viewNames.Exists(viewName))
                        {
                            viewName = viewNames.Uniquify(viewName);
                        }
                        return viewName;
                    });
                }

                protected override void GoToNextStep()
                {
                    GoToStep(new CommitViewModel(Wizard, this));
                }
            }

            public class CommitViewModel : StepViewModel<CreateViewViewModel>
            {
                public override string Title => Strings.Lead_Commit;
                public override string ContinueAction => Strings.AccessText_Finish;
                public DetailsViewModel Details { get; }

                public CommitViewModel(CreateViewViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent)
                {
                    Details = new DetailsViewModel
                    {
                        { Strings.Label_Template, wizard.TemplatePath },
                        { Strings.Label_View, wizard.ViewName }
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
                        Wizard.XTemplate.XProject.XView.Name = Wizard.ViewName;
                        ViewTemplateInstantiator instantiator =
                            new ViewTemplateInstantiator(Wizard.XTemplate, Wizard.Project)
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

        private string TemplatePath { get; set; }
        private XTemplate XTemplate { get; set; }
    }
}
