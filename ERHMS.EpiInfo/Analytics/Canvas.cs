using Epi;
using ERHMS.Common.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Analytics
{
    public class Canvas : Asset
    {
        private static XDocument GetTemplate()
        {
            using (Stream stream = typeof(Canvas).GetManifestResourceStream("Template.cvs7"))
            {
                return XDocument.Load(stream, LoadOptions.PreserveWhitespace);
            }
        }

        public Canvas(View view)
            : base(view) { }

        public override void Save(Stream stream)
        {
            XDocument document = GetTemplate();
            XElement dashboardHelper = document.Root.Element("dashboardHelper");
            dashboardHelper.Element("projectPath").Value = View.Project.FilePath;
            dashboardHelper.Element("viewName").Value = View.Name;
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true
            };
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                document.Save(writer);
            }
        }
    }
}
