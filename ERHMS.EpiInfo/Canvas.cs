using Epi;
using ERHMS.EpiInfo.Templates;
using ERHMS.Utility;
using System.Text;
using System.Xml;

namespace ERHMS.EpiInfo
{
    public class Canvas
    {
        public const string FileExtension = ".cvs7";

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

        public void SetProjectPath(string projectPath)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(Content);
            XmlNode projectPathNode = document.SelectSingleNode("/DashboardCanvas/dashboardHelper/projectPath");
            projectPathNode.InnerText = projectPath;
            StringBuilder builder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                ConformanceLevel = ConformanceLevel.Fragment
            };
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                document.WriteContentTo(writer);
            }
            Content = builder.ToString();
        }

        public override bool Equals(object obj)
        {
            Canvas canvas = obj as Canvas;
            return canvas != null && canvas.CanvasId == CanvasId && canvas.Name == Name && canvas.Content == Content;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GetHashCode(CanvasId, Name, Content);
        }
    }
}
