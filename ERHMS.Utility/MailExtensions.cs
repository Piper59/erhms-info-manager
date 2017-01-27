using System.Text.RegularExpressions;

namespace ERHMS.Utility
{
    public static class MailExtensions
    {
        // https://msdn.microsoft.com/en-us/library/01escwtf(v=vs.110).aspx
        private static readonly Regex Address = new Regex(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase);

        public static bool IsValidAddress(string address)
        {
            return Address.IsMatch(address);
        }
    }
}
