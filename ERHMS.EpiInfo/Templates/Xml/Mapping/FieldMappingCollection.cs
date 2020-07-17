using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templates.Xml.Mapping
{
    public class FieldMappingCollection<TField> : List<IFieldMapping>
    {
        private static PropertyInfo GetProperty<TProperty>(Expression<Func<TField, TProperty>> expression)
        {
            return (PropertyInfo)((MemberExpression)expression.Body).Member;
        }

        public void Add<TProperty>(Expression<Func<TField, TProperty>> expression, string attributeName = null)
        {
            PropertyInfo property = GetProperty(expression);

            TProperty Accessor(XField xField)
            {
                XAttribute attribute = xField.Attribute(attributeName ?? property.Name);
                return (TProperty)Convert.ChangeType(attribute.Value, typeof(TProperty));
            }

            Add(new FieldMapping<TProperty>(Accessor, property));
        }

        public void Add<TProperty, TAttribute>(Expression<Func<TField, TProperty>> expression, Func<XField, TAttribute> accessor)
            where TAttribute : TProperty
        {
            Add(new FieldMapping<TAttribute>(accessor, GetProperty(expression)));
        }
    }
}
