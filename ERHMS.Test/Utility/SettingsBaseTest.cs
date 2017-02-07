using ERHMS.Utility;
using NUnit.Framework;
using System.IO;

namespace ERHMS.Test.Utility
{
    public class SettingsBaseTest
    {
        public class Settings : SettingsBase<Settings>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        [Test]
        public void LoadAndSaveTest()
        {
            try
            {
                bool found;
                Settings settings = Settings.Load(out found);
                Assert.IsFalse(found);
                settings.FirstName = "John";
                settings.LastName = "Doe";
                settings.Save();
                FileInfo file = Settings.GetFile();
                FileAssert.Exists(file);
                Assert.IsTrue(file.CreationTime.IsRecent());
                settings.FirstName = "Jane";
                settings = Settings.Load(out found);
                Assert.IsTrue(found);
                Assert.AreEqual("John", settings.FirstName);
            }
            finally
            {
                Settings.GetFile().Delete();
            }
        }
    }
}
