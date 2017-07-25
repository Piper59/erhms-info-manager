using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public class AutomationElementX
    {
        public class DialogExtensions
        {
            private AutomationElementX parent;

            public DialogExtensions(AutomationElementX parent)
            {
                this.parent = parent;
            }

            public void Close(DialogResult result)
            {
                AutomationElementX button = parent.FindFirstX(TreeScope.Descendants, id: ((int)result).ToString());
                button.Invoke.Invoke();
            }
        }

        public class SelectionExtensions
        {
            private AutomationElementX parent;

            public SelectionExtensions(AutomationElementX parent)
            {
                this.parent = parent;
            }

            public IEnumerable<AutomationElement> Get()
            {
                return parent.GetPattern<SelectionPattern>(SelectionPattern.Pattern).Current.GetSelection();
            }

            private AutomationElementX Find(string name)
            {
                return parent.FindFirstX(TreeScope.Descendants, name: name);
            }

            public void Add(string name)
            {
                Find(name).SelectionItem.AddToSelection();
            }

            public void Set(string name)
            {
                Find(name).SelectionItem.Select();
            }
        }

        private IDictionary<AutomationPattern, BasePattern> patterns;

        public AutomationElement Element { get; protected set; }
        public DialogExtensions Dialog { get; private set; }
        public SelectionExtensions Selection { get; private set; }

        public InvokePattern Invoke
        {
            get { return GetPattern<InvokePattern>(InvokePattern.Pattern); }
        }

        public SelectionItemPattern SelectionItem
        {
            get { return GetPattern<SelectionItemPattern>(SelectionItemPattern.Pattern); }
        }

        public TextPattern Text
        {
            get { return GetPattern<TextPattern>(TextPattern.Pattern); }
        }

        public ValuePattern Value
        {
            get { return GetPattern<ValuePattern>(ValuePattern.Pattern); }
        }

        public WindowPattern Window
        {
            get { return GetPattern<WindowPattern>(WindowPattern.Pattern); }
        }

        public AutomationElementX(AutomationElement element)
        {
            Element = element;
            Dialog = new DialogExtensions(this);
            Selection = new SelectionExtensions(this);
            patterns = new Dictionary<AutomationPattern, BasePattern>();
        }

        public AutomationElement FindFirst(TreeScope scope, string id = null, string name = null, bool immediate = false)
        {
            return Element.FindFirst(scope, id, name, immediate);
        }

        public AutomationElementX FindFirstX(TreeScope scope, string id = null, string name = null, bool immediate = false)
        {
            return new AutomationElementX(Element.FindFirst(scope, id, name, immediate));
        }

        private TPattern GetPattern<TPattern>(AutomationPattern patternId)
            where TPattern : BasePattern
        {
            BasePattern pattern;
            if (!patterns.TryGetValue(patternId, out pattern))
            {
                pattern = (BasePattern)Element.GetCurrentPattern(patternId);
                patterns.Add(patternId, pattern);
            }
            return (TPattern)pattern;
        }
    }
}
