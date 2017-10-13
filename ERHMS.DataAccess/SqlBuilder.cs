using ERHMS.Dapper;
using System.Collections.Generic;

namespace ERHMS.DataAccess
{
    public class SqlBuilder
    {
        private static string Escape(string identifier)
        {
            return IDbConnectionExtensions.Escape(identifier);
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

        public void AddTableFromClause(JoinType joinType, string tableNameTo, string tableNameFrom, string columnName)
        {
            FromClauses.Add(string.Format(
                "{0} JOIN {1} ON {2}.{3} = {1}.{3}",
                joinType.ToSql(),
                Escape(tableNameTo),
                Escape(tableNameFrom),
                Escape(columnName)));
        }

        public void AddTable(string tableName)
        {
            AddTableSelectClause(tableName);
            AddTableFromClause(tableName);
        }

        public void AddTable(JoinType joinType, string tableNameTo, string tableNameFrom, string columnName)
        {
            AddTableSelectClause(tableNameTo);
            AddTableFromClause(joinType, tableNameTo, tableNameFrom, columnName);
        }

        public void AddSeparator()
        {
            string columnName = "Separator" + (separators.Count + 1);
            SelectClauses.Add(string.Format("NULL AS {0}", Escape(columnName)));
            separators.Add(columnName);
        }

        public override string ToString()
        {
            return string.Format(
                "SELECT {0} FROM {1}{2} {3}",
                string.Join(", ", SelectClauses),
                new string('(', FromClauses.Count - 1),
                string.Join(") ", FromClauses),
                OtherClauses);
        }
    }
}
