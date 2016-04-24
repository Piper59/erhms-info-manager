namespace ERHMS.EpiInfo.Communication
{
    public class TemplateEventArgs : EventArgsBase
    {
        public string TemplatePath { get; private set; }

        public TemplateEventArgs(string templatePath, string tag = null)
            : base(tag)
        {
            TemplatePath = templatePath;
        }

        public override string ToString()
        {
            return ToString(TemplatePath);
        }
    }
}
