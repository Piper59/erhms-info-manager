using Epi;
using Epi.ImportExport;
using System;
using System.Xml.Linq;

namespace ERHMS.Console.Utilities
{
    public class UnpackageData : Utility
    {
        private static readonly string sentinel = "[[EPIINFO7_DATAPACKAGE]]";

        public string PackagePath { get; }

        public UnpackageData(string packagePath)
        {
            PackagePath = packagePath;
        }

        public override void Run()
        {
            string content = Configuration.DecryptFileToString(PackagePath, GetPassword());
            if (!content.StartsWith(sentinel))
            {
                throw new InvalidOperationException("File does not appear to be an Epi Info data package.");
            }
            string xml = ImportExportHelper.UnZip(content.Substring(sentinel.Length));
            XDocument document = XDocument.Parse(xml);
            Out.WriteLine(document);
        }
    }
}
