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
                foreach (Mapping mapping in this)
                {
                    yield return mapping.Target;
                }
            }
        }

        public void Add(string source, string target)
        {
            Add(new Mapping(source, target));
        }

        public bool ContainsTarget(string target)
        {
            return this.Any(mapping => mapping.Target.Equals(target, StringComparison.OrdinalIgnoreCase));
        }

        public string GetKeyTarget()
        {
            if (ContainsTarget(ColumnNames.GLOBAL_RECORD_ID))
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
            string keyTarget = GetKeyTarget();
            if (!keyTarget.Equals(ColumnNames.GLOBAL_RECORD_ID, StringComparison.OrdinalIgnoreCase))
            {
                commands.AppendLine(Mapping.GetDefineCommand(keyTarget));
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
