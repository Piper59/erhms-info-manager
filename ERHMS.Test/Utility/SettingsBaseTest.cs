using ERHMS.Utility;
using NUnit.Framework;

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
                Settings settings;
                bool loaded;
                settings = Settings.Load(out loaded);
                Assert.IsFalse(loaded);
                settings.FirstName = "John";
                settings.LastName = "Doe";
                settings.Save();
                FileAssert.Exists(Settings.GetFile());
                settings.FirstName = "Jane";
                settings = Settings.Load(out loaded);
                Assert.IsTrue(loaded);
                Assert.AreEqual("John", settings.FirstName);
            }
            finally
            {
                Settings.GetFile().Delete();
            }
        }
    }
}
