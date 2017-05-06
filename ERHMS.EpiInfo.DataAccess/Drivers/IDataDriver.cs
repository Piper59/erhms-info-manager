using System.Data;
using System.Data.Common;

namespace ERHMS.EpiInfo.DataAccess
{
    public interface IDataDriver
    {
        DataProvider Provider { get; }
        DbConnectionStringBuilder Builder { get; }
        string ConnectionString { get; }
        string DatabaseName { get; }

        string Escape(string identifier);
        string GetParameterName(int index);
        bool DatabaseExists();
        void CreateDatabase();
        bool TableExists(string tableName);
        DataTransaction BeginTransaction();
        DataTable GetSchema(string sql);
        T ExecuteScalar<T>(DataQuery query);
        T ExecuteScalar<T>(string sql);
        DataTable ExecuteQuery(DataQuery query);
        DataTable ExecuteQuery(string sql);
        void ExecuteNonQuery(DataQuery query);
        void ExecuteNonQuery(string sql);
        void ExecuteScript(string script);
    }
}
