using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.Domain;
using System.IO;

namespace ERHMS.Desktop.Infrastructure
{
    public static class CoreProjectExtensions
    {
        public static bool Check(this CoreProject @this)
        {
            string projectPath = Configuration.Instance.GetProjectPath(@this);
            if (File.Exists(projectPath))
            {
                return true;
            }
            else
            {
                WizardViewModel wizard = RecoverProjectViewModels.GetWizard(@this);
                if (wizard.Run().GetValueOrDefault())
                {
                    if (@this == CoreProject.Incident)
                    {
                        int index = Configuration.Instance.IncidentProjectPaths.FindIndex(projectPath.Equals);
                        if (index > 0)
                        {
                            Configuration.Instance.IncidentProjectPaths.RemoveAt(index);
                            Configuration.Instance.Save();
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool Open(this CoreProject @this)
        {
            Directory.CreateDirectory(EpiInfo.Configuration.Instance.Directories.Projects);
            IFileDialogService fileDialog = ServiceLocator.Resolve<IFileDialogService>();
            fileDialog.InitialDirectory = EpiInfo.Configuration.Instance.Directories.Projects;
            fileDialog.Filter = Strings.FileDialog_Filter_Projects;
            if (!fileDialog.Open().GetValueOrDefault())
            {
                return false;
            }
            Configuration.Instance.SetProjectPath(@this, fileDialog.FileName);
            Configuration.Instance.Save();
            return true;
        }
    }
}
