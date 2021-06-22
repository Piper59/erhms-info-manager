using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using ERHMS.Resources;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateViewViewModel
    {
        public static class Blank
        {
            public class SetViewNameViewModel : StepViewModel<CreateViewViewModel>
            {
                public override string Title => Strings.Lead_CreateView_SetViewName;

                private string viewName = "";
                public string ViewName
                {
                    get { return viewName; }
                    set { SetProperty(ref viewName, value); }
                }

                public SetViewNameViewModel(CreateViewViewModel wizard, IStep step)
                    : base(wizard, step) { }

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
                    Wizard.Blank_ViewName = viewName;
                    ContinueTo(new SetWithWorkerInfoViewModel(Wizard, this));
                }
            }

            public class SetWithWorkerInfoViewModel : StepViewModel<CreateViewViewModel>
            {
                public override string Title => Strings.Lead_CreateView_SetWithWorkerInfo;

                private bool withWorkerInfo = true;
                public bool WithWorkerInfo
                {
                    get { return withWorkerInfo; }
                    set { SetProperty(ref withWorkerInfo, value); }
                }

                public SetWithWorkerInfoViewModel(CreateViewViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent) { }

                public override bool CanContinue()
                {
                    return true;
                }

                public override Task ContinueAsync()
                {
                    Wizard.Blank_WithWorkerInfo = withWorkerInfo;
                    ContinueTo(new CommitViewModel(Wizard, this));
                    return Task.CompletedTask;
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
                        { Strings.Label_ViewName, Wizard.Blank_ViewName },
                        { Strings.Label_WithWorkerInfo, Wizard.Blank_WithWorkerInfo.ToLocalizedString() }
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
                        string conjunction = Wizard.Blank_WithWorkerInfo ? "With" : "Without";
                        string resourceName = $"Templates.Forms.BlankForm{conjunction}WorkerInfo.xml";
                        XTemplate xTemplate = ResourceManager.GetXTemplate(resourceName);
                        xTemplate.XProject.XViews.Single().Name = Wizard.Blank_ViewName;
                        ViewTemplateInstantiator instantiator = new ViewTemplateInstantiator(xTemplate, Wizard.Project)
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

        private string Blank_ViewName { get; set; }
        private bool Blank_WithWorkerInfo { get; set; }
    }
}
