using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.DataAccess
{
    public class EntityRepositoryBase<TEntity> where TEntity : EntityBase
    {
        public IDataDriver Driver { get; private set; }

        protected EntityRepositoryBase(IDataDriver driver)
        {
            Driver = driver;
        }

        protected DataTable GetSchema(string tableName)
        {
            string sql = string.Format("SELECT * FROM {0}", Driver.Escape(tableName));
            return Driver.GetSchema(sql);
        }

        protected void Insert(DataTable schema, TEntity entity, DataTransaction transaction = null)
        {
            DataQueryBuilder builder = new DataQueryBuilder(Driver, transaction);
            ICollection<string> columnNames = new List<string>();
            foreach (DataColumn column in schema.Columns)
            {
                if (!column.IsEditable())
                {
                    continue;
                }
                columnNames.Add(Driver.Escape(column.ColumnName));
                builder.Values.Add(entity.GetProperty(column.ColumnName));
            }
            builder.Sql.AppendFormat(
                "INSERT INTO {0} ({1}) VALUES ({2})",
                Driver.Escape(schema.TableName),
                string.Join(", ", columnNames),
                string.Join(", ", Enumerable.Repeat("{@}", columnNames.Count)));
            Driver.ExecuteNonQuery(builder.GetQuery());
        }

        protected void Update(DataTable schema, TEntity entity, DataTransaction transaction = null)
        {
            DataQueryBuilder builder = new DataQueryBuilder(Driver, transaction);
            ICollection<string> assignments = new List<string>();
            ICollection<string> predicates = new List<string>();
            foreach (DataColumn column in schema.Columns)
            {
                if (schema.PrimaryKey.Contains(column) || !column.IsEditable())
                {
                    continue;
                }
                assignments.Add(string.Format("{0} = {{@}}", Driver.Escape(column.ColumnName)));
                builder.Values.Add(entity.GetProperty(column.ColumnName));
            }
            foreach (DataColumn column in schema.PrimaryKey)
            {
                predicates.Add(string.Format("{0} = {{@}}", Driver.Escape(column.ColumnName)));
                builder.Values.Add(entity.GetProperty(column.ColumnName));
            }
            builder.Sql.AppendFormat(
                "UPDATE {0} SET {1} WHERE {2}",
                Driver.Escape(schema.TableName),
                string.Join(", ", assignments),
                string.Join(" AND ", predicates));
            Driver.ExecuteNonQuery(builder.GetQuery());
        }
    }
}
