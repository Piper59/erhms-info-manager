using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System.Linq;
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
                public override string Title => Strings.Lead_CreateView_SetXTemplate;

                private string templatePath;
                public string TemplatePath
                {
                    get { return templatePath; }
                    set { SetProperty(ref templatePath, value); }
                }

                public ICommand BrowseCommand { get; }

                public SetXTemplateViewModel(CreateViewViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent)
                {
                    BrowseCommand = new SyncCommand(Browse);
                }

                public void Browse()
                {
                    IFileDialogService fileDialog = ServiceLocator.Resolve<IFileDialogService>();
                    fileDialog.InitialDirectory = Configuration.Instance.Directories.Templates;
                    fileDialog.Filter = Strings.FileDialog_Filter_Templates;
                    if (fileDialog.Open() != true)
                    {
                        return;
                    }
                    TemplatePath = fileDialog.FileName;
                }

                public override bool CanContinue()
                {
                    return templatePath != null;
                }

                public override async Task ContinueAsync()
                {
                    XTemplate xTemplate = null;
                    IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                    progress.Lead = Strings.Lead_LoadingTemplate;
                    try
                    {
                        xTemplate = await progress.Run(() =>
                        {
                            return XTemplate.Load(templatePath);
                        });
                        if (xTemplate.Level != TemplateLevel.View)
                        {
                            ShowDialog(Strings.Body_InvalidViewTemplatePath);
                            return;
                        }
                    }
                    catch
                    {
                        ShowDialog(Strings.Body_InvalidTemplatePath);
                        return;
                    }
                    Wizard.FromTemplate_TemplatePath = templatePath;
                    Wizard.FromTemplate_XTemplate = xTemplate;
                    ContinueTo(new SetViewNameViewModel(Wizard, this));
                }

                private void ShowDialog(string body)
                {
                    IDialogService dialog = ServiceLocator.Resolve<IDialogService>();
                    dialog.Severity = DialogSeverity.Warning;
                    dialog.Lead = Strings.Lead_InvalidTemplatePath;
                    dialog.Body = string.Format(body, templatePath);
                    dialog.Buttons = DialogButtonCollection.Close;
                    dialog.Show();
                }
            }

            public class SetViewNameViewModel : StepViewModel<CreateViewViewModel>
            {
                public override string Title => Strings.Lead_CreateView_SetViewName;

                private string viewName;
                public string ViewName
                {
                    get { return viewName; }
                    set { SetProperty(ref viewName, value); }
                }

                public SetViewNameViewModel(CreateViewViewModel wizard, IStep step)
                    : base(wizard, step)
                {
                    viewName = wizard.FromTemplate_XView.Name;
                }

                public override bool CanContinue()
                {
                    return true;
                }

                public override async Task ContinueAsync()
                {
                    if (!await Wizard.ValidateAsync(viewName))
                    {
                        return;
                    }
                    Wizard.FromTemplate_ViewName = viewName;
                    ContinueTo(new CommitViewModel(Wizard, this));
                }
            }

            public class CommitViewModel : StepViewModel<CreateViewViewModel>
            {
                public override string Title => Strings.Lead_CreateView_Commit;
                public override string ContinueAction => Strings.AccessText_Finish;
                public DetailsViewModel Details { get; }

                public CommitViewModel(CreateViewViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent)
                {
                    Details = new DetailsViewModel
                    {
                        { Strings.Label_TemplatePath, Wizard.FromTemplate_TemplatePath },
                        { Strings.Label_ViewName, Wizard.FromTemplate_ViewName }
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
                        Wizard.FromTemplate_XView.Name = Wizard.FromTemplate_ViewName;
                        ViewTemplateInstantiator instantiator =
                            new ViewTemplateInstantiator(Wizard.FromTemplate_XTemplate, Wizard.Project)
                            {
                                Progress = progress
                            };
                        instantiator.Instantiate();
                        return instantiator.View;
                    });
                    Commit();
                    SetResult(true);
                    ContinueTo(new CloseViewModel(Wizard, this));
                }
            }
        }

        private string FromTemplate_TemplatePath { get; set; }
        private XTemplate FromTemplate_XTemplate { get; set; }
        private XView FromTemplate_XView => FromTemplate_XTemplate.XProject.XViews.Single();
        private string FromTemplate_ViewName { get; set; }
    }
}
