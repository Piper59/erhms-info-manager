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
            return Create(nameof(Main_OutTest));
        }
        private static void Main_OutTest()
        {
            Console.Out.WriteLine("Standard output should be redirected to a null stream.");
            Out.WriteLine("Hello, world!");
        }

        public static Wrapper InAndOutTest()
        {
            return Create(nameof(Main_InAndOutTest));
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

        public class ArgsTestArgs : WrapperArgsBase
        {
            public string Name { get; set; }
            public int Age { get; set; }
            public bool Male { get; set; }
            public DayOfWeek BirthDay { get; set; }
        }
        public static Wrapper ArgsTest(string name, int age, bool male, DayOfWeek birthDay)
        {
            return Create(nameof(Main_ArgsTest), new ArgsTestArgs
            {
                Name = name,
                Age = age,
                Male = male,
                BirthDay = birthDay
            });
        }
        private static void Main_ArgsTest(ArgsTestArgs args)
        {
            Out.WriteLine("{0} is {1} years old.", args.Name, args.Age);
            Out.WriteLine("{0} will turn {1} on {2}.", args.Male ? "He" : "She", args.Age + 1, args.BirthDay);
        }

        public static Wrapper EventTest()
        {
            return Create(nameof(Main_EventTest));
        }
        private static void Main_EventTest()
        {
            Console.Error.WriteLine("Standard error should be redirected to a null stream.");
            RaiseEvent(WrapperEventType.Default, new
            {
                Empty = "",
                Message = "'Hello, world!'",
                Math = "1 + 2 + 3 = 6",
                Logic = "A & B & C = D",
                Number = 42
            });
        }
    }
}
