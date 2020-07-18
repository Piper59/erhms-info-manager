using Epi;
using Epi.DataSets;
using ERHMS.EpiInfo;
using System.Windows;

namespace ERHMS.Desktop.Properties
{
    internal partial class Settings
    {
        public void WriteTo(Configuration configuration)
        {
            if (FipsCrypto)
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

        public void WriteTo(Window window)
        {
            window.Width = WindowWidth;
            window.Height = WindowHeight;
            window.WindowState = WindowMaximized ? WindowState.Maximized : WindowState.Normal;
        }

        public void ReadFrom(Window window)
        {
            WindowWidth = window.Width;
            WindowHeight = window.Height;
            WindowMaximized = window.WindowState == WindowState.Maximized;
        }
    }
}
