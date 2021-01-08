using Epi.Fields;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ERHMS.EpiInfo.Templating.Xml.Mapping
{
    public class FieldPropertyMapperCollection<TField> : List<IFieldPropertyMapper<TField>>
        where TField : Field
    {
        public void Add<TProperty>(Expression<Func<TField, TProperty>> expression, string attributeName = null)
        {
            Add(new AttributeFieldPropertyMapper<TField, TProperty>(expression, attributeName));
        }

        public void Add<TProperty>(Expression<Func<TField, TProperty>> expression, FieldPropertyValueAccessor<TProperty> accessor)
        {
            Add(new DelegateFieldPropertyMapper<TField, TProperty>(expression, accessor));
        }
    }
}
