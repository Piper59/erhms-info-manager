using ERHMS.Common.Logging;
using ERHMS.EpiInfo.Templating;
using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.Console.Utilities
{
    public class CanonizeTemplate : IUtility
    {
        public string InputPath { get; }
        public string OutputPath { get; }

        public CanonizeTemplate(string inputPath, string outputPath)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
        }

        public CanonizeTemplate(string templatePath)
            : this(templatePath, templatePath) { }

        public void Run()
        {
            XTemplate xTemplate = XTemplate.Load(InputPath);
            TemplateCanonizer canonizer = new TemplateCanonizer(xTemplate)
            {
                Progress = Log.Progress
            };
            canonizer.Canonize();
            xTemplate.Save(OutputPath);
        }
    }
}
