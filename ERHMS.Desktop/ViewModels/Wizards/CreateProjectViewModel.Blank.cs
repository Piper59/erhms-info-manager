using Epi;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Wizards;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels.Wizards
{
    partial class CreateProjectViewModel
    {
        public static class Blank
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

            // TODO: Create base class
            public class CommitViewModel : StepViewModel<CreateProjectViewModel>
            {
                public override string Title => Strings.Lead_Commit;
                public override string ContinueAction => Strings.AccessText_Finish;
                public DetailsViewModel Details { get; }

                public CommitViewModel(CreateProjectViewModel wizard, IStep antecedent)
                    : base(wizard, antecedent)
                {
                    Details = new DetailsViewModel
                    {
                        { Strings.Label_Name, wizard.ProjectCreationInfo.Name },
                        { Strings.Label_Description, wizard.ProjectCreationInfo.Description },
                        { Strings.Label_LocationRoot, wizard.ProjectCreationInfo.LocationRoot },
                        { Strings.Label_DatabaseProvider, wizard.ProjectCreationInfo.Database.Provider },
                        {
                            Strings.Label_ConnectionInfo,
                            wizard.ProjectCreationInfo.Database.GetConnectionStringBuilder()
                        }
                    };
                }

                public override bool CanContinue()
                {
                    return true;
                }

                public override async Task ContinueAsync()
                {
                    IProgressService progress = ServiceLocator.Resolve<IProgressService>();
                    progress.Lead = Strings.Lead_CreatingProject;
                    Wizard.Project = await progress.Run(() =>
                    {
                        if (Wizard.ProjectCreationInfo.Database.Exists())
                        {
                            if (Wizard.ProjectCreationInfo.Database.IsInitialized())
                            {
                                // TODO: Offer to open
                                // TODO: Check for core views
                            }
                            else
                            {
                                // TODO: Offer to initialize and open
                                // TODO: Check for core views
                            }
                        }
                        else
                        {
                            Wizard.ProjectCreationInfo.Database.Create();
                        }
                        Project project = ProjectExtensions.Create(Wizard.ProjectCreationInfo);
                        project.Initialize();
                        return project;
                    });
                    Commit(true);
                    GoToStep(new CloseViewModel(Wizard, this));
                }
            }
        }
    }
}
