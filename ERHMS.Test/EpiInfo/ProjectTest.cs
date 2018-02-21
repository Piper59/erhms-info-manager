using Epi;
using Epi.Fields;
using ERHMS.Dapper;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Project = ERHMS.EpiInfo.Project;

namespace ERHMS.Test.EpiInfo
{
    public abstract class ProjectTest
    {
        protected TempDirectory directory;
        protected Configuration configuration;
        protected IProjectCreator creator;
        protected Project project;

        protected abstract IProjectCreator GetCreator();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(GetType().Name);
            ConfigurationExtensions.Create(directory.FullName).Save();
            configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            creator = GetCreator();
            creator.SetUp();
            project = creator.Project;
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            creator.TearDown();
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        [Test]
        public void CreateTest()
        {
            FileAssert.Exists(project.FilePath);
            XmlDocument document = new XmlDocument();
            document.Load(project.FilePath);
            XmlElement projectElement = document.DocumentElement;
            Assert.AreEqual("Project", projectElement.Name);
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Assert.AreEqual(version.ToString(), projectElement.GetAttribute("erhmsVersion"));
            Assert.AreEqual(version, project.Version);
            Assert.AreEqual(creator.Info.Name, projectElement.GetAttribute("name"));
            Assert.AreEqual(creator.Info.Location, projectElement.GetAttribute("location"));
            XmlElement databaseElement = projectElement.SelectSingleElement("CollectedData/Database");
            Assert.AreEqual(creator.Info.Builder.ConnectionString, Configuration.Decrypt(databaseElement.GetAttribute("connectionString")));
            Assert.IsTrue(project.Driver.TableExists("metaCanvases"));
        }

        [Test]
        public void GetDatabaseTest()
        {
            IDatabase database = project.GetDatabase();
            Assert.IsTrue(database.Exists());
            Assert.IsTrue(database.TableExists("metaDbInfo"));
        }

        [Test]
        public void IsValidViewNameTest()
        {
            project.CreateView("IsValidViewNameTest_View");
            IsValidViewNameTest(InvalidViewNameReason.Empty, null);
            IsValidViewNameTest(InvalidViewNameReason.Empty, "");
            IsValidViewNameTest(InvalidViewNameReason.Empty, " ");
            IsValidViewNameTest(InvalidViewNameReason.InvalidChar, "IsValidViewNameTest-View");
            IsValidViewNameTest(InvalidViewNameReason.InvalidBeginning, "_IsValidViewNameTest_View");
            IsValidViewNameTest(InvalidViewNameReason.TooLong, new string('A', 65));
            IsValidViewNameTest(InvalidViewNameReason.ViewExists, "ISVALIDVIEWNAMETEST_VIEW");
            IsValidViewNameTest(InvalidViewNameReason.TableExists, "METADBINFO");
            IsValidViewNameTest(InvalidViewNameReason.None, "IsValidViewNameTest_View_2");
        }

