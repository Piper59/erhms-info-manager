﻿using ERHMS.Common;
using ERHMS.Common.Reflection;
using ERHMS.Common.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static System.Console;

namespace ERHMS.Console.Utilities
{
    public abstract class Utility : IUtility
    {
        private static readonly IEnumerable<Type> instanceTypes = typeof(IUtility).GetInstanceTypes().ToList();
        private static readonly string executableName = Environment.GetCommandLineArgs()[0];
        private static readonly string helpArg = "/?";

        protected static TextReader In => System.Console.In;
        protected static TextWriter Out => System.Console.Out;
        protected static TextWriter Error => System.Console.Error;

        private static string GetUsage()
        {
            StringBuilder usage = new StringBuilder();
            usage.AppendLine("Usage:");
            usage.AppendLine($"  {executableName} {helpArg}");
            usage.AppendLine($"  {executableName} <utility> {helpArg}");
            usage.AppendLine($"  {executableName} <utility> [<argument> ...]");
            usage.AppendLine();
            usage.Append("Utilities:");
            foreach (Type instanceType in instanceTypes.OrderBy(instanceType => instanceType.Name, Comparers.Arg))
            {
                usage.AppendLine();
                usage.Append($"  {instanceType.Name}");
            }
            return usage.ToString();
        }

        private static string GetUsage(Type instanceType)
        {
            if (instanceType == null)
            {
                return GetUsage();
            }
            StringBuilder usage = new StringBuilder();
            usage.Append("Usage:");
            foreach (ConstructorInfo constructor in instanceType.GetConstructors())
            {
                usage.AppendLine();
                usage.Append($"  {executableName} {instanceType.Name}");
                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    usage.Append($" <{parameter.Name}>");
                }
            }
            return usage.ToString();
        }

        private static Type GetInstanceType(string instanceTypeName)
        {
            foreach (Type instanceType in instanceTypes)
            {
                if (Comparers.Arg.Equals(instanceType.Name, instanceTypeName))
                {
                    return instanceType;
                }
            }
            throw new ArgumentException($"Utility '{instanceTypeName}' does not exist.");
        }

        private static ConstructorInfo GetConstructor(Type instanceType, int parameterCount)
        {
            foreach (ConstructorInfo constructor in instanceType.GetConstructors())
            {
                if (constructor.GetParameters().Length == parameterCount)
                {
                    return constructor;
                }
            }
            throw new ArgumentException(
                $"Utility '{instanceType.Name}' cannot be executed with the specified arguments.");
        }

        private static object GetParameter(ParameterInfo parameter, string arg)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(parameter.ParameterType);
                return converter.ConvertFromString(arg);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"An error occurred while parsing argument '{parameter.Name}': {ex.Message}");
            }
        }

        public static IUtility ParseArgs(string[] args)
        {
            Type instanceType = null;
            try
            {
                if (args.Length == 0)
                {
                    throw new ArgumentException("No utility was specified.");
                }
                if (Comparers.Arg.Equals(args[0], helpArg))
                {
                    Out.WriteLine(GetUsage());
                    Environment.Exit(ErrorCodes.Success);
                    return null;
                }
                instanceType = GetInstanceType(args[0]);
                if (args.Length > 1 && Comparers.Arg.Equals(args[1], helpArg))
                {
                    Out.WriteLine(GetUsage(instanceType));
                    Environment.Exit(ErrorCodes.Success);
                    return null;
                }
                ConstructorInfo constructor = GetConstructor(instanceType, args.Length - 1);
                object[] parameters = constructor.GetParameters()
                    .Zip(args.Skip(1), GetParameter)
                    .ToArray();
                return (IUtility)constructor.Invoke(parameters);
            }
            catch (Exception ex)
            {
                ForegroundColor = ConsoleColor.Red;
                Error.WriteLine(ex.Message);
                ResetColor();
                Error.WriteLine();
                Error.WriteLine(GetUsage(instanceType));
                Environment.Exit(ErrorCodes.InvalidCommandLine);
                return null;
            }
        }

        protected static string GetPassword()
        {
            Error.Write("Password: ");
            try
            {
                return ConsoleExtensions.ReadPassword();
            }
            finally
            {
                Error.WriteLine();
            }
        }

        public abstract void Run();
    }
}
