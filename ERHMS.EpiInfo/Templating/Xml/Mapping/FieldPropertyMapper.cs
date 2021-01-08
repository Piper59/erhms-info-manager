using Epi.Fields;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml.Mapping
{
    public abstract class FieldPropertyMapper<TField, TProperty> : IFieldPropertyMapper<TField>
        where TField : Field
    {
        public PropertyInfo Property { get; }

        protected FieldPropertyMapper(Expression<Func<TField, TProperty>> expression)
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
                throw new FieldPropertyMapperException(xField, Property.Name, ex);
            }
        }
    }

    public class AttributeFieldPropertyMapper<TField, TProperty> : FieldPropertyMapper<TField, TProperty>
        where TField : Field
    {
        private readonly TypeConverter converter = TypeDescriptor.GetConverter(typeof(TProperty));

        public string AttributeName { get; }

        public AttributeFieldPropertyMapper(Expression<Func<TField, TProperty>> expression, string attributeName = null)
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

    public delegate bool FieldPropertyValueAccessor<TProperty>(XField xField, out TProperty value);

    public class DelegateFieldPropertyMapper<TField, TProperty> : FieldPropertyMapper<TField, TProperty>
        where TField : Field
    {
        private readonly FieldPropertyValueAccessor<TProperty> accessor;

        public DelegateFieldPropertyMapper(Expression<Func<TField, TProperty>> expression, FieldPropertyValueAccessor<TProperty> accessor)
            : base(expression)
        {
            this.accessor = accessor;
        }

        protected override bool TryGetValue(XField xField, out TProperty value)
        {
            return accessor(xField, out value);
        }
    }
}
