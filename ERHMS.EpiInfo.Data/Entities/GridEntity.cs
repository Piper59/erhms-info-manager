using Epi;
using System;
using System.Data;

namespace ERHMS.EpiInfo.Data.Entities
{
    public class GridEntity : EpiInfoEntity
    {
        public string UniqueRowId
        {
            get { return GetProperty<string>(ColumnNames.UNIQUE_ROW_ID); }
            internal set { SetProperty(ColumnNames.UNIQUE_ROW_ID, value); }
        }

        public GridEntity()
        {
            UniqueRowId = Guid.NewGuid().ToString();
        }

        public GridEntity(DataRow row) : base(row) { }
    }
}
