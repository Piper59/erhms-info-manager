using ERHMS.EpiInfo.Wrappers;
using System;

namespace ERHMS.Test.Wrappers
{
    public class Test : Wrapper
    {
        internal static void Main(string[] args)
        {
            MainBase(typeof(Test), args);
        }

        public static Wrapper OutTest()
        {
            return Create(() => Main_OutTest());
        }
        private static void Main_OutTest()
        {
            Console.Out.WriteLine("Standard output should be redirected to a null stream.");
            Out.WriteLine("Hello, world!");
        }

        public static Wrapper InAndOutTest()
        {
            return Create(() => Main_InAndOutTest());
        }
        private static void Main_InAndOutTest()
        {
            if (Console.In.Peek() != -1)
            {
                throw new InvalidOperationException("Standard input should be redirected to a null stream.");
            }
            while (true)
            {
                string line = In.ReadLine();
                if (line == null)
                {
                    break;
                }
                Out.WriteLine(line);
            }
        }

        public static Wrapper ArgsTest(string id, string name, double age, bool male)
        {
            return Create(() => Main_ArgsTest(id, name, age, male));
        }
        private static void Main_ArgsTest(string id, string name, double age, bool male)
        {
            int ageYears = Convert.ToInt32(Math.Truncate(age));
            int ageMonths = Convert.ToInt32(Math.Truncate((age - ageYears) * 12));
            Out.WriteLine("ID = {0}", id == null ? "N/A" : id);
            Out.WriteLine("Name = {0}", name);
            Out.WriteLine("Age = {0} years {1} months", ageYears, ageMonths);
            Out.WriteLine("Gender = {0}", male ? "M" : "F");
        }

        public static Wrapper LongArgTest(string value)
        {
            return Create(() => Main_LongArgTest(value));
        }
        private static void Main_LongArgTest(string value)
        {
            Out.WriteLine(value.Length);
        }

        public static Wrapper EventTypeTest()
        {
            return Create(() => Main_EventTypeTest());
        }
        private static void Main_EventTypeTest()
        {
            Console.Error.WriteLine("Standard error should be redirected to a null stream.");
            RaiseEvent(WrapperEventType.Default);
        }

        public static Wrapper EventPropertiesTest()
        {
            return Create(() => Main_EventPropertiesTest());
        }
        private static void Main_EventPropertiesTest()
        {
            RaiseEvent(WrapperEventType.Default, new
            {
                Name = "John Doe",
                Age = 20,
                Male = true
            });
        }
    }
}
