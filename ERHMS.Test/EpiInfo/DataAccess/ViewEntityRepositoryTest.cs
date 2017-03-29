using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Test.EpiInfo.DataAccess
{
    public class ViewEntityRepositoryTest : SampleTestBase
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

            public int? Hospitalized
            {
                get { return GetProperty<int?>(nameof(Hospitalized)); }
                set { SetProperty(nameof(Hospitalized), value); }
            }

            public DateTime? Entered
            {
                get { return GetProperty<DateTime?>(nameof(Entered)); }
                set { SetProperty(nameof(Entered), value); }
            }

            public Surveillance()
            {
                AddSynonym("CaseID", nameof(CaseId));
            }
        }

        private class SurveillanceRepository : ViewEntityRepository<Surveillance>
        {
            public SurveillanceRepository(IDataDriver driver, Project project)
                : base(driver, project.Views["Surveillance"]) { }

            public IEnumerable<Surveillance> SelectByEnteredMonth(int year, int month)
            {
                DateTime start = new DateTime(year, month, 1);
                DateTime end = start.AddMonths(1);
                return Select("[Entered] >= {@} AND [Entered] < {@}", start, end);
            }
        }

        private SurveillanceRepository surveillances;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            IDataDriver driver = DataDriverFactory.CreateDataDriver(project);
            surveillances = new SurveillanceRepository(driver, project);
        }

        [Test]
        [Order(1)]
        public void SelectTest()
        {
            Assert.AreEqual(20, surveillances.Select().Count());
            Assert.AreEqual(3, surveillances.SelectByEnteredMonth(2014, 6).Count());
        }

        [Test]
        [Order(2)]
        public void SelectByGlobalRecordIdTest()
        {
            Surveillance surveillance = surveillances.SelectByGlobalRecordId("993974ab-a5c8-4177-b81d-a060b2e5b9e1");
            Assert.AreEqual("100", surveillance.CaseId);
            Assert.AreEqual("John", surveillance.FirstName);
            Assert.AreEqual("Smith", surveillance.LastName);
        }

        [Test]
        [Order(3)]
        public void SaveTest()
        {
            Surveillance surveillance = surveillances.Create();
            surveillance.CaseId = "2100";
            surveillance.FirstName = "John";
            surveillance.LastName = "Doe";
            surveillance.Hospitalized = 0;
            surveillance.Entered = DateTime.Today;
            Assert.IsTrue(surveillance.New);
            Assert.IsNull(surveillance.UniqueKey);
            Assert.IsNull(surveillance.FirstSaveStamp);
            surveillances.Save(surveillance);
            Assert.IsFalse(surveillance.New);
            Assert.IsNotNull(surveillance.UniqueKey);
            Assert.IsNotNull(surveillance.FirstSaveStamp);
            Assert.AreEqual(surveillance.FirstSaveStamp, surveillance.LastSaveStamp);
            Assert.AreEqual(21, surveillances.Select().Count());
            surveillances.Save(surveillance);
            Assert.AreNotEqual(surveillance.FirstSaveStamp, surveillance.LastSaveStamp);
        }

        [Test]
        [Order(4)]
        public void DeleteTest()
        {
            Surveillance surveillance = surveillances.SelectByGlobalRecordId("993974ab-a5c8-4177-b81d-a060b2e5b9e1");
            Assert.IsFalse(surveillance.Deleted);
            surveillances.Delete(surveillance);
            Assert.IsTrue(surveillance.Deleted);
            Assert.AreEqual(20, surveillances.SelectUndeleted().Count());
        }
    }
}
