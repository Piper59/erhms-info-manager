using Epi;
using System.Linq;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.Naming
{
    public class ViewNameValidator
    {
        private static readonly Regex invalidCharRegex = new Regex(@"[^A-Z0-9_]", RegexOptions.IgnoreCase);
        private static readonly Regex invalidStartCharRegex = new Regex(@"^[^A-Z]", RegexOptions.IgnoreCase);

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

        public bool IsIdentical(string viewName)
        {
            return Project.GetTableNames(TableTypes.All).Contains(viewName, NameComparer.Default);
        }

        public bool IsSimilar(string viewName)
        {
            Regex pageTableNameRegex = GetPageTableNameRegex(viewName);
            foreach (string tableName in Project.GetTableNames(TableTypes.All))
            {
                if (pageTableNameRegex.IsMatch(tableName))
                {
                    return true;
                }
            }
            foreach (View view in Project.Views)
            {
                if (GetPageTableNameRegex(view.TableName).IsMatch(viewName))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsValid(string viewName, out InvalidViewNameReason reason)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                reason = InvalidViewNameReason.Empty;
                return false;
            }
            if (viewName.Length > MaxLength)
            {
                reason = InvalidViewNameReason.TooLong;
                return false;
            }
            if (invalidCharRegex.IsMatch(viewName))
            {
                reason = InvalidViewNameReason.InvalidChar;
                return false;
            }
            if (invalidStartCharRegex.IsMatch(viewName))
            {
                reason = InvalidViewNameReason.InvalidStartChar;
                return false;
            }
            if (IsIdentical(viewName))
            {
                reason = InvalidViewNameReason.Identical;
                return false;
            }
            if (IsSimilar(viewName))
            {
                reason = InvalidViewNameReason.Similar;
                return false;
            }
            reason = InvalidViewNameReason.None;
            return true;
        }
    }
}
