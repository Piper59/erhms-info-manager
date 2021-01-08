using ERHMS.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ERHMS.Console.Utilities
{
    public abstract class Utility : IUtility
    {
        private static readonly IDictionary<string, Type> subclasses = typeof(IUtility).Assembly.GetTypes()
            .Where(type => typeof(IUtility).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
            .ToDictionary(subclass => subclass.Name, StringComparer.OrdinalIgnoreCase);

        private static string ProgramName => Assembly.GetEntryAssembly().GetName().Name;
        protected static TextReader In => System.Console.In;
        protected static TextWriter Out => System.Console.Out;
        protected static TextWriter Error => System.Console.Error;

        public static string GetUsage()
        {
            StringBuilder usage = new StringBuilder();
            usage.AppendLine("usage:");
            usage.AppendLine($"  {ProgramName} UTILITY [ARGUMENT ...]");
            usage.AppendLine();
            usage.Append("utilities:");
            foreach (string subclassName in subclasses.Keys.OrderBy(subclassName => subclassName))
            {
                usage.AppendLine();
                usage.Append($"  {subclassName}");
            }
            return usage.ToString();
        }

        public static string GetUsage(Type subclass)
        {
            StringBuilder usage = new StringBuilder();
            usage.Append("usage:");
            foreach (ConstructorInfo constructor in subclass.GetConstructors())
            {
                usage.AppendLine();
                usage.Append($"  {ProgramName} {subclass.Name}");
                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    usage.Append($" {parameter.Name}");
                }
            }
            return usage.ToString();
        }

        public static IUtility Create(string utilityName, IList<string> args)
        {
            if (!subclasses.TryGetValue(utilityName, out Type subclass))
            {
                throw new ArgumentException($"Utility '{utilityName}' does not exist.");
            }
            ConstructorInfo constructor = GetConstructor(subclass, args.Count);
            object[] parameters = GetParameters(constructor, args).ToArray();
            return (IUtility)constructor.Invoke(parameters);
        }

        private static ConstructorInfo GetConstructor(Type subclass, int parameterCount)
        {
            try
            {
                return subclass.GetConstructors().Single(constructor => constructor.GetParameters().Length == parameterCount);
            }
            catch (InvalidOperationException)
            {
                using (new Highlighter())
                {
                    Error.WriteLine(GetUsage(subclass));
                }
                throw new ArgumentException($"Utility '{subclass.Name}' cannot be invoked with the specified arguments.");
            }
        }

        private static IEnumerable<object> GetParameters(ConstructorInfo constructor, IList<string> args)
        {
            IList<ParameterInfo> parameters = constructor.GetParameters();
            for (int index = 0; index < parameters.Count; index++)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(parameters[index].ParameterType);
                yield return converter.ConvertFromString(args[index]);
            }
        }

        protected abstract void RunCore();

        public void Run()
        {
            Log.Instance.Info("Running");
            try
            {
                RunCore();
                Log.Instance.Info("Completed");
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex.Message);
                Log.Instance.Debug(ex.StackTrace);
                Log.Instance.Warn("Completed with errors");
            }
        }
    }
}
