namespace ERHMS.EpiInfo
{
    public class InMemoryProject : ProjectBase
    {
        public InMemoryProject(string driver, string connectionString)
            : base(driver, connectionString)
        { }

        public override void Save() { }
    }
}
