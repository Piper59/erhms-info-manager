using Epi;
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
    public partial class AnalysisTest : WrapperTest
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
            csvExistingFileDialog.txtFileName.Value.SetValue(directory.FullName);
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

            string csvContent = Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Sample.Surveillance.All.csv");
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
            wrapper = Analysis.OpenPgm.Create(pgm.Content, true);
            wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            mainForm.WaitForReady();

            StringAssert.Contains(message, File.ReadAllText(outputPath));

            // Close window
            mainForm.Window.Close();
        }

        private void ImportCsv(string resourceName, string fileName)
        {
            // Create CSV
            string path = directory.CombinePaths(fileName);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo(resourceName, path);

            // Invoke wrapper
            wrapper = Analysis.Import.Create(project.FilePath, "Surveillance");
            wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();

            // Select CSV
            ReadDialogScreen readDialog = mainForm.GetReadDialogScreen();
            readDialog.cmbDataSourcePlugIns.Selection.Set(csvDriverName);
            readDialog.btnFindDataSource.Invoke.Invoke();
            CsvExistingFileDialogScreen csvExistingFileDialog = readDialog.GetCsvExistingFileDialogScreen();
            csvExistingFileDialog.txtFileName.Value.SetValue(directory.FullName);
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
        }

        private void FieldTest(View view, string name, object value)
        {
            Assert.AreEqual(value, view.Fields.DataFields[name].CurrentRecordValueObject);
        }

        private void RecordTest(View view, string lastName, string firstName, DateTime entered, DateTime updated)
        {
            FieldTest(view, "LastName", lastName);
            FieldTest(view, "FirstName", firstName);
            FieldTest(view, "Entered", entered);
            FieldTest(view, "Updated", updated);
        }

        [Test]
        public void ImportAppendTest()
        {
            ImportCsv("ERHMS.Test.Resources.Sample.Surveillance.Append.csv", "Surveillance_Append.csv");
            View view = project.Views["Surveillance"];
            Assert.AreEqual(21, view.GetRecordCount());
            view.LoadRecord(21);
            RecordTest(view, "Doe", "Jane", new DateTime(2010, 1, 1), new DateTime(2010, 1, 1));
        }

        [Test]
        public void ImportUpdateTest()
        {
            ImportCsv("ERHMS.Test.Resources.Sample.Surveillance.Update.csv", "Surveillance_Update.csv");
            View view = project.Views["Surveillance"];
            view.LoadRecord(1);
            RecordTest(view, "Doe", "John", new DateTime(2007, 1, 7), new DateTime(2010, 1, 1));
        }
    }
}
