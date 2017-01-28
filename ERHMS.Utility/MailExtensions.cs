using System;
using System.Net.Mail;

namespace ERHMS.Utility
{
    public static class MailExtensions
    {
        public static bool IsValidAddress(string address)
        {
            try
            {
                new MailAddress(address);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
