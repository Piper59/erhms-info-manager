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
using View = Epi.View;

namespace ERHMS.Test.EpiInfo.Wrappers
{
#if IGNORE_LONG_TESTS
    [TestFixture(Ignore = "IGNORE_LONG_TESTS")]
#endif
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
        [Order(1)]
        public void ExportTest()
        {
            // Invoke wrapper
            wrapper = Analysis.Export.Create(project.FilePath, "Surveillance");
            wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            mainForm.WaitForReady();

            // Select variables
            WriteDialogScreen writeDialog = mainForm.GetWriteDialogScreen();
            writeDialog.lbxVariables.Selection.Add(ColumnNames.GLOBAL_RECORD_ID);
            writeDialog.lbxVariables.Selection.Add("LastName");
            writeDialog.lbxVariables.Selection.Add("FirstName");
            writeDialog.lbxVariables.Selection.Add("Entered");
            writeDialog.lbxVariables.Selection.Add("Updated");

            // Set export options
            writeDialog.rdbReplace.SelectionItem.Select();
            writeDialog.cmbOutputFormat.Selection.Set(csvDriverName);

            // Select CSV
            writeDialog.btnGetFile.Invoke.Invoke();
            CsvExistingFileDialogScreen csvExistingFileDialog = writeDialog.GetCsvExistingFileDialogScreen();
            csvExistingFileDialog.txtFileName.Value.SetValue(directory.Path);
            csvExistingFileDialog.btnOK.Invoke.Invoke();
            string csvPath = directory.CombinePaths("Surveillance.csv");
            writeDialog.Window.WaitForReady();
            writeDialog.txtFileName.Element.SetFocus();
            SendKeys.SendWait(Path.GetFileName(csvPath));

            // Run export
            writeDialog.btnOK.Invoke.Invoke();
            mainForm.WaitForReady();

            // Close window
            mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);

            string csvContent = Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Surveillance.All.csv");
            Assert.AreEqual(csvContent, File.ReadAllText(csvPath));
        }

        [Test]
        public void OpenPgmTest()
        {
            // Create PGM
            string outputPath = Path.Combine(configuration.Directories.Output, "OpenPgmTest_output.html");
            string message = "Hello, world!";
            StringBuilder content = new StringBuilder();
            content.AppendLine(Commands.RouteOut(outputPath));
            content.AppendLine(Commands.Type(message));
            content.AppendLine(Commands.CloseOut());
            Pgm pgm = new Pgm
            {
                Name = "OpenPgmTest_Pgm",
                Content = content.ToString()
            };
            project.InsertPgm(pgm);

            // Invoke wrapper
            wrapper = Analysis.OpenPgm.Create(project.FilePath, pgm.Name, pgm.Content, true);
            WrapperEventCollection events = new WrapperEventCollection(wrapper);
            wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            mainForm.WaitForReady();

            Assert.AreEqual(content.ToString().Trim(), mainForm.txtTextArea.Text.DocumentRange.GetText(-1).Trim().NormalizeNewLines());
            StringAssert.Contains(message, File.ReadAllText(outputPath));

            // Change PGM
            message = "Goodbye, world!";
            mainForm.txtTextArea.Text.Set(Commands.Type(message));

            // Attempt to close window
            mainForm.Window.Close();

            // Save PGM
            mainForm.GetSaveDialogScreen().Dialog.Close(DialogResult.Yes);
            PgmDialogScreen pgmDialog = mainForm.GetPgmDialogScreen();
            pgmDialog.btnOK.Invoke.Invoke();
            wrapper.Exited.WaitOne();

            StringAssert.Contains(message, project.GetPgmById(pgm.PgmId).Content);
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(WrapperEventType.PgmSaved, events[0].Type);
        }

        private void ImportCsv(string resourceName, string fileName)
        {
            // Create CSV
            string path = directory.CombinePaths(fileName);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo(resourceName, path);

            // Invoke wrapper
            wrapper = Analysis.Import.Create(project.FilePath, "Surveillance");
            wrapper.Invoke();
            WrapperEventCollection events = new WrapperEventCollection(wrapper);
            MainFormScreen mainForm = new MainFormScreen();

            // Select CSV
            ReadDialogScreen readDialog = mainForm.GetReadDialogScreen();
            readDialog.cmbDataSourcePlugIns.Selection.Set(csvDriverName);
            readDialog.btnFindDataSource.Invoke.Invoke();
            CsvExistingFileDialogScreen csvExistingFileDialog = readDialog.GetCsvExistingFileDialogScreen();
            csvExistingFileDialog.txtFileName.Value.SetValue(directory.Path);
            csvExistingFileDialog.btnOK.Invoke.Invoke();
            readDialog.lvDataSourceObjects.Selection.Set(Path.GetFileNameWithoutExtension(fileName) + "#csv");
            readDialog.btnOK.Invoke.Invoke();

            // Map variables
            MappingDialogScreen mappingDialog = mainForm.GetMappingDialogScreen();
            mappingDialog.SetMapping("LName", "LastName");
            mappingDialog.SetMapping("FName", "FirstName");

            // Run import
            mappingDialog.btnOk.Invoke.Invoke();
            mainForm.WaitForReady();

            // Close window
            mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);

            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(WrapperEventType.ViewDataImported, events[0].Type);
        }

        private void RecordTest(View view, string lastName, string firstName, DateTime entered, DateTime updated)
        {
            NamedObjectCollection<IDataField> fields = view.Fields.TableColumnFields;
            Assert.AreEqual(lastName, fields["LastName"].CurrentRecordValueObject);
            Assert.AreEqual(firstName, fields["FirstName"].CurrentRecordValueObject);
            Assert.AreEqual(entered, fields["Entered"].CurrentRecordValueObject);
            Assert.AreEqual(updated, fields["Updated"].CurrentRecordValueObject);
        }

        [Test]
        public void ImportAppendTest()
        {
            ImportCsv("ERHMS.Test.Resources.Surveillance.Append.csv", "Surveillance_Append.csv");
            View view = project.Views["Surveillance"];
            Assert.AreEqual(21, view.GetRecordCount());
            view.LoadLastRecord();
            RecordTest(view, "Doe", "Jane", new DateTime(2010, 1, 1), new DateTime(2010, 1, 1));
        }

        [Test]
        public void ImportUpdateTest()
        {
            ImportCsv("ERHMS.Test.Resources.Surveillance.Update.csv", "Surveillance_Update.csv");
            View view = project.Views["Surveillance"];
            view.LoadRecord(1);
            RecordTest(view, "Doe", "John", new DateTime(2007, 1, 7), new DateTime(2010, 1, 1));
        }
    }
}
