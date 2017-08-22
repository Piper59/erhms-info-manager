using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class LinkedEntityRepository<TEntity, TLink> : EntityRepository<TEntity>
        where TEntity : LinkedEntity<TLink>
        where TLink : Link
    {
        protected TypeMap LinkTypeMap { get; private set; }

        protected virtual PropertyMap LinkPropertyMap
        {
            get { return LinkTypeMap.Get(TypeMap.GetId().Property.Name); }
        }

        protected LinkedEntityRepository(DataContext context)
            : base(context)
        {
            LinkTypeMap = (TypeMap)SqlMapper.GetTypeMap(typeof(TLink));
        }

        protected virtual SqlBuilder GetSelectSql()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable(TypeMap.TableName);
            sql.AddSeparator();
            sql.AddTable(new JoinInfo(JoinType.LeftOuter, LinkTypeMap.TableName, LinkPropertyMap.ColumnName, TypeMap.TableName, TypeMap.GetId().ColumnName));
            sql.AddSeparator();
            sql.AddTable(new JoinInfo(JoinType.LeftOuter, "ERHMS_Incidents", "IncidentId", LinkTypeMap.TableName));
            return sql;
        }

        public override IEnumerable<TEntity> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSelectSql();
                sql.OtherClauses = clauses;
                Func<TEntity, TLink, Incident, TEntity> map = (entity, link, incident) =>
                {
                    if (link.GetProperty(LinkTypeMap.GetId().ColumnName) != null)
                    {
                        entity.Link = link;
                        link.Incident = incident;
                    }
                    return entity;
                };
                return connection.Query(sql.ToString(), map, parameters, transaction, splitOn: sql.SplitOn);
            });
        }

        public IEnumerable<TEntity> SelectUndeleted()
        {
            return Select("WHERE [ERHMS_Incidents].[Deleted] IS NULL OR [ERHMS_Incidents].[Deleted] = 0");
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
