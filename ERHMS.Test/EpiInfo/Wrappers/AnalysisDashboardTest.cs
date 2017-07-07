using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
using NUnit.Framework;
using System.Reflection;
using System.Windows.Forms;

namespace ERHMS.Test.EpiInfo.Wrappers
{
#if IGNORE_LONG_TESTS
    [TestFixture(Ignore = "IGNORE_LONG_TESTS")]
#endif
    public partial class AnalysisDashboardTest : WrapperTestBase
    {
        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            project.Metadata.CreateCanvasesTable();
        }

        [Test]
        public void OpenCanvasTest()
        {
            // Create canvas
            Canvas canvas = new Canvas
            {
                Name = "SampleSurveillance",
                Content = Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Sample.SampleSurveillance.cvs7")
            };
            canvas.SetProjectPath(project.FilePath);
            project.InsertCanvas(canvas);

            // Invoke wrapper
            wrapper = AnalysisDashboard.OpenCanvas.Create(project.FilePath, canvas.Content);
            WrapperEventCollection events = new WrapperEventCollection(wrapper);
            wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();

            // Change canvas
            string message = "Hello, world!";
            mainForm.standardTextBox.Value.SetValue(message);

            // Refresh data source (causes canvas to be marked dirty)
            mainForm.scrollViewer.Element.SetFocus();
            SendKeys.SendWait("+{F10}{UP 2}{ENTER}");
            mainForm.WaitForReady();

            // Attempt to close window
            mainForm.Window.Close();

            // Save canvas
            mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);
            wrapper.Exited.WaitOne();

            CollectionAssert.IsNotEmpty(events);
            WrapperEventArgs @event = events[events.Count - 1];
            Assert.AreEqual(WrapperEventType.CanvasSaved, @event.Type);
            StringAssert.Contains(message, @event.Properties.Content);
        }
    }
}
