using Epi.Data;
using Epi.Data.Services;
using ERHMS.Utility;
using System.Collections.Generic;

namespace ERHMS.EpiInfo
{
    public static class MetadataDbProviderExtensions
    {
        public static void CreateCanvasesTable(this MetadataDbProvider @this)
        {
            Log.Current.Debug("Creating canvases table");
            @this.db.CreateTable("metaCanvases", new List<TableColumn>
            {
                new TableColumn("CanvasId", GenericDbColumnType.Int32, false, true, true),
                new TableColumn("Name", GenericDbColumnType.String, 64, false),
                new TableColumn("Content", GenericDbColumnType.StringLong, false)
            });
        }
    }
}
