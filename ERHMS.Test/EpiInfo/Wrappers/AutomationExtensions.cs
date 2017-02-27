using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public static class AutomationExtensions
    {
        private static readonly TimeSpan WaitMax = TimeSpan.FromSeconds(30.0);
        private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(100.0);

        public static bool TryWait(Func<bool> done)
        {
            DateTime waitStart = DateTime.Now;
            while (true)
            {
                if (done())
                {
                    return true;
                }
                else if ((DateTime.Now - waitStart) > WaitMax)
                {
                    return false;
                }
                Thread.Sleep(Timeout);
            }
        }

        public static AutomationElement FindFirst(this AutomationElement @this, TreeScope scope, AutomationProperty propertyId, object value)
        {
            AutomationElement element = null;
            PropertyCondition condition = new PropertyCondition(propertyId, value);
            TryWait(() =>
            {
                element = @this.FindFirst(scope, condition);
                return element != null;
            });
            return element;
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
        public static void Clear(this TextPattern @this)
        {
            @this.DocumentRange.Select();
            SendKeys.SendWait("{DELETE}");
        }

        public static string Get(this TextPattern @this)
        {
            return @this.DocumentRange.GetText(-1);
        }

        public static void Set(this TextPattern @this, string keys)
        {
            @this.Clear();
            SendKeys.SendWait(keys);
        }
    }

    public static class WindowPatternExtensions
    {
        public static bool WaitForReady(this WindowPattern @this)
        {
            return AutomationExtensions.TryWait(() => @this.Current.WindowInteractionState == WindowInteractionState.ReadyForUserInteraction);
        }
    }
}
