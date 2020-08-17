using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Projects;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using ERHMS.Resources;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    public class CreateViewWizard : WizardViewModel
    {
        public class ChooseTypeStep : StepViewModel<CreateViewWizard>
        {
            public ICommand CreateBlankCommand { get; }
            public ICommand CreateTemplateBasedCommand { get; }
            public ICommand CopyExistingCommand { get; }
            public override ICommand ContinueCommand => Command.Null;

            public ChooseTypeStep(CreateViewWizard wizard)
                : base(wizard)
            {
                CreateBlankCommand = new SyncCommand(CreateBlank);
                CreateTemplateBasedCommand = new SyncCommand(CreateTemplateBased);
                CopyExistingCommand = new SyncCommand(CopyExisting);
            }

            public void CreateBlank()
            {
                GoToStep(new CreateBlankStep());
            }

            public void CreateTemplateBased()
            {
                GoToStep(new CreateTemplateBasedStep());
            }

            public void CopyExisting()
            {
                GoToStep(new CopyExistingStep());
            }
        }

        public abstract class CreateStep : StepViewModel<CreateViewWizard>
        {
            public override string ContinueText => "Create";

            protected string viewName = "";
            public string ViewName
            {
                get { return viewName; }
                set { SetProperty(ref viewName, value); }
            }

            public override ICommand ContinueCommand { get; }

            public CreateStep()
            {
                ContinueCommand = new AsyncCommand(ContinueAsync);
            }

            protected virtual bool Validate()
            {
                if (!Wizard.ViewNameGenerator.IsValid(viewName, out InvalidViewNameReason reason))
                {
                    ServiceProvider.Resolve<IDialogService>().Show(new DialogInfo(DialogInfoPreset.Error)
                    {
                        Lead = ResXExtensions.GetInvalidViewNameLead(reason),
                        Body = ResXExtensions.GetInvalidViewNameBody(reason)
                    });
                    return false;
                }
                else
                {
                    return true;
                }
            }

            protected abstract Epi.View ContinueCore(IProgressService progress);

            public async Task ContinueAsync()
            {
                if (Validate())
                {
                    try
                    {
                        IProgressService progress = ServiceProvider.Resolve<IProgressService>();
                        progress.Title = ResX.CreatingViewTitle;
                        await progress.RunAsync(() =>
                        {
                            Wizard.View = ContinueCore(progress);
                        });
                        Wizard.ViewNameGenerator.Add(Wizard.View.Name);
                        Wizard.Result = true;
                        Wizard.Completed = true;
                        GoToStep(new OpenStep());
                    }
                    catch (Exception ex)
                    {
                        ServiceProvider.Resolve<IDialogService>().Show(new DialogInfo(DialogInfoPreset.Error)
                        {
                            Lead = ResX.CreatingViewErrorLead,
                            Body = ex.Message,
                            Details = ex.ToString()
                        });
                    }
                }
            }
        }

        public class CreateBlankStep : CreateStep
        {
            private bool includeWorkerInfo;
            public bool IncludeWorkerInfo
            {
                get { return includeWorkerInfo; }
                set { SetProperty(ref includeWorkerInfo, value); }
            }

            protected override Epi.View ContinueCore(IProgressService progress)
            {
                string templateName = includeWorkerInfo ? "BlankWorkerInfo" : "Blank";
                string resourceName = $"ERHMS.Resources.Templates.Forms.{templateName}.xml";
                XDocument document;
                using (Stream stream = ResourceProvider.GetResource(resourceName))
                {
                    document = XDocument.Load(stream);
                }
                XTemplate xTemplate = new XTemplate(document.Root);
                xTemplate.XProject.XView.SetName(viewName);
                return Wizard.Project.InstantiateView(xTemplate, progress);
            }
        }

        public class CreateTemplateBasedStep : CreateStep
        {
            private string templatePath = "";
            public string TemplatePath
            {
                get { return templatePath; }
                set { SetProperty(ref templatePath, value); }
            }

            public ICommand BrowseCommand { get; }

            public CreateTemplateBasedStep()
            {
                BrowseCommand = new SyncCommand(Browse);
            }

            public void Browse()
            {
                IFileDialogService dialog = ServiceProvider.Resolve<IFileDialogService>();
                dialog.Title = ResX.OpenTemplateDialogTitle;
                dialog.InitialDirectory = Wizard.Configuration.Directories.Templates;
                dialog.Filter = ResX.TemplateFilter;
                if (dialog.Open(out string path).GetValueOrDefault())
                {
                    TemplatePath = path;
                }
            }

            protected override bool Validate()
            {
                if (templatePath == "")
                {
                    ServiceProvider.Resolve<IDialogService>().Show(new DialogInfo(DialogInfoPreset.Error)
                    {
                        Lead = ResX.TemplatePathEmptyLead,
                        Body = ResX.TemplatePathEmptyBody
                    });
                    return false;
                }
                else
                {
                    return base.Validate();
                }
            }

            protected override Epi.View ContinueCore(IProgressService progress)
            {
                XTemplate xTemplate;
                try
                {
                    XDocument document = XDocument.Load(templatePath);
                    xTemplate = new XTemplate(document.Root);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ResX.TemplateReadErrorMessage, ex);
                }
                if (xTemplate.Level != TemplateLevel.View)
                {
                    throw new InvalidOperationException(string.Format(ResX.TemplateLevelErrorMessage, "form"));
                }
                xTemplate.XProject.XView.SetName(viewName);
                return Wizard.Project.InstantiateView(xTemplate, progress);
            }
        }

        public class CopyExistingStep : StepViewModel<CreateViewWizard>
        {
            public override ICommand ContinueCommand => Command.Null;
        }

        public class OpenStep : StepViewModel<CreateViewWizard>
        {
            public override string ContinueText => "Close";

            private bool openView = true;
            public bool OpenView
            {
                get { return openView; }
                set { SetProperty(ref openView, value); }
            }

            public override ICommand ContinueCommand { get; }

            public OpenStep()
            {
                ContinueCommand = new AsyncCommand(ContinueAsync);
            }

            public async Task ContinueAsync()
            {
                if (openView)
                {
                    await MainViewModel.Current.StartEpiInfoAsync(
                        Module.MakeView,
                        $"/project:{Wizard.Project.FilePath}",
                        $"/view:{Wizard.View.Name}");
                }
                Wizard.Exit();
            }
        }

        private Epi.Configuration Configuration { get; }
        public Project Project { get; }
        private ViewNameGenerator ViewNameGenerator { get; }
        public Epi.View View { get; private set; }

        public CreateViewWizard(Project project)
        {
            Configuration = ConfigurationExtensions.Load();
            Project = project;
            ViewNameGenerator = new ViewNameGenerator(project);
            currentStep = new ChooseTypeStep(this);
        }
    }
}
