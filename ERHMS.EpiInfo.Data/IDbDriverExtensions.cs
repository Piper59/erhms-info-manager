using Epi;
using Epi.Data;
using Epi.Fields;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace ERHMS.EpiInfo.Data
{
    public static class IDbDriverExtensions
    {
        public static string Escape(this IDbDriver @this, string identifier)
        {
            return @this.InsertInEscape(identifier);
        }

        private static Query CreateQuery(this IDbDriver @this, string sql, QueryPredicate predicate)
        {
            if (predicate == null)
            {
                return @this.CreateQuery(sql);
            }
            else
            {
                string sqlWithPredicate = string.Format("{0} WHERE {1}", sql, predicate.Sql);
                Query query = @this.CreateQuery(sqlWithPredicate);
                query.Parameters.AddRange(predicate.Parameters);
                return query;
            }
        }

        private static string GetSelectSql(this IDbDriver @this, string tableName)
        {
            return string.Format("SELECT * FROM {0}", @this.Escape(tableName));
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
            return schema;
        }

        public static DataTable GetViewData(this IDbDriver @this, View view, QueryPredicate predicate = null)
        {
            DataTable data = @this.GetViewSchema(view);
            data.SetPrimaryKey(ColumnNames.GLOBAL_RECORD_ID);
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
            data.SetPrimaryKey(ColumnNames.UNIQUE_KEY);
            return data;
        }

        public static DataTable GetViewData(this IDbDriver @this, View view, IEnumerable<QueryPredicate> predicates)
        {
            return @this.GetViewData(view, QueryPredicate.Combine(predicates));
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

        public static DataTable GetViewDataByForeignKey(this IDbDriver @this, View view, string foreignKey)
        {
            string sql = string.Format("{0} = @ForeignKey", @this.Escape(ColumnNames.FOREIGN_KEY));
            QueryPredicate predicate = new QueryPredicate(sql);
            predicate.AddParameter("@ForeignKey", DbType.String, foreignKey);
            return @this.GetViewData(view, predicate);
        }

        public static DataTable GetGridSchema(this IDbDriver @this, GridField field)
        {
            DataTable schema = new DataTable();
            DbDataAdapter adapter = @this.GetDbAdapter(@this.GetSelectSql(field.TableName));
            adapter.FillSchema(schema, SchemaType.Source);
            return schema;
        }

        public static DataTable GetGridData(this IDbDriver @this, GridField field, QueryPredicate predicate = null)
        {
            Query query = @this.CreateQuery(@this.GetSelectSql(field.TableName), predicate);
            return @this.Select(query);
        }

        public static DataTable GetGridData(this IDbDriver @this, GridField field, IEnumerable<QueryPredicate> predicates)
        {
            return @this.GetGridData(field, QueryPredicate.Combine(predicates));
        }

        public static DataTable GetGridDataByForeignKey(this IDbDriver @this, GridField field, string foreignKey)
        {
            string sql = string.Format("{0} = @ForeignKey", @this.Escape(ColumnNames.FOREIGN_KEY));
            QueryPredicate predicate = new QueryPredicate(sql);
            predicate.AddParameter("@ForeignKey", DbType.String, foreignKey);
            return @this.GetGridData(field, predicate);
        }
    }
}
