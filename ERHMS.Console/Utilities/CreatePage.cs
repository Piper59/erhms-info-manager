using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Naming;
using System;
using System.Linq;

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
            if (view.Pages.Any(_page => NameComparer.Default.Equals(_page.Name, PageName)))
            {
                throw new InvalidOperationException("Page already exists.");
            }
            int maxPosition = view.Pages.Select(_page => _page.Position)
                .DefaultIfEmpty(-1)
                .Max();
            Page page = new Page(view)
            {
                Name = PageName,
                Position = maxPosition + 1
            };
            project.Metadata.InsertPage(page);
            if (view.DataTableExists())
            {
                view.SynchronizeDataTables();
            }
        }
    }
}
