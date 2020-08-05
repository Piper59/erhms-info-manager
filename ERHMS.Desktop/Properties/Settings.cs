using Epi.DataSets;
using ERHMS.EpiInfo;
using System.Windows;

namespace ERHMS.Desktop.Properties
{
    internal partial class Settings
    {
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
            if (window.WindowState == WindowState.Maximized)
            {
                WindowMaximized = true;
            }
            else
            {
                WindowWidth = window.Width;
                WindowHeight = window.Height;
                WindowMaximized = false;
            }
        }
    }
}
