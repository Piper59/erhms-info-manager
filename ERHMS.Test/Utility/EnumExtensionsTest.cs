using ERHMS.Utility;
using NUnit.Framework;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace ERHMS.Test.Utility
{
    public class EnumExtensionsTest
    {
        private enum Number
        {
            [Description("1")]
            One,

            [Description("2")]
            Two,

            [Description("3")]
            Three
        }

        [Test]
        public void GetValuesTest()
        {
            CollectionAssert.AreEqual(EnumExtensions.GetValues<Number>(), new Number[]
            {
                Number.One,
                Number.Two,
                Number.Three
            });
        }

        [Test]
        public void ToDescriptionTest()
        {
            Assert.AreEqual(EnumExtensions.ToDescription(Number.One), "1");
        }

        [Test]
        public void FromDescriptionTest()
        {
            Assert.AreEqual(EnumExtensions.FromDescription<Number>("1"), Number.One);
        }
    }
}
