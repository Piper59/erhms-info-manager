using ERHMS.Utility;
using NUnit.Framework;
using System;

namespace ERHMS.Test.Utility
{
    public class ConvertExtensionsTest
    {
        [Test]
        public void ToNullableGuidTest()
        {
            Assert.IsNull(ConvertExtensions.ToNullableGuid(null));
            Assert.Catch(() =>
            {
                ConvertExtensions.ToNullableGuid("");
            });
            Assert.AreEqual(Guid.Empty, ConvertExtensions.ToNullableGuid("00000000-0000-0000-0000-000000000000"));
            Assert.Catch(() =>
            {
                ConvertExtensions.ToNullableGuid("xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");
            });
        }
    }
}
