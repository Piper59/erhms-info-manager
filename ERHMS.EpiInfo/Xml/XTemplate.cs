using Epi;
using Epi.Data.Services;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public class XTemplate : XElement
    {
        public static bool IsLevelSupported(TemplateLevel level)
        {
            switch (level)
            {
                case TemplateLevel.Project:
                case TemplateLevel.View:
                case TemplateLevel.Page:
                    return true;
                default:
                    return false;
            }
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public string Description
        {
            get { return (string)this.GetAttribute(); }
            set { this.SetAttributeValue(value); }
        }

        public DateTime? CreateDate
        {
            get
            {
                if (DateTime.TryParse((string)this.GetAttribute(), out DateTime result))
                {
                    return result;
                }
                return null;
            }
            set
            {
                this.SetAttributeValue(value?.ToString(XmlExtensions.DateFormat));
            }
        }

        public TemplateLevel Level
        {
            get { return TemplateLevelExtensions.Parse((string)this.GetAttribute()); }
            private set { this.SetAttributeValue(value); }
        }

        private IMetadataProvider Metadata { get; }

        private XTemplate(TemplateLevel level, IMetadataProvider metadata)
            : base(ElementNames.Template)
        {
            Name = "";
            Description = "";
            CreateDate = ConfigurationExtensions.CompatibilityMode ? DateTime.Now : (DateTime?)null;
            Level = level;
            Metadata = metadata;
        }

        public XTemplate(Project project)
            : this(TemplateLevel.Project, project.Metadata)
        {
            Log.Default.Debug("Creating project template");
            Name = project.Name;
            Description = project.Description;
            Add(new XProject(project));
            AddCodeTables();
            AddGridTables();
            AddBackgroundsTable();
        }

        public XTemplate(View view)
            : this(TemplateLevel.View, view.GetMetadata())
        {
            Log.Default.Debug("Creating view template");
            Name = view.Name;
            Add(new XProject(view));
            RemoveRelateFields();
            AddCodeTables();
            AddGridTables();
        }

        public XTemplate(Page page)
            : this(TemplateLevel.Page, page.GetMetadata())
        {
            Log.Default.Debug("Creating page template");
            Name = page.Name;
            Add(new XProject(page));
            RemoveRelateFields();
            AddCodeTables();
            AddGridTables();
        }

        public XTemplate(XElement element)
            : base(ElementNames.Template)
        {
            Add(element.Attributes());
            if (!IsLevelSupported(Level))
            {
                throw new NotSupportedException();
            }
            XElement xProject = element.Elements(ElementNames.Project).Single();
            Add(new XProject(xProject));
            foreach (string elementName in ElementNames.Tables)
            {
                foreach (XElement xTable in element.Elements(elementName))
                {
                    Add(new XTable(xTable));
                }
            }
        }

        private void RemoveRelateFields()
        {
            Log.Default.Debug("Removing relate fields");
            IEnumerable<XField> xFields = Descendants().OfType<XField>()
                .Where(xField => xField.FieldType == MetaFieldType.Relate)
                .ToList();
            foreach (XField xField in xFields)
            {
                xField.Remove();
            }
        }

        private void AddCodeTables()
        {
            Log.Default.Debug("Adding code tables");
            ISet<string> tableNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (XField xField in Descendants().OfType<XField>())
            {
                if (xField.FieldType.HasCodeTable())
                {
                    AddCodeTable(xField.SourceTableName, tableNames);
                }
                else if (xField.FieldType == MetaFieldType.Grid)
                {
                    DataTable gridColumns = Metadata.GetGridColumns(xField.FieldId);
                    foreach (DataRow gridColumn in gridColumns.Rows)
                    {
                        if (!gridColumn.Field<MetaFieldType>(ColumnNames.FIELD_TYPE_ID).HasCodeTable())
                        {
                            continue;
                        }
                        string tableName = gridColumn.Field<string>(ColumnNames.SOURCE_TABLE_NAME);
                        AddCodeTable(tableName, tableNames);
                    }
                }
            }
        }

        private void AddCodeTable(string tableName, ISet<string> tableNames)
        {
            if (string.IsNullOrEmpty(tableName) || tableNames.Contains(tableName))
            {
                return;
            }
            DataTable table = Metadata.GetCodeTableData(tableName);
            table.TableName = tableName;
            Add(new XTable(ElementNames.CodeTable, table));
            tableNames.Add(tableName);
        }

        private void AddGridTables()
        {
            Log.Default.Debug("Adding grid tables");
            foreach (XField xField in Descendants().OfType<XField>())
            {
                if (xField.FieldType != MetaFieldType.Grid)
                {
                    continue;
                }
                DataTable table = Metadata.GetGridColumns(xField.FieldId);
                table.TableName = xField.Name;
                Add(new XTable(ElementNames.GridTable, table));
            }
        }

        private void AddBackgroundsTable()
        {
            Log.Default.Debug("Adding backgrounds table");
            DataTable table = new DataTable(ElementNames.BackgroundsTable);
            Add(new XTable(ElementNames.BackgroundsTable, table));
        }

        public new void Save(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                Save(writer);
            }
        }
    }
}
