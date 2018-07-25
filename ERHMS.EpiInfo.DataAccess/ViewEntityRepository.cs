using Dapper;
using Epi;
using Epi.Fields;
using ERHMS.Dapper;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Web;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;

namespace ERHMS.EpiInfo.DataAccess
{
    public class ViewEntityRepository<TEntity> : IRepository<TEntity>
        where TEntity : ViewEntity, new()
    {
        protected static TypeMap GetTypeMap()
        {
            TypeMap typeMap = new TypeMap(typeof(TEntity));
            typeMap.Get(nameof(ViewEntity.New)).SetComputed();
            typeMap.Get(nameof(ViewEntity.Id)).SetComputed();
            typeMap.Get(nameof(ViewEntity.Guid)).SetComputed();
            typeMap.Set(nameof(ViewEntity.UniqueKey), ColumnNames.UNIQUE_KEY).SetId().SetComputed();
            typeMap.Set(nameof(ViewEntity.GlobalRecordId), ColumnNames.GLOBAL_RECORD_ID);
            typeMap.Set(nameof(ViewEntity.ForeignKey), ColumnNames.FOREIGN_KEY);
            typeMap.Set(nameof(ViewEntity.RecordStatus), ColumnNames.REC_STATUS);
            typeMap.Set(nameof(ViewEntity.CreatedBy), ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME);
            typeMap.Set(nameof(ViewEntity.CreatedOn), ColumnNames.RECORD_FIRST_SAVE_TIME);
            typeMap.Set(nameof(ViewEntity.ModifiedBy), ColumnNames.RECORD_LAST_SAVE_LOGON_NAME);
            typeMap.Set(nameof(ViewEntity.ModifiedOn), ColumnNames.RECORD_LAST_SAVE_TIME);
            typeMap.Get(nameof(ViewEntity.Deleted)).SetComputed();
            return typeMap;
        }

        private IDictionary<string, Type> types;

        public IDatabase Database { get; private set; }
        public View View { get; private set; }

        public IEnumerable<string> TableNames
        {
            get { return View.Pages.Select(page => page.TableName).Prepend(View.TableName); }
        }

        public ViewEntityRepository(IDatabase database, View view)
        {
            Database = database;
            View = view;
        }

        protected string Escape(string identifier)
        {
            return Database.Escape(identifier);
        }

        private string GetSelectSql(string selectClause, string otherClauses)
        {
            StringBuilder fromClause = new StringBuilder();
            fromClause.Append(Escape(View.TableName));
            foreach (Page page in View.Pages)
            {
                fromClause.Insert(0, "(");
                fromClause.AppendFormat(
                    " INNER JOIN {0} ON {0}.{2} = {1}.{2})",
                    Escape(page.TableName),
                    Escape(View.TableName),
                    Escape(ColumnNames.GLOBAL_RECORD_ID));
            }
            return string.Format("SELECT {0} FROM {1} {2}", selectClause, fromClause, otherClauses);
        }

        public int Count(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                string sql = GetSelectSql("COUNT(*)", clauses);
                return connection.ExecuteScalar<int>(sql, parameters, transaction);
            });
        }

        private ICollection<TEntity> SelectSingleQuery(IDbConnection connection, string clauses, object parameters, IDbTransaction transaction)
        {
            ICollection<TEntity> entities = new List<TEntity>();
            string sql = GetSelectSql("*", clauses);
            using (IDataReader reader = connection.ExecuteReader(sql, parameters, transaction))
            {
                while (reader.Read())
                {
                    TEntity entity = new TEntity();
                    entity.SetProperties(reader);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        private ICollection<TEntity> SelectMultiQuery(IDbConnection connection, string clauses, object parameters, IDbTransaction transaction)
        {
            IDictionary<string, TEntity> entities = new Dictionary<string, TEntity>(StringComparer.OrdinalIgnoreCase);
            string sql = GetSelectSql(string.Format("{0}.*", Escape(View.TableName)), clauses);
            using (IDataReader reader = connection.ExecuteReader(sql, parameters, transaction))
            {
                while (reader.Read())
                {
                    TEntity entity = new TEntity();
                    entity.SetProperties(reader);
                    entities.Add(entity.GlobalRecordId, entity);
                }
            }
            foreach (Page page in View.Pages)
            {
                sql = GetSelectSql(string.Format("{0}.*", Escape(page.TableName)), clauses);
                using (IDataReader reader = connection.ExecuteReader(sql, parameters, transaction))
                {
                    int index = reader.GetOrdinal(ColumnNames.GLOBAL_RECORD_ID);
                    while (reader.Read())
                    {
                        TEntity entity = entities[reader.GetString(index)];
                        entity.SetProperties(reader);
                    }
                }
            }
            return entities.Values;
        }

        public IEnumerable<TEntity> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                try
                {
                    return SelectSingleQuery(connection, clauses, parameters, transaction);
                }
                catch (OleDbException ex)
                {
                    if (ex.Errors.Count == 1 && ex.Errors[0].SQLState == OleDbExtensions.Errors.TooManyFieldsDefined)
                    {
                        return SelectMultiQuery(connection, clauses, parameters, transaction);
                    }
                    else
                    {
                        throw;
                    }
                }
            });
        }

        public TEntity SelectById(object id)
        {
            string clauses = string.Format("WHERE {0}.{1} = @Id", Escape(View.TableName), Escape(ColumnNames.GLOBAL_RECORD_ID));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<TEntity> SelectUndeleted()
        {
            string clauses = string.Format("WHERE {0}.{1} <> 0", Escape(View.TableName), Escape(ColumnNames.REC_STATUS));
            return Select(clauses);
        }

        public IEnumerable<TEntity> SelectOrdered()
        {
            string clauses = string.Format("ORDER BY {0}.{1}", Escape(View.TableName), Escape(ColumnNames.UNIQUE_KEY));
            return Select(clauses);
        }

        private IEnumerable<string> GetViewColumnNames(bool includeId)
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

        private IEnumerable<string> GetPageColumnNames(Page page, bool includeId)
        {
            if (includeId)
            {
                yield return ColumnNames.GLOBAL_RECORD_ID;
            }
            foreach (INamedObject field in page.Fields.OfType<IInputField>())
            {
                yield return field.Name;
            }
        }

        public void Insert(TEntity entity)
        {
            entity.Touch();
            Database.Transact((connection, transaction) =>
            {
                Func<string, string> name = columnName => columnName;
                Func<string, object> value = columnName => entity.GetProperty(columnName);
                connection.Insert(View.TableName, GetViewColumnNames(true), name, value, transaction);
                foreach (Page page in View.Pages)
                {
                    connection.Insert(page.TableName, GetPageColumnNames(page, true), name, value, transaction);
                }
                string sql = string.Format(
                    "SELECT {0} FROM {1} WHERE {2} = @Id",
                    Escape(ColumnNames.UNIQUE_KEY),
                    Escape(View.TableName),
                    Escape(ColumnNames.GLOBAL_RECORD_ID));
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@Id", entity.GlobalRecordId);
                entity.UniqueKey = connection.ExecuteScalar<int>(sql, parameters, transaction);
            });
            entity.New = false;
        }

        public void Update(TEntity entity)
        {
            entity.Touch();
            Database.Transact((connection, transaction) =>
            {
                Func<string, string> name = columnName => columnName;
                Func<string, object> value = columnName => entity.GetProperty(columnName);
                connection.Update(View.TableName, ColumnNames.UNIQUE_KEY, GetViewColumnNames(false), name, value, transaction);
                foreach (Page page in View.Pages)
                {
                    connection.Update(page.TableName, ColumnNames.GLOBAL_RECORD_ID, GetPageColumnNames(page, false), name, value, transaction);
                }
            });
        }

        public void Save(TEntity entity)
        {
            if (entity.New)
            {
                Insert(entity);
            }
            else
            {
                Update(entity);
            }
        }

        public void Save(Record record)
        {
            Database.Transact((connection, transaction) =>
            {
                if (types == null)
                {
                    types = new Dictionary<string, Type>();
                    try
                    {
                        foreach (string tableName in View.Pages.Select(page => page.TableName).Prepend(View.TableName))
                        {
                            foreach (DataColumn column in connection.GetSchema(tableName, transaction).Columns)
                            {
                                types[column.ColumnName] = column.DataType;
                            }
                        }
                    }
                    catch
                    {
                        types = null;
                        throw;
                    }
                }
                string globalRecordId = record.EntityId ?? record.GlobalRecordId;
                TEntity entity = SelectById(globalRecordId);
                if (entity == null)
                {
                    entity = new TEntity
                    {
                        New = true,
                        GlobalRecordId = globalRecordId
                    };
                }
                foreach (string key in record.Keys)
                {
                    Type type;
                    if (!types.TryGetValue(key, out type))
                    {
                        continue;
                    }
                    entity.SetProperty(key, record.GetValue(key, type));
                }
                Save(entity);
            });
        }

        public void Delete(string clauses = null, object parameters = null)
        {
            throw new NotSupportedException();
        }

        public void DeleteById(object id)
        {
            throw new NotSupportedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotSupportedException();
        }
    }
}
