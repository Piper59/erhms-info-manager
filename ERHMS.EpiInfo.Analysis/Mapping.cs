using ERHMS.Utility;

namespace ERHMS.EpiInfo.Wrappers
{
    internal class Mapping
    {
        public string Source { get; private set; }
        public string Target { get; private set; }

        public Mapping(string source, string target)
        {
            Source = source;
            Target = target;
        }

        public bool IsIdentity()
        {
            return StringExtensions.EqualsIgnoreCase(Source, Target);
        }
    }
}
