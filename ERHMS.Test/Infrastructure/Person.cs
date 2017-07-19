using ERHMS.Utility;
using System;

namespace ERHMS.Test.Dapper
{
    public class Person
    {
        public string PersonId { get; set; }
        public string GenderId { get; set; }
        public Gender Gender { get; set; }
        public string Name { get; set; }

        private DateTime? birthDate;
        public DateTime? BirthDate
        {
            get { return birthDate; }
            set { birthDate = value?.RemoveMilliseconds(); }
        }

        public double Height { get; set; }
        public double Weight { get; set; }

        public double Bmi
        {
            get { return Weight / (Height * Height * 144.0) * 703.0; }
        }

        public Person()
        {
            PersonId = Guid.NewGuid().ToString();
        }
    }
}
