using ERHMS.EpiInfo.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo.DataAccess
{
    public class TableRepository<TEntity> : RepositoryBase<TEntity> where TEntity : TableEntity, new()
    {
        protected DataTable Schema { get; private set; }
        public string TableName { get; private set; }

        public TableRepository(IDataDriver driver, string tableName)
            : base(driver)
        {
            TableName = tableName;
            Schema = GetSchema(tableName);
        }

        public virtual TEntity Create()
        {
            TEntity entity = new TEntity();
            foreach (DataColumn column in Schema.Columns)
            {
                entity.SetProperty(column.ColumnName, null);
            }
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

        public virtual TEntity SelectById(Guid id)
        {
            DataParameter parameter;
            string sql = GetEqualitySql(GetIdColumn(Schema), id, out parameter);
            DataPredicate predicate = new DataPredicate(sql, parameter);
            return Select(predicate).SingleOrDefault();
        }

        public virtual void Insert(TEntity entity)
        {
            if (!entity.Id.HasValue)
            {
                entity.Id = Guid.NewGuid();
            }
            Insert(entity, Schema);
            entity.IsNew = false;
        }

        public virtual void Update(TEntity entity)
        {
            Update(entity, Schema);
        }

        public virtual void Save(TEntity entity)
        {
            if (entity.IsNew)
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
            string predicate = GetEqualitySql(GetIdColumn(Schema), entity.Id, out parameter);
            string sql = string.Format("DELETE FROM {0} WHERE {1}", Driver.Escape(TableName), predicate);
            Driver.ExecuteNonQuery(sql, parameter);
        }
    }
}
