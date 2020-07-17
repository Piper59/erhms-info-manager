using Epi.Fields;
using System;
using System.Reflection;

namespace ERHMS.EpiInfo.Templates.Xml.Mapping
{
    public class FieldMapping<TAttribute> : IFieldMapping
    {
        public Func<XField, TAttribute> Accessor { get; }
        public PropertyInfo Property { get; }

        public FieldMapping(Func<XField, TAttribute> accessor, PropertyInfo property)
        {
            Accessor = accessor;
            Property = property;
        }

        public void SetProperty(XField xField, Field field)
        {
            try
            {
                Property.SetValue(field, Accessor(xField));
            }
            catch (Exception ex)
            {
                throw new FieldMappingException(xField, Property.Name, ex);
            }
        }
    }
}
