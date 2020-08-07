using Epi.DataSets;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Projects;
using System;
using System.Windows;

namespace ERHMS.Desktop.Properties
{
    internal partial class Settings
    {
        public string GetProjectPath(ProjectType projectType)
        {
            switch (projectType)
            {
                case ProjectType.Worker:
                    return WorkerProjectPath;
                case ProjectType.Incident:
                    return IncidentProjectPath;
                default:
                    throw new ArgumentOutOfRangeException(nameof(projectType));
            }
        }

        public void ApplyTo(Epi.Configuration configuration)
        {
            if (UseFipsCrypto)
            {
                configuration.SetFipsCrypto();
            }
            Config.SettingsRow row = configuration.Settings;
            row.ControlFontSize = ControlFontSize;
            row.DefaultPageHeight = DefaultPageHeight;
            row.DefaultPageWidth = DefaultPageWidth;
            row.EditorFontSize = EditorFontSize;
            row.GridSize = GridSize;
        }

        public void ApplyTo(Window window)
        {
            window.Width = WindowWidth;
            window.Height = WindowHeight;
            window.WindowState = WindowMaximized ? WindowState.Maximized : WindowState.Normal;
        }

        public void UpdateFrom(Window window)
        {
            WindowWidth = window.RestoreBounds.Width;
            WindowHeight = window.RestoreBounds.Height;
            WindowMaximized = window.WindowState == WindowState.Maximized;
        }
    }
}
