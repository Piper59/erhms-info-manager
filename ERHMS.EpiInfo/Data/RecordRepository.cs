using Dapper;
using Epi;
using ERHMS.Data;
using ERHMS.Data.Access;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;

namespace ERHMS.EpiInfo.Data
{
    public class RecordRepository<TRecord> : Repository<TRecord>
        where TRecord : Record, new()
    {
        public View View { get; }

        public RecordRepository(View view)
            : base(view.Project.GetDatabase())
        {
            View = view;
        }

        protected int CountCore(QueryInfo query)
        {
            return ExecuteScalar<int>(query, "COUNT(*)");
        }

        public int Count()
        {
            QueryInfo query = new RecordQueryInfo(View);
            return CountCore(query);
        }

        public int CountByDeleted(bool deleted)
        {
            QueryInfo query = new RecordQueryInfo(View, deleted);
            return CountCore(query);
        }

        private void Map(
            QueryInfo query,
            string selectList,
            IDbConnection connection,
            RecordCollection<TRecord> records)
        {
            string sql = query.GetSql(selectList);
            using (IDataReader reader = connection.ExecuteReader(sql, query.Parameters))
            {
                RecordMapper<TRecord> mapper = new RecordMapper<TRecord>(reader);
                while (reader.Read())
                {
                    records.Map(mapper);
                }
            }
        }

        private IEnumerable<TRecord> SelectJointly(QueryInfo query, IDbConnection connection)
        {
            RecordCollection<TRecord> records = new RecordCollection<TRecord>();
            Map(query, "*", connection, records);
            return records;
        }

        private IEnumerable<TRecord> SelectIncrementally(QueryInfo query, IDbConnection connection)
        {
            RecordCollection<TRecord> records = new RecordCollection<TRecord>();
            Map(query, $"{Quote(View.TableName)}.*", connection, records);
            foreach (Page page in View.Pages)
            {
                Map(query, $"{Quote(page.TableName)}.*", connection, records);
            }
            return records;
        }

        protected IEnumerable<TRecord> SelectCore(QueryInfo query)
        {
            using (IDbConnection connection = Database.Connect())
            {
                try
                {
                    return SelectJointly(query, connection);
                }
                catch (OleDbException ex)
                {
                    if (ex.Errors.Count == 1 && ex.Errors[0].SQLState == ErrorCodes.TooManyFieldsDefined)
                    {
                        return SelectIncrementally(query, connection);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public IEnumerable<TRecord> Select()
        {
            QueryInfo query = new RecordQueryInfo(View);
            return SelectCore(query);
        }

        public IEnumerable<TRecord> SelectByDeleted(bool deleted)
        {
            QueryInfo query = new RecordQueryInfo(View, deleted);
            return SelectCore(query);
        }

        public TRecord SelectByGlobalRecordId(string globalRecordId)
        {
            QueryInfo query = new RecordQueryInfo(View)
            {
                Clauses = $"WHERE {Quote(View.TableName)}.{Quote(ColumnNames.GLOBAL_RECORD_ID)} = @GlobalRecordId",
                Parameters = new ParameterCollection
                {
                    { "@GlobalRecordId", globalRecordId }
                }
            };
            return SelectCore(query).SingleOrDefault();
        }

        public void SetDeleted(TRecord record, bool deleted)
        {
            string sql = $@"
                UPDATE {Quote(View.TableName)}
                SET {Quote(ColumnNames.REC_STATUS)} = @RECSTATUS
                WHERE {Quote(ColumnNames.UNIQUE_KEY)} = @UniqueKey;";
            ParameterCollection parameters = new ParameterCollection
            {
                { "@RECSTATUS", RecordStatusExtensions.FromDeleted(deleted) },
                { "@UniqueKey", record.UniqueKey.Value }
            };
            using (IDbConnection connection = Database.Connect())
            {
                connection.Execute(sql, parameters);
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

    public class RecordRepository : RecordRepository<Record>
    {
        public RecordRepository(View view)
            : base(view) { }
    }
}
