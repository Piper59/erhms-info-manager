namespace ERHMS.Console.Utilities
{
    public class Help : Utility
    {
        protected override void RunCore()
        {
            using (new Highlighter())
            {
                Out.WriteLine(GetUsage());
            }
        }
    }
}
