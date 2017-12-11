using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ERHMS.Test
{
    public static class AutomationExtensions
    {
        private static readonly TimeSpan WaitMax = TimeSpan.FromSeconds(60.0);
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(0.1);

        public static bool Wait(Func<bool> done)
        {
            DateTime start = DateTime.Now;
            while (true)
            {
                if (done())
                {
                    return true;
                }
                else if ((DateTime.Now - start) > WaitMax)
                {
                    return false;
                }
                Thread.Sleep(Timeout);
            }
        }

        public static AutomationElement FindFirst(this AutomationElement @this, TreeScope scope, Condition condition, bool immediate = false)
        {
            AutomationElement element = null;
            Wait(() =>
            {
                element = @this.FindFirst(scope, condition);
                return immediate || element != null;
            });
            return element;
        }

        public static AutomationElement FindFirst(this AutomationElement @this, TreeScope scope, AutomationProperty property, object value, bool immediate = false)
        {
            return @this.FindFirst(scope, new PropertyCondition(property, value), immediate);
        }

        public static AutomationElement FindFirst(this AutomationElement @this, TreeScope scope, string id = null, string name = null, bool immediate = false)
        {
            Condition condition;
            if (id != null)
            {
                condition = new PropertyCondition(AutomationElement.AutomationIdProperty, id);
            }
            else if (name != null)
            {
                condition = new PropertyCondition(AutomationElement.NameProperty, name);
            }
            else
            {
                condition = Condition.TrueCondition;
            }
            return @this.FindFirst(scope, condition, immediate);
        }

        public static AutomationElement GetParent(this AutomationElement @this)
        {
            return TreeWalker.ControlViewWalker.GetParent(@this);
        }

        public static IEnumerable<AutomationElement> GetChildren(this AutomationElement @this)
        {
            return @this.FindAll(TreeScope.Children, Condition.TrueCondition).Cast<AutomationElement>();
        }
    }

    public static class TextPatternExtensions
    {
        private static readonly Regex SpecialCharPattern = new Regex(@"[\+\^\%\~\(\)\{\}\[\]]");

        public static void Clear(this TextPattern @this)
        {
            @this.DocumentRange.Select();
            SendKeys.SendWait("{DELETE}");
        }

        public static string Get(this TextPattern @this)
        {
            return @this.DocumentRange.GetText(-1);
        }

        public static void Set(this TextPattern @this, string text)
        {
            @this.Clear();
            SendKeys.SendWait(Escape(text));
        }

        private static string Escape(string text)
        {
            return SpecialCharPattern.Replace(text, match => string.Format("{{{0}}}", match.Value));
        }
    }

    public static class WindowPatternExtensions
    {
        public static bool WaitForReady(this WindowPattern @this)
        {
            return AutomationExtensions.Wait(() => @this.Current.WindowInteractionState == WindowInteractionState.ReadyForUserInteraction);
        }
    }
}
