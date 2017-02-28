using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
using NUnit.Framework;
using System.Reflection;
using System.Windows.Forms;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public partial class AnalysisDashboardTest : WrapperTestBase
    {
        private Canvas canvas;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            project.Metadata.CreateCanvasesTable();
            canvas = new Canvas
            {
                Name = "SampleSurveillance",
                Content = Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.SampleSurveillance.cvs7")
            };
            canvas.SetProjectPath(project.FilePath);
            project.InsertCanvas(canvas);
        }

        [Test]
        public void OpenCanvasTest()
        {
            wrapper = AnalysisDashboard.OpenCanvas.Create(project.FilePath, canvas.CanvasId, canvas.Content);
            WrapperEventCollection events = new WrapperEventCollection(wrapper);
            wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            string message = "Hello, world!";
            mainForm.standardTextBox.Value.SetValue(message);
            mainForm.scrollViewer.Element.SetFocus();
            SendKeys.SendWait("+{F10}{UP 2}{ENTER}");
            mainForm.WaitForReady();
            mainForm.Window.Close();
            mainForm.GetQuestionDialogScreen().Dialog.Close(DialogResult.Yes);
            wrapper.Exited.WaitOne();
            CollectionAssert.IsNotEmpty(events);
            WrapperEventArgs e = events[events.Count - 1];
            Assert.AreEqual(WrapperEventType.CanvasSaved, e.Type);
            StringAssert.Contains(message, e.Properties.Content);
        }
    }
}
