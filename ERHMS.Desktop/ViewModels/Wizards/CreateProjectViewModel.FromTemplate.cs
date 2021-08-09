using Epi;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateProjectViewModel
    {
        public static class FromTemplate
        {
            public class SetXTemplateViewModel : StepViewModel<CreateProjectViewModel>
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

                public SetXTemplateViewModel(CreateProjectViewModel wizard, IStep antecedent)
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
                }

                public override bool CanContinue()
                {
                    return TemplatePath != null;
                }

                public override Task ContinueAsync()
                {
                    Wizard.TemplatePath = TemplatePath;
                    Wizard.XTemplate = XTemplate;
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
                    Details.Insert(0, Strings.Label_Template, wizard.TemplatePath);
                }

                protected override void CreateViews(Project project, IProgress<string> progress)
                {
                    ProjectTemplateInstantiator instantiator =
                        new ProjectTemplateInstantiator(Wizard.XTemplate, project)
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

        private string TemplatePath { get; set; }
        private XTemplate XTemplate { get; set; }
    }
}
