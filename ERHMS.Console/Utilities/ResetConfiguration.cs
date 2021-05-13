namespace ERHMS.Console.Utilities
{
    public class ResetConfiguration : IUtility
    {
        public bool? FipsCompliant { get; }

        public ResetConfiguration() { }

        public ResetConfiguration(bool fipsCompliant)
            : this()
        {
            FipsCompliant = fipsCompliant;
        }

        public void Run()
        {
            Epi.Configuration configuration = Configuration.Create();
            if (FipsCompliant != null)
            {
                configuration.SetTextEncryptionModule(FipsCompliant.Value);
            }
            Epi.Configuration.Save(configuration);
        }
    }
}
