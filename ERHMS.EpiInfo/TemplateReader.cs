using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Xml;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo
{
    public class TemplateReader
    {
        private const char RelateConditionSeparator = ':';
        private static readonly ISet<TemplateLevel> SupportedLevels = new HashSet<TemplateLevel>
        {
            TemplateLevel.Project,
            TemplateLevel.View,
            TemplateLevel.Page
        };
        private static readonly ISet<string> IgnoredGridColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ColumnNames.UNIQUE_ROW_ID,
            ColumnNames.REC_STATUS,
            ColumnNames.FOREIGN_KEY,
            ColumnNames.GLOBAL_RECORD_ID
        };

        private Stream stream;
        private IProgress<string> progress;
        private IDictionary<string, string> codeTableNameMap;
        private IDictionary<int, int> viewIdMap;
        private IDictionary<int, int> pageIdMap;
        private IDictionary<int, int> fieldIdMap;

        public Project Project { get; set; }

        private View view;
        public View View
        {
            get
            {
                return view;
            }
            set
            {
                Project = view.GetProject();
                view = value;
            }
        }

        public TemplateReader(Stream stream, IProgress<string> progress)
        {
            this.stream = stream;
            this.progress = progress;
        }

        public void Read()
        {
            codeTableNameMap = new Dictionary<string, string>();
            viewIdMap = new Dictionary<int, int>();
            pageIdMap = new Dictionary<int, int>();
            fieldIdMap = new Dictionary<int, int>();
            XDocument xDocument = XDocument.Load(stream);
            XElement xTemplate = xDocument.Root;
            TemplateLevel level = TemplateLevelExtensions.Parse((string)xTemplate.Attribute("Level"));
            if (!SupportedLevels.Contains(level))
            {
                throw new NotSupportedException();
            }
            foreach (XElement xCodeTable in xTemplate.Elements("SourceTable"))
            {
                CreateCodeTable(xCodeTable);
            }
            XElement xProject = xTemplate.Element(ProjectMapper.ElementName);
            ICollection<XElement> xViews = xProject.Elements(ViewMapper.ElementName).ToList();
            switch (level)
            {
                case TemplateLevel.Project:
                    foreach (XElement xView in xViews)
                    {
                        CreateViewAndPages(xView);
                    }
                    foreach (XElement xView in xViews)
                    {
                        CreateFields(xView);
                    }
                    break;
                case TemplateLevel.View:
                    {
                        XElement xView = xProject.Elements(ViewMapper.ElementName).Single();
                        CreateViewAndPages(xView);
                        CreateFields(xView);
                    }
                    break;
                case TemplateLevel.Page:
                    {
                        XElement xView = xProject.Elements(ViewMapper.ElementName).Single();
                        XElement xPage = xView.Elements(PageMapper.ElementName).Single();
                        Page page = CreatePage(xPage, View);
                        CreateFields(xPage, page);
                        AddCheckCode(xView, View);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
            foreach (XElement xView in xViews)
            {
                PostprocessFields(xView);
            }
            foreach (XElement xGridTable in xTemplate.Elements("GridTable"))
            {
                CreateGridTable(xGridTable);
            }
        }

        private DataTable CreateCodeTable(XElement xCodeTable)
        {
            string tableName = (string)xCodeTable.Attribute("TableName");
            progress.Report($"Creating code table: {tableName}");
            DataTable table = xCodeTable.ToTable();
            TableNameGenerator generator = new TableNameGenerator(Project);
            if (generator.Exists(tableName))
            {
                if (table.DataEquals(Project.Metadata.GetCodeTableData(tableName)))
                {
                    return table;
                }
                string original = table.TableName;
                string modified = generator.GetUniqueName(table.TableName);
                codeTableNameMap[original] = modified;
            }
            string[] columnNames = table.Columns.Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();
            Project.Metadata.CreateCodeTable(table.TableName, columnNames);
            Project.Metadata.SaveCodeTableData(table, table.TableName, columnNames);
            return table;
        }

        private View CreateViewAndPages(XElement xView)
        {
            string viewName = (string)xView.Attribute(ColumnNames.NAME);
            progress.Report($"Creating view: {viewName}");
            View view = new View(Project);
            ViewMapper mapper = new ViewMapper();
            mapper.SetProperties(xView, view);
            ViewNameGenerator generator = new ViewNameGenerator(Project);
            if (generator.Exists(view.Name) || generator.Conflicts(view.Name))
            {
                view.Name = generator.GetUniqueName(view.Name);
            }
            Project.Metadata.InsertView(view);
            Project.Views.Add(view);
            viewIdMap[(int)xView.Attribute(ColumnNames.VIEW_ID)] = view.Id;
            foreach (XElement xPage in xView.Elements(PageMapper.ElementName))
            {
                CreatePage(xPage, view);
            }
            return view;
        }

        private Page CreatePage(XElement xPage, View view)
        {
            string pageName = (string)xPage.Attribute(ColumnNames.NAME);
            progress.Report($"Creating page: {view.Name}/{pageName}");
            Page page = new Page(view);
            PageMapper mapper = new PageMapper();
            mapper.SetProperties(xPage, page);
            PageNameGenerator generator = new PageNameGenerator(view);
            if (generator.Exists(page.Name))
            {
                page.Name = generator.GetUniqueName(page.Name);
            }
            page.Position = view.Pages.Count;
            Project.Metadata.InsertPage(page);
            view.Pages.Add(page);
            pageIdMap[(int)xPage.Attribute(ColumnNames.PAGE_ID)] = page.Id;
            return page;
        }

        private void CreateFields(XElement xView)
        {
            int viewId = (int)xView.Attribute(ColumnNames.VIEW_ID);
            View view = Project.GetViewById(viewIdMap[viewId]);
            foreach (XElement xPage in xView.Elements(PageMapper.ElementName))
            {
                int pageId = (int)xPage.Attribute(ColumnNames.PAGE_ID);
                Page page = view.GetPageById(pageIdMap[pageId]);
                CreateFields(xPage, page);
            }
        }

        private void CreateFields(XElement xPage, Page page)
        {
            foreach (XElement xField in xPage.Elements(FieldMapper.ElementName))
            {
                CreateField(xField, page);
            }
        }

        private Field CreateField(XElement xField, Page page)
        {
            View view = page.GetView();
            string fieldName = (string)xField.Attribute(ColumnNames.NAME);
            progress.Report($"Creating field: {view.Name}/{page.Name}/{fieldName}");
            int fieldTypeId = (int)xField.Attribute(ColumnNames.FIELD_TYPE_ID);
            Field field = page.CreateField((MetaFieldType)fieldTypeId);
            FieldMapper mapper = new FieldMapper();
            mapper.SetProperties(xField, field);
            FieldNameGenerator generator = new FieldNameGenerator(view);
            if (generator.Exists(field.Name))
            {
                field.Name = generator.GetUniqueName(field.Name);
            }
            if (field is TableBasedDropDownField dropDown)
            {
                if (codeTableNameMap.TryGetValue(dropDown.SourceTableName, out string tableName))
                {
                    dropDown.SourceTableName = tableName;
                }
            }
            if (field is RelatedViewField relate)
            {
                relate.RelatedViewID = viewIdMap[relate.RelatedViewID];
            }
            field.SaveToDb();
            view.Fields.Add(field);
            view.MustRefreshFieldCollection = false;
            fieldIdMap[(int)xField.Attribute(ColumnNames.FIELD_ID)] = field.Id;
            return field;
        }

        private void AddCheckCode(XElement xView, View view)
        {
            progress.Report("Adding check code");
            string checkCode = ((string)xView.Attribute(ColumnNames.CHECK_CODE)).Trim();
            if (view.CheckCode.Contains(checkCode))
            {
                return;
            }
            view.CheckCode = string.Concat(view.CheckCode.Trim(), "\n", checkCode).Trim();
            Project.Metadata.UpdateView(view);
        }

        private void PostprocessFields(XElement xView)
        {
            progress.Report("Postprocessing fields");
            int viewId = viewIdMap[(int)xView.Attribute(ColumnNames.VIEW_ID)];
            View view = Project.GetViewById(viewId);
            foreach (XElement xField in xView.Descendants(FieldMapper.ElementName))
            {
                int fieldId = fieldIdMap[(int)xField.Attribute(ColumnNames.FIELD_ID)];
                Field field = view.GetFieldById(fieldId);
                MapSourceFieldId(field as MirrorField);
                MapRelateConditions(field as DDLFieldOfCodes);
            }
        }

        private void MapSourceFieldId(MirrorField field)
        {
            if (field == null)
            {
                return;
            }
            field.SourceFieldId = fieldIdMap[field.SourceFieldId];
            field.SaveToDb();
        }

        private void MapRelateConditions(DDLFieldOfCodes field)
        {
            if (field == null || string.IsNullOrEmpty(field.AssociatedFieldInformation))
            {
                return;
            }
            IEnumerable<string> conditions = field.AssociatedFieldInformation.Split(Constants.LIST_SEPARATOR);
            field.AssociatedFieldInformation = string.Join(
                Constants.LIST_SEPARATOR.ToString(),
                conditions.Select(MapRelateCondition));
            field.SaveToDb();
        }

        private string MapRelateCondition(string condition)
        {
            IList<string> chunks = condition.Split(RelateConditionSeparator);
            if (chunks.Count != 2)
            {
                return condition;
            }
            string columnName = chunks[0];
            if (!int.TryParse(chunks[1], out int fieldId))
            {
                return condition;
            }
            return string.Concat(columnName, RelateConditionSeparator, fieldIdMap[fieldId]);
        }

        private DataTable CreateGridTable(XElement xGridTable)
        {
            string tableName = (string)xGridTable.Attribute("TableName");
            progress.Report($"Creating grid table: {tableName}");
            DataTable table = xGridTable.ToTable();
            foreach (DataRow row in table.Rows)
            {
                if (IgnoredGridColumns.Contains(row.Field<string>(ColumnNames.NAME)))
                {
                    continue;
                }
                int fieldId = row.Field<int>(ColumnNames.FIELD_ID);
                row.SetField(ColumnNames.FIELD_ID, fieldIdMap[fieldId]);
                Project.Metadata.AddGridColumn(row);
            }
            return table;
        }
    }
}
