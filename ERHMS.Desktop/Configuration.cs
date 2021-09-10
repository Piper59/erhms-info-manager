﻿using Epi;
using ERHMS.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ERHMS.Desktop
{
    public class Configuration
    {
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(Configuration));

        private static string FilePath =>
            Path.Combine(EpiInfo.Configuration.Instance.Directories.Configuration, "ERHMS.Config.xml");
        public static Configuration Instance { get; private set; }

        public static void Initialize()
        {
            EpiInfo.Configuration.Initialize(ExecutionEnvironment.WindowsApplication);
            if (!File.Exists(FilePath))
            {
                Configuration configuration = new Configuration();
                configuration.Save();
            }
            Instance = Load();
        }

        private static Configuration Load()
        {
            using (Stream stream = File.Open(FilePath, FileMode.Open, FileAccess.Read))
            {
                return (Configuration)serializer.Deserialize(stream);
            }
        }

        public string WorkerProjectPath { get; set; }
        public bool HasWorkerProjectPath => WorkerProjectPath != null;

        public List<string> IncidentProjectPaths { get; set; } = new List<string>();
        public bool HasIncidentProjectPaths => IncidentProjectPaths.Count > 0;

        [XmlIgnore]
        public string IncidentProjectPath
        {
            get
            {
                return HasIncidentProjectPaths ? IncidentProjectPaths[0] : null;
            }
            set
            {
                IncidentProjectPaths.Remove(value);
                IncidentProjectPaths.Insert(0, value);
            }
        }

        public bool WindowMaximized { get; set; }
        public double WindowWidth { get; set; } = 1024;
        public double WindowHeight { get; set; } = 768;

        public event EventHandler Saved;
        private void OnSaved(EventArgs e) => Saved?.Invoke(this, e);
        private void OnSaved() => OnSaved(EventArgs.Empty);

        public void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            using (Stream stream = File.Open(FilePath, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, this);
            }
            OnSaved();
        }

        public bool HasProjectPath(CoreProject coreProject)
        {
            switch (coreProject)
            {
                case CoreProject.Worker:
                    return HasWorkerProjectPath;
                case CoreProject.Incident:
                    return HasIncidentProjectPaths;
                default:
                    throw new ArgumentOutOfRangeException(nameof(coreProject));
            }
        }

        public string GetProjectPath(CoreProject coreProject)
        {
            switch (coreProject)
            {
                case CoreProject.Worker:
                    return WorkerProjectPath;
                case CoreProject.Incident:
                    return IncidentProjectPath;
                default:
                    throw new ArgumentOutOfRangeException(nameof(coreProject));
            }
        }

        public void SetProjectPath(CoreProject coreProject, string path)
        {
            switch (coreProject)
            {
                case CoreProject.Worker:
                    WorkerProjectPath = path;
                    break;
                case CoreProject.Incident:
                    IncidentProjectPath = path;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(coreProject));
            }
        }
    }
}