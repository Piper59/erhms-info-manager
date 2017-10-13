using ERHMS.EpiInfo.Templates;
using System.Data;
using System.Text;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public class Canvas
    {
        public const string FileExtension = ".cvs7";

        public static string GetContentForView(string projectPath, string viewName)
        {
            return new ViewCanvas(projectPath, viewName).TransformText();
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
            foreach (XmlNode node in document.SelectNodes("//projectPath"))
            {
                node.InnerText = path;
            }
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
            return CanvasId;
        }

        public override bool Equals(object obj)
        {
            Canvas canvas = obj as Canvas;
            return canvas != null && canvas.CanvasId != 0 && canvas.CanvasId == CanvasId;
        }
    }
}
