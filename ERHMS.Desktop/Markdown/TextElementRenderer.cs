using System.Windows;
using System.Windows.Documents;

namespace ERHMS.Desktop.Markdown
{
    public abstract class TextElementRenderer
    {
        protected static bool SetStyle(TextElement element, object key)
        {
            if (key != null && Application.Current.TryFindResource(key) is Style style)
            {
                element.Style = style;
                return true;
            }
            else
            {
                return false;
            }
        }

        public RenderingContext Context { get; }

        protected TextElementRenderer(RenderingContext context)
        {
            Context = context;
        }
    }
}
