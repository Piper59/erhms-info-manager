using Dapper;
using Epi;
using ERHMS.Data;
using ERHMS.Data.Databases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace ERHMS.EpiInfo.Data
{
    public class RecordRepository
    {
        private const string TooManyFieldsDefinedError = "3190";

        private IDatabase database;

        public View View { get; }

        public RecordRepository(View view)
        {
            View = view;
            database = DatabaseFactory.GetDatabase(view.Project);
        }

        public string Quote(string identifier) => database.Quote(identifier);
        public bool TableExists() => database.TableExists(View.TableName);

        public string GetFromClause()
        {
            StringBuilder fromClause = new StringBuilder();
            fromClause.Append(Quote(View.TableName));
            foreach (Page page in View.Pages)
            {
                fromClause.Insert(0, "(");
                fromClause.AppendFormat(
                    " INNER JOIN {0} ON {0}.{2} = {1}.{2})",
                    Quote(page.TableName),
                    Quote(View.TableName),
                    Quote(ColumnNames.GLOBAL_RECORD_ID));
            }
            fromClause.Insert(0, "FROM ");
            return fromClause.ToString();
        }

        public string GetWhereDeletedClause(bool? deleted)
        {
            if (deleted == null)
            {
                return null;
            }
            else
            {
                return $"WHERE {Quote(ColumnNames.REC_STATUS)} = {RecordStatus.FromDeleted(deleted.Value)}";
            }
        }

        public string GetSelectSql(string selectList, string clauses)
        {
            StringBuilder sql = new StringBuilder();
            string fromClause = GetFromClause();
            sql.Append($"SELECT {selectList} {fromClause}");
            if (!string.IsNullOrEmpty(clauses))
            {
                sql.Append($" {clauses}");
            }
            sql.Append(";");
            return sql.ToString();
        }

        public int Count(string clauses = null, object parameters = null)
        {
            using (IDbConnection connection = database.Connect())
            {
                string sql = GetSelectSql("COUNT(*)", clauses);
                return connection.ExecuteScalar<int>(sql, parameters);
            }
        }

        private IEnumerable<Record> SelectSingleQuery(IDbConnection connection, string clauses, object parameters)
        {
            string sql = GetSelectSql("*", clauses);
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

        private IEnumerable<Record> SelectMultiQuery(IDbConnection connection, string clauses, object parameters)
        {
            IDictionary<string, Record> records = new Dictionary<string, Record>(StringComparer.OrdinalIgnoreCase);
            {
                string sql = GetSelectSql($"{Quote(View.TableName)}.*", clauses);
                using (IDataReader reader = connection.ExecuteReader(sql, parameters))
                {
                    while (reader.Read())
                    {
                        Record record = new Record();
                        record.SetProperties(reader);
                        records[record.GlobalRecordId] = record;
                    }
                }
            }
            foreach (Page page in View.Pages)
            {
                string sql = GetSelectSql($"{Quote(page.TableName)}.*", clauses);
                using (IDataReader reader = connection.ExecuteReader(sql, parameters))
                {
                    int ordinal = reader.GetOrdinal(ColumnNames.GLOBAL_RECORD_ID);
                    while (reader.Read())
                    {
                        Record record = records[reader.GetString(ordinal)];
                        record.SetProperties(reader);
                    }
                }
            }
            return records.Values;
        }

        public ICollection<Record> Select(string clauses = null, object parameters = null)
        {
            using (IDbConnection connection = database.Connect())
            {
                try
                {
                    return SelectSingleQuery(connection, clauses, parameters).ToList();
                }
                catch (OleDbException ex)
                {
                    if (ex.Errors.Count == 1 && ex.Errors[0].SQLState == TooManyFieldsDefinedError)
                    {
                        return SelectMultiQuery(connection, clauses, parameters).ToList();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }

        public void SetDeleted(Record record, bool deleted)
        {
            using (IDbConnection connection = database.Connect())
            {
                string sql = $@"
                    UPDATE {Quote(View.TableName)}
                    SET {Quote(ColumnNames.REC_STATUS)} = {RecordStatus.FromDeleted(deleted)}
                    WHERE {Quote(ColumnNames.UNIQUE_KEY)} = @UniqueKey";
                ParameterCollection parameters = new ParameterCollection
                {
                    { "@UniqueKey", record.UniqueKey.Value }
                };
                connection.Execute(sql, parameters);
                record.Deleted = deleted;
            }
        }
    }
}
