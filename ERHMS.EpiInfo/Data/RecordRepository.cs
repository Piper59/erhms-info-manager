using Dapper;
using Epi;
using ERHMS.Data;
using ERHMS.Data.Access;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace ERHMS.EpiInfo.Data
{
    public class RecordRepository : Repository<Record>
    {
        public View View { get; }

        public RecordRepository(View view)
            : base(view.Project.GetDatabase())
        {
            View = view;
        }

        protected override string GetTableSource()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(Quote(View.TableName));
            foreach (Page page in View.Pages)
            {
                sql.Insert(0, "(");
                sql.AppendFormat(
                    " INNER JOIN {0} ON {0}.{2} = {1}.{2})",
                    Quote(page.TableName),
                    Quote(View.TableName),
                    Quote(ColumnNames.GLOBAL_RECORD_ID));
            }
            return sql.ToString();
        }

        public int CountByDeleted(bool deleted)
        {
            string op = deleted ? "=" : "<>";
            string clauses = $"WHERE {Quote(ColumnNames.REC_STATUS)} {op} @RECSTATUS";
            ParameterCollection parameters = new ParameterCollection
            {
                { "@RECSTATUS", RecordStatus.Deleted }
            };
            return Count(clauses, parameters);
        }

        private void SelectCore(
            IDbConnection connection,
            string selectList,
            string clauses,
            object parameters,
            RecordCollection records)
        {
            string sql = GetSelectStatement(selectList, clauses);
            using (IDataReader reader = connection.ExecuteReader(sql, parameters))
            {
                RecordMapper mapper = new RecordMapper(reader);
                while (reader.Read())
                {
                    records.Update(mapper);
                }
            }
        }

        private IEnumerable<Record> SelectJointly(IDbConnection connection, string clauses, object parameters)
        {
            RecordCollection records = new RecordCollection();
            SelectCore(connection, "*", clauses, parameters, records);
            return records;
        }

        private IEnumerable<Record> SelectIncrementally(IDbConnection connection, string clauses, object parameters)
        {
            RecordCollection records = new RecordCollection();
            SelectCore(connection, $"{Quote(View.TableName)}.*", clauses, parameters, records);
            foreach (Page page in View.Pages)
            {
                SelectCore(connection, $"{Quote(page.TableName)}.*", clauses, parameters, records);
            }
            return records;
        }

        public override IEnumerable<Record> Select(string clauses, object parameters)
        {
            using (IDbConnection connection = Database.Connect())
            {
                try
                {
                    return SelectJointly(connection, clauses, parameters);
                }
                catch (OleDbException ex)
                {
                    if (ex.Errors.Count == 1 && ex.Errors[0].SQLState == ErrorCodes.TooManyFieldsDefined)
                    {
                        return SelectIncrementally(connection, clauses, parameters);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public Record SelectByGlobalRecordId(string globalRecordId)
        {
            string clauses = $"WHERE {Quote(View.TableName)}.{ColumnNames.GLOBAL_RECORD_ID} = @GlobalRecordId";
            ParameterCollection parameters = new ParameterCollection
            {
                { "@GlobalRecordId", globalRecordId }
            };
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<Record> SelectByDeleted(bool deleted)
        {
            string op = deleted ? "=" : "<>";
            string clauses = $"WHERE {Quote(ColumnNames.REC_STATUS)} {op} @RECSTATUS";
            ParameterCollection parameters = new ParameterCollection
            {
                { "@RECSTATUS", RecordStatus.Deleted }
            };
            return Select(clauses, parameters);
        }

        public void SetDeleted(Record record, bool deleted)
        {
            string sql = $@"
                UPDATE {Quote(View.TableName)}
                SET {Quote(ColumnNames.REC_STATUS)} = @RECSTATUS
                WHERE {Quote(ColumnNames.UNIQUE_KEY)} = @UniqueKey;";
            ParameterCollection parameters = new ParameterCollection
            {
                { "@RECSTATUS", deleted ? RecordStatus.Deleted : RecordStatus.Undeleted },
                { "@UniqueKey", record.UniqueKey.Value }
            };
            using (IDbConnection connection = Database.Connect())
            {
                connection.Execute(sql, parameters);
            }
            record.Deleted = deleted;
        }

        public override void Delete(Record record)
        {
            SetDeleted(record, true);
        }

        public void Undelete(Record record)
        {
            SetDeleted(record, false);
        }
    }
}
