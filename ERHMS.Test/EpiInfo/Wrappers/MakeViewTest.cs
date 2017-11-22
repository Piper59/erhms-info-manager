using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Xml;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public abstract partial class MakeViewTest : WrapperTest
    {
        protected abstract IProjectCreator GetProjectCreator(string name);

        [Test]
        public void OpenViewTest()
        {
            // Invoke wrapper
            Wrapper = MakeView.OpenView.Create(Project.FilePath, "ADDFull");
            Wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();

            // Verify current page, view, and project
            AutomationElement selectedPage = mainForm.projectTree.Selection.Get().Single();
            Assert.AreEqual("PAGE", selectedPage.Current.Name);
            AutomationElement selectedView = selectedPage.GetParent();
            Assert.AreEqual("ADDFull", selectedView.Current.Name);
            AutomationElement selectedProject = selectedView.GetParent();
            Assert.AreEqual("Sample", selectedProject.Current.Name);

            // Close window
            mainForm.Window.Close();
        }

        [Test]
        public void InstantiateProjectTemplateTest()
        {
            string templatePath = TemplateInfo.GetPath(TemplateLevel.Project, "ADDFull");
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Sample.ADDFull.Project.xml", templatePath);
            IProjectCreator creator = GetProjectCreator("ADDFull");
            creator.SetUp();
            try
            {
                Wrapper = MakeView.InstantiateProjectTemplate.Create(creator.Project.FilePath, templatePath);
                Wrapper.Invoke();
                Wrapper.Exited.WaitOne();
                Assert.AreEqual(1, creator.Project.Views.Count);
                Assert.IsTrue(creator.Project.Views.Contains("ADDFull"));
            }
            finally
            {
                creator.TearDown();
                File.Delete(templatePath);
            }
        }

        [Test]
        public void InstantiateViewTemplateTest()
        {
            string templatePath = TemplateInfo.GetPath(TemplateLevel.View, "ADDFull");
            Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.Sample.ADDFull.View.xml", templatePath);
            try
            {
                // Invoke wrapper
                Wrapper = MakeView.InstantiateViewTemplate.Create(Project.FilePath, templatePath, "");
                WrapperEventCollection events = new WrapperEventCollection(Wrapper);
                Wrapper.Invoke();
                MainFormScreen mainForm = new MainFormScreen();

                // Create view
                CreateViewDialogScreen createViewDialog = mainForm.GetCreateViewDialogScreen();
                Assert.AreEqual("ADDFull_2", createViewDialog.txtViewName.Value.Current.Value);
                createViewDialog.btnOk.Invoke.Invoke();

                // Close window
                mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);

                Wrapper.Exited.WaitOne();
                Assert.IsTrue(Project.Views.Contains("ADDFull_2"));
                Assert.AreEqual(1, events.Count);
                Assert.AreEqual("ViewCreated", events[0].Type);
                Assert.AreEqual(events[0].Properties.ViewId, Project.Views["ADDFull_2"].Id);
            }
            finally
            {
                File.Delete(templatePath);
            }
        }

        [Test]
        public void CreateTemplateTest()
        {
            string templatePath = TemplateInfo.GetPath(TemplateLevel.View, "ADDFull");
            try
            {
                // Invoke wrapper
                Wrapper = MakeView.CreateTemplate.Create(Project.FilePath, "ADDFull");
                WrapperEventCollection events = new WrapperEventCollection(Wrapper);
                Wrapper.Invoke();
                MainFormScreen mainForm = new MainFormScreen();

                // Create template
                CreateTemplateDialogScreen createTemplateDialog = mainForm.GetCreateTemplateDialogScreen();
                Assert.AreEqual("ADDFull", createTemplateDialog.txtTemplateName.Value.Current.Value);
                string description = "Description for ADDFull template";
                createTemplateDialog.txtDescription.Text.Set(description);
                createTemplateDialog.btnOk.Invoke.Invoke();

                // Close window
                mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);

                XmlDocument document = new XmlDocument();
                document.Load(templatePath);
                XmlElement templateElement = document.DocumentElement;
                Assert.AreEqual("ADDFull", templateElement.GetAttribute("Name"));
                Assert.AreEqual(description, templateElement.GetAttribute("Description"));
                TemplateTest(document);
                Assert.AreEqual(1, events.Count);
                Assert.AreEqual("TemplateCreated", events[0].Type);
                Assert.AreEqual(templatePath, events[0].Properties.Path);
            }
            finally
            {
                File.Delete(templatePath);
            }
        }

        [Test]
        public void CreateWebTemplateTest()
        {
            Wrapper = MakeView.CreateWebTemplate.Create(Project.FilePath, "ADDFull");
            Wrapper.Invoke();
            XmlDocument document = new XmlDocument();
            document.LoadXml(Wrapper.ReadToEnd());
            TemplateTest(document);
            Wrapper.Exited.WaitOne();
        }

        private void TemplateTest(XmlDocument document)
        {
            ICollection<string> fieldNames = new string[]
            {
                "AddFull",
                "ThisViewIs",
                "GENDER",
                "REPEAT",
                "ENGL",
                "ENGG",
                "OLMAT",
                "KF",
                "GPA",
                "SOCPROB",
                "SCORE2",
                "SCORE4",
                "SCORE5",
                "DROPOUT",
                "ADDSC",
                "IQ"
            };
            foreach (XmlElement element in document.SelectElements("/Template/Project/View/Page/Field"))
            {
                CollectionAssert.Contains(fieldNames, element.GetAttribute("Name"));
            }
        }
    }

    public class AccessMakeViewTest : MakeViewTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new AccessSampleProjectCreator();
        }

        protected override IProjectCreator GetProjectCreator(string name)
        {
            return new AccessProjectCreator(name);
        }
    }

    public class SqlServerMakeViewTest : MakeViewTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new SqlServerSampleProjectCreator();
        }

        protected override IProjectCreator GetProjectCreator(string name)
        {
            return new SqlServerProjectCreator(name);
        }
    }
}
