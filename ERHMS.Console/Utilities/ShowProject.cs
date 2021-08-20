using Epi;
using Epi.Data.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Console.Utilities
{
    public class ShowProject : Utility
    {
        public string ProjectPath { get; }

        public ShowProject(string projectPath)
        {
            ProjectPath = projectPath;
        }

        public override void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            foreach (View view in project.Views)
            {
                Out.WriteLine($"{view.Name}");
                foreach (Page page in view.Pages)
                {
                    Out.WriteLine($"  {page.Name}");
                    IEnumerable<FieldDataRow> fields = ((MetadataDbProvider)project.Metadata)
                        .GetFieldDataTableForPage(page.Id)
                        .OrderBy(field => field, new FieldDataRowComparer.ByTabIndex());
                    foreach (FieldDataRow field in fields)
                    {
                        Out.WriteLine($"    {field.Name}");
                    }
                }
            }
        }
    }
}
