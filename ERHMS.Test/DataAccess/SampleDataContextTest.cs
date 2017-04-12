using Epi;
using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace ERHMS.Test.DataAccess
{
    public class SampleDataContextTest
    {
        private TempDirectory directory;
        private Configuration configuration;
        private DataContext dataContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(nameof(SampleDataContextTest));
            ConfigurationExtensions.Create(directory.Path).Save();
            configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            dataContext = SampleDataContext.Create(configuration);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        [Test]
        public void PgmsTest()
        {
            foreach (Pgm pgm in dataContext.GetPgms())
            {
                StringAssert.Contains(dataContext.Project.FilePath, pgm.Content);
            }
        }

        [Test]
        public void CanvasesTest()
        {
            foreach (Canvas canvas in dataContext.GetCanvases())
            {
                StringAssert.Contains(dataContext.Project.FilePath, canvas.Content);
            }
        }

        [Test]
        public void LinkRepositoryTest()
        {
            Canvas canvas = new Canvas
            {
                Name = "Responders",
                Content = Canvas.GetContentForView(dataContext.Project.Views["Responders"])
            };
            dataContext.Project.InsertCanvas(canvas);
            Incident incident = dataContext.Incidents.Select().Single();
            Assert.AreEqual(1, dataContext.CanvasLinks.SelectItems(null).Count());
            Assert.AreEqual(2, dataContext.CanvasLinks.SelectItems(incident.IncidentId).Count());
        }
    }
}
