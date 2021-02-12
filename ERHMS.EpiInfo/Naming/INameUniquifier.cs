namespace ERHMS.EpiInfo.Naming
{
    public interface INameUniquifier
    {
        bool Exists(string name);
        string Uniquify(string name);
    }
}
