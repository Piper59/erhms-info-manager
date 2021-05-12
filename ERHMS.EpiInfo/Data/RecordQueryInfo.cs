using Epi;
using ERHMS.Data;
using System.Text;

namespace ERHMS.EpiInfo.Data
{
    public class RecordQueryInfo : QueryInfo
    {
        public View View { get; }
        public IDatabase Database { get; }

        public RecordQueryInfo(View view)
        {
            View = view;
            Database = view.Project.GetDatabase();
            StringBuilder tableSource = new StringBuilder();
            tableSource.Append(Database.Quote(View.TableName));
            foreach (Page page in View.Pages)
            {
                tableSource.Insert(0, "(");
                tableSource.AppendFormat(
                    " INNER JOIN {0} ON {0}.{2} = {1}.{2})",
                    Database.Quote(page.TableName),
                    Database.Quote(View.TableName),
                    Database.Quote(ColumnNames.GLOBAL_RECORD_ID));
            }
            TableSource = tableSource.ToString();
        }

        public RecordQueryInfo(View view, bool deleted)
            : this(view)
        {
            string op = deleted ? "=" : "<>";
            Clauses = $"WHERE {Database.Quote(ColumnNames.REC_STATUS)} {op} @RECSTATUS";
            Parameters = new ParameterCollection
            {
                { "@RECSTATUS", RecordStatus.Deleted }
            };
        }
    }
}
