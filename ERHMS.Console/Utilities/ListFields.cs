using Epi;
using Epi.Data.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Metadata;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

namespace ERHMS.Console.Utilities
{
    public class ListFields : IUtility
    {
        public string ProjectPath { get; }

        public ListFields(string projectPath)
        {
            ProjectPath = projectPath;
        }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            foreach (View view in project.Views)
            {
                Out.WriteLine($"{view.Name}");
                foreach (Page page in view.Pages)
                {
                    Out.WriteLine($"  {page.Name}");
                    FieldDataTable fields = ((MetadataDbProvider)project.Metadata).GetFieldDataTableForPage(page.Id);
                    IComparer<FieldDataRow> comparer = new FieldDataRowComparer.ByEffectiveTabIndex(fields);
                    foreach (FieldDataRow field in fields.OrderBy(field => field, comparer))
                    {
                        Out.WriteLine($"    {field.Name}");
                    }
                }
            }
        }
    }
}
