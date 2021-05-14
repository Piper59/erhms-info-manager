using Epi.Fields;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public abstract class FieldPropertySetter<TField, TProperty> : IFieldPropertySetter<TField>
        where TField : Field
    {
        public class Simple : FieldPropertySetter<TField, TProperty>
        {
            private readonly TypeConverter converter = TypeDescriptor.GetConverter(typeof(TProperty));

            public string AttributeName { get; }

            public Simple(Expression<Func<TField, TProperty>> expression, string attributeName = null)
                : base(expression)
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

        public class Delegated : FieldPropertySetter<TField, TProperty>
        {
            private readonly FieldPropertyConverter<TProperty> converter;

            public Delegated(
                Expression<Func<TField, TProperty>> expression,
                FieldPropertyConverter<TProperty> converter)
                : base(expression)
            {
                this.converter = converter;
            }

            protected override bool TryGetValue(XField xField, out TProperty value)
            {
                return converter(xField, out value);
            }
        }

        public PropertyInfo Property { get; }

        protected FieldPropertySetter(Expression<Func<TField, TProperty>> expression)
        {
            Property = (PropertyInfo)((MemberExpression)expression.Body).Member;
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
}
