namespace ERHMS.Common.Naming
{
    public interface INameUniquifier
    {
        bool Exists(string name);
        string Uniquify(string name);
    }
}
