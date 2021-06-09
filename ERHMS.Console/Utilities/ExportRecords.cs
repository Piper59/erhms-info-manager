using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using static System.Console;

namespace ERHMS.Console.Utilities
{
    public class ExportRecords : IUtility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }

        public ExportRecords(string projectPath, string viewName)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
        }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            RecordRepository repository = new RecordRepository(view);
            RecordExporter exporter = new RecordExporter(Out, view);
            exporter.Export(repository.Select());
        }
    }
}
