using System;

namespace ERHMS.EpiInfo.Templating
{
    public class InstantiatingEventArgs : EventArgs
    {
        public TemplateLevel Level { get; }
        public string Name { get; }

        public InstantiatingEventArgs(TemplateLevel level, string name)
        {
            Level = level;
            Name = name;
        }
    }
}
