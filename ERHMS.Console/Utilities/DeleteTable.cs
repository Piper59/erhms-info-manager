using Epi;
using ERHMS.EpiInfo;

namespace ERHMS.Console.Utilities
{
    public class DeleteTable : Utility
    {
        public string ProjectPath { get; }
        public string TableName { get; }

        public DeleteTable(string projectPath, string tableName)
        {
            ProjectPath = projectPath;
            TableName = tableName;
        }

        public override void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            project.CollectedData.DeleteTable(TableName);
        }
    }
}
