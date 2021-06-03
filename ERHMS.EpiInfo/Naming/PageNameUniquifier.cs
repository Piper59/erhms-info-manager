using Epi;
using ERHMS.Common.Naming;

namespace ERHMS.EpiInfo.Naming
{
    public class PageNameUniquifier : NameUniquifier.IntSuffixed
    {
        public View View { get; }

        public PageNameUniquifier(View view)
        {
            View = view;
        }

        public override bool Exists(string name)
        {
            return View.GetPageByName(name) != null;
        }
    }
}
