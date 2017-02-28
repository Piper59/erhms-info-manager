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
    public partial class MakeViewTest : WrapperTestBase
    {
        [Test]
        public void OpenViewTest()
        {
            wrapper = MakeView.OpenView.Create(project.FilePath, "ADDFull");
            wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            AutomationElement selectedPage = mainForm.projectTree.Selection.Get().Single();
            Assert.AreEqual("PAGE", selectedPage.Current.Name);
            AutomationElement selectedView = selectedPage.GetParent();
            Assert.AreEqual("ADDFull", selectedView.Current.Name);
            AutomationElement selectedProject = selectedView.GetParent();
            Assert.AreEqual("Sample", selectedProject.Current.Name);
            mainForm.Window.Close();
        }

        [Test]
        public void InstantiateProjectTemplateTest()
        {
            string templatePath = TemplateInfo.GetPath(TemplateLevel.Project, "ADDFull");
            try
            {
                Project project = AccessProjectTest.Create("ADDFull");
                Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.ADDFull_Project.xml", templatePath);
                wrapper = MakeView.InstantiateProjectTemplate.Create(project.FilePath, templatePath);
                wrapper.Invoke();
                wrapper.Exited.WaitOne();
                Assert.AreEqual(1, project.Views.Count);
                Assert.IsTrue(project.Views.Contains("ADDFull"));
            }
            finally
            {
                File.Delete(templatePath);
            }
        }

        [Test]
        public void InstantiateViewTemplateTest()
        {
            string templatePath = TemplateInfo.GetPath(TemplateLevel.View, "ADDFull");
            try
            {
                Assembly.GetExecutingAssembly().CopyManifestResourceTo("ERHMS.Test.Resources.ADDFull_View.xml", templatePath);
                wrapper = MakeView.InstantiateViewTemplate.Create(project.FilePath, templatePath, "");
                WrapperEventCollection events = new WrapperEventCollection(wrapper);
                wrapper.Invoke();
                MainFormScreen mainForm = new MainFormScreen();
                CreateViewDialogScreen createViewDialog = mainForm.GetCreateViewDialogScreen();
                Assert.AreEqual("ADDFull_2", createViewDialog.txtViewName.Value.Current.Value);
                createViewDialog.btnOk.Invoke.Invoke();
                mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);
                Assert.IsTrue(project.Views.Contains("ADDFull_2"));
                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(WrapperEventType.ViewCreated, events[0].Type);
            }
            finally
            {
                File.Delete(templatePath);
            }
        }

        private void TemplateTest(XmlDocument document)
        {
            ICollection<string> fieldNames = new string[] { "AddFull", "ThisViewIs", "GENDER", "REPEAT", "ENGL", "ENGG", "OLMAT", "KF", "GPA", "SOCPROB", "SCORE2", "SCORE4", "SCORE5", "DROPOUT", "ADDSC", "IQ" };
            foreach (XmlElement fieldElement in document.SelectElements("/Template/Project/View/Page/Field"))
            {
                CollectionAssert.Contains(fieldNames, fieldElement.GetAttribute("Name"));
            }
        }

        [Test]
        public void CreateTemplateTest()
        {
            string templatePath = TemplateInfo.GetPath(TemplateLevel.View, "ADDFull");
            try
            {
                wrapper = MakeView.CreateTemplate.Create(project.FilePath, "ADDFull");
                WrapperEventCollection events = new WrapperEventCollection(wrapper);
                wrapper.Invoke();
                MainFormScreen mainForm = new MainFormScreen();
                CreateTemplateDialogScreen createTemplateDialog = mainForm.GetCreateTemplateDialogScreen();
                Assert.AreEqual("ADDFull", createTemplateDialog.txtTemplateName.Value.Current.Value);
                string description = "Description for ADDFull template";
                createTemplateDialog.txtDescription.Text.Set(description);
                createTemplateDialog.btnOk.Invoke.Invoke();
                mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);
                XmlDocument document = new XmlDocument();
                document.Load(templatePath);
                XmlElement templateElement = document.DocumentElement;
                Assert.AreEqual("ADDFull", templateElement.GetAttribute("Name"));
                Assert.AreEqual(description, templateElement.GetAttribute("Description"));
                TemplateTest(document);
                Assert.AreEqual(1, events.Count);
                Assert.AreEqual(WrapperEventType.TemplateCreated, events[0].Type);
            }
            finally
            {
                File.Delete(templatePath);
            }
        }

        [Test]
        public void CreateWebTemplateTest()
        {
            wrapper = MakeView.CreateWebTemplate.Create(project.FilePath, "ADDFull");
            wrapper.Invoke();
            XmlDocument document = new XmlDocument();
            document.LoadXml(wrapper.ReadToEnd());
            TemplateTest(document);
            wrapper.Exited.WaitOne();
        }

        [Test]
        public void PublishToMobileTest()
        {
            wrapper = MakeView.PublishToMobile.Create(project.FilePath, "ADDFull");
            wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            CopyToAndroidScreen copyToAndroid = mainForm.GetCopyToAndroidScreen();
            Assert.AreEqual("ADDFull", copyToAndroid.txtFormName.Value.Current.Value);
            // TODO: Test copying to Android device
            copyToAndroid.btnCancel.Invoke.Invoke();
        }
    }
}
