using Dapper;
using ERHMS.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ERHMS.Dapper
{
    public class TypeMap : SqlMapper.ITypeMap, IEnumerable<PropertyMap>
    {
        private static readonly Regex PrefixPattern = new Regex(@"^.+\.");

        private DefaultTypeMap @base;
        private IDictionary<PropertyInfo, PropertyMap> maps;

        public Type Type { get; private set; }
        public string TableName { get; set; }

        public TypeMap(Type type)
        {
            @base = new DefaultTypeMap(type);
            Type = type;
            TableName = type.Name;
            maps = new Dictionary<PropertyInfo, PropertyMap>();
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Set(property, property.Name);
            }
        }

        public PropertyMap Set(PropertyInfo property, string columnName)
        {
            PropertyMap map = new PropertyMap(columnName, property);
            maps[property] = map;
            return map;
        }

        public PropertyMap Set(string propertyName, string columnName)
        {
            return Set(Type.GetProperty(propertyName), columnName);
        }

        public PropertyMap Get(PropertyInfo property)
        {
            return maps[property];
        }

        public PropertyMap Get(string propertyName)
        {
            return Get(Type.GetProperty(propertyName));
        }

        public PropertyMap GetId()
        {
            return maps.Values.Single(map => map.Id);
        }

        public IEnumerable<PropertyMap> GetInsertable()
        {
            return maps.Values.Where(map => !map.Computed);
        }

        public IEnumerable<PropertyMap> GetUpdatable()
        {
            return maps.Values.Where(map => !map.Id && !map.Computed);
        }

        public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            return @base.FindConstructor(names, types);
        }

        public ConstructorInfo FindExplicitConstructor()
        {
            return @base.FindExplicitConstructor();
        }

        public SqlMapper.IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            return @base.GetConstructorParameter(constructor, columnName);
        }

        private bool TryGetMember(string columnName, out SqlMapper.IMemberMap result)
        {
            result = maps.Values.SingleOrDefault(map => map.ColumnName.EqualsIgnoreCase(columnName));
            return result != null;
        }

        public SqlMapper.IMemberMap GetMember(string columnName)
        {
            SqlMapper.IMemberMap map;
            if (TryGetMember(columnName, out map))
            {
                return map;
            }
            if (PrefixPattern.IsMatch(columnName))
            {
                if (TryGetMember(PrefixPattern.Replace(columnName, ""), out map))
                {
                    return map;
                }
            }
            return null;
        }

        public IEnumerator<PropertyMap> GetEnumerator()
        {
            return maps.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
