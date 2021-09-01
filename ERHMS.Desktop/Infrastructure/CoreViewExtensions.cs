using ERHMS.Desktop.Properties;
using ERHMS.Domain;

namespace ERHMS.Desktop.Infrastructure
{
    public static class CoreViewExtensions
    {
        public static string GetTitle(this CoreView @this)
        {
            return Strings.ResourceManager.GetString($"CoreView.Title.{@this.Name}");
        }

        public static string GetHelpText(this CoreView @this)
        {
            return Strings.ResourceManager.GetString($"CoreView.HelpText.{@this.Name}");
        }
    }
}
