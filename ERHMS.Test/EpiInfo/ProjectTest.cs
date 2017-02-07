using Epi;
using Epi.Fields;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Configuration = Epi.Configuration;
using Project = ERHMS.EpiInfo.Project;
using Settings = ERHMS.Utility.Settings;

namespace ERHMS.Test.EpiInfo
{
    public abstract class ProjectTestBase
    {
        protected DirectoryInfo directory;
        protected Configuration configuration;
        protected ProjectCreationInfo creationInfo;
        protected Project project;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = Helpers.GetTemporaryDirectory(GetType());
            Settings.Default.RootDirectory = directory.FullName;
            configuration = ConfigurationExtensions.Load();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Settings.Default.Reset();
            File.Delete(Configuration.DefaultConfigurationPath);
            Log.SetDirectory(Log.GetDefaultDirectory());
            directory.Delete(true);
        }

        [Test]
        public void CreateTest()
        {
            FileAssert.Exists(project.FilePath);
            XmlDocument document = new XmlDocument();
            document.Load(project.FilePath);
            XmlElement projectElement = document.DocumentElement;
            Assert.AreEqual("Project", projectElement.Name);
            Version version = Assembly.GetExecutingAssembly().GetVersion();
            Assert.AreEqual(version.ToString(), projectElement.GetAttribute("erhmsVersion"));
            Assert.AreEqual(version, project.Version);
            Assert.AreEqual(creationInfo.Name, projectElement.GetAttribute("name"));
            Assert.AreEqual(creationInfo.Location.FullName, projectElement.GetAttribute("location"));
            XmlElement databaseElement = projectElement.SelectSingleElement("CollectedData/Database");
            Assert.AreEqual(creationInfo.Builder.ConnectionString, Configuration.Decrypt(databaseElement.GetAttribute("connectionString")));
            Assert.IsTrue(project.Driver.TableExists("metaCanvases"));
        }

        [Test]
        public void IsValidViewNameTest()
        {
            project.CreateView("IsValidViewNameTest_View");
            IsValidViewNameTest(null, InvalidViewNameReason.Empty);
            IsValidViewNameTest("", InvalidViewNameReason.Empty);
            IsValidViewNameTest(" ", InvalidViewNameReason.Empty);
            IsValidViewNameTest("IsValidViewNameTest-View", InvalidViewNameReason.InvalidChar);
            IsValidViewNameTest("_IsValidViewNameTest_View", InvalidViewNameReason.InvalidFirstChar);
            IsValidViewNameTest(new string('A', 65), InvalidViewNameReason.TooLong);
            IsValidViewNameTest("ISVALIDVIEWNAMETEST_VIEW", InvalidViewNameReason.ViewExists);
            IsValidViewNameTest("METADBINFO", InvalidViewNameReason.TableExists);
            IsValidViewNameTest("IsValidViewNameTest_View_2", InvalidViewNameReason.None);
        }

