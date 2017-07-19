using Dapper;
using ERHMS.Dapper;
using ERHMS.EpiInfo.DataAccess;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Test.EpiInfo.DataAccess
{
    public class ViewEntityRepositoryTest : SampleProjectTestBase
    {
        private class DataContext : DataContextBase
        {
            public IRepository<Surveillance> Surveillances { get; private set; }

            public DataContext(Project project)
                : base(project)
            {
                Surveillances = new ViewEntityRepository<Surveillance>(this, project.Views["Surveillance"]);
            }
        }

        private DataContext context;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            context = new DataContext(project);
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
            Assert.AreEqual(new DateTime(2007, 1, 7), surveillance.Entered);
            Assert.IsNull(context.Surveillances.SelectById(Guid.Empty.ToString()));
        }

        [Test]
        public void SaveTest()
        {
            using (Transaction transaction = context.Database.BeginTransaction())
            {
                Surveillance original = new Surveillance
                {
                    CaseId = "2100",
                    LastName = "Doe",
                    FirstName = "John",
                    Hospitalized = 0,
                    Entered = DateTime.Now
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
    }
}
