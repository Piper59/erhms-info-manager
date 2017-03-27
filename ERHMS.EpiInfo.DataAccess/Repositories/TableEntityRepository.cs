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
        protected EntityMapper<TEntity> Mapper { get; private set; }

        public TableEntityRepository(IDataDriver driver, string tableName)
            : base(driver)
        {
            Log.Logger.DebugFormat("Opening table repository: {0}", tableName);
            TableName = tableName;
            Schema = GetSchema(tableName);
            Mapper = new EntityMapper<TEntity>();
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

        public virtual IEnumerable<TEntity> Select()
        {
            string sql = string.Format("SELECT * FROM {0}", Driver.Escape(TableName));
            return Mapper.Create(Driver.ExecuteQuery(sql));
        }

        protected virtual IEnumerable<TEntity> Select(string predicate, params object[] values)
        {
            DataQueryBuilder builder = new DataQueryBuilder(Driver);
            builder.Sql.AppendFormat("SELECT * FROM {0} WHERE {1}", Driver.Escape(TableName), predicate);
            foreach (object value in values)
            {
                builder.Values.Add(value);
            }
            return Mapper.Create(Driver.ExecuteQuery(builder.GetQuery()));
        }

        public virtual TEntity SelectByGuid(string guid)
        {
            string predicate = string.Format("{0} = {{@}}", Driver.Escape(Schema.PrimaryKey.Single().ColumnName));
            return Select(predicate, guid).SingleOrDefault();
        }

        public virtual void Insert(TEntity entity)
        {
            if (entity.Guid == null)
            {
                entity.Guid = Guid.NewGuid().ToString();
            }
            Insert(Schema, entity);
            entity.New = false;
        }

        public virtual void Update(TEntity entity)
        {
            Update(Schema, entity);
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

        protected virtual void Delete(string predicate, params object[] values)
        {
            DataQueryBuilder builder = new DataQueryBuilder(Driver);
            builder.Sql.AppendFormat("DELETE FROM {0} WHERE {1}", Driver.Escape(TableName), predicate);
            foreach (object value in values)
            {
                builder.Values.Add(value);
            }
            Driver.ExecuteNonQuery(builder.GetQuery());
        }

        public virtual void Delete(TEntity entity)
        {
            string predicate = string.Format("{0} = {{@}}", Driver.Escape(Schema.PrimaryKey.Single().ColumnName));
            Delete(predicate, entity.Guid);
        }
    }
}
