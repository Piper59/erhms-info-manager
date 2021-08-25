namespace ERHMS.Common.Naming
{
    public interface INameUniquifier
    {
        bool Exists(string name);
        string Uniquify(string name);
    }

    public static class INameUniquifierExtensions
    {
        public static string UniquifyIfExists(this INameUniquifier @this, string name)
        {
            return @this.Exists(name) ? @this.Uniquify(name) : name;
        }
    }
}
