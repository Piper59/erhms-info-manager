using Epi;
using ERHMS.EpiInfo;
using System;

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
            if (project.IsInitialized())
            {
                throw new InvalidOperationException("Project is already initialized.");
            }
            project.Initialize();
        }
    }
}
