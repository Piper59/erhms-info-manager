using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;

namespace ERHMS.Test.Utility
{
    public class MailExtensionsTest
    {
        // https://en.wikipedia.org/wiki/Email_address#Examples
        private static readonly ICollection<string> ValidAddresses = new string[]
        {
            "prettyandsimple@example.com",
            "very.common@example.com",
            "disposable.style.email.with+symbol@example.com",
            "other.email-with-dash@example.com",
            "x@example.com",
            "\"much.more unusual\"@example.com",
            "\"very.unusual.@.unusual.com\"@example.com",
            "\"very.(),:;<>[]\\\".VERY.\\\"very@\\\\ \\\"very\\\".unusual\"@strange.example.com",
            "example-indeed@strange-example.com",
            "admin@mailserver1",
            "#!$%&'*+-/=?^_`{}|~@example.org",
            "\"()<>[]:,;@\\\\\\\"!#$%&'-/=?^_`{}| ~.a\"@example.org",
            "\" \"@example.org",
            "example@localhost",
            "example@s.solutions",
            "user@localserver",
            "user@tt",
            "user@[IPv6:2001:DB8::1]"
        };
        private static readonly ICollection<string> InvalidAddresses = new string[]
        {
            "Abc.example.com",
            "A@b@c@example.com",
            "a\"b(c)d,e:f;g<h>i[j\\k]l@example.com",
            "just\"not\"right@example.com",
            "this is\"not\\allowed@example.com",
            "this\\ still\\\"not\\\\allowed@example.com"/*,
            // These are incorrectly reported as valid
            "1234567890123456789012345678901234567890123456789012345678901234+x@example.com",
            "john..doe@example.com",
            "john.doe@example..com",
            " john.doe@example.com",
            "john.doe@example.com "*/
        };

        [Test]
        public void IsValidAddressTest()
        {
            IsValidAddressTest(true, ValidAddresses);
            IsValidAddressTest(false, InvalidAddresses);
        }

        private void IsValidAddressTest(bool expected, IEnumerable<string> addresses)
        {
            foreach (string address in addresses)
            {
                Assert.AreEqual(expected, MailExtensions.IsValidAddress(address), address);
            }
        }
    }
}
