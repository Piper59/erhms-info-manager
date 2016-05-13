using System.Collections.Generic;
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
        DataTransaction BeginTransaction();
        DataTable GetSchema(string sql);
        DataTable ExecuteQuery(DataTransaction transaction, string sql, IEnumerable<DataParameter> parameters);
        DataTable ExecuteQuery(DataTransaction transaction, string sql, params DataParameter[] parameters);
        DataTable ExecuteQuery(string sql, IEnumerable<DataParameter> parameters);
        DataTable ExecuteQuery(string sql, params DataParameter[] parameters);
        int ExecuteNonQuery(DataTransaction transaction, string sql, IEnumerable<DataParameter> parameters);
        int ExecuteNonQuery(DataTransaction transaction, string sql, params DataParameter[] parameters);
        int ExecuteNonQuery(string sql, IEnumerable<DataParameter> parameters);
        int ExecuteNonQuery(string sql, params DataParameter[] parameters);
        void ExecuteScript(string sql);
    }
}
