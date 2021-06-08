using ERHMS.Desktop.Properties;

namespace ERHMS.Desktop.Infrastructure
{
    public static class BooleanExtensions
    {
        public static string AsString(this bool @this)
        {
            return @this ? ResXResources.Boolean_True : ResXResources.Boolean_False;
        }
    }
}
