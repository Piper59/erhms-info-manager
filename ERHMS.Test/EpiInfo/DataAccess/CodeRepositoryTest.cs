using ERHMS.EpiInfo.DataAccess;
using NUnit.Framework;
using System.Collections.Generic;

namespace ERHMS.Test.EpiInfo.DataAccess
{
    public class CodeRepositoryTest : SampleTestBase
    {
        private CodeRepository sortedSexes;
        private CodeRepository unsortedSexes;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            IDataDriver driver = DataDriverFactory.CreateDataDriver(project);
            sortedSexes = new CodeRepository(driver, "codeSex", "Sex", true);
            unsortedSexes = new CodeRepository(driver, "codeSex", "Sex", false);
        }

        [Test]
        public void SelectTest()
        {
            ICollection<string> sexes = new string[]
            {
                "Female",
                "Male",
                "Unknown"
            };
            CollectionAssert.AreEqual(sexes, sortedSexes.Select());
            CollectionAssert.AreEquivalent(sexes, unsortedSexes.Select());
        }
    }
}
