using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

namespace ERHMS.Test.EpiInfo.Web
{
    public class RecordTest
    {
        private TempDirectory directory;
        private Configuration configuration;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(nameof(RecordTest));
            ConfigurationExtensions.Create(directory.FullName).Save();
            configuration = ConfigurationExtensions.Load();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        [Test]
        public void GetAndSetValuesTest()
        {
            Record record = new Record(Guid.Empty.ToString());
            record.SetValues(Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Record.xml"));
            for (int index = 1; index <= 2; index++)
            {
                Assert.AreEqual("", record.GetValue("Empty" + index, typeof(string)));
            }
            Assert.AreEqual("John Doe", record.GetValue("Name", typeof(string)));
            Assert.AreEqual(30, record.GetValue("Age", typeof(int)));
            for (int index = 1; index <= 3; index++)
            {
                Assert.AreEqual(true, record.GetValue("Male" + index, typeof(bool)));
                Assert.AreEqual(false, record.GetValue("Female" + index, typeof(bool)));
            }
            Assert.AreEqual(new DateTime(1776, 7, 4), record.GetValue("IndependenceDay", typeof(DateTime)));
            Assert.IsNull(record.GetValue("Name", typeof(int)));
            Assert.IsNull(record.GetValue("Name", typeof(bool)));
        }

        [Test]
        public void GetUrlTest()
        {
            Record record = new Record(Guid.Empty.ToString());
            configuration.Settings.WebServiceEndpointAddress = "http://example.com/EIWS/SurveyManagerServiceV2.svc";
            configuration.Save();
            Assert.AreEqual("http://example.com/EIWS/Survey/00000000-0000-0000-0000-000000000000", record.GetUrl().ToString());
        }
    }
}
