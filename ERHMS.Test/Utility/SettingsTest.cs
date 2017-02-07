using ERHMS.Utility;
using NUnit.Framework;

namespace ERHMS.Test.Utility
{
    public class SettingsTest
    {
        [Test]
        public void IsEmailConfiguredTest()
        {
            Settings settings = new Settings
            {
                EmailHost = null,
                EmailPort = null,
                EmailSender = null
            };
            Assert.IsFalse(settings.IsEmailConfigured());
            settings.EmailHost = "localhost";
            Assert.IsFalse(settings.IsEmailConfigured());
            settings.EmailPort = 25;
            Assert.IsFalse(settings.IsEmailConfigured());
            settings.EmailSender = "jdoe";
            Assert.IsFalse(settings.IsEmailConfigured());
            settings.EmailSender = "jdoe@example.com";
            Assert.IsTrue(settings.IsEmailConfigured());
        }
    }
}
