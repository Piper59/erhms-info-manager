using System;
using System.Security.Cryptography;

namespace ERHMS.Utility
{
    public static class CryptographyExtensions
    {
        public static bool IsFipsCryptoRequired()
        {
            try
            {
                new MD5CryptoServiceProvider();
                return false;
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }
    }
}
