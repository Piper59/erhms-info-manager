using Epi;
using ERHMS.Common.Reflection;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Settings = ERHMS.EpiInfo.Properties.Settings;

namespace ERHMS.EpiInfo.Analysis
{
    public class Map
    {
        private static XmlWriterSettings XmlWriterSettings => new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = true
        };

        public View View { get; }
        public string MapServer { get; set; } = Settings.Default.MapServer;
        public Color PointColor { get; set; } = Color.FromArgb(0x80, 0xff, 0x00, 0x00);

        public Map(View view)
        {
            View = view;
        }

        public void Save(string path)
        {
            XDocument document;
            using (Stream stream = typeof(Canvas).GetManifestResourceStream("Map.map7"))
            {
                document = XDocument.Load(stream, LoadOptions.PreserveWhitespace);
            }
            XElement dataLayer = document.Root.Element("dataLayer");
            dataLayer.Element("color").Value = $"#{PointColor.ToArgb():X}";
            if (View.Fields["Latitude"]?.FieldType == MetaFieldType.Number)
            {
                dataLayer.Element("latitude").Value = "Latitude";
            }
            if (View.Fields["Longitude"]?.FieldType == MetaFieldType.Number)
            {
                dataLayer.Element("longitude").Value = "Longitude";
            }
            XElement dashboardHelper = dataLayer.Element("dashboardHelper");
            dashboardHelper.Element("projectPath").Value = View.Project.FilePath;
            dashboardHelper.Element("viewName").Value = View.Name;
            XElement serverName = document.Root
                .Element("referenceLayer")
                .Element("referenceLayer")
                .Element("serverName");
            serverName.Value = MapServer;
            using (XmlWriter writer = XmlWriter.Create(path, XmlWriterSettings))
            {
                document.Save(writer);
            }
        }
    }
}
