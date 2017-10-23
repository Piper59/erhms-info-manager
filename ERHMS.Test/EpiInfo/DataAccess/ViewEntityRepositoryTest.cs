using Dapper;
using Epi;
using ERHMS.Dapper;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.EpiInfo.Web;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Test.EpiInfo.DataAccess
{
    public abstract class ViewEntityRepositoryTest
    {
        private class Surveillance : ViewEntity
        {
            public string CaseId
            {
                get { return GetProperty<string>("CaseID"); }
                set { SetProperty("CaseID", value); }
            }

            public string LastName
            {
                get { return GetProperty<string>(nameof(LastName)); }
                set { SetProperty(nameof(LastName), value); }
            }

            public string FirstName
            {
                get { return GetProperty<string>(nameof(FirstName)); }
                set { SetProperty(nameof(FirstName), value); }
            }

            public byte? Hospitalized
            {
                get { return GetProperty<byte?>(nameof(Hospitalized)); }
                set { SetProperty(nameof(Hospitalized), value); }
            }

            public DateTime? EnteredOn
            {
                get { return GetProperty<DateTime?>("Entered"); }
                set { SetProperty("Entered", value?.RemoveMilliseconds()); }
            }

            public Surveillance(bool @new)
                : base(@new)
            {
                AddSynonym("CaseID", nameof(CaseId));
                AddSynonym("Entered", nameof(EnteredOn));
            }

            public Surveillance()
                : this(false) { }
        }

        private class DataContext : IDataContext
        {
            public IDatabase Database { get; private set; }
            public Project Project { get; private set; }
            public ViewEntityRepository<Surveillance> Surveillances { get; private set; }

            public DataContext(Project project)
            {
                Database = project.GetDatabase();
                Project = project;
                Surveillances = new ViewEntityRepository<Surveillance>(this, project.Views["Surveillance"]);
            }
        }

        private TempDirectory directory;
        private Configuration configuration;
        private ISampleProjectCreator creator;
        private DataContext context;

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
            context = new DataContext(creator.Project);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        [Test]
        public void CountTest()
        {
            Assert.AreEqual(20, context.Surveillances.Count());
            string clauses = "WHERE [ZipCode] = @ZipCode";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@ZipCode", "31061");
            Assert.AreEqual(4, context.Surveillances.Count(clauses, parameters));
        }

        private void SelectTest()
        {
            Assert.AreEqual(20, context.Surveillances.Select().Count());
            Surveillance surveillance = context.Surveillances.Select("ORDER BY [Entered]").First();
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
            context.Database.Invoke((connection, transaction) =>
            {
                connection.Open();
                foreach (string arg in args)
                {
                    connection.Execute(string.Format(format, arg));
                }
            });
        }

        [Test]
        public void SelectMultiQueryTest()
        {
            ICollection<string> columnNames = Enumerable.Range(1, 200)
                .Select(index => "Test" + index)
                .ToList();
            ExecuteForEach("ALTER TABLE [Surveillance] ADD [{0}] INTEGER", columnNames);
            Assert.DoesNotThrow(() =>
            {
                SelectTest();
            });
            ExecuteForEach("ALTER TABLE [Surveillance] DROP COLUMN [{0}]", columnNames);
        }

        [Test]
        public void SelectByIdTest()
        {
            string id = "993974ab-a5c8-4177-b81d-a060b2e5b9e1";
            Surveillance surveillance = context.Surveillances.SelectById(id);
            Assert.IsFalse(surveillance.New);
            Assert.AreEqual(id, surveillance.GlobalRecordId);
            Assert.AreEqual("100", surveillance.CaseId);
            Assert.AreEqual(0, surveillance.Hospitalized);
            Assert.AreEqual(new DateTime(2007, 1, 7), surveillance.EnteredOn);
            Assert.IsNull(context.Surveillances.SelectById(Guid.Empty.ToString()));
        }

        [Test]
        public void SelectUndeletedTest()
        {
            Assert.AreEqual(20, context.Surveillances.SelectUndeleted().Count());
            try
            {
                context.Database.Transact((connection, transaction) =>
                {
                    Surveillance surveillance = context.Surveillances.SelectById("993974ab-a5c8-4177-b81d-a060b2e5b9e1");
                    surveillance.Deleted = true;
                    context.Surveillances.Save(surveillance);
                    Assert.AreEqual(19, context.Surveillances.SelectUndeleted().Count());
                    throw new OperationCanceledException();
                });
            }
            catch (OperationCanceledException) { }
        }

        [Test]
        public void SaveEntityTest()
        {
            try
            {
                context.Database.Transact((connection, transaction) =>
                {
                    Surveillance surveillance = new Surveillance(true)
                    {
                        CaseId = "2100",
                        LastName = "Doe",
                        FirstName = "John",
                        Hospitalized = 0,
                        EnteredOn = DateTime.Now
                    };
                    Assert.IsTrue(surveillance.New);
                    Assert.IsNull(surveillance.UniqueKey);
                    Assert.IsNull(surveillance.CreatedOn);
                    context.Surveillances.Save(surveillance);
                    Assert.IsFalse(surveillance.New);
                    Assert.IsNotNull(surveillance.UniqueKey);
                    Assert.IsNotNull(surveillance.CreatedOn);
                    Assert.AreEqual(surveillance.CreatedOn, surveillance.ModifiedOn);
                    Assert.AreEqual(21, context.Surveillances.Count());
                    surveillance.FirstName = "Jane";
                    context.Surveillances.Save(surveillance);
                    Assert.AreNotEqual(surveillance.CreatedBy, surveillance.ModifiedOn);
                    Assert.AreEqual(surveillance, context.Surveillances.SelectById(surveillance.GlobalRecordId));
                    throw new OperationCanceledException();
                });
            }
            catch (OperationCanceledException) { }
        }

        [Test]
        public void SaveRecordTest()
        {
            try
            {
                context.Database.Transact((connection, transaction) =>
                {
                    Record record = new Record(Guid.NewGuid().ToString());
                    record["CaseID"] = "2100";
                    record["LastName"] = "Doe";
                    record["FirstName"] = "John";
                    record["Hospitalized"] = "0";
                    record["Entered"] = DateTime.Now.ToShortDateString();
                    context.Surveillances.Save(record);
                    Assert.AreEqual(21, context.Surveillances.Count());
                    Surveillance surveillance = context.Surveillances.SelectById(record.GlobalRecordId);
                    Assert.AreEqual(record["CaseID"], surveillance.CaseId);
                    Assert.AreEqual(record["LastName"], surveillance.LastName);
                    Assert.AreEqual(record["FirstName"], surveillance.FirstName);
                    Assert.AreEqual(record["Hospitalized"], surveillance.Hospitalized?.ToString());
                    Assert.AreEqual(record["Entered"], surveillance.EnteredOn?.ToShortDateString());
                    Assert.AreEqual(21, context.Surveillances.Count());
                    throw new OperationCanceledException();
                });
            }
            catch (OperationCanceledException) { }
        }
    }

    public class AccessViewEntityRepositoryTest : ViewEntityRepositoryTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new AccessSampleProjectCreator();
        }
    }

    public class SqlServerViewEntityRepositoryTest : ViewEntityRepositoryTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new SqlServerSampleProjectCreator();
        }
    }
}
