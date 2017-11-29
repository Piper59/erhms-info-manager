using Dapper;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class IncidentEntityRepository<TEntity> : EntityRepository<TEntity>
        where TEntity : IncidentEntity
    {
        protected IncidentEntityRepository(DataContext context)
            : base(context.Database) { }

        protected virtual SqlBuilder GetSqlBuilder()
        {
            SqlBuilder sql = new SqlBuilder();
            sql.AddTable(TypeMap.TableName);
            sql.AddSeparator();
            sql.AddTable(JoinType.Inner, "ERHMS_Incidents", "IncidentId", TypeMap.TableName, "IncidentId");
            return sql;
        }

        protected virtual TEntity Map(TEntity entity, Incident incident)
        {
            entity.Incident = incident;
            return entity;
        }

        public override IEnumerable<TEntity> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                SqlBuilder sql = GetSqlBuilder();
                sql.OtherClauses = clauses;
                return connection.Query<TEntity, Incident, TEntity>(sql.ToString(), Map, parameters, transaction, splitOn: sql.SplitOn);
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
            string clauses = string.Format("WHERE {0}.[IncidentId] = @IncidentId", Escape(TypeMap.TableName));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            return Select(clauses, parameters);
        }
    }
}
