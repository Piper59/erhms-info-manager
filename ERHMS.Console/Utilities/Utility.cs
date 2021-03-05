using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using static System.Console;

namespace ERHMS.Console.Utilities
{
    public static class Utility
    {
        private const string HelpArg = "/?";

        private static readonly StringComparer ArgComparer = StringComparer.OrdinalIgnoreCase;

        private static readonly IReadOnlyCollection<Type> InstanceTypes = typeof(IUtility).Assembly.GetTypes()
            .Where(type => typeof(IUtility).IsAssignableFrom(type) && !type.IsAbstract)
            .ToList();

        private static string ExecutableName => Environment.GetCommandLineArgs()[0];

        private static string GetUsage()
        {
            StringBuilder usage = new StringBuilder();
            usage.AppendLine("Usage:");
            usage.AppendLine($"  {ExecutableName} {HelpArg}");
            usage.AppendLine($"  {ExecutableName} UTILITY {HelpArg}");
            usage.AppendLine($"  {ExecutableName} UTILITY [ARGUMENT ...]");
            usage.AppendLine();
            usage.Append("Utilities:");
            foreach (Type instanceType in InstanceTypes.OrderBy(instanceType => instanceType.Name, ArgComparer))
            {
                usage.AppendLine();
                usage.Append($"  {instanceType.Name}");
            }
            return usage.ToString();
        }

        private static string GetUsage(Type instanceType)
        {
            StringBuilder usage = new StringBuilder();
            usage.Append("Usage:");
            foreach (ConstructorInfo constructor in instanceType.GetConstructors())
            {
                usage.AppendLine();
                usage.Append($"  {ExecutableName} {instanceType.Name}");
                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    usage.Append($" {parameter.Name}");
                }
            }
            return usage.ToString();
        }

        private static Type GetInstanceType(string instanceTypeName)
        {
            Type instanceType = InstanceTypes
                .SingleOrDefault(_instanceType => ArgComparer.Equals(_instanceType.Name, instanceTypeName));
            if (instanceType == null)
            {
                throw new ArgumentException($"Utility '{instanceTypeName}' does not exist.");
            }
            return instanceType;
        }

        private static ConstructorInfo GetConstructor(Type instanceType, int parameterCount)
        {
            ConstructorInfo constructor = instanceType.GetConstructors()
                    .SingleOrDefault(_constructor => _constructor.GetParameters().Length == parameterCount);
            if (constructor == null)
            {
                throw new ArgumentException(
                    $"Utility '{instanceType.Name}' cannot be created with the specified arguments.");
            }
            return constructor;
        }

        private static IEnumerable<object> GetParameterValues(ConstructorInfo constructor, IReadOnlyList<string> args)
        {
            IReadOnlyList<ParameterInfo> parameters = constructor.GetParameters();
            IList<object> parameterValues = new List<object>(parameters.Count);
            for (int index = 0; index < parameters.Count; index++)
            {
                ParameterInfo parameter = parameters[index];
                try
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(parameter.ParameterType);
                    parameterValues.Add(converter.ConvertFromString(args[index]));
                }
                catch (Exception ex)
                {
                    throw new ArgumentException(
                        $"An error occurred while parsing argument '{parameter.Name}': {ex.Message}");
                }
            }
            return parameterValues;
        }

        public static IUtility ParseArgs(IReadOnlyList<string> args)
        {
            Type instanceType = null;
            try
            {
                if (args.Count == 0)
                {
                    throw new ArgumentException("No utility specified.");
                }
                if (ArgComparer.Equals(args[0], HelpArg))
                {
                    Out.WriteLine(GetUsage());
                    Environment.Exit(ErrorCodes.Success);
                    return null;
                }
                instanceType = GetInstanceType(args[0]);
                if (args.Count > 1 && ArgComparer.Equals(args[1], HelpArg))
                {
                    Out.WriteLine(GetUsage(instanceType));
                    Environment.Exit(ErrorCodes.Success);
                    return null;
                }
                ConstructorInfo constructor = GetConstructor(instanceType, args.Count - 1);
                object[] parameterValues = GetParameterValues(constructor, args.Skip(1).ToList()).ToArray();
                return (IUtility)constructor.Invoke(parameterValues);
            }
            catch (Exception ex)
            {
                ForegroundColor = ConsoleColor.Red;
                Error.WriteLine(ex.Message);
                ResetColor();
                Error.WriteLine();
                Error.WriteLine(instanceType == null ? GetUsage() : GetUsage(instanceType));
                Environment.Exit(ErrorCodes.InvalidCommandLine);
                return null;
            }
        }
    }
}
