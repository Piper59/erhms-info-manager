using Dapper;
using System.Collections.Generic;
using System.Text;

namespace ERHMS.Data.Querying
{
    public static class Query
    {
        public class Literal : IQuery
        {
            public string Sql { get; set; }
            public object Parameters { get; set; }
        }

        public class Select : IQuery
        {
            public string SelectList { get; set; }
            public string TableSource { get; set; }
            public IQuery Clauses { get; set; }

            public string Sql
            {
                get
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append($"SELECT {SelectList}");
                    if (TableSource != null)
                    {
                        sql.Append($" FROM {TableSource}");
                    }
                    if (Clauses != null)
                    {
                        sql.Append($" {Clauses.Sql}");
                    }
                    sql.Append(";");
                    return sql.ToString();
                }
            }

            public object Parameters
            {
                get
                {
                    if (Clauses == null)
                    {
                        return null;
                    }
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.AddDynamicParams(Clauses.Parameters);
                    return parameters;
                }
            }
        }

        public abstract class Save : IQuery
        {
            protected static string GetParameterName(int index)
            {
                return $"@P{index + 1:D3}";
            }

            public string TableName { get; set; }
            protected IList<string> ColumnNames { get; } = new List<string>();
            protected IList<object> Values { get; } = new List<object>();
            public string Sql => GetSql();
            public object Parameters => GetParameters();
            protected int ParameterCount => ColumnNames.Count;

            public void AddParameter(string columnName, object value)
            {
                ColumnNames.Add(columnName);
                Values.Add(value);
            }

            protected abstract string GetSql();

            protected virtual DynamicParameters GetParameters()
            {
                DynamicParameters parameters = new DynamicParameters();
                for (int index = 0; index < ParameterCount; index++)
                {
                    parameters.Add(GetParameterName(index), Values[index]);
                }
                return parameters;
            }
        }

        public class Insert : Save
        {
            private IEnumerable<string> GetParameterNames()
            {
                for (int index = 0; index < ParameterCount; index++)
                {
                    yield return GetParameterName(index);
                }
            }

            protected override string GetSql()
            {
                StringBuilder sql = new StringBuilder();
                sql.Append($"INSERT INTO {TableName} (");
                sql.Append(string.Join(", ", ColumnNames));
                sql.Append(") VALUES (");
                sql.Append(string.Join(", ", GetParameterNames()));
                sql.Append(");");
                return sql.ToString();
            }
        }

        public class Update : Save
        {
            public IQuery Clauses { get; set; }

            private IEnumerable<string> GetAssignments()
            {
                for (int index = 0; index < ParameterCount; index++)
                {
                    yield return $"{ColumnNames[index]} = {GetParameterName(index)}";
                }
            }

            protected override string GetSql()
            {
                StringBuilder sql = new StringBuilder();
                sql.Append($"UPDATE {TableName} SET ");
                sql.Append(string.Join(", ", GetAssignments()));
                if (Clauses != null)
                {
                    sql.Append($" {Clauses.Sql}");
                }
                sql.Append(";");
                return sql.ToString();
            }

            protected override DynamicParameters GetParameters()
            {
                DynamicParameters parameters = base.GetParameters();
                if (Clauses != null)
                {
                    parameters.AddDynamicParams(Clauses.Parameters);
                }
                return parameters;
            }
        }

        public class Delete : IQuery
        {
            public string TableName { get; set; }
            public IQuery Clauses { get; set; }

            public string Sql
            {
                get
                {
                    StringBuilder sql = new StringBuilder();
                    sql.Append($"DELETE FROM {TableName}");
                    if (Clauses != null)
                    {
                        sql.Append($" {Clauses.Sql}");
                    }
                    sql.Append(";");
                    return sql.ToString();
                }
            }

            public object Parameters
            {
                get
                {
                    if (Clauses == null)
                    {
                        return null;
                    }
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.AddDynamicParams(Clauses.Parameters);
                    return parameters;
                }
            }
        }
    }
}
