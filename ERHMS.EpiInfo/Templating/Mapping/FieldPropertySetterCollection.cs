using Epi.Fields;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class FieldPropertySetterCollection<TField> : List<IFieldPropertySetter<TField>>
        where TField : Field
    {
        public void Add<TProperty>(Expression<Func<TField, TProperty>> expression, string attributeName = null)
        {
            Add(new FieldPropertySetter<TField, TProperty>.Simple(expression, attributeName));
        }

        public void Add<TProperty>(
            Expression<Func<TField, TProperty>> expression,
            FieldPropertyConverter<TProperty> converter)
        {
            Add(new FieldPropertySetter<TField, TProperty>.Delegated(expression, converter));
        }

    }
}
