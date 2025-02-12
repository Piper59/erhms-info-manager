﻿using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public class RelatedViewFieldMapper : FieldMapper<RelatedViewField>
    {
        protected override MetaFieldType? FieldType => MetaFieldType.Relate;

        protected override FieldPropertySetterCollection<RelatedViewField> Setters { get; }

        public RelatedViewFieldMapper()
        {
            Setters = new FieldPropertySetterCollection<RelatedViewField>
            {
                { field => field.RelatedViewID, TryGetRelatedViewId },
                { field => field.ShouldReturnToParent },
                { field => field.Condition, nameof(XField.RelateCondition) }
            };
        }

        private bool TryGetRelatedViewId(XField xField, out int value)
        {
            if (xField.RelatedViewId == null)
            {
                value = default(int);
                return false;
            }
            else
            {
                return Context.MapViewId(xField.RelatedViewId.Value, out value);
            }
        }

        public override bool MapAttributes(XField xField)
        {
            bool changed = false;
            if (xField.RelatedViewId != null && Context.MapViewId(xField.RelatedViewId.Value, out int result))
            {
                xField.RelatedViewId = result;
                changed = true;
            }
            return changed;
        }
    }
}
