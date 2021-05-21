using Epi;
using ERHMS.Common.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Analysis
{
    public class Canvas
    {
        private static XmlWriterSettings XmlWriterSettings => new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true
        };

        public View View { get; }

        public Canvas(View view)
        {
            View = view;
        }

        public void Save(string path)
        {
            XDocument document;
            using (Stream stream = typeof(Canvas).GetManifestResourceStream("Canvas.cvs7"))
            {
                document = XDocument.Load(stream, LoadOptions.PreserveWhitespace);
            }
            XElement dashboardHelper = document.Root.Element("dashboardHelper");
            dashboardHelper.Element("projectPath").Value = View.Project.FilePath;
            dashboardHelper.Element("viewName").Value = View.Name;
            using (XmlWriter writer = XmlWriter.Create(path, XmlWriterSettings))
            {
                document.Save(writer);
            }
        }
    }
}
