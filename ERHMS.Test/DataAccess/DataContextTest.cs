using Dapper;
using ERHMS.Dapper;
using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Configuration = Epi.Configuration;

namespace ERHMS.Test.DataAccess
{
    public class DataContextTest
    {
        private TempDirectory directory;
        private DataContext context;
        private Incident incident;
        private Epi.View view;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DataContext.Configure();
            directory = new TempDirectory(nameof(DataContextTest));
            ConfigurationExtensions.Create(directory.FullName).Save();
            Configuration configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            context = SampleDataContext.Create();
            incident = context.Incidents.Select().Single();
            view = context.Project.Views["GreensburgTornado_Symptoms"];
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        [Test]
        public void AssignmentRepositoryTest()
        {
            {
                ICollection<Assignment> assignments = context.Assignments.Select().ToList();
                Assert.IsTrue(assignments.All(assignment => assignment.View.Name == view.Name));
                ICollection<string> lastNames = new string[]
                {
                    "Adkins",
                    "Bowen",
                    "Byers"
                };
                CollectionAssert.AreEquivalent(lastNames, assignments.Select(assignment => assignment.Responder.LastName));
            }
            {
                Assignment assignment = context.Assignments.SelectById("195397a3-a0e9-4f0e-9325-8fe0f6dac74f");
                Assert.AreEqual(view.Name, assignment.View.Name);
                Assert.AreEqual("Adkins", assignment.Responder.LastName);
            }
            {
                using (Transaction transaction = context.Database.BeginTransaction())
                {
                    context.Assignments.DeleteByViewId(view.Id);
                    Assert.AreEqual(0, context.Assignments.Count());
                }
            }
        }

        [Test]
        public void CanvasRepositoryTest()
        {
            Assert.IsTrue(context.Canvases.Select().All(canvas => canvas.Incident?.Name == incident.Name));
            Assert.IsTrue(context.CanvasLinks.Select().All(canvasLink => canvasLink.Incident.Name == incident.Name));
            Assert.AreEqual(context.CanvasLinks.Count(), context.Canvases.SelectByIncidentId(incident.IncidentId).Count());
            using (Transaction transaction = context.Database.BeginTransaction())
            {
                Domain.Canvas canvas = context.Canvases.Select().First();
                context.CanvasLinks.DeleteByCanvasId(canvas.CanvasId);
                Assert.IsNull(context.Canvases.SelectById(canvas.CanvasId).Incident);
            }
        }

        [Test]
        public void IncidentRepository()
        {
            Assert.AreEqual(1, context.Incidents.SelectUndeleted().Count());
            using (Transaction transaction = context.Database.BeginTransaction())
            {
                Incident incident = (Incident)this.incident.Clone();
                incident.Deleted = true;
                context.Incidents.Save(incident);
                Assert.AreEqual(0, context.Incidents.SelectUndeleted().Count());
            }
        }

        [Test]
        public void IncidentNoteRepositoryTest()
        {
            Assert.IsTrue(context.IncidentNotes.Select().All(incidentNote => incidentNote.Incident.Name == incident.Name));
        }

        [Test]
        public void LocationRepositoryTest()
        {
            Assert.IsTrue(context.Locations.Select().All(location => location.Incident.Name == incident.Name));
        }

        [Test]
        public void PgmRepositoryTest()
        {
            Assert.IsTrue(context.Pgms.Select().All(pgm => pgm.Incident?.Name == incident.Name));
            Assert.IsTrue(context.PgmLinks.Select().All(pgmLink => pgmLink.Incident.Name == incident.Name));
            Assert.AreEqual(context.PgmLinks.Count(), context.Pgms.SelectByIncidentId(incident.IncidentId).Count());
            using (Transaction transaction = context.Database.BeginTransaction())
            {
                Domain.Pgm pgm = context.Pgms.Select().First();
                context.PgmLinks.DeleteByPgmId(pgm.PgmId);
                Assert.IsNull(context.Pgms.SelectById(pgm.PgmId).Incident);
            }
        }

        [Test]
        public void RosterRepositoryTest()
        {
            {
                ICollection<Roster> rosters = context.Rosters.Select().ToList();
                ICollection<string> lastNames = new string[]
                {
                    "Adkins",
                    "Bowen",
                    "Byers",
                    "Cunningham",
                    "Everett",
                    "Forbes",
                    "Gay",
                    "Hendricks",
                    "Holcomb",
                    "Huffman",
                    "James",
                    "Mccullough",
                    "Myers",
                    "Newman",
                    "Owens",
                    "Perkins",
                    "Pugh",
                    "Randall",
                    "Robertson",
                    "Robertson",
                    "Serrano",
                    "Swanson",
                    "Todd",
                    "Torres",
                    "Vasquez",
                    "Walls",
                    "Wiley",
                    "Winters"
                };
                CollectionAssert.AreEquivalent(lastNames, rosters.Select(roster => roster.Responder.LastName));
                Assert.IsTrue(rosters.All(roster => roster.Incident.Name == incident.Name));
            }
            {
                Roster roster = context.Rosters.SelectById("2b8db049-3daa-4907-a473-f9ce4ef26362");
                Assert.AreEqual("Adkins", roster.Responder.LastName);
                Assert.AreEqual(incident.Name, roster.Incident.Name);
            }
            {
                Assert.AreEqual(context.Rosters.Count(), context.Rosters.SelectByIncidentId(incident.IncidentId).Count());
            }
        }

        [Test]
        public void ViewRepositoryTest()
        {
            string clauses = "WHERE [metaViews].[Name] <> @Name";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Name", context.Responders.View.Name);
            Assert.IsTrue(context.Views.Select(clauses, parameters).All(view => view.Incident?.Name == incident.Name));
            Assert.IsTrue(context.ViewLinks.Select().All(viewLink => viewLink.Incident.Name == incident.Name));
            using (Transaction transaction = context.Database.BeginTransaction())
            {
                View view = context.Views.Select(clauses, parameters).First();
                context.ViewLinks.DeleteByViewId(view.ViewId);
                Assert.IsNull(context.Views.SelectById(view.ViewId).Incident);
            }
        }

        [Test]
        public void WebSurveyRepositoryTest()
        {
            WebSurvey webSurvey = new WebSurvey
            {
                ViewId = view.Id,
                PublishKey = Guid.NewGuid().ToString()
            };
            context.WebSurveys.Insert(webSurvey);
            Assert.AreEqual(view.Name, context.WebSurveys.Select().Single().View.Name);
            Assert.AreEqual(view.Name, context.WebSurveys.SelectById(webSurvey.WebSurveyId).View.Name);
            context.WebSurveys.DeleteByViewId(view.Id);
            Assert.AreEqual(0, context.WebSurveys.Count());
        }
    }
}
