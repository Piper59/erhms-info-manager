using System;

namespace ERHMS.EpiInfo
{
    [Flags]
    public enum TableTypes
    {
        None = 0,
        Existing = 1 << 0,
        View = 1 << 1,
        Page = 1 << 2,
        GridField = 1 << 3,
        All = Existing | View | Page | GridField
    }
}
