using ERHMS.Common;
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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
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
                GoToStep(new CreateBlankStep(Wizard));
            }

            public void CreateTemplateBased()
            {
                GoToStep(new CreateTemplateBasedStep(Wizard));
            }

            public void CopyExisting()
            {
                GoToStep(new CopyExistingStep(Wizard));
            }
        }

        public abstract class CreateStep : StepViewModel<CreateViewWizard>
        {
            public override string ContinueText => "_Create";

            protected string viewName = "";
            public string ViewName
            {
                get { return viewName; }
                set { SetProperty(ref viewName, value); }
            }

            public override ICommand ContinueCommand { get; }

            public CreateStep(CreateViewWizard wizard)
                : base(wizard)
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
                        GoToStep(new OpenStep(Wizard));
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

            public CreateBlankStep(CreateViewWizard wizard)
                : base(wizard) { }

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
                private set { SetProperty(ref templatePath, value); }
            }

            public ICommand OpenTemplateCommand { get; }

            public CreateTemplateBasedStep(CreateViewWizard wizard)
                : base(wizard)
            {
                OpenTemplateCommand = new SyncCommand(OpenTemplate);
            }

            public void OpenTemplate()
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

        public class CopyExistingStep : CreateStep
        {
            private string sourceProjectPath;
            public string SourceProjectPath
            {
                get { return sourceProjectPath; }
                private set { SetProperty(ref sourceProjectPath, value); }
            }

            private List<Epi.View> sourceViews;
            public ICollectionView SourceViews { get; }

            public string TargetProjectPath { get; }

            public string TargetViewName
            {
                get
                {
                    return viewName;
                }
                set
                {
                    SetProperty(ref viewName, value);
                    OnPropertyChanged(nameof(TargetViewName));
                }
            }

            public ICommand OpenSourceProjectCommand { get; }

            public CopyExistingStep(CreateViewWizard wizard)
                : base(wizard)
            {
                sourceProjectPath = wizard.Project.FilePath;
                sourceViews = new List<Epi.View>();
                SourceViews = new ListCollectionView(sourceViews);
                SetSourceViews(wizard.Project);
                TargetProjectPath = wizard.Project.FilePath;
                OpenSourceProjectCommand = new AsyncCommand(OpenSourceProjectAsync);
            }

            private void SetSourceViews(Project project)
            {
                sourceViews.Clear();
                sourceViews.AddRange(project.Views.Cast<Epi.View>());
            }

            public async Task OpenSourceProjectAsync()
            {
                IFileDialogService dialog = ServiceProvider.Resolve<IFileDialogService>();
                dialog.Title = ResX.OpenProjectDialogTitle;
                dialog.InitialDirectory = Wizard.Configuration.Directories.Project;
                dialog.Filter = ResX.ProjectFilter;
                if (dialog.Open(out string path).GetValueOrDefault())
                {
                    IProgressService progress = ServiceProvider.Resolve<IProgressService>();
                    progress.Title = ResX.LoadingProjectTitle;
                    await progress.RunAsync(() =>
                    {
                        SetSourceViews(new Project(path));
                    });
                    SourceProjectPath = path;
                    SourceViews.Refresh();
                    SourceViews.MoveCurrentToPosition(-1);
                }
            }

            protected override bool Validate()
            {
                if (SourceViews.CurrentPosition == -1)
                {
                    ServiceProvider.Resolve<IDialogService>().Show(new DialogInfo(DialogInfoPreset.Error)
                    {
                        Lead = ResX.SourceViewEmptyLead,
                        Body = ResX.SourceViewEmptyBody
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
                Epi.View view = (Epi.View)SourceViews.CurrentItem;
                ViewTemplateCreator creator = new ViewTemplateCreator(view)
                {
                    Progress = new ProgressLogger(progress)
                };
                XTemplate xTemplate = creator.Create();
                xTemplate.XProject.XView.SetName(viewName);
                return Wizard.Project.InstantiateView(xTemplate, progress);
            }
        }

        public class OpenStep : StepViewModel<CreateViewWizard>
        {
            public override string ContinueText => "_Close";

            private bool openView = true;
            public bool OpenView
            {
                get { return openView; }
                set { SetProperty(ref openView, value); }
            }

            public override ICommand ContinueCommand { get; }

            public OpenStep(CreateViewWizard wizard)
                : base(wizard)
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
