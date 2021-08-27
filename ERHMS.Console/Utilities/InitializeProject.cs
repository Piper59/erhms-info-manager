using Epi;
using ERHMS.EpiInfo;

namespace ERHMS.Console.Utilities
{
    public class InitializeProject : Utility
    {
        public string ProjectPath { get; }

        public InitializeProject(string projectPath)
        {
            ProjectPath = projectPath;
        }

        public override void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            project.Initialize();
        }
    }
}
