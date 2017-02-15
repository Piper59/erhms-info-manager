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

        private bool ContainsKeyMapping()
        {
            return ContainsTarget(ColumnNames.GLOBAL_RECORD_ID);
        }

        public string GetKeyTarget()
        {
            if (ContainsKeyMapping())
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
            if (!ContainsKeyMapping())
            {
                yield return GetKeyTarget();
            }
            foreach (Mapping mapping in this)
            {
                yield return mapping.Target;
            }
        }

        public string GetCommands()
        {
            StringBuilder commands = new StringBuilder();
            if (!ContainsKeyMapping())
            {
                commands.AppendLine(Commands.Define(GetKeyTarget()));
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
