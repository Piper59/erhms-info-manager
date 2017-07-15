using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.Dapper
{
    public static class IDbConnectionExtensions
    {
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
            bool closed = @this.State == ConnectionState.Closed;
            try
            {
                if (closed)
                {
                    @this.Open();
                }
                using (IDbTransaction transaction = @this.BeginTransaction())
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
            }
            finally
            {
                if (closed)
                {
                    @this.Close();
                }
            }
        }

        private static TypeMap GetTypeMap(Type type)
        {
            return (TypeMap)SqlMapper.GetTypeMap(type);
        }

        public static IEnumerable<TEntity> Select<TEntity>(this IDbConnection @this, string sql = null, object parameters = null, IDbTransaction transaction = null)
        {
            sql = string.Format("SELECT * FROM {0} {1}", Escape(GetTypeMap(typeof(TEntity)).TableName), sql);
            return @this.Query<TEntity>(sql, parameters, transaction);
        }

        public static TEntity SelectById<TEntity>(this IDbConnection @this, object id, IDbTransaction transaction = null)
        {
            string sql = string.Format("WHERE {0} = @Id", Escape(GetTypeMap(typeof(TEntity)).GetId().ColumnName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return @this.Select<TEntity>(sql, parameters, transaction).SingleOrDefault();
        }

        public static void Insert<TEntity>(this IDbConnection @this, TEntity entity, IDbTransaction transaction = null)
        {
            TypeMap typeMap = GetTypeMap(typeof(TEntity));
            ICollection<string> columnNames = new List<string>();
            ICollection<string> parameterNames = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            int parameterIndex = 0;
            foreach (PropertyMap propertyMap in typeMap.GetInsertable())
            {
                columnNames.Add(Escape(propertyMap.ColumnName));
                string parameterName = GetParameterName(parameterIndex++);
                parameterNames.Add(parameterName);
                parameters.Add(parameterName, propertyMap.Property.GetValue(entity, null));
            }
            string sql = string.Format(
                "INSERT INTO {0} ({1}) VALUES ({2})",
                Escape(typeMap.TableName),
                string.Join(", ", columnNames),
                string.Join(", ", parameterNames));
            @this.Execute(sql, parameters, transaction);
        }

        public static void Update<TEntity>(this IDbConnection @this, TEntity entity, IDbTransaction transaction = null)
        {
            TypeMap typeMap = GetTypeMap(typeof(TEntity));
            ICollection<string> assignments = new List<string>();
            DynamicParameters parameters = new DynamicParameters();
            int parameterIndex = 0;
            foreach (PropertyMap propertyMap in typeMap.GetUpdatable())
            {
                string parameterName = GetParameterName(parameterIndex++);
                assignments.Add(string.Format("{0} = {1}", Escape(propertyMap.ColumnName), parameterName));
                parameters.Add(parameterName, propertyMap.Property.GetValue(entity, null));
            }
            PropertyMap idPropertyMap = typeMap.GetId();
            string sql = string.Format(
                "UPDATE {0} SET {1} WHERE {2} = @Id",
                Escape(typeMap.TableName),
                string.Join(", ", assignments),
                Escape(idPropertyMap.ColumnName));
            parameters.Add("@Id", idPropertyMap.Property.GetValue(entity, null));
            @this.Execute(sql, parameters, transaction);
        }

        public static void Delete<TEntity>(this IDbConnection @this, string sql = null, object parameters = null, IDbTransaction transaction = null)
        {
            sql = string.Format("DELETE FROM {0} {1}", Escape(GetTypeMap(typeof(TEntity)).TableName), sql);
            @this.Execute(sql, parameters, transaction);
        }

        public static void DeleteById<TEntity>(this IDbConnection @this, object id, IDbTransaction transaction = null)
        {
            string sql = string.Format("WHERE {0} = @Id", Escape(GetTypeMap(typeof(TEntity)).GetId().ColumnName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            @this.Delete<TEntity>(sql, parameters, transaction);
        }

        public static void Delete<TEntity>(this IDbConnection @this, TEntity entity, IDbTransaction transaction = null)
        {
            @this.DeleteById<TEntity>(GetTypeMap(typeof(TEntity)).GetId().Property.GetValue(entity, null), transaction);
        }
    }
}
