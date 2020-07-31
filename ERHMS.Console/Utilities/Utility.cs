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
        public static readonly IReadOnlyDictionary<string, Type> Types = typeof(IUtility).Assembly.GetTypes()
            .Where(type => typeof(IUtility).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
            .ToDictionary(type => type.Name, type => type, StringComparer.OrdinalIgnoreCase);

        public static IUtility Create(string typeName, IList<string> args)
        {
            if (!Types.TryGetValue(typeName, out Type type))
            {
                throw new ArgumentException($"Utility '{typeName}' could not be found.");
            }
            ConstructorInfo constructor = type.GetConstructors().Single();
            object[] parameters = GetParameters(constructor, args).ToArray();
            return (IUtility)constructor.Invoke(parameters);
        }

        private static IEnumerable<object> GetParameters(ConstructorInfo constructor, IList<string> args)
        {
            IList<ParameterInfo> parameters = constructor.GetParameters();
            if (args.Count != parameters.Count)
            {
                StringBuilder message = new StringBuilder();
                message.Append($"Utility '{constructor.DeclaringType.Name}' must be invoked with ");
                if (parameters.Count == 0)
                {
                    message.Append("no arguments");
                }
                else
                {
                    message.Append("the following arguments: ");
                    message.Append(string.Join(", ", parameters.Select(parameter => parameter.Name)));
                }
                message.Append(".");
                throw new ArgumentException(message.ToString());
            }
            for (int index = 0; index < parameters.Count; index++)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(parameters[index].ParameterType);
                yield return converter.ConvertFromString(args[index]);
            }
        }

        protected static TextReader In => System.Console.In;
        protected static TextWriter Out => System.Console.Out;
        protected static TextWriter Error => System.Console.Error;

        protected abstract void RunCore();

        public void Run()
        {
            Log.Default.Info("Running");
            try
            {
                RunCore();
                Log.Default.Info("Completed");
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex.Message);
                Log.Default.Debug(ex.StackTrace);
                Log.Default.Warn("Completed with errors");
            }
        }
    }
}
