using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class ResponderViewEntityRepository : IRepository<ResponderViewEntity>
    {
        private static string Escape(string identifier)
        {
            return IDbConnectionExtensions.Escape(identifier);
        }

        public DataContext Context { get; private set; }

        public IDatabase Database
        {
            get { return Context.Database; }
        }

        public ResponderViewEntityRepository(DataContext context)
        {
            Context = context;
        }

        public int Count(string clauses = null, object parameters = null)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<ResponderViewEntity> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                IDictionary<int, Domain.View> views = Context.Views.Select().ToDictionary(view => view.ViewId);
                ICollection<ResponderViewEntity> entities = new List<ResponderViewEntity>();
                DataTable fields = connection.Select("SELECT * FROM [metaFields]");
                foreach (DataRow field in fields.Select("Name = 'ResponderID'"))
                {
                    Domain.View view = views[field.Field<int>("ViewId")];
                    string viewTableName = view.Name;
                    string pageTableName = viewTableName + field.Field<int>("PageId");
                    if (!Database.TableExists(viewTableName))
                    {
                        continue;
                    }
                    SqlBuilder sql = new SqlBuilder();
                    sql.AddTable(viewTableName);
                    sql.SelectClauses.Add(string.Format("{0}.[ResponderID]", Escape(pageTableName)));
                    sql.AddTableFromClause(JoinType.Inner, pageTableName, viewTableName, ColumnNames.GLOBAL_RECORD_ID);
                    sql.OtherClauses = clauses;
                    using (IDataReader reader = connection.ExecuteReader(sql.ToString(), parameters, transaction))
                    {
                        while (reader.Read())
                        {
                            ResponderViewEntity entity = new ResponderViewEntity
                            {
                                View = view
                            };
                            entity.SetProperties(reader);
                            entities.Add(entity);
                        }
                    }
                }
                return entities;
            });
        }

        public ResponderViewEntity SelectById(object id)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<ResponderViewEntity> SelectByResponderId(string responderId)
        {
            string clauses = "WHERE [ResponderID] = @ResponderId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ResponderId", responderId);
            return Select(clauses, parameters);
        }

        public void Insert(ResponderViewEntity entity)
        {
            throw new NotSupportedException();
        }

        public void Update(ResponderViewEntity entity)
        {
            throw new NotSupportedException();
        }

        public void Save(ResponderViewEntity entity)
        {
            throw new NotSupportedException();
        }

        public void Delete(string clauses = null, object parameters = null)
        {
            throw new NotSupportedException();
        }

        public void DeleteById(object id)
        {
            throw new NotSupportedException();
        }

        public void Delete(ResponderViewEntity entity)
        {
            throw new NotSupportedException();
        }
    }
}
