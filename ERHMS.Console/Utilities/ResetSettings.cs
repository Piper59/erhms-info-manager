namespace ERHMS.Console.Utilities
{
    public class ResetSettings : IUtility
    {
        public void Run()
        {
            Desktop.Utilities.ResetSettings utility = new Desktop.Utilities.ResetSettings
            {
                Verbose = false
            };
            utility.Invoke();
        }
    }
}
