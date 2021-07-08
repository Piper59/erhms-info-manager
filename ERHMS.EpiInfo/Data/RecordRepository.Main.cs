using Epi;
using ERHMS.Data;
using ERHMS.Data.Querying;
using System;

namespace ERHMS.EpiInfo.Data
{
    public partial class RecordRepository<TRecord> : Repository
        where TRecord : Record, new()
    {
        public View View { get; }

        public RecordRepository(View view)
            : base(view.Project.GetDatabase())
        {
            View = view;
        }

        protected IQuery GetDeletedPredicate(bool deleted)
        {
            string op = deleted ? "=" : "<>";
            return new Query.Literal
            {
                Sql = $"WHERE {Quote(ColumnNames.REC_STATUS)} {op} @RECSTATUS",
                Parameters = new ParameterCollection
                {
                    { "@RECSTATUS", RecordStatus.Deleted }
                }
            };
        }

        protected IQuery GetGlobalRecordIdPredicate(string globalRecordId)
        {
            if (globalRecordId == null)
            {
                throw new ArgumentNullException(nameof(globalRecordId));
            }
            return new Query.Literal
            {
                Sql = $"WHERE {Quote(ColumnNames.GLOBAL_RECORD_ID)} = @GlobalRecordId",
                Parameters = new ParameterCollection
                {
                    { "@GlobalRecordId", globalRecordId }
                }
            };
        }

        protected IQuery GetDefaultSortClause()
        {
            return new Query.Literal
            {
                Sql = $"ORDER BY {Quote(ColumnNames.UNIQUE_KEY)}"
            };
        }
    }

    public class RecordRepository : RecordRepository<Record>
    {
        public RecordRepository(View view)
            : base(view) { }
    }
}
