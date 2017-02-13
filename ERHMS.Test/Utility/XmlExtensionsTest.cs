using ERHMS.Utility;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace ERHMS.Test.Utility
{
    public class XmlExtensionsTest
    {
        private Stream GetResource()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("ERHMS.Test.Resources.People.xml");
        }

        [Test]
        public void HasAllAttributesTest()
        {
            XmlDocument document = new XmlDocument();
            document.Load(GetResource());
            XmlElement element = (XmlElement)document.SelectSingleNode("/people/person");
            Assert.IsTrue(element.HasAllAttributes("firstName", "lastName"));
            Assert.IsFalse(element.HasAllAttributes("firstName", "middleName", "lastName"));
        }

        [Test]
        public void ReadNextElementTest()
        {
            using (XmlReader reader = XmlReader.Create(GetResource()))
            {
                XmlElement element;
                element = reader.ReadNextElement();
                Assert.AreEqual("people", element.Name);
                element = reader.ReadNextElement();
                Assert.AreEqual("johnd", element.GetAttribute("id"));
                element = reader.ReadNextElement();
                Assert.AreEqual("janed", element.GetAttribute("id"));
                element = reader.ReadNextElement();
                Assert.IsNull(element);
            }
        }

        [Test]
        public void SelectElementsTest()
        {
            XmlDocument document = new XmlDocument();
            document.Load(GetResource());
            Assert.AreEqual(2, document.SelectElements("/people/person").Count());
        }

        [Test]
        public void SelectSingleElementTest()
        {
            XmlDocument document = new XmlDocument();
            document.Load(GetResource());
            Assert.AreEqual("johnd", document.SelectSingleElement("/people/person").GetAttribute("id"));
        }
    }
}
