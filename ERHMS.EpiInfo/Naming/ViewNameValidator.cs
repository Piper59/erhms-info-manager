using Epi;
using System.Linq;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.Naming
{
    public class ViewNameValidator : NameValidator
    {
        public static int MaxLength => 40;

        private static Regex GetPageTableNameRegex(string viewTableName)
        {
            return new Regex($@"^{Regex.Escape(viewTableName)}[0-9]+$", RegexOptions.IgnoreCase);
        }

        public Project Project { get; }

        public ViewNameValidator(Project project)
        {
            Project = project;
        }

        protected override int GetMaxLength()
        {
            return MaxLength;
        }

        public bool IsIdentical(string name)
        {
            return Project.GetTableNames(TableTypes.All).Contains(name, NameComparer.Default);
        }

        public bool IsSimilar(string name)
        {
            Regex pageTableNameRegex = GetPageTableNameRegex(name);
            foreach (string tableName in Project.GetTableNames(TableTypes.All))
            {
                if (pageTableNameRegex.IsMatch(tableName))
                {
                    return true;
                }
            }
            foreach (View view in Project.Views)
            {
                if (GetPageTableNameRegex(view.TableName).IsMatch(name))
                {
                    return true;
                }
            }
            return false;
        }

        public override bool IsValid(string name, out InvalidNameReason reason)
        {
            if (!base.IsValid(name, out reason))
            {
                return false;
            }
            if (IsIdentical(name))
            {
                reason = InvalidNameReason.Identical;
                return false;
            }
            if (IsSimilar(name))
            {
                reason = InvalidNameReason.Similar;
                return false;
            }
            reason = InvalidNameReason.None;
            return true;
        }
    }
}
