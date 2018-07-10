using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.Domain;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using View = ERHMS.Domain.View;

namespace ERHMS.DataAccess
{
    public class ResponderEntityRepository : IRepository<ResponderEntity>
    {
        public static void Configure()
        {
            TypeMap typeMap = new TypeMap(typeof(ResponderEntity));
            SqlMapper.SetTypeMap(typeof(ResponderEntity), typeMap);
        }

        private static string Escape(string identifier)
        {
            return DbExtensions.Escape(identifier);
        }

        public DataContext Context { get; private set; }

        public IDatabase Database
        {
            get { return Context.Database; }
        }

        public ResponderEntityRepository(DataContext context)
        {
            Context = context;
        }

        public int Count(string clauses = null, object parameters = null)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<ResponderEntity> Select(string clauses = null, object parameters = null)
        {
            return Database.Invoke((connection, transaction) =>
            {
                ICollection<ResponderEntity> entities = new List<ResponderEntity>();
                IDictionary<int, View> views = Context.Views.Select().ToDictionary(view => view.ViewId);
                DataTable fields;
                {
                    string sql = "SELECT * FROM [metaFields]";
                    fields = connection.Select(sql, transaction: transaction);
                }
                foreach (DataRow field in fields.Select(string.Format("Name = '{0}'", FieldNames.ResponderId)))
                {
                    View view = views[field.Field<int>("ViewId")];
                    string viewTableName = view.Name;
                    string pageTableName = viewTableName + field.Field<int>("PageId");
                    if (!Database.TableExists(viewTableName))
                    {
                        continue;
                    }
                    SqlBuilder sql = new SqlBuilder();
                    sql.AddTable(viewTableName);
                    sql.SelectClauses.Add(Escape(FieldNames.ResponderId));
                    sql.AddTableFromClause(JoinType.Inner, pageTableName, ColumnNames.GLOBAL_RECORD_ID, viewTableName, ColumnNames.GLOBAL_RECORD_ID);
                    sql.OtherClauses = clauses;
                    using (IDataReader reader = connection.ExecuteReader(sql.ToString(), parameters, transaction))
                    {
                        while (reader.Read())
                        {
                            ResponderEntity entity = new ResponderEntity
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

        public ResponderEntity SelectById(object id)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<ResponderEntity> SelectByResponderId(string responderId)
        {
            string clauses = string.Format("WHERE {0} = @ResponderId", Escape(FieldNames.ResponderId));
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ResponderId", responderId);
            return Select(clauses, parameters);
        }

        public void Insert(ResponderEntity entity)
        {
            throw new NotSupportedException();
        }

        public void Update(ResponderEntity entity)
        {
            throw new NotSupportedException();
        }

        public void Save(ResponderEntity entity)
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

        public void Delete(ResponderEntity entity)
        {
            throw new NotSupportedException();
        }
    }
}
