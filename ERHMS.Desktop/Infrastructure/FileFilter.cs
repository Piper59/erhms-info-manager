using ERHMS.Desktop.Properties;
using ERHMS.EpiInfo;
using System;

namespace ERHMS.Desktop.Infrastructure
{
    public static class FileFilter
    {
        public static string FromModule(Module module)
        {
            switch (module)
            {
                case Module.Analysis:
                    return Strings.FileDialog_Filter_Pgms;
                case Module.AnalysisDashboard:
                    return Strings.FileDialog_Filter_Canvases;
                case Module.Mapping:
                    return Strings.FileDialog_Filter_Maps;
                default:
                    throw new ArgumentOutOfRangeException(nameof(module));
            }
        }
    }
}
