using ERHMS.Desktop.Properties;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace ERHMS.Desktop
{
    public static class Utilities
    {
        public static void Main(string[] args)
        {
            Log.Default.Debug($"Parsing arguments: {string.Join(", ", args)}");
            MethodInfo method = typeof(Utilities).GetMethod(args[0]);
            object[] parameters = GetParameters(method, args).ToArray();
            Log.Default.Debug($"Invoking: {method.Name}");
            method.Invoke(null, GetParameters(method, args).ToArray());
        }

        private static IEnumerable<object> GetParameters(MethodInfo method, string[] args)
        {
            IList<ParameterInfo> parameters = method.GetParameters();
            if (args.Length - 1 != parameters.Count)
            {
                throw new TargetParameterCountException();
            }
            for (int index = 0; index < parameters.Count; index++)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(parameters[index].ParameterType);
                yield return converter.ConvertFromString(args[index + 1]);
            }
        }

        private static void Report(string message)
        {
            MessageBox.Show(message, Resources.AppTitle);
        }

        public static void ResetSettings()
        {
            Settings.Default.Reset();
            Report("Settings have been reset.");
        }
    }
}
