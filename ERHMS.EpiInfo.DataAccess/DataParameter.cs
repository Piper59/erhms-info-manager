using ERHMS.Utility;
using System;
using System.Data.Common;

namespace ERHMS.EpiInfo.DataAccess
{
    public class DataParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public DataParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        private object GetDatabaseValue()
        {
            if (Value == null)
            {
                return DBNull.Value;
            }
            else if (Value is DateTime)
            {
                return ((DateTime)Value).RemoveMilliseconds();
            }
            else
            {
                return Value;
            }
        }

        public DbParameter AddToCommand(DbCommand command)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = Name;
            parameter.Value = GetDatabaseValue();
            command.Parameters.Add(parameter);
            return parameter;
        }
    }
}
