using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public static class FieldMapper
    {
        private static readonly IReadOnlyCollection<Type> instanceTypes = typeof(IFieldMapper).Assembly.GetTypes()
            .Where(type => typeof(IFieldMapper).IsAssignableFrom(type) && !type.IsAbstract)
            .ToList();

        public static IEnumerable<IFieldMapper> GetInstances(IMappingContext mappingContext)
        {
            foreach (Type instanceType in instanceTypes)
            {
                IFieldMapper instance = (IFieldMapper)Activator.CreateInstance(instanceType);
                instance.MappingContext = mappingContext;
                yield return instance;
            }
        }
    }

    public abstract class FieldMapper<TField> : IFieldMapper<TField>
        where TField : Field
    {
        protected abstract MetaFieldType? FieldType { get; }
        protected abstract FieldPropertySetterCollection<TField> PropertySetters { get; }
        public IMappingContext MappingContext { get; set; }

        public bool IsCompatible(Field field)
        {
            return field is TField;
        }

        public bool IsCompatible(XField xField)
        {
            return xField.FieldType == FieldType;
        }

        public bool SetProperties(XField xField, TField field)
        {
            bool changed = false;
            foreach (IFieldPropertySetter<TField> propertySetter in PropertySetters)
            {
                try
                {
                    propertySetter.SetProperty(xField, field);
                    changed = true;
                }
                catch (FieldPropertySetterException ex)
                {
                    MappingContext.OnError(ex, out bool handled);
                    if (!handled)
                    {
                        throw;
                    }
                }
            }
            return changed;
        }

        public bool SetProperties(XField xField, Field field)
        {
            return SetProperties(xField, (TField)field);
        }

        public virtual bool MapProperties(TField field)
        {
            return false;
        }

        public bool MapProperties(Field field)
        {
            return MapProperties((TField)field);
        }

        public virtual bool MapAttributes(XField xField)
        {
            return false;
        }

        public virtual bool Canonize(XField xField)
        {
            return false;
        }
    }
}