        private void IsValidViewNameTest(InvalidViewNameReason expected, string viewName)
        {
            InvalidViewNameReason actual;
            Assert.AreEqual(expected == InvalidViewNameReason.None, project.IsValidViewName(viewName, out actual));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SuggestViewNameTest()
        {
            project.CreateView("SuggestViewNameTest_ViewA");
            Assert.AreEqual("SUGGESTVIEWNAMETEST_ViewA_2", project.SuggestViewName("1234_SUGGESTVIEWNAMETEST_!@#$%^&*()ViewA"));
            Assert.AreEqual("metaDbInfo_2", project.SuggestViewName("metaDbInfo"));
            Assert.AreEqual("SuggestViewNameTest_ViewB", project.SuggestViewName("SuggestViewNameTest_ViewB"));
        }

        [Test]
        public void GetFieldsAsDataTableTest()
        {
            View view = project.CreateView("GetFieldsAsDataTableTest_View");
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
            IEnumerable<string> actual = project.GetFieldsAsDataTable().AsEnumerable()
                .Where(row => row.Field<int>("ViewId") == view.Id)
                .Select(row => row.Field<string>("Name"));
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [Test]
        public void GetSortedFieldIdsTest()
        {
            Random random = new Random();
            View view = project.CreateView("GetSortedFieldIdsTest_View");
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
            CollectionAssert.AreEqual(viewFields.Concat(pageFields).Select(field => field.Id), project.GetSortedFieldIds(view.Id));
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
            project.CreateCodeTable(tableName, columnName);
            project.SaveCodeTableData(genders, tableName, columnName);
            AssertExtensions.AreEqual(project.GetCodes(tableName, columnName, false), "Male", "Female");
            AssertExtensions.AreEqual(project.GetCodes(tableName, columnName, true), "Female", "Male");
        }

        [Test]
        public void GetViewsTest()
        {
            int count = project.GetViews().Count();
            string name = "GetViewsTest_View";
            View view = project.CreateView(name);
            Assert.AreEqual(++count, project.GetViews().Count());
            Assert.AreEqual(view.Id, project.GetViewByName(name).Id);
            Assert.AreEqual(name, project.GetViewById(view.Id).Name);
        }

        [Test]
        public void EnsureDataTablesExistTest()
        {
            View view = project.CreateView("EnsureDataTablesExistTest_View");
            ICollection<Page> pages = new List<Page>();
            for (int index = 0; index < 3; index++)
            {
                pages.Add(view.CreatePage("Page" + index, index));
            }
            Assert.IsFalse(project.Driver.TableExists(view.TableName));
            foreach (Page page in pages)
            {
                Assert.IsFalse(project.Driver.TableExists(page.TableName));
            }
            project.CollectedData.EnsureDataTablesExist(view.Id);
            Assert.IsTrue(project.Driver.TableExists(view.TableName));
            foreach (Page page in pages)
            {
                Assert.IsTrue(project.Driver.TableExists(page.TableName));
            }
            Assert.DoesNotThrow(() =>
            {
                project.CollectedData.EnsureDataTablesExist(view.Id);
            });
        }

        [Test]
        public void DeleteViewTest()
        {
            View parentView = project.CreateView("DeleteViewTest_ParentView");
            Page parentPage = parentView.CreatePage("New Page", 0);
            View view = project.CreateView("DeleteViewTest_View", true);
            Page page = view.CreatePage("New Page", 0);
            View childView = project.CreateView("DeleteViewTest_ChildView");
            Page childPage = childView.CreatePage("New Page", 0);
            CreateRelateField(parentPage, view);
            CreateRelateField(page, childView);
            project.CollectedData.CreateDataTableForView(parentView, 1);
            project.CollectedData.CreateDataTableForView(view, 1);
            project.CollectedData.CreateDataTableForView(childView, 1);
            Assert.IsTrue(parentView.Fields.Contains("Relate"));
            project.DeleteView(view.Id);
            Assert.IsTrue(project.Views.Contains(parentView.Name));
            Assert.IsFalse(project.Views.Contains(view.Name));
            Assert.IsFalse(project.Views.Contains(childView.Name));
            parentView.MustRefreshFieldCollection = true;
            Assert.IsFalse(parentView.Fields.Contains("Relate"));
            project.DeleteView(parentView.Id);
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
        public void SanitizeNameTest()
        {
            Assert.AreEqual("ValidName_View", "ValidName_View");
            Assert.AreEqual("InvalidName_View", ViewExtensions.SanitizeName("1234_InvalidName_!@#$%^&*()View"));
        }

        [Test]
        public void WebSurveyTest()
        {
            View view = project.CreateView("WebSurveyTest_View");
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
            View view = project.CreateView("PgmTest_View");
            Pgm original = new Pgm
            {
                Name = "PgmTest_Pgm",
                Content = Pgm.GetContentForView(project.FilePath, view.Name)
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
            Assert.AreEqual(original, retrieved);
            Assert.AreEqual("", retrieved.Content);
            project.DeletePgm(retrieved.PgmId);
            Assert.AreEqual(0, project.GetPgms().Count());
        }

        [Test]
        public void CanvasTest()
        {
            View view = project.CreateView("CanvasTest_Canvas");
            Canvas original = new Canvas
            {
                Name = "CanvasTest_Canvas",
                Content = Canvas.GetContentForView(project.FilePath, view.Name)
            };
            Assert.AreEqual(0, original.CanvasId);
            Assert.AreEqual(0, project.GetCanvases().Count());
            project.InsertCanvas(original);
            Assert.AreEqual(1, original.CanvasId);
            Assert.AreEqual(1, project.GetCanvases().Count());
            Canvas retrieved = project.GetCanvasById(original.CanvasId);
            Assert.AreEqual(original, retrieved);
            string path = Path.Combine(Path.GetTempPath(), Path.GetFileName(project.FilePath));
            retrieved.SetProjectPath(path);
            project.UpdateCanvas(retrieved);
            retrieved = project.GetCanvasById(original.CanvasId);
            Assert.AreEqual(original, retrieved);
            Regex whitespacePattern = new Regex(@"\s+");
            string expected = whitespacePattern.Replace(original.Content, "").Replace(project.FilePath, path);
            string actual = whitespacePattern.Replace(retrieved.Content, "");
            Assert.AreEqual(expected, actual);
            project.DeleteCanvas(retrieved.CanvasId);
            Assert.AreEqual(0, project.GetCanvases().Count());
        }
    }

    public class AccessProjectTest : ProjectTest
    {
        protected override IProjectCreator GetCreator()
        {
            return new AccessProjectCreator(nameof(AccessProjectTest));
        }

        [Test]
        [Explicit]
        public void ReviewTemplatesTest()
        {
            IOExtensions.CopyDirectory(
                Path.Combine(AssemblyExtensions.GetEntryDirectoryPath(), "Templates"),
                configuration.Directories.Templates);
            foreach (TemplateInfo templateInfo in TemplateInfo.GetByLevel(TemplateLevel.View))
            {
                TestContext.Error.WriteLine(templateInfo.Name);
                Wrapper wrapper = MakeView.InstantiateTemplate.Create(project.FilePath, templateInfo.FilePath);
                int? viewId = null;
                wrapper.Event += (sender, e) =>
                {
                    viewId = e.Properties.ViewId;
                };
                wrapper.Invoke();
                wrapper.Exited.WaitOne();
                Assert.IsTrue(viewId.HasValue);
                project.CollectedData.EnsureDataTablesExist(viewId.Value);
            }
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), project.Name);
            IOExtensions.CopyDirectory(project.Location, path);
            Process.Start(path);
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
