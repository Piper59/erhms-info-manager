using System;

namespace ERHMS.EpiInfo.MakeView
{
    public class MakeView : Wrapper
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            MainBase(typeof(MakeView), args);
        }
    }
}
