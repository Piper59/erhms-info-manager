using Epi;
using ERHMS.EpiInfo;
using System;

namespace ERHMS.Console.Utilities
{
    public class CreateView : Utility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }

        public CreateView(string projectPath, string viewName)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
        }

        public override void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            if (project.Views.Contains(ViewName))
            {
                throw new InvalidOperationException("View already exists.");
            }
            View view = new View(project)
            {
                Name = ViewName
            };
            project.Metadata.InsertView(view);
        }
    }
}
