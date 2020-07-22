using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ERHMS.Console.Utilities
{
    public class CanonizeTemplate : Utility
    {
        public string InputPath { get; }
        public string OutputPath { get; }

        public CanonizeTemplate(string inputPath, string outputPath)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
        }

        protected override void RunCore()
        {
            if (File.Exists(OutputPath))
            {
                throw new ArgumentException("Template already exists.");
            }
            XTemplate xTemplate = new XTemplate(XDocument.Load(InputPath).Root);
            TemplateCanonizer canonizer = new TemplateCanonizer(xTemplate);
            canonizer.Canonize();
            using (Stream stream = File.Create(OutputPath))
            using (XmlWriter writer = XmlWriter.Create(stream, XTemplate.XmlWriterSettings))
            {
                xTemplate.Save(writer);
            }
            Log.Default.Debug("Template has been canonized");
        }
    }
}
