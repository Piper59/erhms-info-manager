using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public abstract class Mapper<TModel>
    {
        public class Mapping
        {
            public static readonly Action<XElement, TModel> Ignored = (element, model) => { };

            public static Mapping Explicit(Action<XElement, TModel> addAttribute, Action<XElement, TModel> setProperty)
            {
                return new Mapping
                {
                    AddAttribute = addAttribute,
                    SetProperty = setProperty
                };
            }

            public static Mapping Constant(object value, Action<XElement, TModel> setProperty, string attributeName)
            {
                return new Mapping
                {
                    attributeName = attributeName,
                    GetValue = model => value,
                    SetProperty = setProperty
                };
            }

            public static Mapping FromFunc(Func<TModel, object> getValue, Action<XElement, TModel> setProperty, string attributeName)
            {
                return new Mapping
                {
                    attributeName = attributeName,
                    GetValue = getValue,
                    SetProperty = setProperty
                };
            }

            public static Mapping FromExpr<TProperty>(
                Expression<Func<TModel, TProperty>> expression,
                Action<XElement, TModel> setProperty = null,
                string attributeName = null)
            {
                PropertyInfo property = (PropertyInfo)((MemberExpression)expression.Body).Member;
                Mapping mapping = new Mapping
                {
                    property = property,
                    attributeName = attributeName ?? property.Name
                };
                if (setProperty != null)
                {
                    mapping.SetProperty = setProperty;
                }
                return mapping;
            }

            private PropertyInfo property;
            private string attributeName;

            private Func<TModel, object> GetValue { get; set; }
            public Action<XElement, TModel> AddAttribute { get; set; }
            public Action<XElement, TModel> SetProperty { get; set; }

            private Mapping()
            {
                GetValue = GetValueDefault;
                AddAttribute = AddAttributeDefault;
                SetProperty = SetPropertyDefault;
            }

            private object GetValueDefault(TModel model)
            {
                return property.GetValue(model);
            }

            private void AddAttributeDefault(XElement element, TModel model)
            {
                try
                {
                    element.Add(new XAttribute(attributeName, GetValue(model)));
                }
                catch { }
            }

            private void SetPropertyDefault(XElement element, TModel model)
            {
                try
                {
                    XAttribute attribute = element.Attribute(attributeName);
                    if (string.IsNullOrEmpty(attribute.Value))
                    {
                        return;
                    }
                    object value = Convert.ChangeType(attribute.Value, property.PropertyType);
                    property.SetValue(model, value);
                }
                catch { }
            }
        }

        protected abstract string ElementName { get; }
        protected ICollection<Mapping> Mappings { get; set; }

        public virtual XElement GetElement(TModel model)
        {
            XElement element = new XElement(ElementName);
            foreach (Mapping mapping in Mappings)
            {
                mapping.AddAttribute(element, model);
            }
            return element;
        }

        public virtual void SetProperties(XElement element, TModel model)
        {
            foreach (Mapping mapping in Mappings)
            {
                mapping.SetProperty(element, model);
            }
        }
    }
}
