using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
using NUnit.Framework;
using System.Reflection;
using System.Windows.Forms;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public abstract partial class AnalysisDashboardTest : WrapperTest
    {
        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            Project.Metadata.CreateCanvasesTable();
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
            canvas.SetProjectPath(Project.FilePath);
            Project.InsertCanvas(canvas);

            // Invoke wrapper
            Wrapper = AnalysisDashboard.OpenCanvas.Create(Project.FilePath, canvas.CanvasId, canvas.Content);
            Wrapper.Invoke();
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
            Wrapper.Exited.WaitOne();

            StringAssert.Contains(message, Project.GetCanvasById(canvas.CanvasId).Content);
        }
    }

    public class AccessAnalysisDashboardTest : AnalysisDashboardTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new AccessSampleProjectCreator();
        }
    }

    public class SqlServerAnalysisDashboardTest : AnalysisDashboardTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new SqlServerSampleProjectCreator();
        }
    }
}
