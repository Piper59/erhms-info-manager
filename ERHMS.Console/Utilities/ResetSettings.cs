namespace ERHMS.Console.Utilities
{
    public class ResetSettings : Utility
    {
        public override void Run()
        {
            Desktop.Utilities.ResetSettings utility = new Desktop.Utilities.ResetSettings
            {
                Verbose = false
            };
            utility.Invoke();
        }
    }
}
