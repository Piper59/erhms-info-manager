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
    public class ViewEntityRepository<TEntity> : IRepository<TEntity> where TEntity : ViewEntity, new()
    {
        protected static string Escape(string identifier)
        {
            return IDbConnectionExtensions.Escape(identifier);
        }

        protected static string GetParameterName(int index)
        {
            return IDbConnectionExtensions.GetParameterName(index);
        }

        private IDictionary<string, Type> types;

        public IDataContext Context { get; private set; }

        public IDatabase Database
        {
            get { return Context.Database; }
        }

        public Project Project
        {
            get { return Context.Project; }
        }

        public View View { get; private set; }

        public ViewEntityRepository(IDataContext context, View view)
        {
            Context = context;
            View = view;
        }

        private string GetSelectSql(string selectClause, string clauses)
        {
            StringBuilder fromClause = new StringBuilder();
            fromClause.Append(Escape(View.TableName));
            foreach (Page page in View.Pages)
            {
                fromClause.Insert(0, "(");
                fromClause.AppendFormat(
                    " INNER JOIN {1} ON {0}.{2} = {1}.{2})",
                    Escape(View.TableName),
                    Escape(page.TableName),
                    Escape(ColumnNames.GLOBAL_RECORD_ID));
            }
            return string.Format("SELECT {0} FROM {1} {2}", selectClause, fromClause, clauses);
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
                    entity.New = false;
                    entity.SetProperties(reader);
                    entities.Add(entity);
                }
            }
            return entities;
        }

        private ICollection<TEntity> SelectMultiQuery(IDbConnection connection, string clauses, object parameters, IDbTransaction transaction)
        {
            IDictionary<string, TEntity> entities = new Dictionary<string, TEntity>(StringComparer.OrdinalIgnoreCase);
            string sql = GetSelectSql(Escape(View.TableName) + ".*", clauses);
            using (IDataReader reader = connection.ExecuteReader(sql, parameters, transaction))
            {
                while (reader.Read())
                {
                    TEntity entity = new TEntity();
                    entity.New = false;
                    entity.SetProperties(reader);
                    entities.Add(entity.GlobalRecordId, entity);
                }
            }
            foreach (Page page in View.Pages)
            {
                sql = GetSelectSql(Escape(page.TableName) + ".*", clauses);
                using (IDataReader reader = connection.ExecuteReader(sql, parameters, transaction))
                {
                    while (reader.Read())
                    {
                        int index = reader.GetOrdinal(ColumnNames.GLOBAL_RECORD_ID);
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

        private IEnumerable<string> GetViewColumnNames(bool includeId)
        {
            if (includeId)
            {
                yield return ColumnNames.UNIQUE_KEY;
            }
            yield return ColumnNames.REC_STATUS;
            yield return ColumnNames.GLOBAL_RECORD_ID;
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

        private void Insert(IDbConnection connection, TEntity entity, string tableName, IEnumerable<string> columnNames, IDbTransaction transaction)
        {
            connection.Insert(
                tableName,
                columnNames,
                columnName => columnName,
                columnName => entity.GetProperty(columnName),
                transaction);
        }

        public void Insert(TEntity entity)
        {
            entity.Touch();
            Database.Transact((connection, transaction) =>
            {
                Insert(connection, entity, View.TableName, GetViewColumnNames(false), transaction);
                foreach (Page page in View.Pages)
                {
                    Insert(connection, entity, page.TableName, GetPageColumnNames(page, true), transaction);
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

        private void Update(IDbConnection connection, TEntity entity, string tableName, string idColumnName, IEnumerable<string> columnNames, IDbTransaction transaction)
        {
            connection.Update(
                tableName,
                idColumnName,
                columnNames,
                columnName => columnName,
                columnName => entity.GetProperty(columnName),
                transaction);
        }

        public void Update(TEntity entity)
        {
            entity.Touch();
            Database.Transact((connection, transaction) =>
            {
                Update(connection, entity, View.TableName, ColumnNames.UNIQUE_KEY, GetViewColumnNames(false), transaction);
                foreach (Page page in View.Pages)
                {
                    Update(connection, entity, page.TableName, ColumnNames.GLOBAL_RECORD_ID, GetPageColumnNames(page, false), transaction);
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
                TEntity entity = SelectById(record.GlobalRecordId);
                if (entity == null)
                {
                    entity = new TEntity();
                    entity.GlobalRecordId = record.GlobalRecordId;
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
