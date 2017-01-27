using System;
using System.Security.Cryptography;

namespace ERHMS.Utility
{
    public static class CryptographyExtensions
    {
        public static bool RequiresFipsCompliance()
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
