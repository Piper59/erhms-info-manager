using Epi;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Wizards;
using ERHMS.Domain;
using ERHMS.EpiInfo.Templating;
using ERHMS.Resources;
using System;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateProjectViewModel
    {
        public static class Standard
        {
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
                    : base(wizard, antecedent) { }

                protected override void ContinueCore(Project project, IProgress<string> progress)
                {
                    foreach (CoreView coreView in CoreView.GetInstances(Wizard.CoreProject))
                    {
                        progress.Report(string.Format(Strings.Body_CreatingView, coreView.Name));
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
