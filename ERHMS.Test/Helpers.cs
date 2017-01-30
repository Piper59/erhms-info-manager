using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

namespace ERHMS.Test
{
    public static class Helpers
    {
        public static DirectoryInfo GetTemporaryDirectory(Expression<Action> expression)
        {
            MethodInfo method = ((MethodCallExpression)expression.Body).Method;
            string directoryName = string.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), directoryName));
        }
    }
}
