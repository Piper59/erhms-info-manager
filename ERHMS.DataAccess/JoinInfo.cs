namespace ERHMS.DataAccess
{
    public class JoinInfo
    {
        public JoinType JoinType { get; set; }
        public string TableNameTo { get; set; }
        public string AliasTo { get; set; }
        public string ColumnNameTo { get; set; }
        public string TableNameOrAliasFrom { get; set; }

        private string columnNameFrom;
        public string ColumnNameFrom
        {
            get { return columnNameFrom ?? ColumnNameTo; }
            set { columnNameFrom = value; }
        }

        public JoinInfo() { }

        public JoinInfo(JoinType joinType, string tableNameTo, string columnNameTo, string tableNameOrAliasFrom, string columnNameFrom = null)
        {
            JoinType = joinType;
            TableNameTo = tableNameTo;
            ColumnNameTo = columnNameTo;
            TableNameOrAliasFrom = tableNameOrAliasFrom;
            ColumnNameFrom = columnNameFrom;
        }
    }
}
