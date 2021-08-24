using Epi;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using ERHMS.Resources;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateViewViewModels
    {
        partial class State
        {
            public bool WithWorkerInfo { get; set; }
        }

        public static class Blank
        {
            public class SetViewNameViewModel : CreateViewViewModels.SetViewNameViewModel
            {
                public SetViewNameViewModel(State state)
                    : base(state) { }

                protected override StepViewModel GetSubsequent()
                {
                    return new SetWithWorkerInfoViewModel(State);
                }
            }

            public class SetWithWorkerInfoViewModel : StepViewModel<State>
            {
                public override string Title => Strings.CreateView_Lead_SetWithWorkerInfo;

                private bool withWorkerInfo = true;
                public bool WithWorkerInfo
                {
                    get { return withWorkerInfo; }
                    set { SetProperty(ref withWorkerInfo, value); }
                }

                public SetWithWorkerInfoViewModel(State state)
                    : base(state) { }

                public override bool CanContinue()
                {
                    return true;
                }

                public override Task ContinueAsync()
                {
                    State.WithWorkerInfo = WithWorkerInfo;
                    Wizard.GoForward(new CommitViewModel(State));
                    return Task.CompletedTask;
                }
            }

            public class CommitViewModel : CreateViewViewModels.CommitViewModel
            {
                public CommitViewModel(State state)
                    : base(state)
                {
                    Details = new DetailsViewModel
                    {
                        { Strings.Label_Name, state.ViewName },
                        { Strings.Label_WithWorkerInfo, state.WithWorkerInfo }
                    };
                }

                protected override View ContinueCore()
                {
                    string conjunction = State.WithWorkerInfo ? "With" : "Without";
                    string resourceName = $"Templates.Forms.BlankForm{conjunction}WorkerInfo.xml";
                    XTemplate xTemplate = ResourceManager.GetXTemplate(resourceName);
                    xTemplate.XProject.XView.Name = State.ViewName;
                    ViewTemplateInstantiator instantiator = new ViewTemplateInstantiator(xTemplate, State.Project)
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
