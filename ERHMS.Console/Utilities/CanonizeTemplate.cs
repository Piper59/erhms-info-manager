using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;
using System;
using System.Xml;

namespace ERHMS.Console.Utilities
{
    public class CanonizeTemplate : IUtility
    {
        public string InputPath { get; }
        public string OutputPath { get; }

        public CanonizeTemplate(string templatePath)
            : this(templatePath, templatePath) { }

        public CanonizeTemplate(string inputPath, string outputPath)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
        }

        public void Run()
        {
            XTemplate xTemplate = XTemplate.Load(InputPath);
            TemplateCanonizer canonizer = new TemplateCanonizer(xTemplate)
            {
                Progress = new Progress<string>(Log.Default.Debug)
            };
            canonizer.Canonize();
            XmlWriterSettings settings = XTemplate.GetXmlWriterSettings();
            settings.NewLineOnAttributes = true;
            xTemplate.Save(OutputPath, settings);
        }
    }
}
