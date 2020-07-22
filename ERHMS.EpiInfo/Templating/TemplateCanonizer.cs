using Epi;
using ERHMS.EpiInfo.Infrastructure;
using ERHMS.EpiInfo.Templating.Xml;
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
            if (xField.FieldType == MetaFieldType.Mirror && xField.SourceFieldId != null)
            {
                xField.SourceFieldId = context.FieldIdMap[xField.SourceFieldId.Value];
            }
            if (xField.FieldType == MetaFieldType.Codes && xField.RelateCondition != null)
            {
                xField.RelateCondition = FieldExtensions.MapRelateConditions(
                    xField.RelateCondition,
                    context.FieldIdMap);
            }
            if (xField.FieldType == MetaFieldType.Relate && xField.RelatedViewId != null)
            {
                xField.RelatedViewId = context.ViewIdMap[xField.RelatedViewId.Value];
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
                if (xField.FieldType == MetaFieldType.Group && xField.List != null)
                {
                    IEnumerable<string> fieldNames = xField.List.Split(Constants.LIST_SEPARATOR)
                        .OrderBy(fieldName => GetTabIndex(fieldName));
                    xField.List = string.Join(Constants.LIST_SEPARATOR.ToString(), fieldNames);
                }
            }
        }
    }
}
