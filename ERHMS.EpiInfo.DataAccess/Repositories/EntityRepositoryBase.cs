using ERHMS.EpiInfo.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.DataAccess
{
    public abstract class EntityRepositoryBase<TEntity> : RepositoryBase where TEntity : EntityBase, new()
    {
        protected static class Mapper
        {
            public static void SetEntity(DataRow row, TEntity entity)
            {
                foreach (DataColumn column in row.Table.Columns)
                {
                    object value = row.IsNull(column) ? null : row[column];
                    entity.SetProperty(column.ColumnName, value);
                }
            }

            public static TEntity GetEntity(DataRow row)
            {
                TEntity entity = new TEntity();
                SetEntity(row, entity);
                entity.New = false;
                return entity;
            }

            private static TEntity FindEntity(DataRow row, DataColumn column, ICollection<TEntity> entities, StringComparison comparisonType)
            {
                object value = row[column];
                return entities.SingleOrDefault(entity => entity.PropertyEquals(column.ColumnName, value, comparisonType));
            }

            public static void SetEntities(DataTable table, DataColumn column, ICollection<TEntity> entities, StringComparison comparisonType = StringComparison.Ordinal)
            {
                foreach (DataRow row in table.Rows)
                {
                    TEntity entity = FindEntity(row, column, entities, comparisonType);
                    if (entity == null)
                    {
                        continue;
                    }
                    SetEntity(row, entity);
                }
            }

            public static IEnumerable<TEntity> GetEntities(DataTable table)
            {
                foreach (DataRow row in table.Rows)
                {
                    yield return GetEntity(row);
                }
            }
        }

        protected static Type GetDataType(string columnName, DataTable schema)
        {
            if (schema.Columns.Contains(columnName))
            {
                return schema.Columns[columnName].DataType;
            }
            else
            {
                return null;
            }
        }

        protected static DataColumn GetKeyColumn(DataTable schema)
        {
            return schema.PrimaryKey.Single();
        }

        protected static bool IsEditable(DataColumn column)
        {
            return !column.AutoIncrement && !column.ReadOnly;
        }

        protected EntityRepositoryBase(IDataDriver driver)
            : base(driver)
        { }

        protected DataParameter GetParameter(DataColumn column, object value)
        {
            return new DataParameter(Driver.GetParameterName(column.Ordinal), value);
        }

        protected DataParameter GetParameter(DataColumn column, TEntity entity)
        {
            return GetParameter(column, entity.GetProperty(column.ColumnName));
        }

        protected string GetConditionalSql(DataColumn column, object value, out DataParameter parameter, string @operator = "=")
        {
            parameter = GetParameter(column, value);
            return string.Format(
                "{0}.{1} {2} {3}",
                Driver.Escape(column.Table.TableName),
                Driver.Escape(column.ColumnName),
                @operator,
                parameter.Name);
        }

        protected string GetConditionalSql(DataColumn column, TEntity entity, out DataParameter parameter, string @operator = "=")
        {
            return GetConditionalSql(column, entity.GetProperty(column.ColumnName), out parameter, @operator);
        }

        protected DataTable GetSchema(string tableName)
        {
            string sql = string.Format("SELECT * FROM {0}", Driver.Escape(tableName));
            return Driver.GetSchema(sql);
        }

        public abstract Type GetDataType(string columnName);
        public abstract IEnumerable<TEntity> Select();
        public abstract IEnumerable<TEntity> Select(DataPredicate predicate);

        public IEnumerable<TEntity> Select(IEnumerable<DataPredicate> predicates)
        {
            ICollection<DataPredicate> predicateCollection = predicates.ToList();
            switch (predicateCollection.Count)
            {
                case 0:
                    return Select();
                case 1:
                    return Select(predicateCollection.First());
                default:
                    return Select(predicateCollection.Combine());
            }
        }

        public IEnumerable<TEntity> Select(params DataPredicate[] predicates)
        {
            return Select((IEnumerable<DataPredicate>)predicates);
        }

        protected void Insert(TEntity entity, DataTable schema, DataTransaction transaction = null)
        {
            ICollection<string> columnNames = new List<string>();
            ICollection<DataParameter> parameters = new List<DataParameter>();
            foreach (DataColumn column in schema.Columns)
            {
                if (!IsEditable(column))
                {
                    continue;
                }
                columnNames.Add(Driver.Escape(column.ColumnName));
                parameters.Add(GetParameter(column, entity));
            }
            string sql = string.Format(
                "INSERT INTO {0} ({1}) VALUES ({2})",
                Driver.Escape(schema.TableName),
                string.Join(", ", columnNames),
                string.Join(", ", parameters.Select(parameter => parameter.Name)));
            if (transaction == null)
            {
                Driver.ExecuteNonQuery(sql, parameters);
            }
            else
            {
                Driver.ExecuteNonQuery(transaction, sql, parameters);
            }
        }

        protected void Update(TEntity entity, DataTable schema, DataTransaction transaction = null)
        {
            DataColumn keyColumn = GetKeyColumn(schema);
            ICollection<string> assignments = new List<string>();
            ICollection<DataParameter> parameters = new List<DataParameter>();
            foreach (DataColumn column in schema.Columns)
            {
                if (column == keyColumn || !IsEditable(column))
                {
                    continue;
                }
                DataParameter parameter;
                assignments.Add(GetConditionalSql(column, entity, out parameter));
                parameters.Add(parameter);
            }
            string predicate;
            {
                DataParameter parameter;
                predicate = GetConditionalSql(keyColumn, entity, out parameter);
                parameters.Add(parameter);
            }
            string sql = string.Format(
                "UPDATE {0} SET {1} WHERE {2}",
                Driver.Escape(schema.TableName),
                string.Join(", ", assignments),
                predicate);
            if (transaction == null)
            {
                Driver.ExecuteNonQuery(sql, parameters);
            }
            else
            {
                Driver.ExecuteNonQuery(transaction, sql, parameters);
            }
        }
    }
}
