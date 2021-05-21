using System.IO;

namespace ERHMS.Console.Utilities
{
    public class DeleteProject : IUtility
    {
        public string ProjectPath { get; }
        public bool Full { get; }

        public DeleteProject(string projectPath)
        {
            ProjectPath = projectPath;
        }

        public DeleteProject(string projectPath, bool full)
            : this(projectPath)
        {
            Full = full;
        }

        public void Run()
        {
            File.Delete(ProjectPath);
            Directory.Delete(Path.GetDirectoryName(ProjectPath), Full);
        }
    }
}
