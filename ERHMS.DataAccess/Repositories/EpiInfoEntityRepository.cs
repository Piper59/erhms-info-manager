using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class EpiInfoEntityRepository<TEntity, TLink> : EntityRepository<TEntity>
        where TEntity : EpiInfoEntity<TLink>
        where TLink : IncidentEntity
    {
        protected TypeMap LinkTypeMap { get; private set; }

        protected EpiInfoEntityRepository(DataContext context)
            : base(context.Database)
        {
            LinkTypeMap = (TypeMap)SqlMapper.GetTypeMap(typeof(TLink));
        }

        protected virtual SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable(TypeMap.TableName);
            sql.AddSeparator();
            sql.AddTable(
                JoinType.LeftOuter,
                LinkTypeMap.TableName,
                LinkTypeMap.Get(TypeMap.GetId().Property.Name).ColumnName,
                TypeMap.TableName,
                TypeMap.GetId().ColumnName);
            sql.AddSeparator();
            sql.AddTable(JoinType.LeftOuter, "ERHMS_Incidents", "IncidentId", LinkTypeMap.TableName, "IncidentId");
            return sql;
        }

        protected virtual TEntity Map(TEntity entity, TLink link, Incident incident)
        {
            if (link.GetProperty(LinkTypeMap.GetId().ColumnName) != null)
            {
                entity.Link = link;
                link.Incident = incident;
            }
            return entity;
        }

        public override IEnumerable<TEntity> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<TEntity, TLink, Incident, TEntity>(sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public IEnumerable<TEntity> SelectUndeleted()
        {
            string clauses = "WHERE [ERHMS_Incidents].[Deleted] IS NULL OR [ERHMS_Incidents].[Deleted] = 0";
            return Select(clauses);
        }

        public override TEntity SelectById(object id)
        {
            string clauses = string.Format("WHERE {0}.{1} = @Id", Escape(TypeMap.TableName), Escape(TypeMap.GetId().ColumnName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            return Select(clauses, parameters).SingleOrDefault();
        }

        public IEnumerable<TEntity> SelectByIncidentId(string incidentId)
        {
            string clauses = string.Format("WHERE {0}.[IncidentId] = @IncidentId", Escape(LinkTypeMap.TableName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }

        public override void Insert(TEntity entity)
        {
            throw new NotSupportedException();
        }

        public override void Update(TEntity entity)
        {
            throw new NotSupportedException();
        }

        public override void Save(TEntity entity)
        {
            throw new NotSupportedException();
        }

        public override void Delete(string clauses = null, object parameters = null)
        {
            throw new NotSupportedException();
        }

        public override void DeleteById(object id)
        {
            throw new NotSupportedException();
        }

        public override void Delete(TEntity entity)
        {
            throw new NotSupportedException();
        }
    }
}
