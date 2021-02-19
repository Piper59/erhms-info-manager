using Epi;
using ERHMS.EpiInfo;
using System;

namespace ERHMS.Console.Utilities
{
    public class InitializeProject : IUtility
    {
        public string ProjectPath { get; }

        public InitializeProject(string projectPath)
        {
            ProjectPath = projectPath;
        }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            if (project.IsInitialized())
            {
                throw new InvalidOperationException("Project is already initialized.");
            }
            project.Initialize();
        }
    }
}
