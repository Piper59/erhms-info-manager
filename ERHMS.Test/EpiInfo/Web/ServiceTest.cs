using Dapper;
using Epi;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Test.EpiInfo.Web
{
    public abstract class ServiceTest
    {
        private TempDirectory directory;
        private Configuration configuration;
        private ISampleProjectCreator creator;

        private Project Project
        {
            get { return creator.Project; }
        }

        private View View
        {
            get { return Project.Views["ADDFull"]; }
        }

        protected abstract ISampleProjectCreator GetCreator();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(GetType().Name);
            ConfigurationExtensions.Create(directory.FullName).Save();
            configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            creator = GetCreator();
            creator.SetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Settings.Default.Reset();
            if (View.IsWebSurvey())
            {
                using (IDbConnection connection = Config.GetWebConnection())
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@SurveyId", View.WebSurveyId);
                    connection.Execute("DELETE FROM [SurveyResponse] WHERE [SurveyId] = @SurveyId", parameters);
                    connection.Execute("DELETE FROM [SurveyMetaData] WHERE [SurveyId] = @SurveyId", parameters);
                }
            }
            creator.TearDown();
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        [Test]
        [Order(1)]
        public void IsConfiguredTest()
        {
            IsConfiguredTest(ConfigurationError.Address);
            IsConfiguredTest(ConfigurationError.Address, "Default.html");
            IsConfiguredTest(ConfigurationError.Address, "SurveyManagerService.html");
            IsConfiguredTest(ConfigurationError.OrganizationKey, "SurveyManagerService.svc");
            IsConfiguredTest(ConfigurationError.OrganizationKey, "SurveyManagerService.svc", 1);
            IsConfiguredTest(ConfigurationError.Version, "SurveyManagerService.svc", 2);
            IsConfiguredTest(ConfigurationError.OrganizationKey, "SurveyManagerServiceV2.svc");
            IsConfiguredTest(ConfigurationError.OrganizationKey, "SurveyManagerServiceV2.svc", 2);
            IsConfiguredTest(ConfigurationError.Version, "SurveyManagerServiceV2.svc", 3);
            IsConfiguredTest(ConfigurationError.OrganizationKey, null);
            IsConfiguredTest(ConfigurationError.OrganizationKey, Guid.Empty);
            IsConfiguredTest(ConfigurationError.None, Config.OrganizationKey);
        }

        private void IsConfiguredTest(ConfigurationError expected, string relativeUrl, int? version = null)
        {
            Uri endpoint = Config.Endpoint;
            if (relativeUrl != null)
            {
                endpoint = new Uri(endpoint, relativeUrl);
            }
            configuration.Settings.WebServiceEndpointAddress = endpoint.ToString();
            configuration.Save();
            IsConfiguredTest(expected, version);
        }

        private void IsConfiguredTest(ConfigurationError expected, Guid organizationKey)
        {
            Settings.Default.OrganizationKey = organizationKey.ToString();
            IsConfiguredTest(expected);
        }

        private void IsConfiguredTest(ConfigurationError expected, int? version = null)
        {
            ConfigurationError actual;
            Assert.AreEqual(expected == ConfigurationError.None, Service.IsConfigured(out actual, version));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [Order(2)]
        public void PublishTest()
        {
            DateTime now = DateTime.Now;
            Survey survey = new Survey
            {
                Title = View.Name,
                StartDate = now,
                EndDate = now.Add(TimeSpan.FromDays(1.0)),
                ResponseType = ResponseType.Single,
                Draft = true,
                PublishKey = Guid.NewGuid()
            };
            Assert.IsNull(survey.SurveyId);
            Assert.AreEqual("", Project.GetViewById(View.Id).WebSurveyId);
            Assert.IsTrue(Service.Publish(View, survey));
            Assert.IsNotNull(survey.SurveyId);
            Assert.AreEqual(survey.SurveyId, View.WebSurveyId);
            Assert.AreEqual(survey.SurveyId, Project.GetViewById(View.Id).WebSurveyId);
            Assert.IsTrue(Service.GetSurvey(survey.SurveyId).Draft);
        }

        [Test]
        [Order(3)]
        public void RepublishTest()
        {
            Survey survey = Service.GetSurvey(View.WebSurveyId);
            survey.Draft = false;
            Assert.IsTrue(Service.Republish(View, survey));
            Assert.IsFalse(Service.GetSurvey(View.WebSurveyId).Draft);
        }

        [Test]
        [Order(4)]
        public void TryAddAndGetRecordsTest()
        {
            Random random = new Random();
            Survey survey = Service.GetSurvey(View.WebSurveyId);
            IDictionary<string, Record> records = new Dictionary<string, Record>(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < 10; index++)
            {
                Record record = new Record();
                record["GENDER"] = random.NextDouble() < 0.5 ? "1" : "2";
                Assert.IsTrue(Service.TryAddRecord(survey, record));
                records.Add(record.GlobalRecordId, record);
            }
            foreach (Record record in Service.GetRecords(survey))
            {
                Assert.IsTrue(records.ContainsKey(record.GlobalRecordId));
                Assert.AreEqual(records[record.GlobalRecordId]["GENDER"], record["GENDER"]);
            }
        }
    }

    public class AccessServiceTest : ServiceTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new AccessSampleProjectCreator();
        }
    }

    public class SqlServerServiceTest : ServiceTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new SqlServerSampleProjectCreator();
        }
    }
}
