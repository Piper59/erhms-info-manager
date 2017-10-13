using Epi;
using Epi.Fields;
using ERHMS.Dapper;
using ERHMS.EpiInfo;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Configuration = Epi.Configuration;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Test.EpiInfo
{
#if IGNORE_LONG_TESTS
    [TestFixture(Ignore = "IGNORE_LONG_TESTS")]
#endif
    public abstract class ProjectTest
    {
        private Configuration configuration;
        private IProjectCreator creator;

        private Project Project
        {
            get { return creator.Project; }
        }

        protected abstract IProjectCreator GetCreator();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            ConfigurationExtensions.Create(AssemblyExtensions.GetEntryDirectoryPath()).Save();
            configuration = ConfigurationExtensions.Load();
            creator = GetCreator();
            creator.SetUp();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
            File.Delete(ConfigurationExtensions.FilePath);
        }

        [Test]
        public void CreateTest()
        {
            FileAssert.Exists(Project.FilePath);
            XmlDocument document = new XmlDocument();
            document.Load(Project.FilePath);
            XmlElement projectElement = document.DocumentElement;
            Assert.AreEqual("Project", projectElement.Name);
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Assert.AreEqual(version.ToString(), projectElement.GetAttribute("erhmsVersion"));
            Assert.AreEqual(version, Project.Version);
            Assert.AreEqual(creator.Info.Name, projectElement.GetAttribute("name"));
            Assert.AreEqual(creator.Info.Location, projectElement.GetAttribute("location"));
            XmlElement databaseElement = projectElement.SelectSingleElement("CollectedData/Database");
            Assert.AreEqual(creator.Info.Builder.ConnectionString, Configuration.Decrypt(databaseElement.GetAttribute("connectionString")));
            Assert.IsTrue(Project.Driver.TableExists("metaCanvases"));
        }

        [Test]
        public void GetDatabaseTest()
        {
            IDatabase database = Project.GetDatabase();
            Assert.IsTrue(database.Exists());
            Assert.IsTrue(database.TableExists("metaDbInfo"));
        }

        [Test]
        public void IsValidViewNameTest()
        {
            Project.CreateView("IsValidViewNameTest_View");
            IsValidViewNameTest(null, InvalidViewNameReason.Empty);
            IsValidViewNameTest("", InvalidViewNameReason.Empty);
            IsValidViewNameTest(" ", InvalidViewNameReason.Empty);
            IsValidViewNameTest("IsValidViewNameTest-View", InvalidViewNameReason.InvalidChar);
            IsValidViewNameTest("_IsValidViewNameTest_View", InvalidViewNameReason.InvalidBeginning);
            IsValidViewNameTest(new string('A', 65), InvalidViewNameReason.TooLong);
            IsValidViewNameTest("ISVALIDVIEWNAMETEST_VIEW", InvalidViewNameReason.ViewExists);
            IsValidViewNameTest("METADBINFO", InvalidViewNameReason.TableExists);
            IsValidViewNameTest("IsValidViewNameTest_View_2", InvalidViewNameReason.None);
        }

        private void IsValidViewNameTest(string viewName, InvalidViewNameReason expected)
        {
            InvalidViewNameReason actual;
            Assert.AreEqual(expected == InvalidViewNameReason.None, Project.IsValidViewName(viewName, out actual));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SuggestViewNameTest()
        {
            Project.CreateView("SuggestViewNameTest_ViewA");
            Assert.AreEqual("SUGGESTVIEWNAMETEST_ViewA_2", Project.SuggestViewName("1234_SUGGESTVIEWNAMETEST_!@#$%^&*()ViewA"));
            Assert.AreEqual("metaDbInfo_2", Project.SuggestViewName("metaDbInfo"));
            Assert.AreEqual("SuggestViewNameTest_ViewB", Project.SuggestViewName("SuggestViewNameTest_ViewB"));
        }

        [Test]
        public void GetFieldsAsDataTableTest()
        {
            View view = Project.CreateView("GetFieldsAsDataTableTest_View");
            ICollection<string> expected = new List<string>
            {
                ColumnNames.UNIQUE_KEY,
                ColumnNames.REC_STATUS,
                ColumnNames.GLOBAL_RECORD_ID
            };
            Page page = view.CreatePage("New Page", 0);
            for (int index = 0; index < 10; index++)
            {
                Field field = page.CreateField(MetaFieldType.Text);
                field.Name = "Text" + index;
                field.SaveToDb();
                expected.Add(field.Name);
            }
            IEnumerable<string> actual = Project.GetFieldsAsDataTable().AsEnumerable()
                .Where(row => row.Field<int>("ViewId") == view.Id)
                .Select(row => row.Field<string>("Name"));
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetSortedFieldIdsTest()
        {
            Random random = new Random();
            View view = Project.CreateView("GetSortedFieldIdsTest_View");
            ICollection<Field> viewFields = new Field[]
            {
                view.UniqueKeyField,
                view.RecStatusField,
                view.GlobalRecordIdField
            };
            ICollection<RenderableField> pageFields = new List<RenderableField>();
            for (int pageIndex = 0; pageIndex < 3; pageIndex++)
            {
                Page page = view.CreatePage("Page" + pageIndex, random.Next(1000));
                for (int fieldIndex = 0; fieldIndex < 10; fieldIndex++)
                {
                    RenderableField field = (RenderableField)page.CreateField(MetaFieldType.Text);
                    field.Name = "Text" + fieldIndex;
                    field.TabIndex = random.Next(1000);
                    field.SaveToDb();
                    pageFields.Add(field);
                }
            }
            pageFields = pageFields.OrderBy(field => field.Page.Position)
                .ThenBy(field => field.TabIndex)
                .ThenBy(field => field.Id)
                .ToList();
            CollectionAssert.AreEqual(viewFields.Concat(pageFields).Select(field => field.Id), Project.GetSortedFieldIds(view.Id));
        }

        [Test]
        public void GetCodesTest()
        {
            string tableName = "codegender1";
            string columnName = "gender";
            DataTable genders = new DataTable(tableName);
            genders.Columns.Add(columnName);
            genders.Rows.Add("Male");
            genders.Rows.Add("Female");
            Project.CreateCodeTable(tableName, columnName);
            Project.SaveCodeTableData(genders, tableName, columnName);
            ICollection<string> expected;
            expected = new string[]
            {
                "Male",
                "Female"
            };
            CollectionAssert.AreEqual(expected, Project.GetCodes(tableName, columnName, false));
            expected = new string[]
            {
                "Female",
                "Male"
            };
            CollectionAssert.AreEqual(expected, Project.GetCodes(tableName, columnName, true));
        }

        [Test]
        public void GetViewsTest()
        {
            int count = Project.GetViews().Count();
            string name = "GetViewsTest_View";
            View view = Project.CreateView(name);
            count++;
            Assert.AreEqual(count, Project.GetViews().Count());
            Assert.AreEqual(view.Id, Project.GetViewByName(name).Id);
            Assert.AreEqual(name, Project.GetViewById(view.Id).Name);
        }

        [Test]
        public void EnsureDataTablesExistTest()
        {
            View view = Project.CreateView("EnsureDataTablesExistTest_View");
            ICollection<Page> pages = new List<Page>();
            for (int index = 0; index < 3; index++)
            {
                pages.Add(view.CreatePage("Page" + index, index));
            }
            Assert.IsFalse(Project.Driver.TableExists(view.TableName));
            foreach (Page page in pages)
            {
                Assert.IsFalse(Project.Driver.TableExists(page.TableName));
            }
            Project.CollectedData.EnsureDataTablesExist(view.Id);
            Assert.IsTrue(Project.Driver.TableExists(view.TableName));
            foreach (Page page in pages)
            {
                Assert.IsTrue(Project.Driver.TableExists(page.TableName));
            }
            Assert.DoesNotThrow(() =>
            {
                Project.CollectedData.EnsureDataTablesExist(view.Id);
            });
        }

        [Test]
        public void DeleteViewTest()
        {
            View parentView = Project.CreateView("DeleteViewTest_ParentView");
            Page parentPage = parentView.CreatePage("New Page", 0);
            View view = Project.CreateView("DeleteViewTest_View", true);
            Page page = view.CreatePage("New Page", 0);
            View childView = Project.CreateView("DeleteViewTest_ChildView");
            Page childPage = childView.CreatePage("New Page", 0);
            CreateRelateField(parentPage, view);
            CreateRelateField(page, childView);
            Project.CollectedData.CreateDataTableForView(parentView, 1);
            Project.CollectedData.CreateDataTableForView(view, 1);
            Project.CollectedData.CreateDataTableForView(childView, 1);
            Assert.IsTrue(parentView.Fields.Contains("Relate"));
            Project.DeleteView(view.Id);
            Assert.IsTrue(Project.Views.Contains(parentView.Name));
            Assert.IsFalse(Project.Views.Contains(view.Name));
            Assert.IsFalse(Project.Views.Contains(childView.Name));
            parentView.MustRefreshFieldCollection = true;
            Assert.IsFalse(parentView.Fields.Contains("Relate"));
            Project.DeleteView(parentView.Id);
            Assert.IsFalse(Project.Views.Contains(parentView.Name));
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
                Assert.IsTrue(Project.Driver.TableExists(tableName), tableName);
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
        public void SanitizeNameTest()
        {
            Assert.AreEqual("ValidName_View", "ValidName_View");
            Assert.AreEqual("InvalidName_View", ViewExtensions.SanitizeName("1234_InvalidName_!@#$%^&*()View"));
        }

        [Test]
        public void WebSurveyTest()
        {
            View view = Project.CreateView("WebSurveyTest_View");
            Assert.IsFalse(view.IsWebSurvey());
            view.WebSurveyId = Guid.Empty.ToString();
            Assert.IsTrue(view.IsWebSurvey());
            configuration.Settings.WebServiceEndpointAddress = "http://example.com/EIWS/SurveyManagerServiceV2.svc";
            configuration.Save();
            Assert.AreEqual("http://example.com/EIWS/Home/00000000-0000-0000-0000-000000000000", view.GetWebSurveyUrl().ToString());
        }

        [Test]
        public void PgmTest()
        {
            View view = Project.CreateView("PgmTest_View");
            Pgm original = new Pgm
            {
                Name = "PgmTest_Pgm",
                Content = Pgm.GetContentForView(Project.FilePath, view.Name)
            };
            Assert.AreEqual(0, original.PgmId);
            Assert.AreEqual(0, Project.GetPgms().Count());
            Project.InsertPgm(original);
            Assert.AreEqual(1, original.PgmId);
            Assert.AreEqual(1, Project.GetPgms().Count());
            Pgm retrieved = Project.GetPgmById(original.PgmId);
            Assert.AreEqual(original, retrieved);
            retrieved.Content = "";
            Project.UpdatePgm(retrieved);
            retrieved = Project.GetPgmById(original.PgmId);
            Assert.AreEqual(original, retrieved);
            Assert.AreEqual("", retrieved.Content);
            Project.DeletePgm(retrieved.PgmId);
            Assert.AreEqual(0, Project.GetPgms().Count());
        }

        [Test]
        public void CanvasTest()
        {
            View view = Project.CreateView("CanvasTest_Canvas");
            Canvas original = new Canvas
            {
                Name = "CanvasTest_Canvas",
                Content = Canvas.GetContentForView(Project.FilePath, view.Name)
            };
            Assert.AreEqual(0, original.CanvasId);
            Assert.AreEqual(0, Project.GetCanvases().Count());
            Project.InsertCanvas(original);
            Assert.AreEqual(1, original.CanvasId);
            Assert.AreEqual(1, Project.GetCanvases().Count());
            Canvas retrieved = Project.GetCanvasById(original.CanvasId);
            Assert.AreEqual(original, retrieved);
            string path = Path.Combine(Path.GetTempPath(), Path.GetFileName(Project.FilePath));
            retrieved.SetProjectPath(path);
            Project.UpdateCanvas(retrieved);
            retrieved = Project.GetCanvasById(original.CanvasId);
            Assert.AreEqual(original, retrieved);
            Regex whitespacePattern = new Regex(@"\s+");
            string expected = whitespacePattern.Replace(original.Content, "").Replace(Project.FilePath, path);
            string actual = whitespacePattern.Replace(retrieved.Content, "");
            Assert.AreEqual(expected, actual);
            Project.DeleteCanvas(retrieved.CanvasId);
            Assert.AreEqual(0, Project.GetCanvases().Count());
        }
    }

    public class AccessProjectTest : ProjectTest
    {
        protected override IProjectCreator GetCreator()
        {
            return new AccessProjectCreator(nameof(AccessProjectTest));
        }
    }

    public class SqlServerProjectTest : ProjectTest
    {
        protected override IProjectCreator GetCreator()
        {
            return new SqlServerProjectCreator(nameof(SqlServerProjectTest));
        }
    }
}
