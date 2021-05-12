using Epi.Data;

namespace ERHMS.EpiInfo.Data
{
    public static class IDbDriverExtensions
    {
        public static string Quote(this IDbDriver @this, string identifier)
        {
            return @this.InsertInEscape(identifier);
        }
    }
}
