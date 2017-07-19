using Epi;
using ERHMS.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERHMS.EpiInfo.Wrappers
{
    internal class MappingCollection : List<Mapping>
    {
        public void Add(string source, string target)
        {
            Add(new Mapping(source, target));
        }

        private bool ContainsTarget(string target)
        {
            return this.Any(mapping => mapping.Target.EqualsIgnoreCase(target));
        }

        private bool ContainsIdMapping()
        {
            return ContainsTarget(ColumnNames.GLOBAL_RECORD_ID);
        }

        public string GetIdTarget()
        {
            if (ContainsIdMapping())
            {
                return ColumnNames.GLOBAL_RECORD_ID;
            }
            else
            {
                return ("Empty" + ColumnNames.GLOBAL_RECORD_ID).MakeUnique("{0}{1}", value => ContainsTarget(value));
            }
        }

        public IEnumerable<string> GetTargets()
        {
            if (!ContainsIdMapping())
            {
                yield return GetIdTarget();
            }
            foreach (Mapping mapping in this)
            {
                yield return mapping.Target;
            }
        }

        public string GetCommands()
        {
            StringBuilder commands = new StringBuilder();
            if (!ContainsIdMapping())
            {
                commands.AppendLine(Commands.Define(GetIdTarget()));
            }
            foreach (Mapping mapping in this)
            {
                if (mapping.IsIdentity())
                {
                    continue;
                }
                commands.AppendLine(Commands.Define(mapping.Target));
                commands.AppendLine(Commands.Assign(mapping.Source, mapping.Target));
            }
            return commands.ToString().Trim();
        }
    }
}
