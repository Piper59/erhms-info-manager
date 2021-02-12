using Epi;
using System.Linq;
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
            return View.Pages.Any(page => NameComparer.Default.Equals(page.Name, name));
        }

        protected override string Format(string baseName, int suffix)
        {
            return $"{baseName} {suffix}";
        }
    }
}
