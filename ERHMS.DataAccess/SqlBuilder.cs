using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace ERHMS.DataAccess
{
    public class SqlBuilder
    {
        private class Column
        {
            public string TableName { get; private set; }
            public string ColumnName { get; private set; }

            public Column(string value)
            {
                IList<string> names = value.Split(new char[] { '.' }, 2);
                if (names.Count != 2)
                {
                    throw new ArgumentException("Value is not in the correct format.", nameof(value));
                }
                TableName = names[0];
                ColumnName = names[1];
            }
        }

        private static string Escape(string identifier)
        {
            return DbExtensions.Escape(identifier);
        }

        public static string GetWhereClause(IEnumerable<string> conditions)
        {
            return string.Format("WHERE {0}", string.Join(" AND ", conditions));
        }

        private ICollection<string> separators;

        public IList<string> SelectClauses { get; private set; }
        public IList<string> FromClauses { get; private set; }
        public string OtherClauses { get; set; }

        public string SplitOn
        {
            get { return string.Join(", ", separators); }
        }

        public SqlBuilder()
        {
            SelectClauses = new List<string>();
            FromClauses = new List<string>();
            separators = new List<string>();
        }

        public void AddTableSelectClause(string tableName)
        {
            SelectClauses.Add(string.Format("{0}.*", Escape(tableName)));
        }

        public void AddTableFromClause(string tableName)
        {
            FromClauses.Add(Escape(tableName));
        }

        public void AddTableFromClause(
            JoinType joinType,
            string tableNameTo,
            string columnNameTo,
            string tableNameFrom,
            string columnNameFrom,
            string alias = null)
        {
            if (alias == null)
            {
                FromClauses.Add(string.Format(
                    "{0} JOIN {1} ON {1}.{2} = {3}.{4}",
                    joinType.ToSql(),
                    Escape(tableNameTo),
                    Escape(columnNameTo),
                    Escape(tableNameFrom),
                    Escape(columnNameFrom)));
            }
            else
            {
                FromClauses.Add(string.Format(
                    "{0} JOIN {1} AS {2} ON {2}.{3} = {4}.{5}",
                    joinType.ToSql(),
                    Escape(tableNameTo),
                    Escape(alias),
                    Escape(columnNameTo),
                    Escape(tableNameFrom),
                    Escape(columnNameFrom)));
            }
        }

        public void AddTableFromClause(JoinType joinType, string valueTo, string valueFrom, string alias = null)
        {
            Column columnTo = new Column(valueTo);
            Column columnFrom = new Column(valueFrom);
            AddTableFromClause(joinType, columnTo.TableName, columnTo.ColumnName, columnFrom.TableName, columnFrom.ColumnName, alias);
        }

        public void AddTable(string tableName)
        {
            AddTableSelectClause(tableName);
            AddTableFromClause(tableName);
        }

        public void AddTable(
            JoinType joinType,
            string tableNameTo,
            string columnNameTo,
            string tableNameFrom,
            string columnNameFrom,
            string alias = null)
        {
            AddTableSelectClause(alias ?? tableNameTo);
            AddTableFromClause(joinType, tableNameTo, columnNameTo, tableNameFrom, columnNameFrom, alias);
        }

        public void AddTable(JoinType joinType, string valueTo, string valueFrom, string alias = null)
        {
            Column columnTo = new Column(valueTo);
            Column columnFrom = new Column(valueFrom);
            AddTable(joinType, columnTo.TableName, columnTo.ColumnName, columnFrom.TableName, columnFrom.ColumnName, alias);
        }

        public void AddSeparator()
        {
            string columnName = "Separator" + (separators.Count + 1);
            SelectClauses.Add(string.Format("NULL AS {0}", Escape(columnName)));
            separators.Add(columnName);
        }

        public override string ToString()
        {
            return string.Format("SELECT {0} FROM {1} {2}", GetSelectSql(), GetFromSql(), OtherClauses).Trim();
        }

        private string GetSelectSql()
        {
            return string.Join(", ", SelectClauses);
        }

        private string GetFromSql()
        {
            StringBuilder builder = new StringBuilder();
            foreach (Iterator<string> fromClause in FromClauses.Iterate())
            {
                switch (fromClause.Index)
                {
                    case 0:
                        break;
                    case 1:
                        builder.Append(" ");
                        break;
                    default:
                        builder.Insert(0, "(");
                        builder.Append(") ");
                        break;
                }
                builder.Append(fromClause.Value);
            }
            return builder.ToString();
        }
    }
}
