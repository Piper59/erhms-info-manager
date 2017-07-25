using Dapper;
using System;
using System.Reflection;

namespace ERHMS.Dapper
{
    public class PropertyMap : SqlMapper.IMemberMap
    {
        public string ColumnName { get; private set; }
        public PropertyInfo Property { get; private set; }
        public bool Id { get; set; }
        public bool Computed { get; set; }

        public FieldInfo Field
        {
            get { return null; }
        }

        public ParameterInfo Parameter
        {
            get { return null; }
        }

        public Type MemberType
        {
            get { return Property.PropertyType; }
        }

        public PropertyMap(string columnName, PropertyInfo property)
        {
            ColumnName = columnName;
            Property = property;
        }

        public PropertyMap SetId()
        {
            Id = true;
            return this;
        }

        public PropertyMap SetComputed()
        {
            Computed = true;
            return this;
        }

        public object GetValue(object obj)
        {
            return Property.GetValue(obj, null);
        }
    }
}
