using Epi;
using ERHMS.EpiInfo.Templating.Xml;
using ERHMS.EpiInfo.Templating.Xml.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating
{
    public class TemplateCanonizer
    {
        private class Context
        {
            private static void Add(int id, IDictionary<int, int> idMap)
            {
                idMap.Add(id, idMap.Count + 1);
            }

            public IDictionary<int, int> ViewIdMap { get; }
            public IDictionary<int, int> PageIdMap { get; }
            public IDictionary<int, int> FieldIdMap { get; }

            public Context()
            {
                ViewIdMap = new Dictionary<int, int>();
                PageIdMap = new Dictionary<int, int>();
                FieldIdMap = new Dictionary<int, int>();
            }

            public void Add(XElement element)
            {
                if (element is XField xField)
                {
                    Add(xField.FieldId, FieldIdMap);
                }
                else if (element is XPage xPage)
                {
                    Add(xPage.PageId, PageIdMap);
                }
                else if (element is XView xView)
                {
                    Add(xView.ViewId, ViewIdMap);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        private Context context;

        public XTemplate XTemplate { get; }

        public TemplateCanonizer(XTemplate xTemplate)
        {
            XTemplate = xTemplate;
        }

        public void Canonize()
        {
            context = new Context();
            foreach (XElement element in XTemplate.XProject.Descendants())
            {
                context.Add(element);
            }
            foreach (XView xView in XTemplate.XProject.XViews)
            {
                xView.ViewId = context.ViewIdMap[xView.ViewId];
                foreach (XPage xPage in xView.XPages)
                {
                    xPage.ViewId = xView.ViewId;
                    xPage.PageId = context.PageIdMap[xPage.PageId];
                    int tabIndex = 1;
                    foreach (XField xField in xPage.XFields)
                    {
                        xField.PageId = xPage.PageId;
                        xField.FieldId = context.FieldIdMap[xField.FieldId];
                        xField.TabIndex = tabIndex++;
                        OnFieldCanonizing(xField);
                    }
                }
                OnViewCanonized(xView);
            }
            foreach (XTable xTable in XTemplate.XGridTables)
            {
                int gridColumnId = 1;
                foreach (XElement xItem in xTable.XItems)
                {
                    xItem.SetAttributeValue(ColumnNames.GRID_COLUMN_ID, gridColumnId++);
                    xItem.SetAttributeValue(
                        ColumnNames.FIELD_ID,
                        context.FieldIdMap[(int)xItem.Attribute(ColumnNames.FIELD_ID)]);
                }
            }
        }

        private void OnFieldCanonizing(XField xField)
        {
            if (xField.FieldType == MetaFieldType.Mirror)
            {
                int? sourceFieldId = xField.SourceFieldId;
                if (sourceFieldId != null)
                {
                    xField.SourceFieldId = context.FieldIdMap[sourceFieldId.Value];
                }
            }
            if (xField.FieldType == MetaFieldType.Codes)
            {
                string conditions = xField.RelateCondition;
                if (conditions != null)
                {
                    conditions = DDLFieldOfCodesMapper.MapRelateConditions(conditions, context.FieldIdMap);
                    xField.RelateCondition = conditions;
                }
            }
            if (xField.FieldType == MetaFieldType.Relate)
            {
                int? relatedViewId = xField.RelatedViewId;
                if (relatedViewId != null)
                {
                    xField.RelatedViewId = context.ViewIdMap[relatedViewId.Value];
                }
            }
            if (xField.FieldType == MetaFieldType.Group)
            {
                int? backgroundColor = xField.BackgroundColor;
                if (backgroundColor != null)
                {
                    xField.BackgroundColor = -1;
                }
            }
        }

        private void OnViewCanonized(XView xView)
        {
            IDictionary<string, double> tabIndices = xView.XFields.ToDictionary(
                field => field.Name,
                field => field.TabIndex.Value,
                StringComparer.OrdinalIgnoreCase);

            double GetTabIndex(string fieldName)
            {
                return tabIndices.TryGetValue(fieldName, out double tabIndex) ? tabIndex : double.MaxValue;
            }

            foreach (XField xField in xView.XFields)
            {
                if (xField.FieldType == MetaFieldType.Group)
                {
                    string fieldNames = xField.List;
                    if (fieldNames != null)
                    {
                        xField.List = string.Join(
                            Constants.LIST_SEPARATOR.ToString(),
                            fieldNames.Split(Constants.LIST_SEPARATOR).OrderBy(GetTabIndex));
                    }
                }
            }
        }
    }
}
