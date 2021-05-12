using Epi;
using System.Linq;

namespace ERHMS.EpiInfo.Naming
{
    public class FieldNameUniquifier : NameUniquifier.IntSuffixed
    {
        public View View { get; }

        public FieldNameUniquifier(View view)
        {
            View = view;
        }

        public override bool Exists(string name)
        {
            return View.Fields.Keys.Contains(name);
        }
    }
}
