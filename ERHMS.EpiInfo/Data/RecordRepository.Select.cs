using Epi;
using ERHMS.Data.Access;
using ERHMS.Data.Querying;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Text;

namespace ERHMS.EpiInfo.Data
{
    partial class RecordRepository<TRecord>
    {
        private IQuery GetSelectQuery(string selectList, IQuery clauses)
        {
            StringBuilder tableSource = new StringBuilder();
            tableSource.Append(Quote(View.TableName));
            foreach (Page page in View.Pages)
            {
                tableSource.Insert(0, "(");
                tableSource.AppendFormat(
                    " INNER JOIN {0} ON {0}.{2} = {1}.{2})",
                    Quote(page.TableName),
                    Quote(View.TableName),
                    Quote(ColumnNames.GLOBAL_RECORD_ID));
            }
            return new Query.Select
            {
                SelectList = selectList,
                TableSource = tableSource.ToString(),
                Clauses = clauses
            };
        }

        protected int CountCore(IQuery clauses)
        {
            IQuery query = GetSelectQuery("COUNT(*)", clauses);
            return Database.ExecuteScalar<int>(query);
        }

        public int Count()
        {
            return CountCore(null);
        }

        public int CountByDeleted(bool deleted)
        {
            return CountCore(GetDeletedPredicate(deleted));
        }

        private IEnumerable<TRecord> SelectJointly(IQuery clauses)
        {
            IQuery query = GetSelectQuery("*", clauses);
            using (IDataReader reader = Database.ExecuteReader(query))
            {
                RecordMapper<TRecord> mapper = new RecordMapper<TRecord>(reader);
                while (reader.Read())
                {
                    yield return mapper.Create();
                }
            }
        }

        private IEnumerable<TRecord> SelectIncrementally(IQuery clauses)
        {
            RecordCollection<TRecord> records = new RecordCollection<TRecord>();

            void Map(string selectList)
            {
                IQuery query = GetSelectQuery(selectList, clauses);
                using (IDataReader reader = Database.ExecuteReader(query))
                {
                    RecordMapper<TRecord> mapper = new RecordMapper<TRecord>(reader);
                    while (reader.Read())
                    {
                        records.Map(mapper);
                    }
                }
            }

            Map($"{Quote(View.TableName)}.*");
            foreach (Page page in View.Pages)
            {
                Map($"{Quote(page.TableName)}.*");
            }
            return records;
        }

        protected IEnumerable<TRecord> SelectCore(IQuery clauses)
        {
            try
            {
                return SelectJointly(clauses);
            }
            catch (OleDbException ex)
            {
                if (ex.Errors.Count == 1 && ex.Errors[0].SQLState == ErrorCodes.TooManyFieldsDefined)
                {
                    return SelectIncrementally(clauses);
                }
                else
                {
                    throw;
                }
            }
        }

        public IEnumerable<TRecord> Select()
        {
            return SelectCore(GetDefaultSortClause());
        }

        public IEnumerable<TRecord> SelectByDeleted(bool deleted)
        {
            return SelectCore(GetDeletedPredicate(deleted));
        }
    }
}
