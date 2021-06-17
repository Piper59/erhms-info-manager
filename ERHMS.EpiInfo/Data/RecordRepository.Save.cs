using Epi;
using Epi.Fields;
using ERHMS.Common;
using ERHMS.Data;
using ERHMS.Data.Querying;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.Data
{
    partial class RecordRepository<TRecord>
    {
        private static IEnumerable<string> GetMetadataColumnNames(bool includeId)
        {
            yield return ColumnNames.REC_STATUS;
            if (includeId)
            {
                yield return ColumnNames.GLOBAL_RECORD_ID;
            }
            yield return ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME;
            yield return ColumnNames.RECORD_FIRST_SAVE_TIME;
            yield return ColumnNames.RECORD_LAST_SAVE_LOGON_NAME;
            yield return ColumnNames.RECORD_LAST_SAVE_TIME;
            yield return ColumnNames.FOREIGN_KEY;
        }

        private static IEnumerable<string> GetDataColumnNames(Page page, bool includeId)
        {
            if (includeId)
            {
                yield return ColumnNames.GLOBAL_RECORD_ID;
            }
            foreach (Field field in page.Fields.OfType<IDataField>())
            {
                yield return field.Name;
            }
        }

        private TQuery GetSaveQuery<TQuery>(TRecord record, string tableName, IEnumerable<string> columnNames)
            where TQuery : Query.Save, new()
        {
            TQuery query = new TQuery
            {
                TableName = Quote(tableName)
            };
            foreach (string columnName in columnNames)
            {
                if (record.TryGetProperty(columnName, out object value))
                {
                    if (value is DateTime dateTime)
                    {
                        value = dateTime.Floor();
                    }
                    query.AddParameter(Quote(columnName), value);
                }
            }
            return query;
        }

        private void InsertCore(TRecord record, string tableName, IEnumerable<string> columnNames)
        {
            Query.Insert query = GetSaveQuery<Query.Insert>(record, tableName, columnNames);
            int count = Database.Execute(query);
            if (count != 1)
            {
                throw new DataException($"Unexpected number of inserted rows {count} (expected 1).");
            }
        }

        public void Insert(TRecord record)
        {
            if (record.GlobalRecordId == null)
            {
                record.GlobalRecordId = Guid.NewGuid().ToString();
            }
            record.Touch(true);
            using (ITransactor transactor = Transact())
            {
                InsertCore(record, View.TableName, GetMetadataColumnNames(true));
                foreach (Page page in View.Pages)
                {
                    InsertCore(record, page.TableName, GetDataColumnNames(page, true));
                }
                record.UniqueKey = Database.GetLastId();
                transactor.Commit();
            }
        }

        private bool UpdateCore(TRecord record, string tableName, IEnumerable<string> columnNames)
        {
            Query.Update query = GetSaveQuery<Query.Update>(record, tableName, columnNames);
            query.Clauses = GetGlobalRecordIdPredicate(record.GlobalRecordId);
            int count = Database.Execute(query);
            if (count != 0 && count != 1)
            {
                throw new DataException($"Unexpected number of updated rows {count} (expected 0 or 1).");
            }
            return count == 1;
        }

        public bool Update(TRecord record)
        {
            record.Touch(false);
            using (ITransactor transactor = Transact())
            {
                if (!UpdateCore(record, View.TableName, GetMetadataColumnNames(false)))
                {
                    return false;
                }
                foreach (Page page in View.Pages)
                {
                    if (!UpdateCore(record, page.TableName, GetDataColumnNames(page, false)))
                    {
                        InsertCore(record, page.TableName, GetDataColumnNames(page, true));
                    }
                }
                transactor.Commit();
            }
            return true;
        }

        public void Save(TRecord record)
        {
            if (record.GlobalRecordId == null)
            {
                Insert(record);
            }
            else
            {
                if (!Update(record))
                {
                    Insert(record);
                }
            }
        }
    }
}
