using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;

namespace ERHMS.EpiInfo.Data.Entities
{
    public abstract class EntityBase : DynamicObject, ICloneable
    {
        protected IDictionary<string, object> Properties { get; set; }

        protected EntityBase()
        {
            Properties = new Dictionary<string, object>();
        }

        protected EntityBase(DataRow row) : this()
        {
            SetProperties(row);
        }

        public sealed override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetProperty(binder.Name, out result);
        }

        public sealed override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetProperty(binder.Name, value);
            return true;
        }

        public bool HasProperty(string name)
        {
            return Properties.ContainsKey(name);
        }

        public object GetProperty(string name)
        {
            return Properties[name];
        }

        public T GetProperty<T>(string name)
        {
            return (T)GetProperty(name);
        }

        public bool TryGetProperty(string name, out object value)
        {
            return Properties.TryGetValue(name, out value);
        }

        public bool TryGetProperty<T>(string name, out T value)
        {
            object valueObject;
            if (TryGetProperty(name, out valueObject))
            {
                value = (T)valueObject;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public void SetProperty(string name, object value)
        {
            Properties[name] = value;
        }

        public void SetProperties(DataRow row)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                object value = row.IsNull(column) ? null : row[column];
                SetProperty(column.ColumnName, value);
            }
        }

        public object Clone()
        {
            EntityBase clone = (EntityBase)Activator.CreateInstance(GetType());
            foreach (KeyValuePair<string, object> property in Properties)
            {
                object value = property.Value;
                ICloneable cloneableValue = value as ICloneable;
                if (cloneableValue == null)
                {
                    clone.SetProperty(property.Key, value);
                }
                else
                {
                    clone.SetProperty(property.Key, cloneableValue.Clone());
                }
            }
            return clone;
        }
    }
}
