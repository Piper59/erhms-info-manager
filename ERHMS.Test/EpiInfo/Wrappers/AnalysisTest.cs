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
    public abstract partial class AnalysisTest : WrapperTest
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

            public static string TypeOut(string value)
            {
                return string.Format("TYPEOUT \"{0}\"", value.Replace("\"", ""));
            }
        }

        private string csvDriverName;

        [OneTimeSetUp]
        public new void OneTimeSetUp()
        {
            csvDriverName = Configuration.DataDrivers.Single(driver => driver.Type == Configuration.CsvDriver).DisplayName;
        }

        [Test]
        [Order(1)]
        public void ExportTest()
        {
            // Invoke wrapper
            Wrapper = Analysis.Export.Create(Project.FilePath, "Surveillance");
            Wrapper.Invoke();
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
            csvExistingFileDialog.txtFileName.Value.SetValue(Project.Location);
            csvExistingFileDialog.btnOK.Invoke.Invoke();
            writeDialog.Window.WaitForReady();
            writeDialog.txtFileName.Element.SetFocus();
            string fileName = "Surveillance.csv";
            SendKeys.SendWait(fileName);

            // Run export
            writeDialog.btnOK.Invoke.Invoke();
            mainForm.WaitForReady();

            // Close window
            mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);

            string content = Assembly.GetExecutingAssembly().GetManifestResourceText("ERHMS.Test.Resources.Sample.Surveillance.All.csv");
            Assert.AreEqual(content, File.ReadAllText(Path.Combine(Project.Location, fileName)));
        }

        [Test]
        public void OpenPgmTest()
        {
            // Create PGM
            string path = Path.Combine(Configuration.Directories.Output, "OpenPgmTest.html");
            string message = "Hello, world!";
            StringBuilder content = new StringBuilder();
            content.AppendLine(Commands.RouteOut(path));
            content.AppendLine(Commands.TypeOut(message));
            content.AppendLine(Commands.CloseOut());
            Pgm pgm = new Pgm
            {
                Name = "OpenPgmTest_Pgm",
                Content = content.ToString()
            };
            Project.InsertPgm(pgm);

            // Invoke wrapper
            Wrapper = Analysis.OpenPgm.Create(pgm.Content, true);
            Wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            mainForm.WaitForReady();

            StringAssert.Contains(message, File.ReadAllText(path));

            // Close window
            mainForm.Window.Close();
        }

        private WrapperEventCollection ImportCsv(string resourceName, string fileName)
        {
            // Create CSV
            string path = Path.Combine(Project.Location, fileName);
            Assembly.GetExecutingAssembly().CopyManifestResourceTo(resourceName, path);

            // Invoke wrapper
            Wrapper = Analysis.Import.Create(Project.FilePath, "Surveillance");
            WrapperEventCollection events = new WrapperEventCollection(Wrapper);
            Wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();

            // Select CSV
            ReadDialogScreen readDialog = mainForm.GetReadDialogScreen();
            readDialog.cmbDataSourcePlugIns.Selection.Set(csvDriverName);
            readDialog.btnFindDataSource.Invoke.Invoke();
            CsvExistingFileDialogScreen csvExistingFileDialog = readDialog.GetCsvExistingFileDialogScreen();
            csvExistingFileDialog.txtFileName.Value.SetValue(Project.Location);
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

            Wrapper.Exited.WaitOne();
            return events;
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
            WrapperEventCollection events = ImportCsv("ERHMS.Test.Resources.Sample.Surveillance.Append.csv", "Surveillance_Append.csv");
            View view = Project.Views["Surveillance"];
            Assert.AreEqual(21, view.GetRecordCount());
            view.LoadRecord(21);
            RecordTest(view, "Doe", "Jane", new DateTime(2010, 1, 1), new DateTime(2010, 1, 1));
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("DataImported", events[0].Type);
            Assert.AreEqual(events[0].Properties.ViewId, view.Id);
        }

        [Test]
        public void ImportUpdateTest()
        {
            WrapperEventCollection events = ImportCsv("ERHMS.Test.Resources.Sample.Surveillance.Update.csv", "Surveillance_Update.csv");
            View view = Project.Views["Surveillance"];
            view.LoadRecord(1);
            RecordTest(view, "Doe", "John", new DateTime(2007, 1, 7), new DateTime(2010, 1, 1));
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual("DataImported", events[0].Type);
            Assert.AreEqual(events[0].Properties.ViewId, view.Id);
        }
    }

    public class AccessAnalysisTest : AnalysisTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new AccessSampleProjectCreator();
        }
    }

    public class SqlServerAnalysisTest : AnalysisTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new SqlServerSampleProjectCreator();
        }
    }
}
