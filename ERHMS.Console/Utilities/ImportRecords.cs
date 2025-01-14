﻿using Epi;
using ERHMS.Common.Linq;
using ERHMS.Common.Logging;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System;
using System.IO;

namespace ERHMS.Console.Utilities
{
    public class ImportRecords : Utility
    {
        public string ProjectPath { get; }
        public string ViewName { get; }
        public string InputPath { get; }

        public ImportRecords(string projectPath, string viewName, string inputPath)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
            InputPath = inputPath;
        }

        public override void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            using (Stream stream = File.Open(InputPath, FileMode.Open, FileAccess.Read))
            using (TextReader reader = new StreamReader(stream))
            using (RecordImporter importer = new RecordImporter(view, reader))
            {
                foreach ((int index, string header) in importer.Headers.Iterate())
                {
                    if (view.Fields.DataFields.Contains(header))
                    {
                        importer.Map(index, view.Fields[header]);
                    }
                }
                importer.Import();
                foreach (Exception error in importer.Errors)
                {
                    Log.Instance.Warn(error.Message);
                    if (error.InnerException != null)
                    {
                        Log.Instance.Debug($"  {error.InnerException.Message}");
                    }
                }
            }
        }
    }
}
