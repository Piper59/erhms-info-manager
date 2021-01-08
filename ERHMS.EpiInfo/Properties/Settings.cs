using Epi;
using Epi.DataSets;

namespace ERHMS.EpiInfo.Properties
{
    partial class Settings
    {
        public void ApplyTo(Configuration configuration)
        {
            configuration.SetTextEncryptionModule(FipsCompliant);
            Config.SettingsRow row = configuration.Settings;
            row.ControlFontSize = ControlFontSize;
            row.DefaultPageHeight = DefaultPageHeight;
            row.DefaultPageWidth = DefaultPageWidth;
            row.EditorFontSize = EditorFontSize;
            row.GridSize = GridSize;
        }
    }
}
