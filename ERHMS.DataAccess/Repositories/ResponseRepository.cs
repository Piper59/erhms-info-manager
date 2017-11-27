using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.DataAccess
{
    public class ResponseRepository : IRepository<Record>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(Record));
            SqlMapper.SetTypeMap(typeof(Record), typeMap);
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

        public IEnumerable<Record> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                IDictionary<int, Domain.View> views = Context.Views.Select().ToDictionary(view => view.ViewId);
                ICollection<Record> responses = new List<Record>();
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
                    sql.SelectClauses.Add(string.Format("{0}.[ResponderID]", Database.Escape(pageTableName)));
                    sql.AddTableFromClause(JoinType.Inner, pageTableName, viewTableName, ColumnNames.GLOBAL_RECORD_ID);
                    sql.OtherClauses = clauses;
                    using (IDataReader reader = connection.ExecuteReader(sql.ToString(), parameters, transaction))
                    {
                        while (reader.Read())
                        {
                            Record response = new Record
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

        public Record SelectById(object id)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<Record> SelectByResponderId(string responderId)
        {
            string clauses = "WHERE [ResponderID] = @ResponderId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ResponderId", responderId);
            return Select(clauses, parameters);
        }

        public void Insert(Record entity)
        {
            throw new NotSupportedException();
        }

        public void Update(Record entity)
        {
            throw new NotSupportedException();
        }

        public void Save(Record entity)
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

        public void Delete(Record entity)
        {
            throw new NotSupportedException();
        }
    }
}
