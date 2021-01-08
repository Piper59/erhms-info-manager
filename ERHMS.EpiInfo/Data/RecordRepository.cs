using Dapper;
using Epi;
using ERHMS.Data;
using ERHMS.Data.Databases;
using ERHMS.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace ERHMS.EpiInfo.Data
{
    public class RecordRepository : Repository<Record>
    {
        private static bool IsTooManyFieldsDefinedError(OleDbException exception)
        {
            return exception.Errors.Count == 1 && exception.Errors[0].SQLState == "3190";
        }

        public View View { get; }

        public RecordRepository(View view)
            : base(DatabaseFactory.GetDatabase(view.Project))
        {
            View = view;
        }

        protected override string GetFromClause()
        {
            StringBuilder clause = new StringBuilder();
            clause.Append(Quote(View.TableName));
            foreach (Page page in View.Pages)
            {
                clause.Insert(0, "(");
                clause.AppendFormat(
                    " INNER JOIN {0} ON {0}.{2} = {1}.{2})",
                    Quote(page.TableName),
                    Quote(View.TableName),
                    Quote(ColumnNames.GLOBAL_RECORD_ID));
            }
            clause.Insert(0, "FROM ");
            return clause.ToString();
        }

        private IEnumerable<Record> SingleQuerySelect(IDbConnection connection, string clauses, object parameters)
        {
            string sql = GetSelectQuery("*", clauses);
            using (IDataReader reader = connection.ExecuteReader(sql, parameters))
            {
                while (reader.Read())
                {
                    Record record = new Record();
                    record.SetProperties(reader);
                    yield return record;
                }
            }
        }

        private IEnumerable<Record> MultipleQuerySelect(IDbConnection connection, string clauses, object parameters)
        {
            string keyColumnName = ColumnNames.GLOBAL_RECORD_ID;
            IDictionary<string, Record> records = new Dictionary<string, Record>(StringComparer.OrdinalIgnoreCase);
            {
                string sql = GetSelectQuery($"{Quote(View.TableName)}.*", clauses);
                using (IDataReader reader = connection.ExecuteReader(sql, parameters))
                {
                    int keyOrdinal = reader.GetOrdinal(keyColumnName);
                    while (reader.Read())
                    {
                        string key = reader.GetString(keyOrdinal);
                        Record record = new Record();
                        record.SetProperties(reader);
                        records[key] = record;
                    }
                }
            }
            foreach (Page page in View.Pages)
            {
                string sql = GetSelectQuery($"{Quote(page.TableName)}.*", clauses);
                using (IDataReader reader = connection.ExecuteReader(sql, parameters))
                {
                    int keyOrdinal = reader.GetOrdinal(keyColumnName);
                    while (reader.Read())
                    {
                        string key = reader.GetString(keyOrdinal);
                        Record record = records[key];
                        record.SetProperties(reader);
                    }
                }
            }
            return records.Values;
        }

        public override IEnumerable<Record> Select(string clauses = null, object parameters = null)
        {
            using (IDbConnection connection = Database.Connect())
            {
                try
                {
                    return SingleQuerySelect(connection, clauses, parameters);
                }
                catch (OleDbException ex)
                {
                    if (IsTooManyFieldsDefinedError(ex))
                    {
                        return MultipleQuerySelect(connection, clauses, parameters);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public IEnumerable<Record> SelectByDeleted(bool deleted)
        {
            string @operator = deleted ? "=" : "<>";
            string clauses = $"WHERE {Quote(ColumnNames.REC_STATUS)} {@operator} @RECSTATUS";
            ParameterCollection parameters = new ParameterCollection
            {
                { "@RECSTATUS", RecordStatuses.Deleted }
            };
            return Select(clauses, parameters);
        }

        public void SetDeleted(Record record, bool deleted)
        {
            using (IDbConnection connection = Database.Connect())
            {
                string sql = $@"
                    UPDATE {Quote(View.TableName)}
                    SET {Quote(ColumnNames.REC_STATUS)} = @RECSTATUS
                    WHERE {Quote(ColumnNames.GLOBAL_RECORD_ID)} = @GlobalRecordId;";
                ParameterCollection parameters = new ParameterCollection
                {
                    { "@RECSTATUS", deleted ? RecordStatuses.Deleted : RecordStatuses.Undeleted },
                    { "@GlobalRecordId", record.GlobalRecordId }
                };
                connection.Execute(sql, parameters);
            }
            record.Deleted = deleted;
        }

        public void Delete(Record record)
        {
            SetDeleted(record, true);
        }

        public void Undelete(Record record)
        {
            SetDeleted(record, false);
        }
    }
}
