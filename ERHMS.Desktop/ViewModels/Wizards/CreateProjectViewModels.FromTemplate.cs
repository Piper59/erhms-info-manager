using Epi;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
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
            public string TemplatePath { get; set; }
            public XTemplate XTemplate { get; set; }
        }

        public static class FromTemplate
        {
            public class SetXTemplateViewModel : StepViewModel<State>
            {
                private readonly IFileDialogService fileDialog;

                public override string Title => Strings.CreateProject_Lead_SetXTemplate;

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
                    fileDialog.InitialDirectory = EpiInfo.Configuration.Instance.Directories.Templates;
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
                    if (xTemplate == null || xTemplate.Level != TemplateLevel.Project)
                    {
                        IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                        dialog.Severity = DialogSeverity.Warning;
                        dialog.Lead = Strings.Lead_InvalidTemplatePath;
                        dialog.Body = string.Format(Strings.Body_InvalidProjectTemplatePath, fileDialog.FileName);
                        dialog.Buttons = DialogButtonCollection.Close;
                        dialog.Show();
                        return;
                    }
                    TemplatePath = fileDialog.FileName;
                    XTemplate = xTemplate;
                    Command.OnCanExecuteChanged();
                }

                public override bool CanContinue()
                {
                    return TemplatePath != null;
                }

                public override Task ContinueAsync()
                {
                    State.TemplatePath = TemplatePath;
                    State.XTemplate = XTemplate;
                    Wizard.GoForward(new CommitViewModel(State));
                    return Task.CompletedTask;
                }
            }

            public class CommitViewModel : CreateProjectViewModels.CommitViewModel
            {
                public CommitViewModel(State state)
                    : base(state)
                {
                    Details.Add(Strings.Label_Strategy_CreateProject, Strings.Strategy_FromTemplate);
                    Details.Add(Strings.Label_Template, state.TemplatePath);
                }

                protected override void ContinueCore(Project project)
                {
                    ProjectTemplateInstantiator instantiator =
                        new ProjectTemplateInstantiator(State.XTemplate, project)
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
