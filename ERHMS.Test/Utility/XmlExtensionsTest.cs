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

        private XmlDocument GetDocument()
        {
            XmlDocument document = new XmlDocument();
            document.Load(GetResource());
            return document;
        }

        [Test]
        public void HasAllAttributesTest()
        {
            XmlElement element = (XmlElement)GetDocument().SelectSingleNode("/people/person");
            Assert.IsTrue(element.HasAllAttributes("firstName", "lastName"));
            Assert.IsFalse(element.HasAllAttributes("firstName", "middleName", "lastName"));
        }

        [Test]
        public void ReadNextElementTest()
        {
            using (XmlReader reader = XmlReader.Create(GetResource()))
            {
                Assert.AreEqual("people", reader.ReadNextElement().Name);
                Assert.AreEqual("johnd", reader.ReadNextElement().GetAttribute("id"));
                Assert.AreEqual("janed", reader.ReadNextElement().GetAttribute("id"));
                Assert.IsNull(reader.ReadNextElement());
            }
        }

        [Test]
        public void SelectElementsTest()
        {
            Assert.AreEqual(2, GetDocument().SelectElements("/people/person").Count());
        }

        [Test]
        public void SelectSingleElementTest()
        {
            Assert.AreEqual("johnd", GetDocument().SelectSingleElement("/people/person").GetAttribute("id"));
        }
    }
}
