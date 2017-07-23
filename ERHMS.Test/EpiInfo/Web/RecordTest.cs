using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using NUnit.Framework;
using System;

namespace ERHMS.Test.EpiInfo.Web
{
    public class RecordTest : SampleProjectTestBase
    {
        [Test]
        public void GetValueTest()
        {
            Record record = new Record(new
            {
                Name = "John Doe",
                Age = 30,
                Male1 = true,
                Male2 = 1,
                Male3 = "Yes",
                Female1 = false,
                Female2 = 0,
                Female3 = "No"
            });
            Assert.AreEqual("John Doe", record.GetValue("Name", typeof(string)));
            Assert.AreEqual(30, record.GetValue("Age", typeof(int)));
            for (int index = 1; index <= 3; index++)
            {
                Assert.AreEqual(true, record.GetValue("Male" + index, typeof(bool)));
            }
            for (int index = 1; index <= 3; index++)
            {
                Assert.AreEqual(false, record.GetValue("Female" + index, typeof(bool)));
            }
            Assert.IsNull(record.GetValue("Name", typeof(int)));
            Assert.IsNull(record.GetValue("Name", typeof(bool)));
        }

        [Test]
        public void GetUrlTest()
        {
            Record record = new Record
            {
                GlobalRecordId = Guid.Empty.ToString()
            };
            configuration.Settings.WebServiceEndpointAddress = "http://example.com/EIWS/SurveyManagerServiceV2.svc";
            configuration.Save();
            Assert.AreEqual("http://example.com/EIWS/Survey/00000000-0000-0000-0000-000000000000", record.GetUrl().ToString());
        }
    }
}
