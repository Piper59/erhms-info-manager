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
            Number[] numbers = new Number[]
            {
                Number.One,
                Number.Two,
                Number.Three
            };
            CollectionAssert.AreEqual(numbers, EnumExtensions.GetValues<Number>());
        }

        [Test]
        public void ToDescriptionTest()
        {
            Assert.AreEqual("1", EnumExtensions.ToDescription(Number.One));
        }

        [Test]
        public void FromDescriptionTest()
        {
            Assert.AreEqual(Number.One, EnumExtensions.FromDescription<Number>("1"));
        }
    }
}
