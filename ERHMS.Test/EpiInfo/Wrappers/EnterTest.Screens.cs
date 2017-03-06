using System.Windows.Automation;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public partial class EnterTest
    {
        private class MainFormScreen : AutomationElementX
        {
            public MainFormScreen()
                : base(AutomationElement.RootElement.FindFirst(TreeScope.Children, id: "EnterMainForm")) { }

            public AutomationElementX GetCloseDialogScreen()
            {
                AutomationElement element = null;
                Condition condition = new PropertyCondition(AutomationElement.ProcessIdProperty, Element.Current.ProcessId);
                AutomationExtensions.TryWait(() =>
                {
                    foreach (AutomationElement window in AutomationElement.RootElement.FindAll(TreeScope.Children, condition))
                    {
                        element = window.FindFirst(TreeScope.Children, name: "Enter", immediate: true);
                        if (element != null)
                        {
                            return true;
                        }
                    }
                    return false;
                });
                return new AutomationElementX(element);
            }

            public void WaitForReady()
            {
                AutomationExtensions.TryWait(() =>
                {
                    AutomationElement splashScreen = FindFirst(TreeScope.Children, id: "SplashScreenForm", immediate: true);
                    return splashScreen == null || splashScreen.Current.IsOffscreen;
                });
            }

            private AutomationElementX GetField(string id)
            {
                Condition condition = new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, id),
                    new PropertyCondition(AutomationElement.IsValuePatternAvailableProperty, true));
                return new AutomationElementX(Element.FindFirst(TreeScope.Descendants, condition));
            }

            public string GetValue(string id)
            {
                return GetField(id).Value.Current.Value;
            }

            public void SetValue(string id, string value)
            {
                GetField(id).Value.SetValue(value);
            }
        }
    }
}
