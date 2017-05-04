﻿using Epi;
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
        private DataContext dataContext;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(nameof(SampleDataContextTest));
            ConfigurationExtensions.Create(directory.Path).Save();
            Configuration configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            dataContext = SampleDataContext.Create();
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
            foreach (Pgm pgm in dataContext.Project.GetPgms())
            {
                StringAssert.Contains(dataContext.Project.FilePath, pgm.Content);
            }
        }

        [Test]
        public void CanvasesTest()
        {
            foreach (Canvas canvas in dataContext.Project.GetCanvases())
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
            Assert.AreEqual(3, dataContext.CanvasLinks.SelectItems().Count());
            Assert.AreEqual(2, dataContext.CanvasLinks.SelectItemsByIncidentId(incident.IncidentId).Count());
        }
    }
}
