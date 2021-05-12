using Epi;
using Epi.Fields;
using ERHMS.Common.Reflection;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public static class FieldMapper
    {
        private static readonly IEnumerable<Type> instanceTypes = typeof(IFieldMapper).GetInstanceTypes().ToList();

        public static IEnumerable<IFieldMapper> GetInstances(IMappingContext context)
        {
            foreach (Type instanceType in instanceTypes)
            {
                IFieldMapper instance = (IFieldMapper)Activator.CreateInstance(instanceType);
                instance.Context = context;
                yield return instance;
            }
        }
    }

    public abstract class FieldMapper<TField> : IFieldMapper<TField>
        where TField : Field
    {
        protected abstract MetaFieldType? FieldType { get; }
        protected abstract FieldPropertySetterCollection<TField> Setters { get; }
        public IMappingContext Context { get; set; }

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
            foreach (IFieldPropertySetter<TField> setter in Setters)
            {
                try
                {
                    setter.SetProperty(xField, field);
                    changed = true;
                }
                catch (FieldPropertySetterException ex)
                {
                    Context.OnError(ex, out bool handled);
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