        private void IsValidViewNameTest(string viewName, InvalidViewNameReason expected)
        {
            InvalidViewNameReason actual;
            Assert.AreEqual(expected == InvalidViewNameReason.None, project.IsValidViewName(viewName, out actual));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SuggestViewNameTest()
        {
            project.CreateView("SuggestViewNameTest_View");
            Assert.AreEqual("SUGGESTVIEWNAMETEST_View_2", project.SuggestViewName("1234_SUGGESTVIEWNAMETEST_!@#$%^&*()View"));
            Assert.AreEqual("metaDbInfo_2", project.SuggestViewName("metaDbInfo"));
        }

        [Test]
        public void WebSurveyTest()
        {
            View view = project.CreateView("WebSurveyTest_View");
            Assert.IsFalse(view.IsWebSurvey());
            view.WebSurveyId = Guid.Empty.ToString();
            view.SaveToDb();
            Assert.IsTrue(view.IsWebSurvey());
            configuration.Settings.WebServiceEndpointAddress = "http://localhost:8080/EIWS/SurveyManagerServiceV2.svc";
            configuration.Save();
            Assert.AreEqual("http://localhost:8080/EIWS/Home/00000000-0000-0000-0000-000000000000", view.GetWebSurveyUrl().ToString());
        }

        [Test]
        public void EnsureDataTablesExistTest()
        {
            View view = project.CreateView("EnsureDataTablesExistTest_View");
            ICollection<Page> pages = new List<Page>();
            for (int index = 0; index < 3; index++)
            {
                pages.Add(view.CreatePage(string.Format("Page {0}", index + 1), index));
            }
            Assert.IsFalse(project.Driver.TableExists(view.TableName));
            foreach (Page page in pages)
            {
                Assert.IsFalse(project.Driver.TableExists(page.TableName));
            }
            project.CollectedData.EnsureDataTablesExist(view);
            Assert.IsTrue(project.Driver.TableExists(view.TableName));
            foreach (Page page in pages)
            {
                Assert.IsTrue(project.Driver.TableExists(page.TableName));
            }
            Assert.DoesNotThrow(() =>
            {
                project.CollectedData.EnsureDataTablesExist(view);
            });
        }

        [Test]
        public void DeleteViewTest()
        {
            View parentView = project.CreateView("DeleteViewTest_ParentView");
            Page parentPage = parentView.CreatePage("Page 1", 0);
            View view = project.CreateView("DeleteViewTest_View", true);
            Page page = view.CreatePage("Page 1", 0);
            View childView = project.CreateView("DeleteViewTest_ChildView");
            Page childPage = childView.CreatePage("Page 1", 0);
            CreateRelateField(parentPage, view);
            CreateRelateField(page, childView);
            project.CollectedData.CreateDataTableForView(parentView, 1);
            project.CollectedData.CreateDataTableForView(view, 1);
            project.CollectedData.CreateDataTableForView(childView, 1);
            Assert.IsTrue(parentView.Fields.Contains("Relate"));
            project.DeleteView(view);
            Assert.IsTrue(project.Views.Contains(parentView.Name));
            Assert.IsFalse(project.Views.Contains(view.Name));
            Assert.IsFalse(project.Views.Contains(childView.Name));
            Assert.IsFalse(project.Views[parentView.Name].Fields.Contains("Relate"));
            project.DeleteView(parentView);
            Assert.IsFalse(project.Views.Contains(parentView.Name));
            ICollection<string> tableNames = new string[]
            {
                parentView.TableName,
                parentPage.TableName,
                view.TableName,
                page.TableName,
                childView.TableName,
                childPage.TableName
            };
            foreach (string tableName in tableNames)
            {
                Assert.IsTrue(project.Driver.TableExists(tableName), tableName);
            }
        }

        private void CreateRelateField(Page parent, View child)
        {
            RelatedViewField field = (RelatedViewField)parent.CreateField(MetaFieldType.Relate);
            field.Name = "Relate";
            field.RelatedViewID = child.Id;
            field.SaveToDb();
        }

        [Test]
        public void PgmTest()
        {
            View view = project.CreateView("PgmTest_View");
            Pgm original = new Pgm
            {
                Name = "PgmTest_Pgm",
                Content = Pgm.GetContentForView(view)
            };
            Assert.AreEqual(0, original.PgmId);
            Assert.AreEqual(0, project.GetPgms().Count());
            project.InsertPgm(original);
            Assert.AreEqual(1, original.PgmId);
            Assert.AreEqual(1, project.GetPgms().Count());
            Pgm retrieved = project.GetPgmById(original.PgmId);
            Assert.AreEqual(original, retrieved);
            retrieved.Content = "";
            project.UpdatePgm(retrieved);
            retrieved = project.GetPgmById(original.PgmId);
            Assert.AreNotEqual(original, retrieved);
            Assert.AreEqual("", retrieved.Content);
            project.DeletePgm(retrieved);
            Assert.AreEqual(0, project.GetPgms().Count());
        }

        [Test]
        public void CanvasTest()
        {
            View view = project.CreateView("CanvasTest_Canvas");
            Canvas original = new Canvas
            {
                Name = "CanvasTest_Canvas",
                Content = Canvas.GetContentForView(view)
            };
            Assert.AreEqual(0, original.CanvasId);
            Assert.AreEqual(0, project.GetCanvases().Count());
            project.InsertCanvas(original);
            Assert.AreEqual(1, original.CanvasId);
            Assert.AreEqual(1, project.GetCanvases().Count());
            Canvas retrieved = project.GetCanvasById(original.CanvasId);
            Assert.AreEqual(original, retrieved);
            retrieved.Content = "";
            project.UpdateCanvas(retrieved);
            retrieved = project.GetCanvasById(original.CanvasId);
            Assert.AreNotEqual(original, retrieved);
            Assert.AreEqual("", retrieved.Content);
            project.DeleteCanvas(retrieved);
            Assert.AreEqual(0, project.GetCanvases().Count());
        }
    }

    public class AccessProjectTest : ProjectTestBase
    {
        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            string name = "AccessTest";
            DirectoryInfo location = new DirectoryInfo(configuration.Directories.Project).CreateSubdirectory(name);
            FileInfo database = location.GetFile(string.Format("{0}.mdb", name));
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.EpiInfo.Empty.mdb", database);
            creationInfo = new ProjectCreationInfo
            {
                Name = name,
                Location = location,
                Driver = Configuration.AccessDriver,
                Builder = new OleDbConnectionStringBuilder
                {
                    Provider = "Microsoft.Jet.OLEDB.4.0",
                    DataSource = database.FullName
                },
                DatabaseName = name,
                Initialize = true
            };
            project = Project.Create(creationInfo);
        }
    }

    public class SqlServerProjectTest : ProjectTestBase
    {
        private SqlConnectionStringBuilder builder;
        private bool created;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            string name = "SqlServerTest";
            string connectionString = ConfigurationManager.ConnectionStrings["ERHMS_Test"].ConnectionString;
            builder = new SqlConnectionStringBuilder(connectionString)
            {
                Pooling = false
            };
            ExecuteMaster("CREATE DATABASE {0}", builder.InitialCatalog);
            created = true;
            creationInfo = new ProjectCreationInfo
            {
                Name = name,
                Location = new DirectoryInfo(configuration.Directories.Project).GetSubdirectory(name),
                Driver = Configuration.SqlDriver,
                Builder = builder,
                DatabaseName = builder.InitialCatalog,
                Initialize = true
            };
            project = Project.Create(creationInfo);
        }

        [OneTimeTearDown]
        public new void OneTimeTearDown()
        {
            if (created)
            {
                ExecuteMaster("DROP DATABASE {0}", builder.InitialCatalog);
            }
            else
            {
                TestContext.Error.WriteLine("Database '{0}' must be manually dropped.", builder.InitialCatalog);
            }
        }

        private int ExecuteMaster(string sql, params string[] identifiers)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ERHMS_Test"].ConnectionString;
            SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString)
            {
                InitialCatalog = "master"
            };
            SqlCommandBuilder commandBuilder = new SqlCommandBuilder();
            string[] quotedIdentifiers = identifiers.Select(identifier => commandBuilder.QuoteIdentifier(identifier)).ToArray();
            using (SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString))
            using (SqlCommand command = new SqlCommand(string.Format(sql, quotedIdentifiers), connection))
            {
                connection.Open();
                return command.ExecuteNonQuery();
            }
        }
    }
}
