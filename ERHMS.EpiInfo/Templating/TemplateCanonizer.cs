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
        private class TabIndexComparer : IComparer<string>
        {
            private IDictionary<string, double> tabIndices;

            public TabIndexComparer(IEnumerable<XField> fields)
            {
                tabIndices = fields.ToDictionary(
                    field => field.Name,
                    field => field.TabIndex.Value,
                    StringComparer.OrdinalIgnoreCase);
            }

            private double GetTabIndex(string fieldName)
            {
                return tabIndices.TryGetValue(fieldName, out double tabIndex) ? tabIndex : double.MaxValue;
            }

            public int Compare(string x, string y)
            {
                return GetTabIndex(x).CompareTo(GetTabIndex(y));
            }
        }

        private class Context
        {
            private static void Add(int id, IDictionary<int, int> idMap)
            {
                idMap.Add(id, idMap.Count + 1);
            }

            public IDictionary<int, int> ViewIdMap { get; }
            public IDictionary<int, int> PageIdMap { get; }
            public IDictionary<int, int> FieldIdMap { get; }

            public Context(XProject xProject)
            {
                ViewIdMap = new Dictionary<int, int>();
                PageIdMap = new Dictionary<int, int>();
                FieldIdMap = new Dictionary<int, int>();
                foreach (XView xView in xProject.XViews)
                {
                    Add(xView.ViewId, ViewIdMap);
                    foreach (XPage xPage in xView.XPages)
                    {
                        Add(xPage.PageId, PageIdMap);
                        foreach (XField xField in xPage.XFields)
                        {
                            Add(xField.FieldId, FieldIdMap);
                        }
                    }
                }
            }
        }

        private const int DefaultBackgroundId = 0;
        private const int Opaque = unchecked((int)0xff000000);

        private Context context;

        public XTemplate XTemplate { get; }

        public TemplateCanonizer(XTemplate xTemplate)
        {
            XTemplate = xTemplate;
        }

        public void Canonize()
        {
            context = new Context(XTemplate.XProject);
            CanonizeTemplate();
            CanonizeProject(XTemplate.XProject);
            foreach (XView xView in XTemplate.XProject.XViews)
            {
                CanonizeView(xView);
                foreach (XPage xPage in xView.XPages)
                {
                    CanonizePage(xPage);
                    int nextTabIndex = 1;
                    foreach (XField xField in xPage.XFields)
                    {
                        CanonizeField(xField, nextTabIndex++);
                    }
                    ProcessPageFields(xPage);
                }
            }
            int nextGridColumnId = 1;
            foreach (XTable xTable in XTemplate.XGridTables)
            {
                foreach (XElement xItem in xTable.XItems)
                {
                    CanonizeGridTableItem(xItem, nextGridColumnId++);
                }
            }
        }

        private void CanonizeTemplate()
        {
            XTemplate.CreateDate = null;
        }

        private void CanonizeProject(XProject xProject)
        {
            if (XTemplate.Level == TemplateLevel.Project)
            {
                xProject.Id = null;
                xProject.Location = null;
                xProject.CreateDate = null;
                foreach (string settingName in XProject.ConfigurationSettingNames)
                {
                    xProject.Attribute(settingName)?.Remove();
                }
            }
        }

        private void CanonizeView(XView xView)
        {
            xView.ViewId = context.ViewIdMap[xView.ViewId];
            xView.CheckCode = xView.CheckCode.Trim();
            xView.SurveyId = null;
        }

        private void CanonizePage(XPage xPage)
        {
            xPage.PageId = context.PageIdMap[xPage.PageId];
            xPage.BackgroundId = DefaultBackgroundId;
            xPage.ViewId = xPage.XView.ViewId;
        }

        private void CanonizeField(XField xField, int tabIndex)
        {
            xField.PageId = xField.XPage.PageId;
            xField.FieldId = context.FieldIdMap[xField.FieldId];
            xField.UniqueId = null;
            xField.TabIndex = tabIndex;
            foreach (XAttribute attribute in xField.Attributes().ToList())
            {
                if (attribute.Name.LocalName.StartsWith("Expr"))
                {
                    attribute.Remove();
                }
            }
            switch (xField.FieldType)
            {
                case MetaFieldType.Mirror:
                    if (xField.SourceFieldId != null)
                    {
                        xField.SourceFieldId = context.FieldIdMap[xField.SourceFieldId.Value];
                    }
                    break;
                case MetaFieldType.Codes:
                    if (xField.RelateCondition != null)
                    {
                        xField.RelateCondition = DDLFieldOfCodesMapper.MapAssociatedFieldInformation(xField.RelateCondition, context.FieldIdMap);
                    }
                    break;
                case MetaFieldType.Relate:
                    if (xField.RelatedViewId != null)
                    {
                        xField.RelatedViewId = context.ViewIdMap[xField.RelatedViewId.Value];
                    }
                    break;
                case MetaFieldType.Group:
                    if (xField.BackgroundColor != null)
                    {
                        xField.BackgroundColor = xField.BackgroundColor.Value | Opaque;
                    }
                    break;
            }
        }

        private void ProcessPageFields(XPage xPage)
        {
            TabIndexComparer tabIndexComparer = new TabIndexComparer(xPage.XFields);
            foreach (XField xField in xPage.XFields)
            {
                if (xField.FieldType == MetaFieldType.Group)
                {
                    if (xField.List != null)
                    {
                        string[] childFieldNames = xField.List.Split(Constants.LIST_SEPARATOR);
                        Array.Sort(childFieldNames, tabIndexComparer);
                        xField.List = string.Join(Constants.LIST_SEPARATOR.ToString(), childFieldNames);
                    }
                }
            }
        }

        private void CanonizeGridTableItem(XElement xItem, int gridColumnId)
        {
            xItem.SetAttributeValue(ColumnNames.GRID_COLUMN_ID, gridColumnId);
            xItem.SetAttributeValue(ColumnNames.FIELD_ID, context.FieldIdMap[(int)xItem.Attribute(ColumnNames.FIELD_ID)]);
        }
    }
}
