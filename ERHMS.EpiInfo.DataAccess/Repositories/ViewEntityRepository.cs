using Epi;
using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace ERHMS.EpiInfo.DataAccess
{
    public class ViewEntityRepository<TEntity> : EntityRepositoryBase<TEntity> where TEntity : ViewEntity, new()
    {
        public View View { get; private set; }
        protected DataTable BaseSchema { get; private set; }
        protected DataTable UpdatableBaseSchema { get; private set; }
        protected DataSet PageSchemas { get; private set; }

        protected string BaseTableName
        {
            get { return BaseSchema.TableName; }
        }

        protected IEnumerable<string> PageTableNames
        {
            get
            {
                return PageSchemas.Tables
                    .Cast<DataTable>()
                    .Select(schema => schema.TableName);
            }
        }

        public ViewEntityRepository(IDataDriver driver, View view)
            : base(driver)
        {
            Log.Current.DebugFormat("Opening view repository: {0}", view.Name);
            View = view;
            BaseSchema = GetSchema(view.TableName);
            UpdatableBaseSchema = BaseSchema.Clone();
            UpdatableBaseSchema.Columns.Remove(ColumnNames.GLOBAL_RECORD_ID);
            PageSchemas = new DataSet();
            foreach (Page page in view.Pages)
            {
                PageSchemas.Tables.Add(GetSchema(page.TableName));
            }
        }

        protected string GetJoinSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(Driver.Escape(BaseTableName));
            foreach (string pageTableName in PageTableNames)
            {
                sql.Insert(0, "(");
                sql.Append(string.Format(
                    " INNER JOIN {1} ON {0}.{2} = {1}.{2})",
                    Driver.Escape(BaseTableName),
                    Driver.Escape(pageTableName),
                    Driver.Escape(ColumnNames.GLOBAL_RECORD_ID)));
            }
            return sql.ToString();
        }

        protected DataPredicate GetDeletedPredicate(bool deleted)
        {
            DataParameter parameter;
            string sql = GetConditionalSql(BaseSchema.Columns[ColumnNames.REC_STATUS], (short)0, out parameter, deleted ? "=" : ">");
            return new DataPredicate(sql, parameter);
        }

        public override Type GetDataType(string columnName)
        {
            foreach (DataTable pageSchema in PageSchemas.Tables)
            {
                Type dataType = GetDataType(columnName, pageSchema);
                if (dataType != null)
                {
                    return dataType;
                }
            }
            return null;
        }

        public virtual TEntity Create()
        {
            TEntity entity = new TEntity();
            foreach (DataColumn column in BaseSchema.Columns)
            {
                entity.SetProperty(column.ColumnName, null);
            }
            foreach (DataTable pageSchema in PageSchemas.Tables)
            {
                foreach (DataColumn column in pageSchema.Columns)
                {
                    entity.SetProperty(column.ColumnName, null);
                }
            }
            entity.GlobalRecordId = Guid.NewGuid().ToString();
            return entity;
        }

        public override IEnumerable<TEntity> Select()
        {
            ICollection<TEntity> entities;
            {
                string sql = string.Format("SELECT * FROM {0}", Driver.Escape(BaseTableName));
                entities = Mapper.GetEntities(Driver.ExecuteQuery(sql)).ToList();
            }
            foreach (string pageTableName in PageTableNames)
            {
                string sql = string.Format("SELECT * FROM {0}", Driver.Escape(pageTableName));
                DataTable data = Driver.ExecuteQuery(sql);
                Mapper.SetEntities(data, data.Columns[ColumnNames.GLOBAL_RECORD_ID], entities, StringComparison.OrdinalIgnoreCase);
            }
            return entities;
        }

        public override IEnumerable<TEntity> Select(DataPredicate predicate)
        {
            ICollection<TEntity> entities;
            string sqlFormat = string.Format("SELECT {{0}}.* FROM {0} WHERE {{1}}", GetJoinSql());
            {
                string sql = string.Format(sqlFormat, Driver.Escape(BaseTableName), predicate.Sql);
                entities = Mapper.GetEntities(Driver.ExecuteQuery(sql, predicate.Parameters)).ToList();
            }
            foreach (string pageTableName in PageTableNames)
            {
                string sql = string.Format(sqlFormat, Driver.Escape(pageTableName), predicate.Sql);
                DataTable data = Driver.ExecuteQuery(sql, predicate.Parameters);
                Mapper.SetEntities(data, data.Columns[ColumnNames.GLOBAL_RECORD_ID], entities, StringComparison.OrdinalIgnoreCase);
            }
            return entities;
        }

        public virtual IEnumerable<TEntity> SelectByDeleted(bool deleted)
        {
            return Select(GetDeletedPredicate(deleted));
        }

        public virtual IEnumerable<TEntity> SelectByDeleted(bool deleted, DataPredicate predicate)
        {
            return Select(GetDeletedPredicate(deleted), predicate);
        }

        public virtual IEnumerable<TEntity> SelectByDeleted(bool deleted, IEnumerable<DataPredicate> predicates)
        {
            return Select(predicates.Prepend(GetDeletedPredicate(deleted)));
        }

        public virtual IEnumerable<TEntity> SelectByDeleted(bool deleted, params DataPredicate[] predicates)
        {
            return Select(predicates.Prepend(GetDeletedPredicate(deleted)));
        }

        public virtual TEntity SelectByGlobalRecordId(string globalRecordId)
        {
            DataParameter parameter;
            string sql = GetConditionalSql(BaseSchema.Columns[ColumnNames.GLOBAL_RECORD_ID], globalRecordId, out parameter);
            DataPredicate predicate = new DataPredicate(sql, parameter);
            return Select(predicate).SingleOrDefault();
        }

        public virtual void Insert(TEntity entity, IIdentity user = null)
        {
            if (entity.GlobalRecordId == null)
            {
                entity.GlobalRecordId = Guid.NewGuid().ToString();
            }
            if (!entity.RecordStatus.HasValue)
            {
                entity.SetDeleted(false);
            }
            entity.SetAuditProperties(true, true, user);
            using (DataTransaction transaction = Driver.BeginTransaction())
            {
                Insert(entity, BaseSchema, transaction);
                foreach (DataTable pageSchema in PageSchemas.Tables)
                {
                    Insert(entity, pageSchema, transaction);
                }
                transaction.Commit();
            }
            DataParameter parameter;
            string sql = string.Format(
                "SELECT {0} FROM {1} WHERE {2}",
                Driver.Escape(ColumnNames.UNIQUE_KEY),
                Driver.Escape(BaseTableName),
                GetConditionalSql(BaseSchema.Columns[ColumnNames.GLOBAL_RECORD_ID], entity, out parameter));
            entity.UniqueKey = Driver.ExecuteQuery(sql, parameter).AsEnumerable()
                .Single()
                .Field<int>(ColumnNames.UNIQUE_KEY);
            entity.New = false;
        }

        public virtual void Update(TEntity entity, IIdentity user = null)
        {
            entity.SetAuditProperties(false, true, user);
            using (DataTransaction transaction = Driver.BeginTransaction())
            {
                Update(entity, UpdatableBaseSchema, transaction);
                foreach (DataTable pageSchema in PageSchemas.Tables)
                {
                    Update(entity, pageSchema, transaction);
                }
                transaction.Commit();
            }
        }

        public virtual void Save(TEntity entity, IIdentity user = null)
        {
            if (entity.New)
            {
                Insert(entity, user);
            }
            else
            {
                Update(entity, user);
            }
        }

        public virtual void Delete(TEntity entity, IIdentity user = null)
        {
            entity.SetDeleted(true);
            Save(entity, user);
        }

        public virtual void Undelete(TEntity entity, IIdentity user = null)
        {
            entity.SetDeleted(false);
            Save(entity, user);
        }
    }
}
