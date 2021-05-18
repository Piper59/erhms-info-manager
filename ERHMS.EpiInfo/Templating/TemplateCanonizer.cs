using Epi;
using ERHMS.Common.Logging;
using ERHMS.EpiInfo.Templating.Mapping;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.EpiInfo.Templating
{
    public class TemplateCanonizer
    {
        private class ContextImpl : IMappingContext
        {
            private int pageCount;
            private int pageFieldCount;
            private int gridColumnCount;
            private readonly IDictionary<int, int> viewIdMap = new Dictionary<int, int>();
            private readonly IDictionary<int, int> fieldIdMap = new Dictionary<int, int>();

            public IEnumerable<IFieldMapper> FieldMappers { get; }

            public ContextImpl()
            {
                FieldMappers = FieldMapper.GetInstances(this).ToList();
            }

            public void OnXViewCanonizing(XView xView, TemplateLevel level)
            {
                int viewId = viewIdMap.Count + 1;
                if (level >= TemplateLevel.View)
                {
                    viewIdMap[xView.ViewId] = viewId;
                }
                xView.ViewId = viewId;
            }

            public void OnXPageCanonizing(XPage xPage)
            {
                xPage.PageId = ++pageCount;
                pageFieldCount = 0;
            }

            public void OnXFieldCanonizing(XField xField)
            {
                int fieldId = fieldIdMap.Count + 1;
                fieldIdMap[xField.FieldId] = fieldId;
                xField.FieldId = fieldId;
                xField.TabIndex = ++pageFieldCount;
            }

            public void OnXGridColumnCanonizing(XItem xItem)
            {
                xItem.SetAttributeValue(ColumnNames.GRID_COLUMN_ID, ++gridColumnCount);
            }

            public void OnError(Exception exception, out bool handled)
            {
                Log.Instance.Warn(exception);
                handled = true;
            }

            public bool MapViewId(int value, out int result)
            {
                return viewIdMap.TryGetValue(value, out result);
            }

            public bool MapFieldId(int value, out int result)
            {
                return fieldIdMap.TryGetValue(value, out result);
            }

            public bool MapTableName(string value, out string result)
            {
                result = default;
                return false;
            }

            public bool MapFieldName(string value, out string result)
            {
                result = default;
                return false;
            }
        }

        public XTemplate XTemplate { get; }
        public IProgress<string> Progress { get; set; }
        private ContextImpl Context { get; set; }

        public TemplateCanonizer(XTemplate xTemplate)
        {
            XTemplate = xTemplate;
        }

        public void Canonize()
        {
            Context = new ContextImpl();
            try
            {
                XTemplate.Canonize();
                CanonizeXProject();
                MapXFieldAttributes();
                CanonizeXGridTables();
            }
            finally
            {
                Context = null;
            }
        }

        private void CanonizeXProject()
        {
            XTemplate.XProject.Canonize(XTemplate.Level);
            foreach (XView xView in XTemplate.XProject.XViews)
            {
                CanonizeXView(xView);
            }
        }

        private void CanonizeXView(XView xView)
        {
            Progress?.Report($"Canonizing view: {xView.Name}");
            Context.OnXViewCanonizing(xView, XTemplate.Level);
            xView.Canonize(XTemplate.Level);
            foreach (XPage xPage in xView.XPages)
            {
                CanonizeXPage(xPage);
            }
        }

        private void CanonizeXPage(XPage xPage)
        {
            Progress?.Report($"Canonizing page: {xPage.Name}");
            Context.OnXPageCanonizing(xPage);
            xPage.Canonize(XTemplate.Level);
            foreach (XField xField in xPage.XFields)
            {
                CanonizeXField(xField);
            }
        }

        private void CanonizeXField(XField xField)
        {
            Progress?.Report($"Canonizing field: {xField.Name}");
            Context.OnXFieldCanonizing(xField);
            xField.Canonize();
            foreach (IFieldMapper mapper in Context.FieldMappers)
            {
                if (mapper.IsCompatible(xField))
                {
                    mapper.Canonize(xField);
                }
            }
        }

        private void MapXFieldAttributes()
        {
            foreach (XField xField in XTemplate.XProject.XFields)
            {
                foreach (IFieldMapper mapper in Context.FieldMappers)
                {
                    if (mapper.IsCompatible(xField))
                    {
                        mapper.MapAttributes(xField);
                    }
                }
            }
        }

        private void CanonizeXGridTables()
        {
            foreach (XTable xTable in XTemplate.XGridTables)
            {
                Progress?.Report($"Canonizing grid table: {xTable.TableName}");
                foreach (XItem xItem in xTable.XItems)
                {
                    Context.OnXGridColumnCanonizing(xItem);
                    int fieldId = (int)xItem.Attribute(ColumnNames.FIELD_ID);
                    if (Context.MapFieldId(fieldId, out fieldId))
                    {
                        xItem.SetAttributeValue(ColumnNames.FIELD_ID, fieldId);
                    }
                }
            }
        }
    }
}
