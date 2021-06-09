using Epi.ImportExport;
using System;
using System.Xml.Linq;
using static System.Console;

namespace ERHMS.Console.Utilities
{
    public class UnpackageData : IUtility
    {
        private static readonly string sentinel = "[[EPIINFO7_DATAPACKAGE]]";

        public string DataPath { get; }
        public string Password { get; }

        public UnpackageData(string dataPath, string password)
        {
            DataPath = dataPath;
            Password = password;
        }

        public UnpackageData(string dataPath)
            : this(dataPath, "") { }

        public void Run()
        {
            string content = Epi.Configuration.DecryptFileToString(DataPath, Password);
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
