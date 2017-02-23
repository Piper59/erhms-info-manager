using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public class AutomationElementX
    {
        public class DialogExtensions
        {
            private AutomationElementX element;

            public DialogExtensions(AutomationElementX element)
            {
                this.element = element;
            }

            public void Close(DialogResult result)
            {
                string id = ((int)result).ToString();
                AutomationElementX button = element.FindFirstX(TreeScope.Descendants, id: id);
                button.Invoke.Invoke();
            }
        }

        public class SelectionExtensions
        {
            private AutomationElementX element;

            public SelectionExtensions(AutomationElementX element)
            {
                this.element = element;
            }

            private AutomationElementX FindItem(string name)
            {
                return element.FindFirstX(TreeScope.Descendants, name: name);
            }

            public void AddToSelection(string name)
            {
                FindItem(name).SelectionItem.AddToSelection();
            }

            public void Select(string name)
            {
                FindItem(name).SelectionItem.Select();
            }
        }

        private IDictionary<AutomationPattern, BasePattern> patterns;

        public AutomationElement Element { get; private set; }
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

        public AutomationElement FindFirst(TreeScope scope, string id = null, string name = null)
        {
            AutomationProperty propertyId;
            object value;
            if (name == null)
            {
                propertyId = AutomationElement.AutomationIdProperty;
                value = id;
            }
            else
            {
                propertyId = AutomationElement.NameProperty;
                value = name;
            }
            return Element.FindFirst(scope, propertyId, value);
        }

        public AutomationElementX FindFirstX(TreeScope scope, string id = null, string name = null)
        {
            return new AutomationElementX(FindFirst(scope, id, name));
        }

        private TPattern GetPattern<TPattern>(AutomationPattern patternId) where TPattern : BasePattern
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
