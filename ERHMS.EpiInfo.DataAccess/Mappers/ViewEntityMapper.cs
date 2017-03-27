using Epi;
using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using System.Data;

namespace ERHMS.EpiInfo.DataAccess
{
    public class ViewEntityMapper<TEntity> : EntityMapper<TEntity> where TEntity : ViewEntity, new()
    {
        protected override bool AreEqual(DataRow row, TEntity entity)
        {
            return StringExtensions.EqualsIgnoreCase(row.Field<string>(ColumnNames.GLOBAL_RECORD_ID), entity.GlobalRecordId);
        }
    }
}
