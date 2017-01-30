using ERHMS.Utility;
using NUnit.Framework;
using System.Reflection;
using System.Xml;

namespace ERHMS.Test.Utility
{
    public class XmlExtensionsTest
    {
        [Test]
        public void HasAllAttributesTest()
        {
            XmlDocument document = new XmlDocument();
            XmlElement element = document.CreateElement("person");
            element.SetAttribute("firstName", "John");
            element.SetAttribute("lastName", "Doe");
            Assert.IsTrue(element.HasAllAttributes("firstName", "lastName"));
            Assert.IsFalse(element.HasAllAttributes("firstName", "middleName", "lastName"));
        }

        [Test]
        public void ReadNextElementTest()
        {
            using (XmlReader reader = XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream("ERHMS.Test.Utility.People.xml")))
            {
                XmlElement element;
                element = reader.ReadNextElement();
                Assert.AreEqual(element.Name, "people");
                element = reader.ReadNextElement();
                Assert.AreEqual(element.Name, "person");
                Assert.AreEqual(element.Attributes["firstName"].Value, "John");
                Assert.AreEqual(element.Attributes["lastName"].Value, "Doe");
                element = reader.ReadNextElement();
                Assert.AreEqual(element.Name, "person");
                Assert.AreEqual(element.Attributes["firstName"].Value, "Jane");
                Assert.AreEqual(element.Attributes["lastName"].Value, "Doe");
                element = reader.ReadNextElement();
                Assert.IsNull(element);
            }
        }
    }
}
