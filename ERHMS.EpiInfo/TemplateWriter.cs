using Epi;
using Epi.Data.Services;
using ERHMS.EpiInfo.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo
{
    public class TemplateWriter
    {
        private static readonly ISet<MetaFieldType> CodeTableFieldTypes = new HashSet<MetaFieldType>
        {
            MetaFieldType.LegalValues,
            MetaFieldType.CommentLegal,
            MetaFieldType.Codes
        };

        private Stream stream;
        private TemplateLevel level;
        private IMetadataProvider metadata;
        private ISet<string> codeTableNames;
        private ICollection<XElement> xGridFields;

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        private string GetCreateDate()
        {
            return ConfigurationExtensions.CompatibilityMode ? DateTime.Now.ToString("F") : "";
        }

        public TemplateWriter(Stream stream)
        {
            this.stream = stream;
        }

        private XDocument GetDocument()
        {
            codeTableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            xGridFields = new List<XElement>();
            return new XDocument(
                new XElement("Template",
                    new XAttribute("Name", Name),
                    new XAttribute("Description", Description),
                    new XAttribute("CreateDate", GetCreateDate()),
                    new XAttribute("Level", level)
                )
            );
        }

        public void Write(Project project)
        {
            level = TemplateLevel.Project;
            metadata = project.Metadata;
            XDocument xDocument = GetDocument();
            XElement xTemplate = xDocument.Root;
            ProjectMapper mapper = new ProjectMapper();
            XElement xProject = mapper.GetElement(project);
            xTemplate.Add(xProject);
            xProject.Add(
                new XElement("CollectedData",
                    new XElement("Database",
                        new XAttribute("Source", ""),
                        new XAttribute("DataDriver", "")
                    )
                ),
                new XElement("Metadata",
                    new XAttribute("Source", "")
                ),
                new XElement("EnterMakeviewInterpreter",
                    new XAttribute("Source", project.EnterMakeviewIntepreter)
                )
            );
            foreach (View view in project.Views)
            {
                WriteInternal(view, xProject);
            }
            WriteCodeTables(xTemplate);
            WriteGridTables(xTemplate);
            WriteBackgrounds(xTemplate);
            xDocument.Save(stream);
        }

        public void Write(View view)
        {
            level = TemplateLevel.View;
            metadata = view.GetMetadata();
            XDocument xDocument = GetDocument();
            XElement xTemplate = xDocument.Root;
            XElement xProject = new XElement("Project");
            xTemplate.Add(xProject);
            WriteInternal(view, xProject);
            WriteCodeTables(xTemplate);
            WriteGridTables(xTemplate);
            xDocument.Save(stream);
        }

        public void Write(Page page)
        {
            level = TemplateLevel.Page;
            metadata = page.GetMetadata();
            XDocument xDocument = GetDocument();
            XElement xTemplate = xDocument.Root;
            XElement xProject = new XElement("Project");
            xTemplate.Add(xProject);
            XElement xView = new XElement("View",
                new XAttribute("CheckCode", page.view.CheckCode)
            );
            WriteCodeTables(xTemplate);
            WriteGridTables(xTemplate);
            xDocument.Save(stream);
        }

        private XElement WriteInternal(View view, XElement xProject)
        {
            ViewMapper mapper = new ViewMapper();
            XElement xView = mapper.GetElement(view);
            xProject.Add(xView);
            foreach (Page page in view.Pages)
            {
                WriteInternal(page, xView);
            }
            return xView;
        }

        private XElement WriteInternal(Page page, XElement xView)
        {
            PageMapper mapper = new PageMapper();
            XElement xPage = mapper.GetElement(page);
            xView.Add(xPage);
            foreach (XElement xField in mapper.GetFieldElements(page))
            {
                int fieldTypeId = (int)xField.Attribute(ColumnNames.FIELD_TYPE_ID);
                MetaFieldType fieldType = (MetaFieldType)fieldTypeId;
                if (fieldType == MetaFieldType.Relate && level != TemplateLevel.Project)
                {
                    continue;
                }
                xPage.Add(xField);
                TryAddCodeTableName(fieldType, (string)xField.Attribute(ColumnNames.SOURCE_TABLE_NAME));
                if (fieldType == MetaFieldType.Grid)
                {
                    xGridFields.Add(xField);
                    int fieldId = (int)xField.Attribute(ColumnNames.FIELD_ID);
                    DataTable table = page.GetMetadata().GetGridColumns(fieldId);
                    foreach (DataRow row in table.Rows)
                    {
                        TryAddCodeTableName(
                            row.Field<MetaFieldType>(ColumnNames.FIELD_TYPE_ID),
                            row.Field<string>(ColumnNames.SOURCE_TABLE_NAME));
                    }
                }
            }
            return xPage;
        }

        private void TryAddCodeTableName(MetaFieldType fieldType, string tableName)
        {
            if (CodeTableFieldTypes.Contains(fieldType) || string.IsNullOrEmpty(tableName))
            {
                return;
            }
            codeTableNames.Add(tableName);
        }

        private void WriteCodeTables(XElement xTemplate)
        {
            foreach (string tableName in codeTableNames.OrderBy(tableName => tableName))
            {
                DataTable table = metadata.GetCodeTableData(tableName);
                table.TableName = tableName;
                xTemplate.Add(table.ToElement("SourceTable"));
            }
        }

        private void WriteGridTables(XElement xTemplate)
        {
            foreach (XElement xField in xGridFields)
            {
                DataTable table = metadata.GetGridColumns((int)xField.Attribute(ColumnNames.FIELD_ID));
                table.TableName = (string)xField.Attribute(ColumnNames.NAME);
                xTemplate.Add(table.ToElement("GridTable"));
            }
        }

        private void WriteBackgrounds(XElement xTemplate)
        {
            DataTable table = metadata.GetBackgroundData();
            table.TableName = "Backgrounds";
            xTemplate.Add(table.ToElement(table.TableName));
        }
    }
}
