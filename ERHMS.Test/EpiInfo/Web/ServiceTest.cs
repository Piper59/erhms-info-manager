using Dapper;
using Epi;
using Epi.Core.ServiceClient;
using Epi.SurveyManagerServiceV2;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Web;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Test.EpiInfo.Web
{
    public abstract class ServiceTest
    {
        private TempDirectory directory;
        private Epi.Configuration configuration;
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
                using (IDbConnection connection = Infrastructure.Configuration.GetWebConnection())
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
            IsConfiguredTest(ConfigurationError.Address, new Uri(Infrastructure.Configuration.Endpoint, "Default.html"));
            IsConfiguredTest(ConfigurationError.Address, new Uri(Infrastructure.Configuration.Endpoint, "SurveyManagerService.html"));
            IsConfiguredTest(ConfigurationError.OrganizationKey, new Uri(Infrastructure.Configuration.Endpoint, "SurveyManagerService.svc"));
            IsConfiguredTest(ConfigurationError.OrganizationKey, new Uri(Infrastructure.Configuration.Endpoint, "SurveyManagerServiceV2.svc"));
            IsConfiguredTest(ConfigurationError.OrganizationKey, Infrastructure.Configuration.Endpoint);
            IsConfiguredTest(ConfigurationError.OrganizationKey, Guid.Empty);
            IsConfiguredTest(ConfigurationError.None, Infrastructure.Configuration.OrganizationKey);
        }

        private void IsConfiguredTest(ConfigurationError expected, Uri endpoint)
        {
            configuration.Settings.WebServiceEndpointAddress = endpoint.ToString();
            configuration.Save();
            IsConfiguredTest(expected);
        }

        private void IsConfiguredTest(ConfigurationError expected, Guid organizationKey)
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
                Title = View.Name,
                StartDate = now,
                EndDate = now.Add(new TimeSpan(1, 0, 0, 0)),
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
        public void GetRecordsTest()
        {
            Survey survey = Service.GetSurvey(View.WebSurveyId);
            IDictionary<string, Record> records = AddRecords(survey).ToDictionary(record => record.GlobalRecordId, StringComparer.OrdinalIgnoreCase);
            foreach (Record record in Service.GetRecords(survey))
            {
                Assert.IsTrue(records.ContainsKey(record.GlobalRecordId));
                Assert.AreEqual(records[record.GlobalRecordId]["GENDER"], record["GENDER"]);
            }
        }

        private IEnumerable<Record> AddRecords(Survey survey)
        {
            Random random = new Random();
            Guid organizationKey = new Guid(Settings.Default.OrganizationKey);
            Guid surveyId = new Guid(survey.SurveyId);
            using (ManagerServiceV2Client client = ServiceClient.GetClientV2())
            {
                for (int index = 0; index < 10; index++)
                {
                    Record record = new Record(Guid.NewGuid().ToString());
                    record["GENDER"] = random.NextDouble() < 0.5 ? "1" : "2";
                    PreFilledAnswerRequest request = new PreFilledAnswerRequest
                    {
                        AnswerInfo = new PreFilledAnswerDTO
                        {
                            OrganizationKey = organizationKey,
                            SurveyId = surveyId,
                            UserPublishKey = survey.PublishKey,
                            SurveyQuestionAnswerList = record
                        }
                    };
                    PreFilledAnswerResponse response = client.SetSurveyAnswer(request);
                    Assert.AreEqual("Success", response.Status);
                    record.GlobalRecordId = response.SurveyResponseID;
                    yield return record;
                }
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
