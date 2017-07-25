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

        protected virtual PropertyMap LinkedId
        {
            get { return LinkTypeMap.Get(TypeMap.GetId().Property.Name); }
        }

        public new DataContext Context { get; private set; }

        protected LinkedEntityRepository(DataContext context)
            : base(context)
        {
            LinkTypeMap = (TypeMap)SqlMapper.GetTypeMap(typeof(TLink));
            Context = context;
        }

        public override IEnumerable<TEntity> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                string format = @"
                    SELECT {0}.*, NULL AS Separator1, {1}.*, NULL AS Separator2, [ERHMS_Incidents].*
                    FROM ({0}
                    LEFT OUTER JOIN {1} ON {0}.{2} = {1}.{3})
                    LEFT OUTER JOIN [ERHMS_Incidents] ON {1}.[IncidentId] = [ERHMS_Incidents].[IncidentId]
                    {4}";
                string sql = string.Format(
                    format,
                    Escape(TypeMap.TableName),
                    Escape(LinkTypeMap.TableName),
                    Escape(TypeMap.GetId().ColumnName),
                    Escape(LinkedId.ColumnName),
                    clauses);
                Func<TEntity, TLink, Incident, TEntity> map = (entity, link, incident) =>
                {
                    entity.New = false;
                    link.New = false;
                    incident.New = false;
                    if (link.GetProperty(LinkedId.ColumnName) != null)
                    {
                        entity.Link = link;
                        link.Incident = incident;
                    }
                    return entity;
                };
                return connection.Query(sql, map, parameters, transaction, splitOn: "Separator1, Separator2");
            });
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
