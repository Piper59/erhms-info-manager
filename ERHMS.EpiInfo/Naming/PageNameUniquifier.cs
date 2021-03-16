using Epi;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.Naming
{
    public class PageNameUniquifier : NameUniquifier.IntSuffixed
    {
        public View View { get; }
        protected override Regex NameRegex => null;
        protected override string InitialBaseName => "Page";
        protected override int InitialSuffix => View.Pages.Count + 1;

        public PageNameUniquifier(View view)
        {
            View = view;
        }

        public override bool Exists(string name)
        {
            return View.GetPageByName(name) != null;
        }

        protected override string Format(string baseName, int suffix)
        {
            return $"{baseName} {suffix}";
        }
    }
}
