using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Common
{
    public static class ConsoleExtensions
    {
        public static string ReadPassword()
        {
            IList<char> password = new List<char>();
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (password.Count > 0)
                    {
                        password.RemoveAt(password.Count - 1);
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    password.Add(keyInfo.KeyChar);
                }
            }
            return new string(password.ToArray());
        }
    }
}
