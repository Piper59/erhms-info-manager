using Epi;
using ERHMS.Data.Querying;
using System.Data;

namespace ERHMS.EpiInfo.Data
{
    partial class RecordRepository<TRecord>
    {
        public void SetDeleted(TRecord record, bool deleted)
        {
            Query.Update query = new Query.Update
            {
                TableName = Quote(View.TableName),
                Clauses = GetGlobalRecordIdPredicate(record.GlobalRecordId)
            };
            query.AddParameter(Quote(ColumnNames.REC_STATUS), RecordStatusExtensions.FromDeleted(deleted));
            int count = Database.Execute(query);
            if (count != 1)
            {
                throw new DataException($"Unexpected number of affected rows {count} (expected 1).");
            }
            record.Deleted = deleted;
        }

        public void Delete(TRecord record)
        {
            SetDeleted(record, true);
        }

        public void Undelete(TRecord record)
        {
            SetDeleted(record, false);
        }
    }
}
