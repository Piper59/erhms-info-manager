using Epi;
using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using NUnit.Framework;
using System.IO;
using System.Linq;
using Canvas = ERHMS.Domain.Canvas;
using Pgm = ERHMS.Domain.Pgm;

namespace ERHMS.Test.DataAccess
{
    public class SampleDataContextTest
    {
        private TempDirectory directory;
        private DataContext context;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(nameof(SampleDataContextTest));
            ConfigurationExtensions.Create(directory.FullName).Save();
            Configuration configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            context = SampleDataContext.Create();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        [Test]
        public void CreateTest()
        {
            Incident incident = context.Incidents.Select().Single();
            foreach (Pgm pgm in context.Pgms.Select())
            {
                StringAssert.Contains(context.Project.FilePath, pgm.Content);
                Assert.AreEqual(incident.IncidentId, pgm.Incident.IncidentId);
            }
            foreach (Canvas canvas in context.Canvases.Select())
            {
                StringAssert.Contains(context.Project.FilePath, canvas.Content);
                Assert.AreEqual(incident.IncidentId, canvas.Incident.IncidentId);
            }
        }
    }
}
