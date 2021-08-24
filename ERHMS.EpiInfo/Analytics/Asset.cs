using Epi;
using System;
using System.IO;

namespace ERHMS.EpiInfo.Analytics
{
    public abstract class Asset
    {
        public static string GetFileExtension(Module module)
        {
            switch (module)
            {
                case Module.Analysis:
                    return FileExtensions.Pgm;
                case Module.AnalysisDashboard:
                    return FileExtensions.Canvas;
                case Module.Mapping:
                    return FileExtensions.Map;
                default:
                    throw new ArgumentOutOfRangeException(nameof(module));
            }
        }

        public static Asset Create(Module module, View view)
        {
            switch (module)
            {
                case Module.Analysis:
                    return new Pgm(view);
                case Module.AnalysisDashboard:
                    return new Canvas(view);
                case Module.Mapping:
                    return new Map(view);
                default:
                    throw new ArgumentOutOfRangeException(nameof(module));
            }
        }

        public View View { get; }

        public Asset(View view)
        {
            View = view;
        }

        public abstract void Save(Stream stream);
    }
}
