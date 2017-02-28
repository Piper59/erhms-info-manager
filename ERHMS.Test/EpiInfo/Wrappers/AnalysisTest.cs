using Epi;
using Epi.Collections;
using Epi.Fields;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Utility;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public partial class AnalysisTest : WrapperTestBase
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

        private string csvDriverName;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            csvDriverName = configuration.DataDrivers.Single(driver => driver.Type == Configuration.CsvDriver).DisplayName;
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
            WrapperEventCollection events = new WrapperEventCollection(wrapper);
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
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(WrapperEventType.PgmSaved, events[0].Type);
            StringAssert.Contains(message, events[0].Properties.Content);
        }

        private void ImportCsv(string resourceName, string fileName)
        {
            string path = directory.CombinePaths(fileName);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo(resourceName, path);
            wrapper = Analysis.Import.Create(project.FilePath, "Surveillance");
            wrapper.Invoke();
            WrapperEventCollection events = new WrapperEventCollection(wrapper);
            MainFormScreen mainForm = new MainFormScreen();
            ReadDialogScreen readDialog = mainForm.GetReadDialogScreen();
            readDialog.cmbDataSourcePlugIns.Selection.Set(csvDriverName);
            readDialog.btnFindDataSource.Invoke.Invoke();
            CsvExistingFileDialogScreen csvExistingFileDialog = readDialog.GetCsvExistingFileDialogScreen();
            csvExistingFileDialog.txtFileName.Value.SetValue(directory.Path);
            csvExistingFileDialog.btnOK.Invoke.Invoke();
            readDialog.lvDataSourceObjects.Selection.Set(Path.GetFileNameWithoutExtension(fileName) + "#csv");
            readDialog.btnOK.Invoke.Invoke();
            MappingDialogScreen mappingDialog = mainForm.GetMappingDialogScreen();
            mappingDialog.SetMapping("LName", "LastName");
            mappingDialog.SetMapping("FName", "FirstName");
            mappingDialog.btnOk.Invoke.Invoke();
            mainForm.WaitForReady();
            mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(WrapperEventType.ViewDataImported, events[0].Type);
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
            writeDialog.lbxVariables.Selection.Add(ColumnNames.GLOBAL_RECORD_ID);
            writeDialog.lbxVariables.Selection.Add("LastName");
            writeDialog.lbxVariables.Selection.Add("FirstName");
            writeDialog.lbxVariables.Selection.Add("Entered");
            writeDialog.lbxVariables.Selection.Add("Updated");
            writeDialog.rdbReplace.SelectionItem.Select();
            writeDialog.cmbOutputFormat.Selection.Set(csvDriverName);
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
            string csvContent = Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Surveillance_All.csv");
            Assert.AreEqual(csvContent, File.ReadAllText(csvPath));
        }
    }
}
