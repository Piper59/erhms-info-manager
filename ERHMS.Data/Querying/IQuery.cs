namespace ERHMS.Data.Querying
{
    public interface IQuery
    {
        string Sql { get; }
        object Parameters { get; }
    }
}
