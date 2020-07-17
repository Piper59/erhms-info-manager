using Epi.Fields;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ERHMS.EpiInfo.Templates.Xml.Mapping
{
    public class FieldMappingCollection<TField> : List<IFieldMapping<TField>>
        where TField : Field
    {
        public void Add<TProperty>(
            Expression<Func<TField, TProperty>> expression,
            string attributeName = null)
        {
            Add(new AttributeFieldMapping<TField, TProperty>(expression, attributeName));
        }

        public void Add<TProperty>(
            Expression<Func<TField, TProperty>> expression,
            FieldMapping<TField, TProperty>.TryGetValueDelegate @delegate)
        {
            Add(new DelegateFieldMapping<TField, TProperty>(expression, @delegate));
        }
    }
}
