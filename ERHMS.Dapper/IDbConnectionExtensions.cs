using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.Dapper
{
    public static class IDbConnectionExtensions
    {
        private class ConnectionOpener : IDisposable
        {
            private bool closed;

            public IDbConnection Connection { get; private set; }

            public ConnectionOpener(IDbConnection connection)
            {
                Connection = connection;
                closed = connection.State == ConnectionState.Closed;
                if (closed)
                {
                    connection.Open();
                }
            }

            public void Dispose()
            {
                if (closed && Connection.State != ConnectionState.Closed)
                {
                    Connection.Close();
                }
            }
        }

        public static string Escape(string identifier)
        {
            return string.Format("[{0}]", identifier.Replace("]", "]]"));
        }

        public static string GetParameterName(int index)
        {
            return "@P" + index;
        }

        public static void Execute(this IDbConnection @this, Script script)
        {
            using (new ConnectionOpener(@this))
            using (IDbTransaction transaction = @this.BeginTransaction())
            {
                try
                {
                    foreach (string sql in script)
                    {
                        if (string.IsNullOrWhiteSpace(sql))
                        {
                            continue;
                        }
                        @this.Execute(sql, transaction: transaction);
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public static DataTable GetSchema(this IDbConnection @this, string tableName, IDbTransaction transaction = null)
        {
            using (new ConnectionOpener(@this))
            using (IDbCommand command = @this.CreateCommand())
            {
                command.CommandText = string.Format("SELECT * FROM {0} WHERE 1 = 0", Escape(tableName));
                command.Transaction = transaction;
                using (IDataReader reader = command.ExecuteReader())
                {
                    DataTable schema = new DataTable();
                    schema.TableName = tableName;
                    schema.Load(reader);
                    return schema;
                }
            }
        }

        private static TypeMap GetTypeMap<TEntity>()
        {
            return (TypeMap)SqlMapper.GetTypeMap(typeof(TEntity));
        }

        public static IEnumerable<TEntity> Select<TEntity>(this IDbConnection @this, string clauses = null, object parameters = null, IDbTransaction transaction = null)
        {
            string sql = string.Format("SELECT * FROM {0} {1}", Escape(GetTypeMap<TEntity>().TableName), clauses);
            return @this.Query<TEntity>(sql, parameters, transaction);
        }

        public static TEntity SelectById<TEntity>(this IDbConnection @this, object id, IDbTransaction transaction = null)
        {
            string clauses = string.Format("WHERE {0} = @Id", Escape(GetTypeMap<TEntity>().GetId().ColumnName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return @this.Select<TEntity>(clauses, parameters, transaction).SingleOrDefault();
        }

        public static void Insert<TColumn>(this IDbConnection @this, string tableName, IEnumerable<TColumn> columns, Func<TColumn, string> name, Func<TColumn, object> value, IDbTransaction transaction = null)
        {
            ICollection<string> columnNames = new List<string>();
            ICollection<string> parameterNames = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            int parameterIndex = 0;
            foreach (TColumn column in columns)
            {
                columnNames.Add(Escape(name(column)));
                string parameterName = GetParameterName(parameterIndex++);
                parameterNames.Add(parameterName);
                parameters.Add(parameterName, value(column));
            }
            string sql = string.Format(
                "INSERT INTO {0} ({1}) VALUES ({2})",
                Escape(tableName),
                string.Join(", ", columnNames),
                string.Join(", ", parameterNames));
            @this.Execute(sql, parameters, transaction);
        }

        public static void Insert<TEntity>(this IDbConnection @this, TEntity entity, IDbTransaction transaction = null)
        {
            TypeMap typeMap = GetTypeMap<TEntity>();
            @this.Insert(
                typeMap.TableName,
                typeMap.GetInsertable(),
                propertyMap => propertyMap.ColumnName,
                propertyMap => propertyMap.Property.GetValue(entity, null),
                transaction);
        }

        public static void Update<TColumn>(this IDbConnection @this, string tableName, TColumn idColumn, IEnumerable<TColumn> columns, Func<TColumn, string> name, Func<TColumn, object> value, IDbTransaction transaction = null)
        {
            ICollection<string> assignments = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            int parameterIndex = 0;
            foreach (TColumn column in columns)
            {
                string parameterName = GetParameterName(parameterIndex++);
                assignments.Add(string.Format("{0} = {1}", Escape(name(column)), parameterName));
                parameters.Add(parameterName, value(column));
            }
            string sql = string.Format(
                "UPDATE {0} SET {1} WHERE {2} = @Id",
                Escape(tableName),
                string.Join(", ", assignments),
                Escape(name(idColumn)));
            parameters.Add("@Id", value(idColumn));
            @this.Execute(sql, parameters, transaction);
        }

        public static void Update<TEntity>(this IDbConnection @this, TEntity entity, IDbTransaction transaction = null)
        {
            TypeMap typeMap = GetTypeMap<TEntity>();
            @this.Update(
                typeMap.TableName,
                typeMap.GetId(),
                typeMap.GetUpdatable(),
                propertyMap => propertyMap.ColumnName,
                propertyMap => propertyMap.Property.GetValue(entity, null),
                transaction);
        }

        public static void Delete<TEntity>(this IDbConnection @this, string clauses = null, object parameters = null, IDbTransaction transaction = null)
        {
            string sql = string.Format("DELETE FROM {0} {1}", Escape(GetTypeMap<TEntity>().TableName), clauses);
            @this.Execute(sql, parameters, transaction);
        }

        public static void DeleteById<TEntity>(this IDbConnection @this, object id, IDbTransaction transaction = null)
        {
            string clauses = string.Format("WHERE {0} = @Id", Escape(GetTypeMap<TEntity>().GetId().ColumnName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            @this.Delete<TEntity>(clauses, parameters, transaction);
        }

        public static void Delete<TEntity>(this IDbConnection @this, TEntity entity, IDbTransaction transaction = null)
        {
            @this.DeleteById<TEntity>(GetTypeMap<TEntity>().GetId().Property.GetValue(entity, null), transaction);
        }
    }
}
