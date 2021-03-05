using System.IO;

namespace ERHMS.Console.Utilities
{
    public class DeleteProject : IUtility
    {
        public string ProjectPath { get; }

        public DeleteProject(string projectPath)
        {
            ProjectPath = projectPath;
        }

        public void Run()
        {
            File.Delete(ProjectPath);
            Directory.Delete(Path.GetDirectoryName(ProjectPath));
        }
    }
}
