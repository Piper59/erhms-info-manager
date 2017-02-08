using ERHMS.EpiInfo;
using NUnit.Framework;
using System.Linq;

namespace ERHMS.Test.EpiInfo
{
    public class WrapperTest
    {
        [Test]
        public void OutTest()
        {
            Wrapper wrapper = TestProgram.OutTest();
            Assert.AreEqual("Hello, world!", wrapper.ReadLine());
            wrapper.Exited.WaitOne();
        }

        [Test]
        public void InAndOutTest()
        {
            Wrapper wrapper = TestProgram.InAndOutTest();
            for (int value = 1; value <= 10; value++)
            {
                wrapper.WriteLine(value);
                Assert.AreEqual(value.ToString(), wrapper.ReadLine());
            }
            wrapper.EndWrite();
            wrapper.Exited.WaitOne();
        }

        [Test]
        public void ArgsTest()
        {
            Wrapper wrapper = TestProgram.ArgsTest(Enumerable.Range(1, 10));
            Assert.AreEqual("55", wrapper.ReadLine());
            wrapper.Exited.WaitOne();
        }
    }
}
