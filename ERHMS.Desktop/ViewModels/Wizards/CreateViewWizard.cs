using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Dialogs;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Projects;
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

            private string viewName = "";
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

            protected abstract Epi.View ContinueCore(string viewName);

            public async Task ContinueAsync()
            {
                if (!Wizard.ViewNameGenerator.IsValid(viewName, out InvalidViewNameReason reason))
                {
                    ServiceProvider.Resolve<IDialogService>().Show(new DialogInfo(DialogInfoPreset.Error)
                    {
                        Lead = ResXExtensions.GetInvalidViewNameLead(reason),
                        Body = ResXExtensions.GetInvalidViewNameBody(reason)
                    });
                }
                else
                {
                    try
                    {
                        IProgressService progress = ServiceProvider.Resolve<IProgressService>();
                        progress.Title = ResX.CreatingViewTitle;
                        await progress.RunAsync(() =>
                        {
                            Wizard.View = ContinueCore(viewName);
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

            protected override Epi.View ContinueCore(string viewName)
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
                return Wizard.Project.InstantiateView(xTemplate);
            }
        }

        public class CreateTemplateBasedStep : StepViewModel<CreateViewWizard>
        {
            public override ICommand ContinueCommand => Command.Null;
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

        public Project Project { get; }
        private ViewNameGenerator ViewNameGenerator { get; }
        public Epi.View View { get; private set; }

        public CreateViewWizard(Project project)
        {
            Project = project;
            ViewNameGenerator = new ViewNameGenerator(project);
            currentStep = new ChooseTypeStep(this);
        }
    }
}
