using Epi;
using Epi.Collections;
using Epi.Fields;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Project = ERHMS.EpiInfo.Project;
using View = Epi.View;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public partial class AnalysisTest
    {
        private static class Commands
        {
            public static string CloseOut()
            {
                return "CLOSEOUT";
            }

            public static string RouteOut(string path)
            {
                return string.Format("ROUTEOUT \"{0}\" REPLACE", path);
            }

            public static string Type(string value)
            {
                return string.Format("TYPEOUT \"{0}\"", value.Replace("\"", ""));
            }
        }

        private TempDirectory directory;
        private Configuration configuration;
        private string csvDriverName;
        private Project project;
        private Wrapper wrapper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            directory = new TempDirectory(nameof(AnalysisTest));
            ConfigurationExtensions.Create(directory.Path).Save();
            configuration = ConfigurationExtensions.Load();
            configuration.CreateUserDirectories();
            csvDriverName = configuration.DataDrivers.Single(driver => driver.Type == Configuration.CsvDriver).DisplayName;
            string location = Path.Combine(configuration.Directories.Project, "Sample");
            Directory.CreateDirectory(location);
            string projectPath = Path.Combine(location, "Sample.prj");
            Assembly assembly = Assembly.GetExecutingAssembly();
            assembly.CopyManifestResourceTo("ERHMS.Test.Resources.Sample.prj", projectPath);
            assembly.CopyManifestResourceTo("ERHMS.Test.Resources.Sample.mdb", Path.ChangeExtension(projectPath, ".mdb"));
            ProjectInfo.Get(projectPath).SetAccessConnectionString();
            project = new Project(projectPath);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(ConfigurationExtensions.FilePath);
            directory.Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            if (wrapper != null)
            {
                if (!wrapper.Exited.WaitOne(10000))
                {
                    Assert.Fail("Wrapper is not responding.");
                }
                wrapper = null;
            }
        }

        [Test]
        public void OpenPgmTest()
        {
            string outputPath = Path.Combine(configuration.Directories.Output, "OpenPgmTest_output.html");
            string message = "Hello, world!";
            StringBuilder content = new StringBuilder();
            content.AppendLine(Commands.RouteOut(outputPath));
            content.AppendLine(Commands.Type(message));
            content.AppendLine(Commands.CloseOut());
            string pgmName = "OpenPgmTest_Pgm";
            wrapper = Analysis.OpenPgm.Create(pgmName, content.ToString(), true);
            wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            mainForm.WaitForReady();
            Assert.AreEqual(content.ToString().Trim(), mainForm.txtTextArea.Text.DocumentRange.GetText(-1).Trim().NormalizeNewLines());
            StringAssert.Contains(message, File.ReadAllText(outputPath));
            message = "Goodbye, world!";
            mainForm.txtTextArea.Text.Set(Commands.Type(message));
            mainForm.Window.Close();
            mainForm.GetSaveDialogScreen().Dialog.Close(DialogResult.Yes);
            PgmDialogScreen pgmDialog = mainForm.GetPgmDialogScreen();
            pgmDialog.btnFindProject.Invoke.Invoke();
            OpenDialogScreen openDialog = pgmDialog.GetOpenDialogScreen();
            openDialog.txtFileName.Value.SetValue(project.FilePath);
            openDialog.Dialog.Close(DialogResult.OK);
            pgmDialog.Window.WaitForReady();
            pgmDialog.cmbPrograms.Element.SetFocus();
            SendKeys.SendWait(pgmName);
            pgmDialog.btnOK.Invoke.Invoke();
            wrapper.Exited.WaitOne();
            Assert.IsTrue(project.GetPgms().Any(pgm => pgm.Content.Contains(message)));
        }

        private void ImportCsv(string resourceName, string fileName)
        {
            string path = directory.CombinePaths(fileName);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo(resourceName, path);
            wrapper = Analysis.Import.Create(project.FilePath, "Surveillance");
            wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            ReadDialogScreen readDialog = mainForm.GetReadDialogScreen();
            readDialog.cmbDataSourcePlugIns.Selection.Select(csvDriverName);
            readDialog.btnFindDataSource.Invoke.Invoke();
            CsvExistingFileDialogScreen csvExistingFileDialog = readDialog.GetCsvExistingFileDialogScreen();
            csvExistingFileDialog.txtFileName.Value.SetValue(directory.Path);
            csvExistingFileDialog.btnOK.Invoke.Invoke();
            readDialog.lvDataSourceObjects.Selection.Select(Path.GetFileNameWithoutExtension(fileName) + "#csv");
            readDialog.btnOK.Invoke.Invoke();
            MappingDialogScreen mappingDialog = mainForm.GetMappingDialogScreen();
            mappingDialog.SetMapping("LName", "LastName");
            mappingDialog.SetMapping("FName", "FirstName");
            mappingDialog.btnOk.Invoke.Invoke();
            mainForm.WaitForReady();
            mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);
            wrapper.Exited.WaitOne();
        }

        private void RecordTest(View view, string lastName, string firstName, DateTime entered, DateTime updated)
        {
            NamedObjectCollection<IDataField> fields = view.Fields.DataFields;
            Assert.AreEqual(lastName, fields["LastName"].CurrentRecordValueObject);
            Assert.AreEqual(firstName, fields["FirstName"].CurrentRecordValueObject);
            Assert.AreEqual(entered, fields["Entered"].CurrentRecordValueObject);
            Assert.AreEqual(updated, fields["Updated"].CurrentRecordValueObject);
        }

        [Test]
        public void ImportAppendTest()
        {
            ImportCsv("ERHMS.Test.Resources.Surveillance_Append.csv", "Surveillance_Append.csv");
            View view = project.Views["Surveillance"];
            Assert.AreEqual(21, view.GetRecordCount());
            view.LoadLastRecord();
            RecordTest(view, "Doe", "Jane", new DateTime(2010, 1, 1), new DateTime(2010, 1, 1));
        }

        [Test]
        public void ImportUpdateTest()
        {
            ImportCsv("ERHMS.Test.Resources.Surveillance_Update.csv", "Surveillance_Update.csv");
            View view = project.Views["Surveillance"];
            view.LoadRecord(1);
            RecordTest(view, "Doe", "John", new DateTime(2007, 1, 7), new DateTime(2010, 1, 1));
        }

        [Test]
        [Order(1)]
        public void ExportTest()
        {
            wrapper = Analysis.Export.Create(project.FilePath, "Surveillance");
            wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            mainForm.WaitForReady();
            WriteDialogScreen writeDialog = mainForm.GetWriteDialogScreen();
            writeDialog.lbxVariables.Selection.AddToSelection(ColumnNames.GLOBAL_RECORD_ID);
            writeDialog.lbxVariables.Selection.AddToSelection("LastName");
            writeDialog.lbxVariables.Selection.AddToSelection("FirstName");
            writeDialog.lbxVariables.Selection.AddToSelection("Entered");
            writeDialog.lbxVariables.Selection.AddToSelection("Updated");
            writeDialog.rdbReplace.SelectionItem.Select();
            writeDialog.cmbOutputFormat.Selection.Select(csvDriverName);
            writeDialog.btnGetFile.Invoke.Invoke();
            CsvExistingFileDialogScreen csvExistingFileDialog = writeDialog.GetCsvExistingFileDialogScreen();
            csvExistingFileDialog.txtFileName.Value.SetValue(directory.Path);
            csvExistingFileDialog.btnOK.Invoke.Invoke();
            string csvPath = directory.CombinePaths("Surveillance.csv");
            writeDialog.Window.WaitForReady();
            writeDialog.txtFileName.Element.SetFocus();
            SendKeys.SendWait(Path.GetFileName(csvPath));
            writeDialog.btnOK.Invoke.Invoke();
            mainForm.WaitForReady();
            mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);
            wrapper.Exited.WaitOne();
            string csvContent = Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Surveillance.csv");
            Assert.AreEqual(csvContent, File.ReadAllText(csvPath));
        }
    }
}
