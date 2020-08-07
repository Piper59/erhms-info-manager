using Epi.Fields;
using ERHMS.EpiInfo.Infrastructure;
using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating.Xml.Mapping
{
    public delegate bool TryGetValueFunc<TProperty>(XField xField, out TProperty value);

    public abstract class FieldMapping<TField, TProperty> : IFieldMapping<TField>
        where TField : Field
    {
        public PropertyInfo Property { get; }

        protected FieldMapping(Expression<Func<TField, TProperty>> expression)
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
                throw new FieldMappingException(xField, Property.Name, ex);
            }
        }
    }

    public class AttributeFieldMapping<TField, TProperty> : FieldMapping<TField, TProperty>
        where TField : Field
    {
        private readonly TypeConverter converter;

        public string AttributeName { get; }

        public AttributeFieldMapping(Expression<Func<TField, TProperty>> expression, string attributeName = null)
            : base(expression)
        {
            converter = TypeDescriptor.GetConverter(typeof(TProperty));
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
                value = default(TProperty);
                return false;
            }
        }
    }

    public class DelegateFieldMapping<TField, TProperty> : FieldMapping<TField, TProperty>
        where TField : Field
    {
        private readonly TryGetValueFunc<TProperty> tryGetValueFunc;

        public DelegateFieldMapping(Expression<Func<TField, TProperty>> expression, TryGetValueFunc<TProperty> tryGetValueFunc)
            : base(expression)
        {
            this.tryGetValueFunc = tryGetValueFunc;
        }

        protected override bool TryGetValue(XField xField, out TProperty value) => tryGetValueFunc(xField, out value);
    }
}
