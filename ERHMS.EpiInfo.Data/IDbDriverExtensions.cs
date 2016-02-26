using Epi;
using Epi.Data;
using Epi.Data.Office;
using ERHMS.Utility;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.Data
{
    public static class IDbDriverExtensions
    {
        private const int OleDbQueryColumnCountMax = 255;

        /// <summary>
        /// Matches a (potentially duplicate) global record ID column name.
        /// </summary>
        /// <remarks>
        /// Consider a query that returns multiple global record ID columns (e.g., a join between a view table and its corresponding page tables).
        /// Since each table contains a global record ID column, it is ambiguous as to which of these columns came from which table.
        /// Different data providers resolve this ambiguity differently when returning data to the client.
        /// OLE DB prefixes each duplicate with the table name (e.g., View.GlobalRecordId, View1.GlobalRecordId, View2.GlobalRecordId, etc.).
        /// SQL Server appends sequential integers to each duplicate after the first (e.g., GlobalRecordId, GlobalRecordId1, GlobalRecordId2, etc.).
        /// This regular expression matches all such column names.
        /// </remarks>
        private static readonly Regex IdColumnName = new Regex(
            string.Format(@"^(?:.+\.)?{0}\d*$", Regex.Escape(ColumnNames.GLOBAL_RECORD_ID)),
            RegexOptions.IgnoreCase);

        public static string Escape(this IDbDriver @this, string identifier)
        {
            return @this.InsertInEscape(identifier);
        }

        private static Query CreateQuery(this IDbDriver @this, string sql, QueryPredicate predicate)
        {
            string sqlWithPredicate = string.Format("{0} WHERE {1}", sql, predicate.Sql);
            Query query = @this.CreateQuery(sqlWithPredicate);
            query.Parameters.AddRange(predicate.Parameters);
            return query;
        }

        private static Query CreateQuery(this IDbDriver @this, string sql, IEnumerable<QueryPredicate> predicates)
        {
            if (predicates.Any())
            {
                return @this.CreateQuery(sql, QueryPredicate.Combine(predicates));
            }
            else
            {
                return @this.CreateQuery(sql);
            }
        }

        private static string GetSelectSql(this IDbDriver @this, string tableName)
        {
            return string.Format("SELECT * FROM {0}", @this.Escape(tableName));
        }

        private static IEnumerable<DataColumn> GetIdColumns(DataTable table)
        {
            return table.Columns
                .Cast<DataColumn>()
                .Where(column => IdColumnName.IsMatch(column.ColumnName));
        }

        private static void SetPrimaryKey(DataTable table)
        {
            DataColumn keyColumn = null;
            foreach (Iterator<DataColumn> column in GetIdColumns(table).Iterate().ToList())
            {
                if (column.Index == 0)
                {
                    keyColumn = column.Value;
                }
                else
                {
                    table.Columns.Remove(column.Value);
                }
            }
            keyColumn.ColumnName = ColumnNames.GLOBAL_RECORD_ID;
            table.PrimaryKey = new DataColumn[] { keyColumn };
        }

        public static DataTable GetViewSchema(this IDbDriver @this, View view)
        {
            DataTable schema = new DataTable();
            DbDataAdapter adapter = @this.GetDbAdapter(@this.GetSelectSql(view.TableName));
            using (DbConnection connection = adapter.SelectCommand.Connection)
            {
                connection.Open();
                adapter.FillSchema(schema, SchemaType.Source);
                foreach (Page page in view.Pages)
                {
                    adapter.SelectCommand.CommandText = @this.GetSelectSql(page.TableName);
                    adapter.FillSchema(schema, SchemaType.Source);
                }
            }
            SetPrimaryKey(schema);
            schema.TableName = view.TableName;
            return schema;
        }

        public static DataTable GetViewData(this IDbDriver @this, View view, params QueryPredicate[] predicates)
        {
            DataTable data = @this.GetViewSchema(view);
            int queryColumnCount = data.Columns.Count + view.Pages.Count;  // Accounts for duplicate global record ID column in each page table
            if (@this is OleDbDatabase && queryColumnCount > OleDbQueryColumnCountMax)
            {
                QueryPredicate predicate = QueryPredicate.Combine(predicates);
                {
                    string sql = string.Format("SELECT t.* {0}", view.FromViewSQL);
                    Query query = @this.CreateQuery(sql, predicate);
                    @this.Select(query).CopyRowsTo(data);
                }
                foreach (Page page in view.Pages)
                {
                    string sql = string.Format("SELECT {0}.* {1}", @this.Escape(page.TableName), view.FromViewSQL);
                    Query query = @this.CreateQuery(sql, predicate);
                    @this.Select(query).CopyDataTo(data);
                }
            }
            else
            {
                string sql = string.Format("SELECT * {0}", view.FromViewSQL);
                Query query = @this.CreateQuery(sql, predicates);
                data = @this.Select(query);
                SetPrimaryKey(data);
            }
            data.TableName = view.TableName;
            return data;
        }

        public static DataTable GetViewData(this IDbDriver @this, View view, IEnumerable<QueryPredicate> predicates)
        {
            return @this.GetViewData(view, predicates.ToArray());
        }

        public static DataRow GetViewDataById(this IDbDriver @this, View view, string globalRecordId)
        {
            string sql = string.Format("t.{0} = @GlobalRecordId", @this.Escape(ColumnNames.GLOBAL_RECORD_ID));
            QueryPredicate predicate = new QueryPredicate(sql);
            predicate.AddParameter("@GlobalRecordId", DbType.String, globalRecordId);
            return @this.GetViewData(view, predicate)
                .AsEnumerable()
                .SingleOrDefault();
        }
    }
}
