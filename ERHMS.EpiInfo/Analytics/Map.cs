using Epi;
using ERHMS.Common.Reflection;
using ERHMS.EpiInfo.Metadata;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Settings = ERHMS.EpiInfo.Properties.Settings;

namespace ERHMS.EpiInfo.Analytics
{
    public class Map : Asset
    {
        private static XmlWriterSettings XmlWriterSettings => new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true
        };

        private static XDocument GetTemplate()
        {
            using (Stream stream = typeof(Map).GetManifestResourceStream("Template.map7"))
            {
                return XDocument.Load(stream, LoadOptions.PreserveWhitespace);
            }
        }

        public string Server { get; set; } = Settings.Default.MapServer;
        public Color PointColor { get; set; } = Color.FromArgb(Settings.Default.MapPointColor);

        public Map(View view)
            : base(view) { }

        public override void Save(Stream stream)
        {
            XDocument document = GetTemplate();
            XElement dataLayer = document.Root.Element("dataLayer");
            dataLayer.Element("color").Value = $"#{PointColor.ToArgb():X}";
            if (View.Fields.Contains("Latitude", MetaFieldType.Number)
                && View.Fields.Contains("Longitude", MetaFieldType.Number))
            {
                dataLayer.Element("latitude").Value = "Latitude";
                dataLayer.Element("longitude").Value = "Longitude";
            }
            XElement dashboardHelper = dataLayer.Element("dashboardHelper");
            dashboardHelper.Element("projectPath").Value = View.Project.FilePath;
            dashboardHelper.Element("viewName").Value = View.Name;
            XElement serverName = document.Root.Element("referenceLayer")
                .Element("referenceLayer")
                .Element("serverName");
            serverName.Value = Server;
            using (XmlWriter writer = XmlWriter.Create(stream, XmlWriterSettings))
            {
                document.Save(writer);
            }
        }
    }
}
