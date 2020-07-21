using ERHMS.Desktop.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ERHMS.Desktop.Utilities
{
    public abstract class Utility : IUtility
    {
        public static readonly IDictionary<string, Type> Types = typeof(IUtility).Assembly.GetTypes()
            .Where(type => typeof(IUtility).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
            .ToDictionary(type => type.Name, type => type, StringComparer.OrdinalIgnoreCase);

        public static IUtility Create(string typeName, IList<string> args)
        {
            if (!Types.TryGetValue(typeName, out Type type))
            {
                throw new ArgumentException($"The utility '{typeName}' could not be found.");
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
                message.Append($"The '{constructor.DeclaringType.Name}' utility must be invoked with ");
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
                yield return Convert.ChangeType(args[index], parameters[index].ParameterType);
            }
        }

        public abstract bool LongRunning { get; }
        public IProgress<string> Progress { get; set; }

        protected abstract Task<string> RunCoreAsync();

        public async Task RunAsync()
        {
            Log.Default.Debug($"Running: {this}");
            Progress?.Report("Running");
            string result;
            try
            {
                result = await RunCoreAsync();
                Progress?.Report("Completed");
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex);
                result = $"An error occurred while running the '{this}' utility.";
                Progress?.Report(ex.ToString());
                Progress?.Report("Completed with errors");
            }
            Log.Default.Debug($"Completed: {this}");
            MessageBox.Show(result, Resources.AppTitle);
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
