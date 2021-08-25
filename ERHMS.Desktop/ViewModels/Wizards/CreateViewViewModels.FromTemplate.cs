using Epi;
using ERHMS.Common.Logging;
using ERHMS.Common.Naming;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
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
            public string TemplatePath { get; set; }
            public XTemplate XTemplate { get; set; }
        }

        public static class FromTemplate
        {
            public class SetXTemplateViewModel : StepViewModel<State>
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

                public SetXTemplateViewModel(State state)
                    : base(state)
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
                    State.TemplatePath = TemplatePath;
                    State.XTemplate = XTemplate;
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
                        return viewNames.UniquifyIfExists(State.XTemplate.XProject.XView.Name);
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
                        { Strings.Label_Template, state.TemplatePath },
                        { Strings.Label_Name, state.ViewName }
                    };
                }

                protected override View ContinueCore()
                {
                    State.XTemplate.XProject.XView.Name = State.ViewName;
                    ViewTemplateInstantiator instantiator =
                        new ViewTemplateInstantiator(State.XTemplate, State.Project)
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
