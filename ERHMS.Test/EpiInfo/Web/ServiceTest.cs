using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Test.EpiInfo.Web
{
#if IGNORE_LONG_TESTS
    [TestFixture(Ignore = "IGNORE_LONG_TESTS")]
#endif
    public class ServiceTest : SampleTestBase
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
            if (view.IsWebSurvey())
            {
                Execute("DELETE FROM dbo.SurveyResponse WHERE SurveyId = @SurveyId", new
                {
                    SurveyId = view.WebSurveyId
                });
                Execute("DELETE FROM dbo.SurveyMetaData WHERE SurveyId = @SurveyId", new
                {
                    SurveyId = view.WebSurveyId
                });
            }
        }

        private void Execute(string sql, object parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["EIWS"].ConnectionString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                if (parameters != null)
                {
                    foreach (PropertyInfo property in parameters.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        command.Parameters.AddWithValue("@" + property.Name, property.GetValue(parameters, null));
                    }
                }
                command.ExecuteNonQuery();
            }
        }

        [Test]
        [Order(1)]
        public void IsConfiguredTest()
        {
            IsConfiguredTest(ConfigurationError.EndpointAddress);
            Uri endpointUrl = new Uri(ConfigurationManager.AppSettings["EndpointAddress"]);
            IsConfiguredTest(new Uri(endpointUrl, "SurveyManagerService.html"), ConfigurationError.EndpointAddress);
            IsConfiguredTest(new Uri(endpointUrl, "SurveyManagerService.svc"), ConfigurationError.Version);
            IsConfiguredTest(endpointUrl, ConfigurationError.OrganizationKey);
            IsConfiguredTest(Guid.Empty, ConfigurationError.OrganizationKey);
            Guid organizationKey = new Guid(ConfigurationManager.AppSettings["OrganizationKey"]);
            IsConfiguredTest(organizationKey, ConfigurationError.None);
        }

        private void IsConfiguredTest(Uri endpointUrl, ConfigurationError expected)
        {
            configuration.Settings.WebServiceEndpointAddress = endpointUrl.ToString();
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
            DateTime today = DateTime.Today;
            Survey survey = new Survey
            {
                Title = view.Name,
                StartDate = today,
                EndDate = today.Add(new TimeSpan(1, 0, 0, 0)),
                ResponseType = ResponseType.Single,
                Draft = true
            };
            Assert.IsTrue(Service.Publish(view, survey));
            Assert.IsTrue(Service.GetSurvey(view).Draft);
        }

        [Test]
        [Order(3)]
        public void RepublishTest()
        {
            Survey survey = Service.GetSurvey(view);
            survey.Draft = false;
            Assert.IsTrue(Service.Republish(view, survey));
            Assert.IsFalse(Service.GetSurvey(view).Draft);
        }

        [Test]
        [Order(4)]
        public void TryAddAndGetRecordsTest()
        {
            Survey survey = Service.GetSurvey(view);
            ICollection<Record> originals = new List<Record>();
            for (int index = 0; index < 3; index++)
            {
                Record original = new Record();
                Assert.IsTrue(Service.TryAddRecord(view, survey, original));
                originals.Add(original);
            }
            foreach (Record retrieved in Service.GetRecords(survey))
            {
                Assert.AreEqual(1, originals.Count(original => original.GlobalRecordId.EqualsIgnoreCase(retrieved.GlobalRecordId)));
            }
        }
    }
}
