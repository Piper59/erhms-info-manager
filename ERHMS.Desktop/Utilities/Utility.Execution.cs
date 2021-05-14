using ERHMS.Common.Logging;
using ERHMS.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Utilities
{
    public static partial class Utility
    {
        private static readonly IEnumerable<Type> instanceTypes = typeof(IUtility).GetInstanceTypes().ToList();

        public static async Task ExecuteAsync(string[] args)
        {
            Type instanceType = GetInstanceType(args[0]);
            Log.Instance.Debug($"Executing utility: {instanceType.Name}");
            IUtility utility = (IUtility)Activator.CreateInstance(instanceType);
            utility.Parameters = args.Skip(1);
            Console.Out.Write(await utility.ExecuteAsync());
            Console.Out.Close();
        }

        private static Type GetInstanceType(string instanceTypeName)
        {
            foreach (Type instanceType in instanceTypes)
            {
                if (instanceType.Name == instanceTypeName)
                {
                    return instanceType;
                }
            }
            throw new ArgumentException($"Utility '{instanceTypeName}' does not exist.");
        }
    }
}
