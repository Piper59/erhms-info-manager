namespace ERHMS.Console.Utilities
{
    public class ResetConfiguration : Utility
    {
        public bool? FipsCompliant { get; }

        public ResetConfiguration() { }

        public ResetConfiguration(bool fipsCompliant)
            : this()
        {
            FipsCompliant = fipsCompliant;
        }

        public override void Run()
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
