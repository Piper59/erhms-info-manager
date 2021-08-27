namespace ERHMS.Desktop.Markdown
{
    public abstract class TextElementRenderer
    {
        public RenderingContext Context { get; }

        protected TextElementRenderer(RenderingContext context)
        {
            Context = context;
        }
    }
}
