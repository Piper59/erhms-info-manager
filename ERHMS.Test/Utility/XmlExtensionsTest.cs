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
                Assert.AreEqual("people", element.Name);
                element = reader.ReadNextElement();
                Assert.AreEqual("person", element.Name);
                Assert.AreEqual("John", element.Attributes["firstName"].Value);
                Assert.AreEqual("Doe", element.Attributes["lastName"].Value);
                element = reader.ReadNextElement();
                Assert.AreEqual("person", element.Name);
                Assert.AreEqual("Jane", element.Attributes["firstName"].Value);
                Assert.AreEqual("Doe", element.Attributes["lastName"].Value);
                element = reader.ReadNextElement();
                Assert.IsNull(element);
            }
        }
    }
}
