using System;
using System.Collections.Generic;
using System.Data.Common;

namespace ERHMS.EpiInfo.DataAccess
{
    public class DataParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        private static DateTime RemoveMilliseconds(DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Kind);
        }

        public DataParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public object GetDbValue()
        {
            if (Value == null)
            {
                return DBNull.Value;
            }
            else if (Value is DateTime)
            {
                return RemoveMilliseconds((DateTime)Value);
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
            parameter.Value = GetDbValue();
            command.Parameters.Add(parameter);
            return parameter;
        }
    }

    public static class DataParameterExtensions
    {
        public static void AddToCommand(this IEnumerable<DataParameter> @this, DbCommand command)
        {
            foreach (DataParameter parameter in @this)
            {
                parameter.AddToCommand(command);
            }
        }
    }
}
