using Dapper;
using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Test.EpiInfo.Web
{
#if IGNORE_LONG_TESTS
    [TestFixture(Ignore = "IGNORE_LONG_TESTS")]
#endif
    public class ServiceTest : SampleProjectTest
    {
        private View view;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            view = project.Views["ADDFull"];
        }

        [OneTimeTearDown]
        public new void OneTimeTearDown()
        {
            Settings.Default.Reset();
            if (view.IsWebSurvey())
            {
                using (IDbConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["EIWS"].ConnectionString))
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@SurveyId", view.WebSurveyId);
                    connection.Execute("DELETE FROM SurveyResponse WHERE SurveyId = @SurveyId", parameters);
                    connection.Execute("DELETE FROM SurveyMetaData WHERE SurveyId = @SurveyId", parameters);
                }
            }
        }

        [Test]
        [Order(1)]
        public void IsConfiguredTest()
        {
            IsConfiguredTest(ConfigurationError.Address);
            Uri endpoint = new Uri(ConfigurationManager.AppSettings["Endpoint"]);
            IsConfiguredTest(new Uri(endpoint, "SurveyManagerService.html"), ConfigurationError.Address);
            IsConfiguredTest(new Uri(endpoint, "SurveyManagerService.svc"), ConfigurationError.Version);
            IsConfiguredTest(endpoint, ConfigurationError.OrganizationKey);
            IsConfiguredTest(Guid.Empty, ConfigurationError.OrganizationKey);
            Guid organizationKey = new Guid(ConfigurationManager.AppSettings["OrganizationKey"]);
            IsConfiguredTest(organizationKey, ConfigurationError.None);
        }

        private void IsConfiguredTest(Uri endpoint, ConfigurationError expected)
        {
            configuration.Settings.WebServiceEndpointAddress = endpoint.ToString();
            configuration.Save();
            IsConfiguredTest(expected);
        }

        private void IsConfiguredTest(Guid organizationKey, ConfigurationError expected)
        {
            Settings.Default.OrganizationKey = organizationKey.ToString();
            IsConfiguredTest(expected);
        }

        private void IsConfiguredTest(ConfigurationError expected)
        {
            ConfigurationError actual;
            Assert.AreEqual(expected == ConfigurationError.None, Service.IsConfigured(out actual));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Order(2)]
        public void PublishTest()
        {
            DateTime now = DateTime.Now;
            Survey survey = new Survey
            {
                Title = view.Name,
                StartDate = now,
                EndDate = now.Add(new TimeSpan(1, 0, 0, 0)),
                ResponseType = ResponseType.Single,
                Draft = true,
                PublishKey = Guid.NewGuid()
            };
            Assert.IsNull(survey.SurveyId);
            Assert.AreEqual("", project.GetViewById(view.Id).WebSurveyId);
            Assert.IsTrue(Service.Publish(view, survey));
            Assert.IsNotNull(survey.SurveyId);
            Assert.AreEqual(survey.SurveyId, view.WebSurveyId);
            Assert.AreEqual(survey.SurveyId, project.GetViewById(view.Id).WebSurveyId);
            Assert.IsTrue(Service.GetSurvey(survey.SurveyId).Draft);
        }

        [Test]
        [Order(3)]
        public void RepublishTest()
        {
            Survey survey = Service.GetSurvey(view.WebSurveyId);
            survey.Draft = false;
            Assert.IsTrue(Service.Republish(view, survey));
            Assert.IsFalse(Service.GetSurvey(view.WebSurveyId).Draft);
        }

        [Test]
        [Order(4)]
        public void TryAddAndGetRecordsTest()
        {
            Survey survey = Service.GetSurvey(view.WebSurveyId);
            ICollection<string> ids = new List<string>();
            for (int index = 0; index < 3; index++)
            {
                Record record = new Record();
                Assert.IsTrue(Service.TryAddRecord(survey, record));
                ids.Add(record.GlobalRecordId);
            }
            CollectionAssert.AreEquivalent(ids, Service.GetRecords(survey).Select(record => record.GlobalRecordId));
        }
    }
}
