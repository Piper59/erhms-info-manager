﻿using System.IO;

namespace ERHMS.Console.Utilities
{
    public class DeleteProject : Utility
    {
        public string ProjectPath { get; }
        public bool Recursive { get; }

        public DeleteProject(string projectPath)
        {
            ProjectPath = projectPath;
        }

        public DeleteProject(string projectPath, bool recursive)
            : this(projectPath)
        {
            Recursive = recursive;
        }

        public override void Run()
        {
            File.Delete(ProjectPath);
            Directory.Delete(Path.GetDirectoryName(ProjectPath), Recursive);
        }
    }
}
