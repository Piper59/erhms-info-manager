using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Web;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Test.EpiInfo.DataAccess
{
    public class ViewEntityRepositoryTest : SampleProjectTest
    {
        private class DataContext : DataContextBase
        {
            public ViewEntityRepository<Surveillance> Surveillances { get; private set; }

            public DataContext(Project project)
                : base(project)
            {
                Surveillances = new ViewEntityRepository<Surveillance>(this, project.Views["Surveillance"]);
            }
        }

        private DataContext context;
        private View view;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            context = new DataContext(project);
            view = project.Views["Surveillance"];
        }

        [OneTimeTearDown]
        public new void OneTimeTearDown()
        {
#if IGNORE_LONG_TESTS
            return;
#endif
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
        public void CountTest()
        {
            Assert.AreEqual(20, context.Surveillances.Count());
            string clauses = "WHERE ZipCode = @ZipCode";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ZipCode", "31061");
            Assert.AreEqual(4, context.Surveillances.Count(clauses, parameters));
        }

        private void SelectTest()
        {
            Assert.AreEqual(20, context.Surveillances.Select().Count());
            Surveillance surveillance = context.Surveillances.Select("ORDER BY Entered").First();
            Assert.IsFalse(surveillance.New);
            Assert.AreEqual("Smith", surveillance.LastName);
            Assert.AreEqual("John", surveillance.FirstName);
        }

        [Test]
        public void SelectSingleQueryTest()
        {
            SelectTest();
        }

        private void ExecuteForEach(string format, IEnumerable<string> args)
        {
            using (IDbConnection connection = context.Database.GetConnection())
            {
                connection.Open();
                foreach (string arg in args)
                {
                    connection.Execute(string.Format(format, arg));
                }
            }
        }

        [Test]
        public void SelectMultiQueryTest()
        {
            ICollection<string> columnNames = Enumerable.Range(1, 200)
                .Select(index => "Test" + index)
                .ToList();
            ExecuteForEach("ALTER TABLE Surveillance ADD {0} INTEGER", columnNames);
            try
            {
                SelectTest();
            }
            finally
            {
                ExecuteForEach("ALTER TABLE Surveillance DROP COLUMN {0}", columnNames);
            }
        }

        [Test]
        public void SelectByIdTest()
        {
            string globalRecordId = "993974ab-a5c8-4177-b81d-a060b2e5b9e1";
            Surveillance surveillance = context.Surveillances.SelectById(globalRecordId);
            Assert.IsFalse(surveillance.New);
            Assert.AreEqual(globalRecordId, surveillance.GlobalRecordId);
            Assert.AreEqual("100", surveillance.CaseId);
            Assert.AreEqual(0, surveillance.Hospitalized);
            Assert.AreEqual(new DateTime(2007, 1, 7), surveillance.EnteredOn);
            Assert.IsNull(context.Surveillances.SelectById(Guid.Empty.ToString()));
        }

        [Test]
        public void SelectUndeletedTest()
        {
            Assert.AreEqual(20, context.Surveillances.SelectUndeleted().Count());
            using (Transaction transaction = context.Database.BeginTransaction())
            {
                Surveillance surveillance = context.Surveillances.SelectById("993974ab-a5c8-4177-b81d-a060b2e5b9e1");
                surveillance.Deleted = true;
                context.Surveillances.Save(surveillance);
                Assert.AreEqual(19, context.Surveillances.SelectUndeleted().Count());
            }
        }

        [Test]
        public void SaveEntityTest()
        {
            using (Transaction transaction = context.Database.BeginTransaction())
            {
                Surveillance original = new Surveillance
                {
                    CaseId = "2100",
                    LastName = "Doe",
                    FirstName = "John",
                    Hospitalized = 0,
                    EnteredOn = DateTime.Now
                };
                Assert.IsTrue(original.New);
                Assert.IsNull(original.UniqueKey);
                Assert.IsNull(original.CreatedOn);
                context.Surveillances.Save(original);
                Assert.IsFalse(original.New);
                Assert.IsNotNull(original.UniqueKey);
                Assert.IsNotNull(original.CreatedOn);
                Assert.AreEqual(original.CreatedOn, original.ModifiedOn);
                Assert.AreEqual(21, context.Surveillances.Count());
                original.FirstName = "Jane";
                context.Surveillances.Save(original);
                Assert.AreNotEqual(original.CreatedBy, original.ModifiedOn);
                Assert.AreEqual(original, context.Surveillances.SelectById(original.GlobalRecordId));
            }
        }

        private int SaveRecords(Survey survey)
        {
            ViewEntityRepository<ViewEntity> entities = new ViewEntityRepository<ViewEntity>(context, view);
            ICollection<Record> records = Service.GetRecords(survey).ToList();
            foreach (Record record in records)
            {
                entities.Save(record);
            }
            return records.Count;
        }

        [Test]
#if IGNORE_LONG_TESTS
        [Ignore("IGNORE_LONG_TESTS")]
#endif
        public void SaveRecordTest()
        {
            configuration.Settings.WebServiceEndpointAddress = ConfigurationManager.AppSettings["Endpoint"];
            configuration.Save();
            Settings.Default.OrganizationKey = ConfigurationManager.AppSettings["OrganizationKey"];
            DateTime now = DateTime.Now;
            Survey survey = new Survey
            {
                Title = view.Name,
                StartDate = now,
                EndDate = now.Add(new TimeSpan(1, 0, 0, 0)),
                ResponseType = ResponseType.Single,
                Draft = false,
                PublishKey = Guid.NewGuid()
            };
            Assert.IsTrue(Service.Publish(view, survey));
            Record original = new Record(new
            {
                CaseID = "2100",
                LastName = "Doe",
                FirstName = "John",
                Hospitalized = 0,
                Entered = DateTime.Now
            });
            Assert.IsTrue(Service.TryAddRecord(survey, original));
            using (Transaction transaction = context.Database.BeginTransaction())
            {
                Assert.AreEqual(1, SaveRecords(survey));
                Assert.AreEqual(21, context.Surveillances.Count());
                Surveillance retrieved = context.Surveillances.SelectById(original.GlobalRecordId);
                Assert.AreEqual(original["CaseID"], retrieved.CaseId);
                Assert.AreEqual(original["LastName"], retrieved.LastName);
                Assert.AreEqual(original["FirstName"], Convert.ToString(retrieved.FirstName));
                Assert.AreEqual(original["Hospitalized"], Convert.ToString(retrieved.Hospitalized));
                Assert.AreEqual(original["Entered"], Convert.ToString(retrieved.EnteredOn));
                Assert.AreEqual(1, SaveRecords(survey));
                Assert.AreEqual(21, context.Surveillances.Count());
            }
        }
    }
}
