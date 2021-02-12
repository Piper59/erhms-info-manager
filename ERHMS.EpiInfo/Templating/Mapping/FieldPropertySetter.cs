using Epi.Fields;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public static class FieldPropertySetter<TField, TProperty>
        where TField : Field
    {
        public abstract class Base : IFieldPropertySetter<TField>
        {
            public PropertyInfo Property { get; }

            protected Base(Expression<Func<TField, TProperty>> getter)
            {
                Property = (PropertyInfo)((MemberExpression)getter.Body).Member;
            }

            protected abstract bool TryGetValue(XField xField, out TProperty value);

            public void SetProperty(XField xField, TField field)
            {
                try
                {
                    if (TryGetValue(xField, out TProperty value))
                    {
                        Property.SetValue(field, value);
                    }
                }
                catch (Exception ex)
                {
                    throw new FieldPropertySetterException(xField, Property.Name, ex);
                }
            }
        }

        public class Simple : Base
        {
            private readonly TypeConverter converter = TypeDescriptor.GetConverter(typeof(TProperty));

            public string AttributeName { get; }

            public Simple(Expression<Func<TField, TProperty>> getter, string attributeName = null)
                : base(getter)
            {
                AttributeName = attributeName ?? Property.Name;
            }

            protected override bool TryGetValue(XField xField, out TProperty value)
            {
                if (xField.TryGetAttribute(AttributeName, out XAttribute attribute))
                {
                    value = (TProperty)converter.ConvertFromString(attribute.Value);
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }
        }

        public class Delegated : Base
        {
            private readonly FieldPropertyConverter<TProperty> converter;

            public Delegated(Expression<Func<TField, TProperty>> getter, FieldPropertyConverter<TProperty> converter)
                : base(getter)
            {
                this.converter = converter;
            }

            protected override bool TryGetValue(XField xField, out TProperty value)
            {
                return converter(xField, out value);
            }
        }
    }
}
