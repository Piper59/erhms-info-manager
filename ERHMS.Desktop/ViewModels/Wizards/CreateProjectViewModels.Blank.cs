using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo.Data;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateProjectViewModels
    {
        public static class Blank
        {
            public class CommitViewModel : CreateProjectViewModels.CommitViewModel
            {
                public CommitViewModel(State state)
                    : base(state)
                {
                    if (state.DatabaseStatus != DatabaseStatus.Initialized)
                    {
                        Details.Add(Strings.Label_Strategy_CreateProject, Strings.Strategy_Blank);
                    }
                }

                protected override void ContinueCore(Project project) { }
            }
        }
    }
}
