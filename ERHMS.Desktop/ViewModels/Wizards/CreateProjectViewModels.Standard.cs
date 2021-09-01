using Epi;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Properties;
using ERHMS.Domain;
using ERHMS.EpiInfo.Templating;
using ERHMS.Resources;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateProjectViewModels
    {
        public static class Standard
        {
            public class CommitViewModel : CreateProjectViewModels.CommitViewModel
            {
                public CommitViewModel(State state)
                    : base(state)
                {
                    Details.Add(Strings.Label_Strategy_CreateProject, Strings.Strategy_Standard);
                }

                protected override void ContinueCore(Project project)
                {
                    foreach (CoreView coreView in CoreView.GetInstances(State.CoreProject))
                    {
                        Progress.Report(string.Format(Strings.Body_CreatingView, coreView.Name));
                        ViewTemplateInstantiator instantiator =
                            new ViewTemplateInstantiator(ResourceManager.GetXTemplate(coreView), project)
                            {
                                Progress = Log.Progress
                            };
                        instantiator.Instantiate();
                    }
                }
            }
        }
    }
}
