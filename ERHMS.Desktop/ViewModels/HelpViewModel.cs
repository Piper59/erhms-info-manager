using System.Collections.Generic;

namespace ERHMS.Desktop.ViewModels
{
    public class HelpViewModel
    {
        public IEnumerable<string> Sections { get; } = new string[]
        {
            "Home",
            "Phases",
            "Projects",
            "Views",
            "Records",
            "Canvases",
            "Pgms",
            "Maps",
            "Security"
        };
    }
}
