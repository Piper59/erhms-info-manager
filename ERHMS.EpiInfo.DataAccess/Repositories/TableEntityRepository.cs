using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.DataAccess
{
    public class TableEntityRepository<TEntity> : EntityRepositoryBase<TEntity> where TEntity : TableEntity, new()
    {
        public string TableName { get; private set; }
        protected DataTable Schema { get; private set; }

        public TableEntityRepository(IDataDriver driver, string tableName)
            : base(driver)
        {
            Log.Current.DebugFormat("Opening table repository: {0}", tableName);
            TableName = tableName;
            Schema = GetSchema(tableName);
        }

        public override Type GetDataType(string columnName)
        {
            return GetDataType(columnName, Schema);
        }

        public virtual TEntity Create()
        {
            TEntity entity = new TEntity();
            foreach (DataColumn column in Schema.Columns)
            {
                entity.SetProperty(column.ColumnName, null);
            }
            entity.Guid = Guid.NewGuid().ToString();
            return entity;
        }

        public override IEnumerable<TEntity> Select()
        {
            string sql = string.Format("SELECT * FROM {0}", Driver.Escape(TableName));
            return Mapper.GetEntities(Driver.ExecuteQuery(sql));
        }

        public override IEnumerable<TEntity> Select(DataPredicate predicate)
        {
            string sql = string.Format("SELECT * FROM {0} WHERE {1}", Driver.Escape(TableName), predicate.Sql);
            return Mapper.GetEntities(Driver.ExecuteQuery(sql, predicate.Parameters));
        }

        public virtual TEntity SelectByGuid(string guid)
        {
            DataParameter parameter;
            string sql = GetConditionalSql(GetKeyColumn(Schema), guid, out parameter);
            DataPredicate predicate = new DataPredicate(sql, parameter);
            return Select(predicate).SingleOrDefault();
        }

        public virtual void Insert(TEntity entity)
        {
            if (entity.Guid == null)
            {
                entity.Guid = Guid.NewGuid().ToString();
            }
            Insert(entity, Schema);
            entity.New = false;
        }

        public virtual void Update(TEntity entity)
        {
            Update(entity, Schema);
        }

        public virtual void Save(TEntity entity)
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

        public virtual void Delete(TEntity entity)
        {
            DataParameter parameter;
            string predicate = GetConditionalSql(GetKeyColumn(Schema), entity, out parameter);
            string sql = string.Format("DELETE FROM {0} WHERE {1}", Driver.Escape(TableName), predicate);
            Driver.ExecuteNonQuery(sql, parameter);
        }

        public virtual void Delete(DataPredicate predicate)
        {
            string sql = string.Format("DELETE FROM {0} WHERE {1}", Driver.Escape(TableName), predicate.Sql);
            Driver.ExecuteNonQuery(sql, predicate.Parameters);
        }
    }
}
