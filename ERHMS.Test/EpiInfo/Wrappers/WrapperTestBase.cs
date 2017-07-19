using ERHMS.EpiInfo.Wrappers;
using NUnit.Framework;
using System.Collections.Generic;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public class WrapperTestBase : SampleProjectTestBase
    {
        protected class WrapperEventCollection : List<WrapperEventArgs>
        {
            public WrapperEventCollection(Wrapper wrapper)
            {
                wrapper.Event += (sender, e) =>
                {
                    Add(e);
                };
            }
        }

        protected Wrapper wrapper;

        [TearDown]
        public void TearDown()
        {
            if (wrapper != null)
            {
                if (!wrapper.Exited.WaitOne(10000))
                {
                    TestContext.Error.WriteLine("Wrapper is not responding.");
                }
                wrapper = null;
            }
        }
    }
}
