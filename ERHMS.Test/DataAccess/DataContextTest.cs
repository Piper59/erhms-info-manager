using Dapper;
using Epi;
using Epi.Fields;
using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ERHMS.Test.DataAccess
{
    public abstract class DataContextTest
    {
        private class IncidentInfo
        {
            public Incident Incident { get; set; }

            public string IncidentId
            {
                get { return Incident.IncidentId; }
            }

            public ICollection<Responder> Responders { get; set; }
            public IncidentRole TeamRole { get; set; }
            public IncidentRole JobRole { get; set; }
            public Team Team1 { get; set; }
            public Team Team2 { get; set; }
            public Location Location1 { get; set; }
            public Location Location2 { get; set; }
            public Job Job { get; set; }
            public Domain.View View { get; set; }

            public IncidentInfo(Incident incident)
            {
                Incident = incident;
                Responders = new List<Responder>();
            }
        }

        private TempDirectory directory;
        private IProjectCreator creator;
        private DataContext context;
        private Role teamRole;
        private Role jobRole;
        private ICollection<Responder> responders;
        private IncidentInfo incidentInfo1;
        private IncidentInfo incidentInfo2;
        private Domain.View view;

        protected abstract IProjectCreator GetCreator();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(GetType().Name);
            ConfigurationExtensions.Create(directory.FullName).Save();
            Configuration configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            creator = GetCreator();
            creator.SetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        private void DateRangeTest(int expected, DateTime start, DateTime end, Func<DateTime?, DateTime?, int> select)
        {
            DateTime preStart = start.AddSeconds(-1.0);
            DateTime postStart = start.AddSeconds(1.0);
            DateTime preEnd = end.AddSeconds(-1.0);
            DateTime postEnd = end.AddSeconds(1.0);
            Assert.AreEqual(expected, select(null, null));
            Assert.AreEqual(0, select(null, preStart));
            Assert.AreEqual(1, select(null, postStart));
            Assert.AreEqual(expected - 1, select(null, preEnd));
            Assert.AreEqual(expected, select(null, postEnd));
            Assert.AreEqual(expected, select(preStart, null));
            Assert.AreEqual(expected - 1, select(postStart, null));
            Assert.AreEqual(1, select(preEnd, null));
            Assert.AreEqual(0, select(postEnd, null));
            Assert.AreEqual(0, select(preStart.AddSeconds(-1.0), preStart));
            Assert.AreEqual(1, select(preStart, postStart));
            Assert.AreEqual(expected - 1, select(preStart, preEnd));
            Assert.AreEqual(expected, select(preStart, postEnd));
            Assert.AreEqual(expected - 2, select(postStart, preEnd));
            Assert.AreEqual(expected - 1, select(postStart, postEnd));
            Assert.AreEqual(1, select(preEnd, postEnd));
            Assert.AreEqual(0, select(postEnd, postEnd.AddSeconds(1.0)));
        }

        private void DateRangeTest(int expected, bool starts, bool ends, DateTime start, DateTime end, Func<DateTime?, DateTime?, int> select)
        {
            DateTime preStart = start.AddSeconds(-1.0);
            DateTime postStart = start.AddSeconds(1.0);
            DateTime preEnd = end.AddSeconds(-1.0);
            DateTime postEnd = end.AddSeconds(1.0);
            Assert.AreEqual(expected, select(null, null));
            Assert.AreEqual(starts ? 0 : expected, select(null, preStart));
            Assert.AreEqual(expected, select(null, postStart));
            Assert.AreEqual(expected, select(null, preEnd));
            Assert.AreEqual(expected, select(null, postEnd));
            Assert.AreEqual(expected, select(preStart, null));
            Assert.AreEqual(expected, select(postStart, null));
            Assert.AreEqual(expected, select(preEnd, null));
            Assert.AreEqual(ends ? 0 : expected, select(postEnd, null));
            Assert.AreEqual(starts ? 0 : expected, select(preStart.AddSeconds(-1.0), preStart));
            Assert.AreEqual(expected, select(preStart, postStart));
            Assert.AreEqual(expected, select(preStart, preEnd));
            Assert.AreEqual(expected, select(preStart, postEnd));
            Assert.AreEqual(expected, select(postStart, preEnd));
            Assert.AreEqual(expected, select(postStart, postEnd));
            Assert.AreEqual(expected, select(preEnd, postEnd));
            Assert.AreEqual(ends ? 0 : expected, select(postEnd, postEnd.AddSeconds(1.0)));
        }

        [Test]
        [Order(1)]
        public void CreateTest()
        {
            context = DataContext.Create(creator.Project);
            foreach (string tableName in context.Responders.TableNames)
            {
                Assert.IsTrue(context.Database.TableExists(tableName));
            }
            Assert.IsTrue(context.Database.TableExists("Responders"));
            Assert.IsTrue(context.Database.TableExists("ERHMS_Incidents"));
            Assert.IsTrue(context.Database.TableExists("ERHMS_Jobs"));
            Assert.IsTrue(context.Database.TableExists("ERHMS_UniquePairs"));
            Assert.AreEqual(Assembly.GetExecutingAssembly().GetName().Version, context.Project.Version);
        }

        [Test]
        [Order(2)]
        public void CodesTest()
        {
            Assert.AreEqual(6, context.Prefixes.Count());
            Assert.AreEqual(13, context.Suffixes.Count());
            Assert.AreEqual(3, context.Genders.Count());
            Assert.AreEqual(61, context.States.Count());
        }

        [Test]
        [Order(3)]
        public void RolesTest()
        {
            context.Roles.Delete();
            teamRole = RolesTest("Team Role");
            jobRole = RolesTest("Job Role");
            Assert.AreEqual(2, context.Roles.Select().Count());
        }

        private Role RolesTest(string name)
        {
            Role original = new Role(true)
            {
                Name = name
            };
            context.Roles.Save(original);
            Role retrieved = context.Roles.SelectById(original.RoleId);
            Assert.AreEqual(original.Name, retrieved.Name);
            return retrieved;
        }

        [Test]
        [Order(4)]
        public void RespondersTest()
        {
            responders = new List<Responder>();
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int index = 0; index < 24; index++)
            {
                RespondersTest(alphabet[index].ToString(), index % 2 == 1);
            }
            Assert.AreEqual(responders.Count, context.Responders.Select().Count());
            Assert.AreEqual(responders.Count / 2, context.Responders.SelectUndeleted().Count());
        }

        private void RespondersTest(string middleInitial, bool deleted)
        {
            Responder original = new Responder(true)
            {
                FirstName = "John",
                MiddleInitial = middleInitial,
                LastName = "Doe",
                EmailAddress = string.Format("j{0}doe@example.com", middleInitial.ToLower()),
                Deleted = deleted
            };
            context.Responders.Save(original);
            Responder retrieved = context.Responders.SelectById(original.ResponderId);
            Assert.AreEqual(original.EmailAddress, retrieved.EmailAddress);
            responders.Add(retrieved);
        }

        [Test]
        [Order(5)]
        public void UniquePairsTest()
        {
            IList<string> responderIds = responders.Take(3)
                .Select(responder => responder.ResponderId)
                .ToList();
            UniquePairsTest(responderIds[0], responderIds[1]);
            UniquePairsTest(responderIds[2], responderIds[0]);
            Assert.AreEqual(2, context.UniquePairs.Select().Count());
            ILookup<string, string> lookup = context.UniquePairs.SelectLookup();
            AssertExtensions.AreEquivalent(lookup[responderIds[0]], responderIds[1], responderIds[2]);
            AssertExtensions.AreEquivalent(lookup[responderIds[1]], responderIds[0]);
            AssertExtensions.AreEquivalent(lookup[responderIds[2]], responderIds[0]);
        }

        private void UniquePairsTest(string responder1Id, string responder2Id)
        {
            UniquePair original = new UniquePair(true)
            {
                Responder1Id = responder1Id,
                Responder2Id = responder2Id
            };
            context.UniquePairs.Save(original);
            UniquePair retrieved = context.UniquePairs.SelectById(original.UniquePairId);
            Assert.AreEqual(responder1Id, retrieved.Responder1Id);
            Assert.AreEqual(responder2Id, retrieved.Responder2Id);
        }

        [Test]
        [Order(6)]
        public void IncidentsTest()
        {
            incidentInfo1 = IncidentsTest(0, false);
            incidentInfo2 = IncidentsTest(1, true);
            Assert.AreEqual(2, context.Incidents.Select().Count());
            Assert.AreEqual(1, context.Incidents.SelectUndeleted().Count());
        }

        private IncidentInfo IncidentsTest(int index, bool deleted)
        {
            Incident original = new Incident(true)
            {
                Name = string.Format("Test Incident {0}", index + 1),
                Deleted = deleted
            };
            context.Incidents.Save(original);
            Incident retrieved = context.Incidents.SelectById(original.IncidentId);
            Assert.AreEqual(original.Name, retrieved.Name);
            CollectionAssert.AreEquivalent(
                context.Roles.Select().Select(role => role.Name),
                context.IncidentRoles.SelectByIncidentId(original.IncidentId).Select(incidentRole => incidentRole.Name));
            return new IncidentInfo(retrieved);
        }

        [Test]
        [Order(7)]
        public void IncidentNotesTest()
        {
            IncidentNotesTest(incidentInfo1);
            IncidentNotesTest(incidentInfo2);
            Assert.AreEqual(6, context.IncidentNotes.Select().Count());
        }

        private void IncidentNotesTest(IncidentInfo incidentInfo)
        {
            for (int day = 1; day <= 3; day++)
            {
                IncidentNotesTest(incidentInfo, day);
            }
            ICollection<DateTime> dates = context.IncidentNotes.SelectByIncidentId(incidentInfo.IncidentId)
                .Select(incidentNote => incidentNote.Date)
                .ToList();
            Assert.AreEqual(3, dates.Count);
            DateRangeTest(3, dates.Min(), dates.Max(), (dateMin, dateMax) =>
            {
                return context.IncidentNotes.SelectByIncidentIdAndDateRange(incidentInfo.IncidentId, dateMin, dateMax).Count();
            });
        }

        private void IncidentNotesTest(IncidentInfo incidentInfo, int day)
        {
            DateTime date = new DateTime(2000, 1, day);
            IncidentNote original = new IncidentNote(true)
            {
                IncidentId = incidentInfo.IncidentId,
                Content = date.ToString("g"),
                Date = date
            };
            context.IncidentNotes.Save(original);
            IncidentNote retrieved = context.IncidentNotes.SelectById(original.IncidentNoteId);
            Assert.AreEqual(original.Content, retrieved.Content);
            Assert.AreEqual(incidentInfo.Incident.Name, retrieved.Incident.Name);
        }

        [Test]
        [Order(8)]
        public void RostersTest()
        {
            RostersTest(incidentInfo1, 0);
            RostersTest(incidentInfo2, 1);
            Assert.AreEqual(responders.Count, context.Rosters.Select().Count());
        }

        private void RostersTest(IncidentInfo incidentInfo, int index)
        {
            foreach (Iterator<Responder> responder in responders.Iterate())
            {
                if (responder.Index % 4 / 2 == index)
                {
                    RostersTest(incidentInfo, responder.Value);
                }
            }
            Assert.AreEqual(responders.Count / 4, context.Rosters.SelectUndeletedByIncidentId(incidentInfo.IncidentId).Count());
            Assert.AreEqual(responders.Count / 4, context.Responders.SelectRosterable(incidentInfo.IncidentId).Count());
        }

        private void RostersTest(IncidentInfo incidentInfo, Responder responder)
        {
            Roster original = new Roster(true)
            {
                ResponderId = responder.ResponderId,
                IncidentId = incidentInfo.IncidentId
            };
            context.Rosters.Save(original);
            Roster retrieved = context.Rosters.SelectById(original.RosterId);
            Assert.AreEqual(responder.EmailAddress, retrieved.Responder.EmailAddress);
            Assert.AreEqual(incidentInfo.Incident.Name, retrieved.Incident.Name);
            Assert.AreEqual(incidentInfo.Incident.Deleted ? 0 : 1, context.Incidents.SelectUndeletedByResponderId(responder.ResponderId).Count());
            incidentInfo.Responders.Add(responder);
        }

        private IncidentRole GetIncidentRole(string incidentId, string name)
        {
            string clauses = "WHERE [ERHMS_IncidentRoles].[IncidentId] = @IncidentId AND [ERHMS_IncidentRoles].[Name] LIKE @Name";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@IncidentId", incidentId);
            parameters.Add("@Name", name);
            return context.IncidentRoles.Select(clauses, parameters).Single();
        }

        [Test]
        [Order(9)]
        public void IncidentRolesTest()
        {
            IncidentRolesTest(incidentInfo1);
            IncidentRolesTest(incidentInfo2);
            ICollection<IncidentRole> incidentRoles = context.IncidentRoles.Select().ToList();
            Assert.AreEqual(4, incidentRoles.Count);
            Assert.IsTrue(incidentRoles.All(incidentRole => !incidentRole.InUse));
        }

        private void IncidentRolesTest(IncidentInfo incidentInfo)
        {
            Assert.AreEqual(2, context.IncidentRoles.SelectByIncidentId(incidentInfo.IncidentId).Count());
            incidentInfo.TeamRole = GetIncidentRole(incidentInfo.IncidentId, teamRole.Name);
            Assert.AreEqual(incidentInfo.Incident.Name, incidentInfo.TeamRole.Incident.Name);
            incidentInfo.JobRole = GetIncidentRole(incidentInfo.IncidentId, jobRole.Name);
            Assert.AreEqual(incidentInfo.Incident.Name, incidentInfo.JobRole.Incident.Name);
        }

        [Test]
        [Order(10)]
        public void TeamsTest()
        {
            TeamsTest(incidentInfo1);
            TeamsTest(incidentInfo2);
            Assert.AreEqual(4, context.Teams.Select().Count());
            Assert.AreEqual(8, context.TeamResponders.Select().Count());
            context.TeamResponders.DeleteByTeamId(incidentInfo2.Team2.TeamId);
            Assert.AreEqual(6, context.TeamResponders.Select().Count());
        }

        private void TeamsTest(IncidentInfo incidentInfo)
        {
            incidentInfo.Team1 = TeamsTest(incidentInfo, 0);
            incidentInfo.Team2 = TeamsTest(incidentInfo, 1);
            Assert.AreEqual(2, context.Teams.SelectByIncidentId(incidentInfo.IncidentId).Count());
            Assert.IsTrue(context.IncidentRoles.SelectById(incidentInfo.TeamRole.IncidentRoleId).InUse);
            TeamsWithRespondersTest(incidentInfo.Responders.Take(4), incidentInfo, false);
            TeamsWithRespondersTest(incidentInfo.Responders.ElementsAt(0, 2), incidentInfo, true);
        }

        private Team TeamsTest(IncidentInfo incidentInfo, int index)
        {
            Team original = new Team(true)
            {
                IncidentId = incidentInfo.IncidentId,
                Name = string.Format("{0} Team {1}", incidentInfo.Incident.Name, index + 1)
            };
            context.Teams.Save(original);
            Team retrieved = context.Teams.SelectById(original.TeamId);
            Assert.AreEqual(original.Name, retrieved.Name);
            Assert.AreEqual(incidentInfo.Incident.Name, retrieved.Incident.Name);
            TeamRespondersTest(incidentInfo, retrieved, index);
            return retrieved;
        }

        private void TeamRespondersTest(IncidentInfo incidentInfo, Team team, int index)
        {
            Assert.AreEqual(responders.Count / 4, context.Responders.SelectTeamable(team.IncidentId, team.TeamId).Count());
            Assert.AreEqual(0, context.TeamResponders.SelectUndeletedByTeamId(team.TeamId).Count());
            foreach (Responder responder in incidentInfo.Responders.Skip(index * 2).Take(2))
            {
                TeamRespondersTest(team, responder, incidentInfo.TeamRole);
            }
            Assert.AreEqual(responders.Count / 4 - 1, context.Responders.SelectTeamable(team.IncidentId, team.TeamId).Count());
            Assert.AreEqual(1, context.TeamResponders.SelectUndeletedByTeamId(team.TeamId).Count());
        }

        private void TeamRespondersTest(Team team, Responder responder, IncidentRole incidentRole)
        {
            TeamResponder original = new TeamResponder(true)
            {
                TeamId = team.TeamId,
                ResponderId = responder.ResponderId,
                IncidentRoleId = incidentRole.IncidentRoleId
            };
            context.TeamResponders.Save(original);
            TeamResponder retrieved = context.TeamResponders.SelectById(original.TeamResponderId);
            Assert.AreEqual(team.Name, retrieved.Team.Name);
            Assert.AreEqual(team.Incident.Name, retrieved.Team.Incident.Name);
            Assert.AreEqual(responder.EmailAddress, retrieved.Responder.EmailAddress);
            Assert.AreEqual(incidentRole.Name, retrieved.IncidentRole.Name);
            Assert.AreEqual(incidentRole.Incident.Name, retrieved.IncidentRole.Incident.Name);
            Assert.AreEqual(team.Incident.Deleted ? 0 : 1, context.TeamResponders.SelectUndeletedByResponderId(responder.ResponderId).Count());
        }

        private void TeamsWithRespondersTest(IEnumerable<Responder> expected, IncidentInfo incidentInfo, bool undeleted)
        {
            CollectionAssert.AreEquivalent(expected, context.Teams.SelectByIncidentId(incidentInfo.IncidentId)
                .WithResponders(context, undeleted)
                .SelectMany(team => team.Responders));
        }

        [Test]
        [Order(11)]
        public void LocationsTest()
        {
            LocationsTest(incidentInfo1);
            LocationsTest(incidentInfo2);
            Assert.AreEqual(4, context.Locations.Select().Count());
        }

        private void LocationsTest(IncidentInfo incidentInfo)
        {
            incidentInfo.Location1 = LocationsTest(incidentInfo, 0);
            incidentInfo.Location2 = LocationsTest(incidentInfo, 1);
            Assert.AreEqual(2, context.Locations.SelectByIncidentId(incidentInfo.IncidentId).Count());
        }

        private Location LocationsTest(IncidentInfo incidentInfo, int index)
        {
            Location original = new Location(true)
            {
                IncidentId = incidentInfo.IncidentId,
                Name = string.Format("{0} Location {1}", incidentInfo.Incident.Name, index + 1)
            };
            context.Locations.Save(original);
            Location retrieved = context.Locations.SelectById(original.LocationId);
            Assert.AreEqual(original.Name, retrieved.Name);
            Assert.AreEqual(incidentInfo.Incident.Name, retrieved.Incident.Name);
            return retrieved;
        }

        [Test]
        [Order(12)]
        public void JobsTest()
        {
            JobsTest(incidentInfo1);
            JobsWithRespondersTest(incidentInfo1.Responders.Take(6), incidentInfo1, false);
            JobsWithRespondersTest(incidentInfo1.Responders.ElementsAt(0, 2, 4), incidentInfo1, true);
            JobsTest(incidentInfo2);
            JobsWithRespondersTest(incidentInfo2.Responders.ElementsAt(0, 1, 4, 5), incidentInfo2, false);
            JobsWithRespondersTest(incidentInfo2.Responders.ElementsAt(0, 4), incidentInfo2, true);
            JobTicketsTest();
            Assert.AreEqual(2, context.Jobs.Select().Count());
            Assert.AreEqual(6, context.JobNotes.Select().Count());
            context.JobNotes.DeleteByJobId(incidentInfo2.Job.JobId);
            Assert.AreEqual(3, context.JobNotes.Select().Count());
            Assert.AreEqual(4, context.JobTeams.Select().Count());
            context.JobTeams.DeleteByJobId(incidentInfo2.Job.JobId);
            Assert.AreEqual(2, context.JobTeams.Select().Count());
            context.JobTeams.DeleteByTeamId(incidentInfo1.Team2.TeamId);
            Assert.AreEqual(1, context.JobTeams.Select().Count());
            Assert.AreEqual(4, context.JobResponders.Select().Count());
            context.JobResponders.DeleteByJobId(incidentInfo2.Job.JobId);
            Assert.AreEqual(2, context.JobResponders.Select().Count());
            Assert.AreEqual(4, context.JobLocations.Select().Count());
            context.JobLocations.DeleteByJobId(incidentInfo2.Job.JobId);
            Assert.AreEqual(2, context.JobLocations.Select().Count());
            context.JobLocations.DeleteByLocationId(incidentInfo1.Location2.LocationId);
            Assert.AreEqual(1, context.JobLocations.Select().Count());
        }

        private Job JobsTest(IncidentInfo incidentInfo)
        {
            Job original = new Job(true)
            {
                IncidentId = incidentInfo.IncidentId,
                Name = string.Format("{0} Job", incidentInfo.Incident.Name)
            };
            context.Jobs.Save(original);
            Job retrieved = context.Jobs.SelectById(original.JobId);
            Assert.AreEqual(original.Name, retrieved.Name);
            Assert.AreEqual(incidentInfo.Incident.Name, retrieved.Incident.Name);
            incidentInfo.Job = retrieved;
            JobDateRangeTest(retrieved);
            JobNotesTest(retrieved);
            JobTeamsTest(incidentInfo);
            JobRespondersTest(incidentInfo);
            JobLocationsTest(incidentInfo);
            Assert.AreEqual(1, context.Jobs.SelectByIncidentId(incidentInfo.IncidentId).Count());
            Assert.IsTrue(context.IncidentRoles.SelectById(incidentInfo.JobRole.IncidentRoleId).InUse);
            return retrieved;
        }

        private void JobDateRangeTest(Job job)
        {
            JobDateRangeTest(job, false, false);
            JobDateRangeTest(job, true, false);
            JobDateRangeTest(job, false, true);
            JobDateRangeTest(job, true, true);
        }

        private void JobDateRangeTest(Job job, bool starts, bool ends)
        {
            DateTime startDate = new DateTime(2000, 1, 1);
            DateTime endDate = new DateTime(2000, 1, 2);
            job.StartDate = starts ? startDate : (DateTime?)null;
            job.EndDate = ends ? endDate : (DateTime?)null;
            context.Jobs.Save(job);
            DateRangeTest(1, starts, ends, startDate, endDate, (dateMin, dateMax) =>
            {
                return context.Jobs.SelectByIncidentIdAndDateRange(job.IncidentId, dateMin, dateMax).Count();
            });
        }

        private void JobNotesTest(Job job)
        {
            for (int day = 1; day <= 3; day++)
            {
                JobNotesTest(job, day);
            }
            ICollection<DateTime> dates = context.JobNotes.SelectByJobId(job.JobId)
                .Select(jobNote => jobNote.Date)
                .ToList();
            Assert.AreEqual(3, dates.Count);
            DateRangeTest(3, dates.Min(), dates.Max(), (dateMin, dateMax) =>
            {
                return context.JobNotes.SelectByIncidentIdAndDateRange(job.IncidentId, dateMin, dateMax).Count();
            });
        }

        private void JobNotesTest(Job job, int day)
        {
            DateTime date = new DateTime(2000, 1, day);
            JobNote original = new JobNote(true)
            {
                JobId = job.JobId,
                Content = date.ToString("g"),
                Date = date
            };
            context.JobNotes.Save(original);
            JobNote retrieved = context.JobNotes.SelectById(original.JobNoteId);
            Assert.AreEqual(original.Content, retrieved.Content);
            Assert.AreEqual(job.Name, retrieved.Job.Name);
            Assert.AreEqual(job.Incident.Name, retrieved.Job.Incident.Name);
        }

        private void JobTeamsTest(IncidentInfo incidentInfo)
        {
            Job job = incidentInfo.Job;
            Assert.AreEqual(2, context.Teams.SelectJobbable(job.IncidentId, job.JobId).Count());
            Assert.AreEqual(0, context.JobTeams.SelectByJobId(job.JobId).Count());
            JobTeamsTest(job, incidentInfo.Team1);
            JobTeamsTest(job, incidentInfo.Team2);
            Assert.AreEqual(0, context.Teams.SelectJobbable(job.IncidentId, job.JobId).Count());
            Assert.AreEqual(2, context.JobTeams.SelectByJobId(job.JobId).Count());
            DateRangeTest(2, true, true, job.StartDate.Value, job.EndDate.Value, (dateMin, dateMax) =>
            {
                return context.JobTeams.SelectByIncidentIdAndDateRange(job.IncidentId, dateMin, dateMax).Count();
            });
        }

        private void JobTeamsTest(Job job, Team team)
        {
            JobTeam original = new JobTeam(true)
            {
                JobId = job.JobId,
                TeamId = team.TeamId
            };
            context.JobTeams.Save(original);
            JobTeam retrieved = context.JobTeams.SelectById(original.JobTeamId);
            Assert.AreEqual(job.Name, retrieved.Job.Name);
            Assert.AreEqual(job.Incident.Name, retrieved.Job.Incident.Name);
            Assert.AreEqual(team.Name, retrieved.Team.Name);
            Assert.AreEqual(team.Incident.Name, retrieved.Team.Incident.Name);
        }

        private void JobRespondersTest(IncidentInfo incidentInfo)
        {
            Job job = incidentInfo.Job;
            Assert.AreEqual(responders.Count / 4, context.Responders.SelectJobbable(job.IncidentId, job.JobId).Count());
            Assert.AreEqual(0, context.JobResponders.SelectUndeletedByJobId(job.JobId).Count());
            foreach (Responder responder in incidentInfo.Responders.Skip(4).Take(2))
            {
                JobRespondersTest(job, responder, incidentInfo.JobRole);
            }
            Assert.AreEqual(responders.Count / 4 - 1, context.Responders.SelectJobbable(job.IncidentId, job.JobId).Count());
            Assert.AreEqual(1, context.JobResponders.SelectUndeletedByJobId(job.JobId).Count());
        }

        private void JobRespondersTest(Job job, Responder responder, IncidentRole incidentRole)
        {
            JobResponder original = new JobResponder(true)
            {
                JobId = job.JobId,
                ResponderId = responder.ResponderId,
                IncidentRoleId = incidentRole.IncidentRoleId
            };
            context.JobResponders.Save(original);
            JobResponder retrieved = context.JobResponders.SelectById(original.JobResponderId);
            Assert.AreEqual(job.Name, retrieved.Job.Name);
            Assert.AreEqual(job.Incident.Name, retrieved.Job.Incident.Name);
            Assert.AreEqual(responder.EmailAddress, retrieved.Responder.EmailAddress);
            Assert.AreEqual(incidentRole.Name, retrieved.IncidentRole.Name);
            Assert.AreEqual(incidentRole.Incident.Name, retrieved.IncidentRole.Incident.Name);
        }

        private void JobLocationsTest(IncidentInfo incidentInfo)
        {
            Job job = incidentInfo.Job;
            Assert.AreEqual(2, context.Locations.SelectJobbable(job.IncidentId, job.JobId).Count());
            Assert.AreEqual(0, context.JobLocations.SelectByJobId(job.JobId).Count());
            JobLocationsTest(job, incidentInfo.Location1);
            JobLocationsTest(job, incidentInfo.Location2);
            Assert.AreEqual(0, context.Locations.SelectJobbable(job.IncidentId, job.JobId).Count());
            Assert.AreEqual(2, context.JobLocations.SelectByJobId(job.JobId).Count());
        }

        private void JobLocationsTest(Job job, Location location)
        {
            JobLocation original = new JobLocation(true)
            {
                JobId = job.JobId,
                LocationId = location.LocationId
            };
            context.JobLocations.Save(original);
            JobLocation retrieved = context.JobLocations.SelectById(original.JobLocationId);
            Assert.AreEqual(job.Name, retrieved.Job.Name);
            Assert.AreEqual(job.Incident.Name, retrieved.Job.Incident.Name);
            Assert.AreEqual(location.Name, retrieved.Location.Name);
            Assert.AreEqual(location.Incident.Name, retrieved.Location.Incident.Name);
        }

        private void JobsWithRespondersTest(IEnumerable<Responder> expected, IncidentInfo incidentInfo, bool undeleted)
        {
            CollectionAssert.AreEquivalent(expected, context.Jobs.SelectByIncidentId(incidentInfo.IncidentId)
                .WithResponders(context, undeleted)
                .SelectMany(job => job.Responders));
        }

        private void JobTicketsTest()
        {
            Assert.AreEqual(10, context.JobTickets.Select().Count());
            JobTicketsDateRangeTest(3, incidentInfo1);
            JobTicketsDateRangeTest(2, incidentInfo2);
            foreach (Iterator<Responder> responder in incidentInfo1.Responders.Iterate())
            {
                ICollection<JobTicket> jobTickets = context.JobTickets.SelectUndeletedByResponderId(responder.Value.ResponderId)
                    .WithLocations(context)
                    .ToList();
                if (responder.Index < 6)
                {
                    Assert.AreEqual(1, jobTickets.Count);
                    JobTicket jobTicket = jobTickets.First();
                    Job job = incidentInfo1.Job;
                    Assert.AreEqual(job.Name, jobTicket.Job.Name);
                    Assert.AreEqual(job.Incident.Name, jobTicket.Incident.Name);
                    Team team = null;
                    if (responder.Index < 4)
                    {
                        team = responder.Index < 2 ? incidentInfo1.Team1 : incidentInfo1.Team2;
                    }
                    Assert.AreEqual(team?.Name, jobTicket.Team?.Name);
                    Assert.AreEqual(responder.Value.EmailAddress, jobTicket.Responder.EmailAddress);
                    IncidentRole incidentRole = responder.Index < 4 ? incidentInfo1.TeamRole : incidentInfo1.JobRole;
                    Assert.AreEqual(incidentRole.Name, jobTicket.IncidentRole.Name);
                    AssertExtensions.AreEquivalent(
                        jobTicket.Locations.Select(location => location.Name),
                        incidentInfo1.Location1.Name,
                        incidentInfo1.Location2.Name);
                }
                else
                {
                    Assert.AreEqual(0, jobTickets.Count);
                }
            }
            foreach (Responder responder in incidentInfo2.Responders)
            {
                Assert.AreEqual(0, context.JobTickets.SelectUndeletedByResponderId(responder.ResponderId).Count());
            }
        }

        private void JobTicketsDateRangeTest(int expected, IncidentInfo incidentInfo)
        {
            Assert.AreEqual(expected, context.JobTickets.SelectUndeletedByIncidentId(incidentInfo.IncidentId).Count());
            DateRangeTest(expected, true, true, incidentInfo.Job.StartDate.Value, incidentInfo.Job.EndDate.Value, (dateMin, dateMax) =>
            {
                return context.JobTickets.SelectUndeletedByIncidentIdAndDateRange(incidentInfo.IncidentId, dateMin, dateMax).Count();
            });
        }

        [Test]
        [Order(13)]
        public void ViewsTest()
        {
            view = ViewsTest(null);
            incidentInfo1.View = ViewsTest(incidentInfo1.Incident);
            incidentInfo2.View = ViewsTest(incidentInfo2.Incident);
            Assert.AreEqual(4, context.Views.Select().Count());
            Assert.AreEqual(3, context.Views.SelectUndeleted().Count());
        }

        private Domain.View ViewsTest(Incident incident)
        {
            string viewName = string.Format("{0}_Test_View", incident?.Name);
            Epi.View original = context.Project.CreateView(context.Project.SuggestViewName(viewName));
            Page page = original.CreatePage("New Page", 0);
            if (incident == null)
            {
                Field field = page.CreateField(MetaFieldType.Text);
                field.Name = "ResponderID";
                field.SaveToDb();
            }
            else
            {
                context.ViewLinks.Save(new ViewLink(true)
                {
                    ViewId = original.Id,
                    IncidentId = incident.IncidentId
                });
            }
            context.Project.CollectedData.EnsureDataTablesExist(original.Id);
            Domain.View retrieved = context.Views.SelectById(original.Id);
            Assert.AreEqual(original.Name, retrieved.Name);
            if (incident == null)
            {
                Assert.IsTrue(retrieved.HasResponderIdField);
            }
            else
            {
                Assert.IsFalse(retrieved.HasResponderIdField);
                Assert.AreEqual(incident.Name, retrieved.Incident.Name);
                Assert.AreEqual(1, context.Views.SelectByIncidentId(incident.IncidentId).Count());
            }
            return retrieved;
        }

        [Test]
        [Order(14)]
        public void AssignmentsTest()
        {
            foreach (Responder responder in responders)
            {
                AssignmentsTest(view, responder);
            }
            AssignmentsTest(incidentInfo1);
            AssignmentsTest(incidentInfo2);
            Assert.AreEqual(responders.Count * 2, context.Assignments.Select().Count());
            Assert.AreEqual(responders.Count * 3 / 4, context.Assignments.SelectUndeleted().Count());
            context.Assignments.DeleteByViewId(incidentInfo2.View.ViewId);
            Assert.AreEqual(responders.Count * 3 / 2, context.Assignments.Select().Count());
        }

        private void AssignmentsTest(IncidentInfo incidentInfo)
        {
            foreach (Responder responder in incidentInfo.Responders)
            {
                AssignmentsTest(incidentInfo.View, responder);
            }
            Assert.AreEqual(responders.Count / 4, context.Assignments.SelectUndeletedByIncidentId(incidentInfo.IncidentId).Count());
        }

        private void AssignmentsTest(Domain.View view, Responder responder)
        {
            Assignment original = new Assignment(true)
            {
                ViewId = view.ViewId,
                ResponderId = responder.ResponderId
            };
            context.Assignments.Save(original);
            Assignment retrieved = context.Assignments.SelectById(original.AssignmentId);
            Assert.AreEqual(view.Name, retrieved.View.Name);
            Assert.AreEqual(view.HasResponderIdField, retrieved.View.HasResponderIdField);
            Assert.AreEqual(view.Incident?.Name, retrieved.View.Incident?.Name);
            Assert.AreEqual(responder.EmailAddress, retrieved.Responder.EmailAddress);
        }

        [Test]
        [Order(15)]
        public void RecordsTest()
        {
            ViewEntityRepository<ViewEntity> entities = new ViewEntityRepository<ViewEntity>(context.Database, context.Project.Views[view.Name]);
            foreach (Responder responder in responders)
            {
                Assert.AreEqual(0, context.Records.SelectByResponderId(responder.ResponderId).Count());
                ViewEntity entity = new ViewEntity(true);
                entity.SetProperty("ResponderID", responder.ResponderId);
                entities.Save(entity);
                Assert.AreEqual(1, context.Records.SelectByResponderId(responder.ResponderId).Count());
            }
            Assert.AreEqual(responders.Count, context.Records.Select().Count());
        }

        [Test]
        [Order(16)]
        public void WebSurveysTest()
        {
            WebSurveysTest(view);
            WebSurveysTest(incidentInfo1.View);
            WebSurveysTest(incidentInfo2.View);
            Assert.AreEqual(3, context.WebSurveys.Select().Count());
            context.WebSurveys.DeleteByViewId(incidentInfo2.View.ViewId);
            Assert.AreEqual(2, context.WebSurveys.Select().Count());
        }

        private void WebSurveysTest(Domain.View view)
        {
            WebSurvey original = new WebSurvey(true)
            {
                ViewId = view.ViewId,
                PublishKey = Guid.NewGuid().ToString()
            };
            context.WebSurveys.Save(original);
            WebSurvey retrieved = context.WebSurveys.SelectById(original.WebSurveyId);
            Assert.AreEqual(original.PublishKey, retrieved.PublishKey);
            Assert.AreEqual(view.Name, retrieved.View.Name);
            Assert.AreEqual(view.HasResponderIdField, retrieved.View.HasResponderIdField);
            Assert.AreEqual(view.Incident?.Name, retrieved.View.Incident?.Name);
        }

        [Test]
        [Order(17)]
        public void PgmsTest()
        {
            PgmTest(null);
            PgmTest(incidentInfo1.Incident);
            PgmTest(incidentInfo2.Incident);
            Assert.AreEqual(3, context.Pgms.Select().Count());
            Assert.AreEqual(2, context.Pgms.SelectUndeleted().Count());
        }

        private void PgmTest(Incident incident)
        {
            ERHMS.EpiInfo.Pgm original = new ERHMS.EpiInfo.Pgm
            {
                Name = string.Format("{0} Test Analysis", incident?.Name).Trim(),
                Content = ""
            };
            context.Project.InsertPgm(original);
            if (incident != null)
            {
                context.PgmLinks.Save(new PgmLink(true)
                {
                    PgmId = original.PgmId,
                    IncidentId = incident.IncidentId
                });
            }
            Domain.Pgm retrieved = context.Pgms.SelectById(original.PgmId);
            Assert.AreEqual(original.Name, retrieved.Name);
            Assert.AreEqual(incident?.Name, retrieved.Incident?.Name);
            if (incident != null)
            {
                Assert.AreEqual(1, context.Pgms.SelectByIncidentId(incident.IncidentId).Count());
            }
        }

        [Test]
        [Order(18)]
        public void CanvasesTest()
        {
            CanvasTest(null);
            CanvasTest(incidentInfo1.Incident);
            CanvasTest(incidentInfo2.Incident);
            Assert.AreEqual(3, context.Canvases.Select().Count());
            Assert.AreEqual(2, context.Canvases.SelectUndeleted().Count());
        }

        private void CanvasTest(Incident incident)
        {
            ERHMS.EpiInfo.Canvas original = new ERHMS.EpiInfo.Canvas
            {
                Name = string.Format("{0} Test Dashboard", incident?.Name).Trim(),
                Content = ""
            };
            context.Project.InsertCanvas(original);
            if (incident != null)
            {
                context.CanvasLinks.Save(new CanvasLink(true)
                {
                    CanvasId = original.CanvasId,
                    IncidentId = incident.IncidentId
                });
            }
            Domain.Canvas retrieved = context.Canvases.SelectById(original.CanvasId);
            Assert.AreEqual(original.Name, retrieved.Name);
            Assert.AreEqual(incident?.Name, retrieved.Incident?.Name);
            if (incident != null)
            {
                Assert.AreEqual(1, context.Canvases.SelectByIncidentId(incident.IncidentId).Count());
            }
        }
    }

    public class AccessDataContextTest : DataContextTest
    {
        protected override IProjectCreator GetCreator()
        {
            return new AccessProjectCreator(nameof(AccessDataContextTest));
        }
    }

    public class SqlServerDataContextTest : DataContextTest
    {
        protected override IProjectCreator GetCreator()
        {
            return new SqlServerProjectCreator(nameof(SqlServerDataContextTest));
        }
    }
}
