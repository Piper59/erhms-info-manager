using Epi;
using Epi.DataSets;
using ERHMS.EpiInfo;

namespace ERHMS.Desktop.Properties
{
    internal partial class Settings
    {
        public void Apply(Configuration configuration)
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
    }
}
