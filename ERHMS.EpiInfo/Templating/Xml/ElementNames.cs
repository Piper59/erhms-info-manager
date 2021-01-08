using System.Collections.Generic;

namespace ERHMS.EpiInfo.Templating.Xml
{
    public static class ElementNames
    {
        public const string Template = "Template";
        public const string Project = "Project";
        public const string View = "View";
        public const string Page = "Page";
        public const string Field = "Field";
        public const string SourceTable = "SourceTable";
        public const string GridTable = "GridTable";
        public const string Item = "Item";

        public static IEnumerable<string> Tables { get; } = new string[]
        {
            SourceTable,
            GridTable
        };
    }
}
