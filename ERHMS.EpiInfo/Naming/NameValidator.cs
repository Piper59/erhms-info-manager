using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.Naming
{
    public abstract class NameValidator
    {
        private static readonly Regex invalidCharRegex = new Regex(@"[^A-Z0-9_]", RegexOptions.IgnoreCase);
        private static readonly Regex invalidStartCharRegex = new Regex(@"^[^A-Z]", RegexOptions.IgnoreCase);

        protected abstract int GetMaxLength();

        public virtual bool IsValid(string name, out InvalidNameReason reason)
        {
            if (string.IsNullOrEmpty(name))
            {
                reason = InvalidNameReason.Empty;
                return false;
            }
            if (name.Length > GetMaxLength())
            {
                reason = InvalidNameReason.TooLong;
                return false;
            }
            if (invalidCharRegex.IsMatch(name))
            {
                reason = InvalidNameReason.InvalidChar;
                return false;
            }
            if (invalidStartCharRegex.IsMatch(name))
            {
                reason = InvalidNameReason.InvalidStartChar;
                return false;
            }
            reason = InvalidNameReason.None;
            return true;
        }
    }
}
