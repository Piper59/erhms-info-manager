using Epi;
using ERHMS.EpiInfo;

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
            Configuration configuration = ConfigurationExtensions.Create();
            if (FipsCompliant != null)
            {
                configuration.SetTextEncryptionModule(FipsCompliant.Value);
            }
            configuration.Save();
        }
    }
}
