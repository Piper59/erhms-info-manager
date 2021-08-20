using Epi;
using ERHMS.Common.Logging;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Templating;
using ERHMS.Resources;

namespace ERHMS.Console.Utilities
{
    public class CreateCoreViews : Utility
    {
        public string ProjectPath { get; }
        public CoreProject CoreProject { get; }

        public CreateCoreViews(string projectPath, CoreProject coreProject)
        {
            ProjectPath = projectPath;
            CoreProject = coreProject;
        }

        public override void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            foreach (CoreView coreView in CoreView.GetInstances(CoreProject))
            {
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
