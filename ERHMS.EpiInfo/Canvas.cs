using Epi;
using ERHMS.EpiInfo.Templates;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public class Canvas
    {
        public const string FileExtension = ".cvs7";
        private static readonly ICollection<Func<Canvas, object>> Identifiers = new Func<Canvas, object>[]
        {
            @this => @this.CanvasId,
            @this => @this.Name,
            @this => @this.Content
        };

        public static string GetContentForView(View view)
        {
            return new ViewCanvas(view).TransformText();
        }

        public static string GetContentForTable(string connectionString, string tableName)
        {
            return new TableCanvas(connectionString, tableName).TransformText();
        }

        public int CanvasId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public Canvas() { }

        internal Canvas(DataRow row)
        {
            CanvasId = row.Field<int>("CanvasId");
            Name = row.Field<string>("Name");
            Content = row.Field<string>("Content");
        }

        public void SetProjectPath(string path)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(Content);
            XmlNode node = document.SelectSingleNode("/DashboardCanvas/dashboardHelper/projectPath");
            node.InnerText = path;
            StringBuilder content = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment
            };
            using (XmlWriter writer = XmlWriter.Create(content, settings))
            {
                document.WriteContentTo(writer);
            }
            Content = content.ToString();
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GetHashCode(this, Identifiers);
        }

        public override bool Equals(object obj)
        {
            return ObjectExtensions.Equals(this, obj, Identifiers);
        }
    }
}
