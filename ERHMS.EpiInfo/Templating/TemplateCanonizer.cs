using Epi;
using ERHMS.EpiInfo.Templating.Mapping;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Templating
{
    public class TemplateCanonizer
    {
        private class ContextImpl : IMappingContext, IDisposable
        {
            private int pageCount;
            private int pageFieldCount;
            private int gridColumnCount;
            private readonly IDictionary<int, int> viewIdMap = new Dictionary<int, int>();
            private readonly IDictionary<int, int> fieldIdMap = new Dictionary<int, int>();

            public TemplateCanonizer Owner { get; }
            public IReadOnlyCollection<IFieldMapper> FieldMappers { get; }

            public ContextImpl(TemplateCanonizer owner)
            {
                Owner = owner;
                FieldMappers = FieldMapper.GetInstances(this).ToList();
            }

            public void OnXViewCanonizing(XView xView)
            {
                int viewId = viewIdMap.Count + 1;
                viewIdMap[xView.ViewId] = viewId;
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

            public void OnXGridTableItemCanonizing(XElement xItem)
            {
                xItem.SetAttributeValue(ColumnNames.GRID_COLUMN_ID, ++gridColumnCount);
            }

            public void OnError(Exception exception, out bool handled)
            {
                Log.Default.Warn(exception);
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

            public void Dispose()
            {
                Owner.Context = null;
            }
        }

        private static readonly IReadOnlyCollection<string> XProjectAttributeNames = new HashSet<string>
        {
            nameof(XProject.Id),
            nameof(XProject.Name),
            nameof(XProject.Location),
            nameof(XProject.Description),
            nameof(XProject.EpiVersion),
            nameof(XProject.CreateDate)
        };
        private static readonly IReadOnlyDictionary<string, string> XFieldAttributeNameMap =
            new Dictionary<string, string>
            {
                { "Expr1015", "ControlFontFamily" },
                { "Expr1016", "ControlFontSize" },
                { "Expr1017", "ControlFontStyle" }
            };
        private static readonly Regex DuplicateXFieldAttributeNameRegex =
            new Regex(@"^ControlFont(?:Family|Size|Style)1$");

        private static IEnumerable<XAttribute> GetNormalizedAttributes(XProject xProject)
        {
            foreach (XAttribute attribute in xProject.Attributes())
            {
                if (XProjectAttributeNames.Contains(attribute.Name.LocalName))
                {
                    yield return attribute;
                }
            }
        }

        private static IEnumerable<XAttribute> GetNormalizedAttributes(XField xField)
        {
            bool usedNameMap = false;
            foreach (XAttribute attribute in xField.Attributes())
            {
                if (XFieldAttributeNameMap.TryGetValue(attribute.Name.LocalName, out string attributeName))
                {
                    usedNameMap = true;
                    yield return new XAttribute(attributeName, attribute.Value);
                }
                else if (usedNameMap && XFieldAttributeNameMap.Values.Contains(attribute.Name.LocalName))
                {
                    continue;
                }
                else if (DuplicateXFieldAttributeNameRegex.IsMatch(attribute.Name.LocalName))
                {
                    continue;
                }
                else if (attribute.Name.LocalName.EndsWith("Percentage"))
                {
                    if (double.TryParse(attribute.Value, out double value))
                    {
                        attribute.Value = value.ToString("F6");
                    }
                    yield return attribute;
                }
                else
                {
                    yield return attribute;
                }
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
            using (Context = new ContextImpl(this))
            {
                CanonizeXTemplate();
                CanonizeXProject(XTemplate.XProject);
                foreach (XView xView in XTemplate.XProject.XViews)
                {
                    CanonizeXView(xView);
                }
                MapXFieldAttributes();
                CanonizeXGridTables();
            }
        }

        private void CanonizeXTemplate()
        {
            XTemplate.CreateDate = null;
        }

        private void CanonizeXProject(XProject xProject)
        {
            if (XTemplate.Level == TemplateLevel.Project)
            {
                Progress?.Report($"Canonizing project: {xProject.Name}");
                xProject.Id = null;
                xProject.Location = null;
                xProject.CreateDate = null;
                xProject.ReplaceAttributes(GetNormalizedAttributes(xProject));
            }
            else
            {
                xProject.RemoveAttributes();
            }
        }

        private void CanonizeXView(XView xView)
        {
            string checkCode = xView.CheckCode?.Trim();
            if (XTemplate.Level >= TemplateLevel.View)
            {
                Progress?.Report($"Canonizing view: {xView.Name}");
                Context.OnXViewCanonizing(xView);
                xView.CheckCode = checkCode;
                xView.SurveyId = null;
            }
            else
            {
                xView.RemoveAttributes();
                xView.CheckCode = checkCode;
            }
            foreach (XPage xPage in xView.XPages)
            {
                CanonizeXPage(xPage);
            }
        }

        private void CanonizeXPage(XPage xPage)
        {
            if (XTemplate.Level >= TemplateLevel.Page)
            {
                Progress?.Report($"Canonizing page: {xPage.Name}");
                Context.OnXPageCanonizing(xPage);
                xPage.BackgroundId = 0;
                xPage.ViewId = XTemplate.Level == TemplateLevel.Page ? 1 : xPage.XView.ViewId;
            }
            else
            {
                xPage.RemoveAttributes();
            }
            foreach (XField xField in xPage.XFields)
            {
                CanonizeXField(xField);
            }
        }

        private void CanonizeXField(XField xField)
        {
            Progress?.Report($"Canonizing field: {xField.Name}");
            Context.OnXFieldCanonizing(xField);
            xField.PageId = xField.XPage.PageId;
            xField.UniqueId = null;
            xField.ReplaceAttributes(GetNormalizedAttributes(xField));
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
                CanonizeXGridTable(xTable);
            }
        }

        private void CanonizeXGridTable(XTable xTable)
        {
            Progress?.Report($"Canonizing grid table: {xTable.TableName}");
            foreach (XElement xItem in xTable.XItems)
            {
                Context.OnXGridTableItemCanonizing(xItem);
                int fieldId = (int)xItem.Attribute(ColumnNames.FIELD_ID);
                if (Context.MapFieldId(fieldId, out fieldId))
                {
                    xItem.SetAttributeValue(ColumnNames.FIELD_ID, fieldId);
                }
            }
        }
    }
}
