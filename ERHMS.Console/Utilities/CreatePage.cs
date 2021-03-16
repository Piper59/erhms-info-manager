using Epi;
using ERHMS.EpiInfo;
using System;

namespace ERHMS.Console.Utilities
{
    public class CreatePage : IUtility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }
        public string PageName { get; }

        public CreatePage(string projectPath, string viewName, string pageName)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
            PageName = pageName;
        }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            if (view.GetPageByName(PageName) != null)
            {
                throw new InvalidOperationException("Page already exists.");
            }
            Page page = new Page(view)
            {
                Name = PageName,
                Position = view.GetMaxPagePosition() + 1
            };
            project.Metadata.InsertPage(page);
            if (view.TableExists())
            {
                view.Synchronize();
            }
        }
    }
}
