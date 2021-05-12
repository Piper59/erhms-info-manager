using Epi;
using System.Linq;

namespace ERHMS.EpiInfo.Naming
{
    public class TableNameUniquifier : NameUniquifier.IntSuffixed
    {
        public Project Project { get; }

        public TableNameUniquifier(Project project)
        {
            Project = project;
        }

        public override bool Exists(string name)
        {
            return Project.GetTableNames(TableTypes.All).Contains(name, NameComparer.Default);
        }
    }
}
