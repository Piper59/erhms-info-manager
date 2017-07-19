using System;

namespace ERHMS.Test.Dapper
{
    public class Gender
    {
        public string GenderId { get; set; }
        public string Name { get; set; }
        public string Pronouns { get; set; }

        public Gender()
        {
            GenderId = Guid.NewGuid().ToString();
        }
    }
}
