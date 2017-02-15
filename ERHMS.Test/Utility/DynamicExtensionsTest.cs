using ERHMS.Utility;
using NUnit.Framework;

namespace ERHMS.Test.Utility
{
    public class DynamicExtensionsTest
    {
        [Test]
        public void SerializeAndDeserializeTest()
        {
            object original = new
            {
                Name = "John Doe",
                Age = 20,
                Male = true
            };
            string value = DynamicExtensions.Serialize(original);
            dynamic converted = DynamicExtensions.Deserialize(value);
            Assert.AreEqual("John Doe", converted.Name);
            Assert.AreEqual(20, converted.Age);
            Assert.AreEqual(true, converted.Male);
        }
    }
}
