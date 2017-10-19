using ERHMS.EpiInfo.Wrappers;
using NUnit.Framework;
using System;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public abstract partial class EnterTest : WrapperTest
    {
        private void FieldTest(View view, string name, object value)
        {
            Assert.AreEqual(value, view.Fields.DataFields[name].CurrentRecordValueObject);
        }

        [Test]
        public void OpenRecordTest()
        {
            // Invoke wrapper
            Wrapper = Enter.OpenRecord.Create(Project.FilePath, "Surveillance", 1);
            Wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            mainForm.WaitForReady();

            Assert.AreEqual("100", mainForm.GetField("CaseID"));
            Assert.AreEqual("John", mainForm.GetField("FirstName"));
            Assert.AreEqual("Smith", mainForm.GetField("LastName"));
            Assert.AreEqual("5/20/1968", mainForm.GetField("BirthDate"));

            // Change record
            mainForm.SetField("LastName", "Doe");
            mainForm.SetField("BirthDate", "1/1/1980");

            // Attempt to close window
            mainForm.Window.Close();

            // Save record
            mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);

            Wrapper.Exited.WaitOne();
            View view = Project.Views["Surveillance"];
            view.LoadRecord(1);
            FieldTest(view, "LastName", "Doe");
            FieldTest(view, "BirthDate", new DateTime(1980, 1, 1));
        }

        [Test]
        public void OpenNewRecordTest()
        {
            // Invoke wrapper
            dynamic record = new
            {
                CaseID = "2100",
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(1980, 1, 1)
            };
            Wrapper = Enter.OpenNewRecord.Create(Project.FilePath, "Surveillance", record);
            Wrapper.Invoke();
            MainFormScreen mainForm = new MainFormScreen();
            mainForm.WaitForReady();

            // Attempt to close window
            mainForm.Window.Close();

            // Save record
            mainForm.GetCloseDialogScreen().Dialog.Close(DialogResult.Yes);

            Wrapper.Exited.WaitOne();
            View view = Project.Views["Surveillance"];
            Assert.AreEqual(21, view.GetRecordCount());
            view.LoadRecord(21);
            FieldTest(view, "CaseID", record.CaseID);
            FieldTest(view, "FirstName", record.FirstName);
            FieldTest(view, "LastName", record.LastName);
            FieldTest(view, "BirthDate", record.BirthDate);
        }
    }

    public class AccessEnterTest : EnterTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new AccessSampleProjectCreator();
        }
    }

    public class SqlServerEnterTest : EnterTest
    {
        protected override ISampleProjectCreator GetCreator()
        {
            return new SqlServerSampleProjectCreator();
        }
    }
}
