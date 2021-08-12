using ERHMS.Common.Logging;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using ERHMS.Resources;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateViewViewModel
    {
        public static class Blank
        {
            public class SetViewNameViewModel : CreateViewViewModel.SetViewNameViewModel
            {
                public SetViewNameViewModel(CreateViewViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent) { }

                protected override void GoToNextStep()
                {
                    GoToStep(new SetWithWorkerInfoViewModel(Wizard, this));
                }
            }

            public class SetWithWorkerInfoViewModel : StepViewModel<CreateViewViewModel>
            {
                public override string Title => Strings.CreateView_Lead_SetWithWorkerInfo;

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
                    Wizard.WithWorkerInfo = WithWorkerInfo;
                    GoToStep(new CommitViewModel(Wizard, this));
                    return Task.CompletedTask;
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
                        { Strings.Label_Name, wizard.ViewName },
                        { Strings.Label_WithWorkerInfo, wizard.WithWorkerInfo }
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
                        string conjunction = Wizard.WithWorkerInfo ? "With" : "Without";
                        string resourceName = $"Templates.Forms.BlankForm{conjunction}WorkerInfo.xml";
                        XTemplate xTemplate = ResourceManager.GetXTemplate(resourceName);
                        xTemplate.XProject.XView.Name = Wizard.ViewName;
                        ViewTemplateInstantiator instantiator = new ViewTemplateInstantiator(xTemplate, Wizard.Project)
                        {
                            Progress = Log.Progress
                        };
                        instantiator.Instantiate();
                        return instantiator.View;
                    });
                    Commit(true);
                    GoToStep(new CloseViewModel(Wizard, this));
                }
            }
        }

        private bool WithWorkerInfo { get; set; }
    }
}
