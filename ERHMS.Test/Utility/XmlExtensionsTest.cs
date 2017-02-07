using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace ERHMS.Test.Utility
{
    public class XmlExtensionsTest
    {
        private const string ResourceName = "ERHMS.Test.Utility.People.xml";

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

        private void PersonTest(XmlElement element, string firstName, string lastName)
        {
            Assert.AreEqual("person", element.Name);
            Assert.AreEqual(firstName, element.GetAttribute("firstName"));
            Assert.AreEqual(lastName, element.GetAttribute("lastName"));
        }

        [Test]
        public void ReadNextElementTest()
        {
            using (XmlReader reader = XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName)))
            {
                XmlElement element;
                element = reader.ReadNextElement();
                Assert.AreEqual("people", element.Name);
                element = reader.ReadNextElement();
                PersonTest(element, "John", "Doe");
                element = reader.ReadNextElement();
                PersonTest(element, "Jane", "Doe");
                element = reader.ReadNextElement();
                Assert.IsNull(element);
            }
        }

        [Test]
        public void SelectElementsTest()
        {
            XmlDocument document = new XmlDocument();
            document.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName));
            IList<XmlElement> elements = document.SelectElements("/people/person").ToList();
            PersonTest(elements[0], "John", "Doe");
            PersonTest(elements[1], "Jane", "Doe");
        }

        [Test]
        public void SelectSingleElementTest()
        {
            XmlDocument document = new XmlDocument();
            document.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream(ResourceName));
            PersonTest(document.SelectSingleElement("/people/person"), "John", "Doe");
        }
    }
}
