using ERHMS.EpiInfo.Domain;
using System;
using System.Collections.Generic;
using System.Data;

namespace ERHMS.EpiInfo.DataAccess
{
    public class EntityMapper<TEntity> where TEntity : EntityBase, new()
    {
        protected virtual bool AreEqual(DataRow row, TEntity entity)
        {
            throw new NotSupportedException();
        }

        public void Update(DataRow row, TEntity entity)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                object value = row.IsNull(column) ? null : row[column];
                entity.SetProperty(column.ColumnName, value);
            }
        }

        public void Update(DataTable table, ICollection<TEntity> entities)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (TEntity entity in entities)
                {
                    if (AreEqual(row, entity))
                    {
                        Update(row, entity);
                        break;
                    }
                }
            }
        }

        public TEntity Create(DataRow row)
        {
            TEntity entity = new TEntity();
            Update(row, entity);
            entity.New = false;
            return entity;
        }

        public IEnumerable<TEntity> Create(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                yield return Create(row);
            }
        }
    }
}
