using Epi;
using System;
using System.Collections.Generic;
using System.Data;

namespace ERHMS.EpiInfo.DataAccess
{
    public interface IDataDriver : IDisposable
    {
        Project Project { get; }

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
    }
}
