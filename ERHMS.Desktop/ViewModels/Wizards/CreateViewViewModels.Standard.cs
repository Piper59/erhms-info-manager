using Epi;
using ERHMS.Common.Logging;
using ERHMS.Common.Naming;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Shared;
using ERHMS.Domain;
using ERHMS.EpiInfo.Naming;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using ERHMS.Resources;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateViewViewModels
    {
        partial class State
        {
            public CoreView CoreView { get; set; }
        }

        public static class Standard
        {
            public class SetCoreViewViewModel : StepViewModel<State>
            {
                public override string Title => Strings.CreateView_Lead_SetCoreView;
                public ListCollectionView<CoreView> CoreViews { get; }

                public SetCoreViewViewModel(State state)
                    : base(state)
                {
                    CoreViews = new ListCollectionView<CoreView>(CoreView.Instances);
                    CoreViews.GroupDescriptions.Add(new PropertyGroupDescription(nameof(CoreView.CoreProject)));
                }

                public override bool CanContinue()
                {
                    return CoreViews.HasCurrent();
                }

                public override async Task ContinueAsync()
                {
                    State.CoreView = CoreViews.CurrentItem;
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
                        return viewNames.UniquifyIfExists(State.CoreView.Name);
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
                        { Strings.Label_Strategy_CreateView, Strings.Strategy_Standard },
                        { Strings.Label_View, state.CoreView },
                        { Strings.Label_Name, state.ViewName }
                    };
                }

                protected override View ContinueCore()
                {
                    XTemplate xTemplate = ResourceManager.GetXTemplate(State.CoreView);
                    ViewTemplateInstantiator instantiator =
                        new ViewTemplateInstantiator(xTemplate, State.Project)
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
