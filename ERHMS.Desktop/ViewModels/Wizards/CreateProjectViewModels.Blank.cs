using Epi;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateProjectViewModels
    {
        public static class Blank
        {
            public class SetProjectCreationInfoViewModel : CreateProjectViewModels.SetProjectCreationInfoViewModel
            {
                public SetProjectCreationInfoViewModel(State state)
                    : base(state) { }

                protected override StepViewModel GetSubsequent()
                {
                    return new CommitViewModel(State);
                }
            }

            public class CommitViewModel : CreateProjectViewModels.CommitViewModel
            {
                public CommitViewModel(State state)
                    : base(state) { }

                protected override void ContinueCore(Project project) { }
            }
        }
    }
}
