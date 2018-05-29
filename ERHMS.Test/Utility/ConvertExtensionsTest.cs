using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class ConvertExtensionsTest
    {
        [Test]
        public void ToDegreesTest()
        {
            Assert.AreEqual(0.0, ConvertExtensions.ToDegrees(0.0));
            Assert.AreEqual(180.0 / Math.PI, ConvertExtensions.ToDegrees(1.0));
            Assert.AreEqual(360.0, ConvertExtensions.ToDegrees(2.0 * Math.PI));
        }

        [Test]
        public void ToRadiansTest()
        {
            Assert.AreEqual(0.0, ConvertExtensions.ToRadians(0.0));
            Assert.AreEqual(Math.PI / 180.0, ConvertExtensions.ToRadians(1.0));
            Assert.AreEqual(2.0 * Math.PI, ConvertExtensions.ToRadians(360.0));
        }

        [Test]
        public void ToNullableGuidTest()
        {
            Assert.IsNull(ConvertExtensions.ToNullableGuid(null));
            Assert.IsNull(ConvertExtensions.ToNullableGuid(""));
            Assert.AreEqual(Guid.Empty, ConvertExtensions.ToNullableGuid("00000000-0000-0000-0000-000000000000"));
            Assert.Catch(() =>
            {
                ConvertExtensions.ToNullableGuid("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
            });
        }
    }
}
