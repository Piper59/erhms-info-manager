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
        protected ViewEntityMapper<TEntity> Mapper { get; private set; }

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
            Log.Logger.DebugFormat("Opening view repository: {0}", view.Name);
            View = view;
            BaseSchema = GetSchema(view.TableName);
            UpdatableBaseSchema = BaseSchema.Clone();
            UpdatableBaseSchema.Columns.Remove(ColumnNames.GLOBAL_RECORD_ID);
            PageSchemas = new DataSet();
            foreach (Page page in view.Pages)
            {
                PageSchemas.Tables.Add(GetSchema(page.TableName));
            }
            Mapper = new ViewEntityMapper<TEntity>();
        }

        public Type GetDataType(string columnName)
        {
            foreach (DataTable pageSchema in PageSchemas.Tables)
            {
                if (pageSchema.Columns.Contains(columnName))
                {
                    return pageSchema.Columns[columnName].DataType;
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

        public virtual IEnumerable<TEntity> Select()
        {
            ICollection<TEntity> entities;
            {
                string sql = string.Format("SELECT * FROM {0}", Driver.Escape(BaseTableName));
                entities = Mapper.Create(Driver.ExecuteQuery(sql)).ToList();
            }
            foreach (string pageTableName in PageTableNames)
            {
                string sql = string.Format("SELECT * FROM {0}", Driver.Escape(pageTableName));
                Mapper.Update(Driver.ExecuteQuery(sql), entities);
            }
            return entities;
        }

        protected virtual IEnumerable<TEntity> Select(string predicate, params object[] values)
        {
            StringBuilder source = new StringBuilder();
            source.Append(Driver.Escape(BaseTableName));
            foreach (string pageTableName in PageTableNames)
            {
                source.Insert(0, "(");
                source.AppendFormat(
                    " INNER JOIN {1} ON {0}.{2} = {1}.{2})",
                    Driver.Escape(BaseTableName),
                    Driver.Escape(pageTableName),
                    Driver.Escape(ColumnNames.GLOBAL_RECORD_ID));
            }
            ICollection<TEntity> entities;
            {
                DataQueryBuilder builder = new DataQueryBuilder(Driver);
                builder.Sql.AppendFormat("SELECT {0}.* FROM {1} WHERE {2}", Driver.Escape(BaseTableName), source, predicate);
                foreach (object value in values)
                {
                    builder.Values.Add(value);
                }
                entities = Mapper.Create(Driver.ExecuteQuery(builder.GetQuery())).ToList();
            }
            foreach (string pageTableName in PageTableNames)
            {
                DataQueryBuilder builder = new DataQueryBuilder(Driver);
                builder.Sql.AppendFormat("SELECT {0}.* FROM {1} WHERE {2}", Driver.Escape(pageTableName), source, predicate);
                foreach (object value in values)
                {
                    builder.Values.Add(value);
                }
                Mapper.Update(Driver.ExecuteQuery(builder.GetQuery()), entities);
            }
            return entities;
        }

        public virtual IEnumerable<TEntity> SelectUndeleted()
        {
            string predicate = string.Format("{0}.{1} <> {2}", Driver.Escape(BaseTableName), ColumnNames.REC_STATUS, RecordStatus.Deleted);
            return Select(predicate);
        }

        public virtual TEntity SelectByGlobalRecordId(string globalRecordId)
        {
            string predicate = string.Format("{0}.{1} = {{@}}", Driver.Escape(BaseTableName), ColumnNames.GLOBAL_RECORD_ID);
            return Select(predicate, globalRecordId).SingleOrDefault();
        }

        public virtual void Insert(TEntity entity, IIdentity user = null)
        {
            if (entity.GlobalRecordId == null)
            {
                entity.GlobalRecordId = Guid.NewGuid().ToString();
            }
            if (!entity.RecordStatus.HasValue)
            {
                entity.Deleted = false;
            }
            entity.Touch(true, true, user);
            using (DataTransaction transaction = Driver.BeginTransaction())
            {
                Insert(BaseSchema, entity, transaction);
                foreach (DataTable pageSchema in PageSchemas.Tables)
                {
                    Insert(pageSchema, entity, transaction);
                }
                transaction.Commit();
            }
            DataQueryBuilder builder = new DataQueryBuilder(Driver);
            builder.Sql.AppendFormat(
                "SELECT {0} FROM {1} WHERE {2} = {{@}}",
                Driver.Escape(ColumnNames.UNIQUE_KEY),
                Driver.Escape(BaseTableName),
                Driver.Escape(ColumnNames.GLOBAL_RECORD_ID));
            builder.Values.Add(entity.GlobalRecordId);
            entity.UniqueKey = Driver.ExecuteScalar<int>(builder.GetQuery());
            entity.New = false;
        }

        public virtual void Update(TEntity entity, IIdentity user = null)
        {
            entity.Touch(false, true, user);
            using (DataTransaction transaction = Driver.BeginTransaction())
            {
                Update(UpdatableBaseSchema, entity, transaction);
                foreach (DataTable pageSchema in PageSchemas.Tables)
                {
                    Update(pageSchema, entity, transaction);
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
            entity.Deleted = true;
            Save(entity, user);
        }

        public virtual void Undelete(TEntity entity, IIdentity user = null)
        {
            entity.Deleted = false;
            Save(entity, user);
        }
    }
}
