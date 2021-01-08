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
            private static void AddToIdMap(int id, IDictionary<int, int> idMap)
            {
                idMap.Add(id, idMap.Count + 1);
            }

            public IDictionary<int, int> ViewIdMap { get; } = new Dictionary<int, int>();
            public IDictionary<int, int> PageIdMap { get; } = new Dictionary<int, int>();
            public IDictionary<int, int> FieldIdMap { get; } = new Dictionary<int, int>();

            public Context(XProject xProject)
            {
                foreach (XView xView in xProject.XViews)
                {
                    AddToIdMap(xView.ViewId, ViewIdMap);
                    foreach (XPage xPage in xView.XPages)
                    {
                        AddToIdMap(xPage.PageId, PageIdMap);
                        foreach (XField xField in xPage.XFields)
                        {
                            AddToIdMap(xField.FieldId, FieldIdMap);
                        }
                    }
                }
            }
        }

        private const int DefaultBackgroundId = 0;
        private const int Opaque = unchecked((int)0xff000000);

        private Context context;

        public XTemplate XTemplate { get; }
        public IProgress<string> Progress { get; set; }

        public TemplateCanonizer(XTemplate xTemplate)
        {
            XTemplate = xTemplate;
        }

        public void Canonize()
        {
            Progress?.Report($"Canonizing Epi Info template: {XTemplate.Name}");
            context = new Context(XTemplate.XProject);
            CanonizeTemplate();
            if (XTemplate.Level == TemplateLevel.Project)
            {
                CanonizeProject(XTemplate.XProject);
            }
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
                }
            }
            CanonizeGridTables();
        }

        private void CanonizeTemplate()
        {
            XTemplate.CreateDate = null;
        }

        private void CanonizeProject(XProject xProject)
        {
            Progress?.Report($"Canonizing project: {xProject.Name}");
            xProject.Id = null;
            xProject.Location = null;
            xProject.CreateDate = null;
        }

        private void CanonizeView(XView xView)
        {
            Progress?.Report($"Canonizing view: {xView.Name}");
            xView.ViewId = context.ViewIdMap[xView.ViewId];
            xView.CheckCode = xView.CheckCode.Trim();
            xView.SurveyId = null;
        }

        private void CanonizePage(XPage xPage)
        {
            XView xView = xPage.XView;
            Progress?.Report($"Canonizing page: {xView.Name}/{xPage.Name}");
            xPage.PageId = context.PageIdMap[xPage.PageId];
            xPage.BackgroundId = DefaultBackgroundId;
            xPage.ViewId = xPage.XView.ViewId;
        }

        private void CanonizeField(XField xField, int tabIndex)
        {
            XPage xPage = xField.XPage;
            XView xView = xPage.XView;
            Progress?.Report($"Canonizing field: {xView.Name}/{xPage.Name}/{xField.Name}");
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
                        xField.RelateCondition = DDLFieldOfCodesMapper.MapAssociatedFieldInformation(
                            xField.RelateCondition,
                            context.FieldIdMap.TryGetValue);
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
                    if (xField.List != null)
                    {
                        IDictionary<string, double> tabIndices = xField.XPage.XFields.ToDictionary(
                            pageXField => pageXField.Name,
                            pageXField => pageXField.TabIndex.Value,
                            StringComparer.OrdinalIgnoreCase);
                        xField.ListItems = xField.ListItems.OrderBy(fieldName => tabIndices[fieldName]);
                    }
                    break;
            }
        }

        private void CanonizeGridTables()
        {
            Progress?.Report("Canonizing grid tables");
            int nextGridColumnId = 1;
            foreach (XTable xTable in XTemplate.XGridTables)
            {
                CanonizeGridTable(xTable, nextGridColumnId++);
            }
        }

        private void CanonizeGridTable(XTable xTable, int gridColumnId)
        {
            Progress?.Report($"Canonizing grid table: {xTable.TableName}");
            foreach (XElement xItem in xTable.XItems)
            {
                xItem.SetAttributeValue(ColumnNames.GRID_COLUMN_ID, gridColumnId);
                int fieldId = (int)xItem.Attribute(ColumnNames.FIELD_ID);
                xItem.SetAttributeValue(ColumnNames.FIELD_ID, context.FieldIdMap[fieldId]);
            }
        }
    }
}
