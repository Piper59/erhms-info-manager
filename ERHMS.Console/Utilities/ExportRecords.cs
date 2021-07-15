using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.IO;

namespace ERHMS.Console.Utilities
{
    public class ExportRecords : IUtility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }
        public string OutputPath { get; }

        public ExportRecords(string projectPath, string viewName, string outputPath)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
            OutputPath = outputPath;
        }

        public void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            using (Stream stream = File.Open(OutputPath, FileMode.Create, FileAccess.Write))
            using (TextWriter writer = new StreamWriter(stream))
            using (RecordExporter exporter = new RecordExporter(view, writer))
            {
                exporter.Export();
            }
        }
    }
}
