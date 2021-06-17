using Epi;
using ERHMS.Common.Linq;
using ERHMS.Common.Logging;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using static System.Console;

namespace ERHMS.Console.Utilities
{
    public class ImportRecords : IUtility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }

        public ImportRecords(string projectPath, string viewName)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
        }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            RecordImporter importer = new RecordImporter(view, In);
            foreach (Iterator<string> header in importer.Headers.Iterate())
            {
                if (view.Fields.DataFields.Contains(header.Value))
                {
                    importer.MapField(header.Index, view.Fields[header.Value]);
                }
            }
            importer.Import();
            foreach (string error in importer.Errors)
            {
                Log.Instance.Warn(error);
            }
        }
    }
}
