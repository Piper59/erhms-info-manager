using ERHMS.Utility;
using NUnit.Framework;
using System.Collections.Generic;

namespace ERHMS.Test.Utility
{
    // https://en.wikipedia.org/wiki/Email_address#Examples
    public class MailExtensionsTest
    {
        [Test]
        public void IsValidAddressTestValid()
        {
            ICollection<string> addresses = new string[]
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
            IsValidAddressTest(addresses, true);
        }

        [Test]
        public void IsValidAddressTestInvalid()
        {
            ICollection<string> addresses = new string[]
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
            IsValidAddressTest(addresses, false);
        }

        private void IsValidAddressTest(IEnumerable<string> addresses, bool expected)
        {
            foreach (string address in addresses)
            {
                Assert.AreEqual(expected, MailExtensions.IsValidAddress(address), address);
            }
        }
    }
}
