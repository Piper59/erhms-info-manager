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
    public class ResponseRepository : IRepository<Response>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Response));
            SqlMapper.SetTypeMap(typeof(Response), typeMap);
        }

        private static string Escape(string identifier)
        {
            return IDbConnectionExtensions.Escape(identifier);
        }

        public DataContext Context { get; private set; }

        public IDatabase Database
        {
            get { return Context.Database; }
        }

        public ResponseRepository(DataContext context)
        {
            Context = context;
        }

        public int Count(string clauses = null, object parameters = null)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<Response> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                IDictionary<int, Domain.View> views = Context.Views.Select().ToDictionary(view => view.ViewId);
                ICollection<Response> responses = new List<Response>();
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
                            Response response = new Response
                            {
                                View = view
                            };
                            response.SetProperties(reader);
                            responses.Add(response);
                        }
                    }
                }
                return responses;
            });
        }

        public Response SelectById(object id)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<Response> SelectByResponderId(string responderId)
        {
            string clauses = "WHERE [ResponderID] = @ResponderId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ResponderId", responderId);
            return Select(clauses, parameters);
        }

        public void Insert(Response entity)
        {
            throw new NotSupportedException();
        }

        public void Update(Response entity)
        {
            throw new NotSupportedException();
        }

        public void Save(Response entity)
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

        public void Delete(Response entity)
        {
            throw new NotSupportedException();
        }
    }
}
