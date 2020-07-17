using System.Collections.Generic;

namespace ERHMS.EpiInfo.Templates.Xml
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
        public static readonly ICollection<string> Tables = new string[]
        {
            SourceTable,
            GridTable
        };
        public const string Row = "Item";
    }
}
