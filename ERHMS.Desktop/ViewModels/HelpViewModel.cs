using ERHMS.Desktop.Properties;
using System.Collections.Generic;

namespace ERHMS.Desktop.ViewModels
{
    public class HelpViewModel
    {
        public class Section
        {
            public string Name { get; }
            public string Title { get; }
            public string Content { get; }

            public Section(string name)
            {
                Name = name;
                Title = Strings.ResourceManager.GetString($"Title.{name}");
                Content = Strings.ResourceManager.GetString($"Markdown.{name}");
            }
        }

        public IEnumerable<Section> Sections { get; } = new Section[]
        {
            new Section("Introduction"),
            new Section("Phases"),
            new Section("Projects"),
            new Section("Views"),
            new Section("Records"),
            new Section("Canvases"),
            new Section("Pgms"),
            new Section("Maps"),
            new Section("Security")
        };
    }
}
