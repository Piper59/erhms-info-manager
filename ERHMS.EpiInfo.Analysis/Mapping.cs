using System;

namespace ERHMS.EpiInfo.Analysis
{
    internal class Mapping
    {
        public static string GetDefineCommand(string target)
        {
            return string.Format("DEFINE {0}", target);
        }

        public string Source { get; set; }
        public string Target { get; set; }

        public Mapping() { }

        public Mapping(string source, string target)
        {
            Source = source;
            Target = target;
        }

        public bool IsIdentity()
        {
            return string.Equals(Source, Target, StringComparison.OrdinalIgnoreCase);
        }

        public string GetDefineCommand()
        {
            return GetDefineCommand(Target);
        }

        public string GetAssignCommand()
        {
            return string.Format("ASSIGN {0} = [{1}]", Target, Source);
        }
    }
}
