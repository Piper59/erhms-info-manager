using ERHMS.EpiInfo.Wrappers;
using System;

namespace ERHMS.Test.Wrappers
{
    public class Test
    {
        internal static void Main(string[] args)
        {
            Wrapper.MainBase(args);
        }

        public class OutTest : Wrapper
        {
            public static Wrapper Create()
            {
                return Create(() => Execute());
            }

            private static void Execute()
            {
                Console.Out.WriteLine("Standard output should be redirected to a null stream.");
                Out.WriteLine("Hello, world!");
            }
        }

        public class InAndOutTest : Wrapper
        {
            public static Wrapper Create()
            {
                return Create(() => Execute());
            }

            private static void Execute()
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
        }

        public class ArgsTest : Wrapper
        {
            public static Wrapper Create(string id, string name, double age, bool male)
            {
                return Create(() => Execute(id, name, age, male));
            }

            private static void Execute(string id, string name, double age, bool male)
            {
                int ageYears = Convert.ToInt32(Math.Truncate(age));
                int ageMonths = Convert.ToInt32(Math.Truncate((age - ageYears) * 12));
                Out.WriteLine("ID = {0}", id == null ? "N/A" : id);
                Out.WriteLine("Name = {0}", name);
                Out.WriteLine("Age = {0} years {1} months", ageYears, ageMonths);
                Out.WriteLine("Gender = {0}", male ? "M" : "F");
            }
        }

        public class ArgLengthTest : Wrapper
        {
            public static Wrapper Create(string value)
            {
                return Create(() => Execute(value));
            }

            private static void Execute(string value)
            {
                Out.WriteLine(value.Length);
            }
        }

        public class EventTypeTest : Wrapper
        {
            public static Wrapper Create()
            {
                return Create(() => Execute());
            }

            private static void Execute()
            {
                Console.Error.WriteLine("Standard error should be redirected to a null stream.");
                RaiseEvent(WrapperEventType.Default);
            }
        }

        public class EventPropertiesTest : Wrapper
        {
            public static Wrapper Create()
            {
                return Create(() => Execute());
            }

            private static void Execute()
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
}
