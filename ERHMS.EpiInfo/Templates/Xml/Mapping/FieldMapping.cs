using Epi.Fields;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templates.Xml.Mapping
{
    public abstract class FieldMapping<TField, TProperty> : IFieldMapping<TField>
        where TField : Field
    {
        public delegate bool TryGetValueDelegate(XField xField, out TProperty value);

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
        public string AttributeName { get; }

        public AttributeFieldMapping(Expression<Func<TField, TProperty>> expression, string attributeName = null)
            : base(expression)
        {
            AttributeName = attributeName ?? Property.Name;
        }

        protected override bool TryGetValue(XField xField, out TProperty value)
        {
            XAttribute attribute = xField.Attribute(AttributeName);
            if (attribute == null || attribute.Value == "")
            {
                value = default(TProperty);
                return false;
            }
            value = (TProperty)Convert.ChangeType(attribute.Value, typeof(TProperty));
            return true;
        }
    }

    public class DelegateFieldMapping<TField, TProperty> : FieldMapping<TField, TProperty>
        where TField : Field
    {
        public TryGetValueDelegate Delegate { get; }

        public DelegateFieldMapping(Expression<Func<TField, TProperty>> expression, TryGetValueDelegate @delegate)
            : base(expression)
        {
            Delegate = @delegate;
        }

        protected override bool TryGetValue(XField xField, out TProperty value)
        {
            return Delegate(xField, out value);
        }
    }
}
