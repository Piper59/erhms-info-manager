using Epi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERHMS.EpiInfo.Analysis
{
    internal class MappingCollection : List<Mapping>
    {
        public IEnumerable<string> Targets
        {
            get
            {
                if (!ContainsNonEmptyKeyTarget())
                {
                    yield return GetKeyTarget();
                }
                foreach (Mapping mapping in this)
                {
                    yield return mapping.Target;
                }
            }
        }

        public IEnumerable<string> EscapedTargets
        {
            get { return Targets.Select(target => string.Format("[{0}]", target)); }
        }

        public void Add(string source, string target)
        {
            Add(new Mapping(source, target));
        }

        public bool ContainsTarget(string target)
        {
            return this.Any(mapping => mapping.Target.Equals(target, StringComparison.OrdinalIgnoreCase));
        }

        public bool ContainsNonEmptyKeyTarget()
        {
            return ContainsTarget(ColumnNames.GLOBAL_RECORD_ID);
        }

        public string GetKeyTarget()
        {
            if (ContainsNonEmptyKeyTarget())
            {
                return ColumnNames.GLOBAL_RECORD_ID;
            }
            else
            {
                string baseTarget = string.Format("Empty{0}", ColumnNames.GLOBAL_RECORD_ID);
                if (!ContainsTarget(baseTarget))
                {
                    return baseTarget;
                }
                for (int index = 2; ; index++)
                {
                    string candidateTarget = string.Format("{0}{1}", baseTarget, index);
                    if (!ContainsTarget(candidateTarget))
                    {
                        return candidateTarget;
                    }
                }
            }
        }

        public string GetCommands()
        {
            StringBuilder commands = new StringBuilder();
            if (!ContainsNonEmptyKeyTarget())
            {
                commands.AppendLine(Mapping.GetDefineCommand(GetKeyTarget()));
            }
            foreach (Mapping mapping in this)
            {
                if (mapping.IsIdentity())
                {
                    continue;
                }
                commands.AppendLine(mapping.GetDefineCommand());
                commands.AppendLine(mapping.GetAssignCommand());
            }
            return commands.ToString().Trim();
        }
    }
}
