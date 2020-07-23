using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ERHMS.Console.Utilities
{
    public class CanonizeTemplate : Utility
    {
        public string TemplatePath { get; }

        public CanonizeTemplate(string templatePath)
        {
            TemplatePath = templatePath;
        }

        protected override void RunCore()
        {
            XTemplate xTemplate = new XTemplate(XDocument.Load(TemplatePath).Root);
            TemplateCanonizer canonizer = new TemplateCanonizer(xTemplate);
            canonizer.Canonize();
            using (Stream stream = File.Create(TemplatePath))
            using (XmlWriter writer = XmlWriter.Create(stream, XTemplate.XmlWriterSettings))
            {
                xTemplate.Save(writer);
            }
            Log.Default.Debug("Template has been canonized");
        }
    }
}
