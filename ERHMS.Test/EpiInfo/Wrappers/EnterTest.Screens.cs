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
                AutomationExtensions.Wait(() =>
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
                AutomationExtensions.Wait(() =>
                {
                    AutomationElement splashScreen = FindFirst(TreeScope.Children, id: "SplashScreenForm", immediate: true);
                    return splashScreen == null || splashScreen.Current.IsOffscreen;
                });
            }

            private AutomationElementX GetFieldInternal(string name)
            {
                Condition condition = new AndCondition(
                    new PropertyCondition(AutomationElement.AutomationIdProperty, name),
                    new PropertyCondition(AutomationElement.IsValuePatternAvailableProperty, true));
                return new AutomationElementX(Element.FindFirst(TreeScope.Descendants, condition));
            }

            public string GetField(string name)
            {
                return GetFieldInternal(name).Value.Current.Value;
            }

            public void SetField(string name, string value)
            {
                GetFieldInternal(name).Value.SetValue(value);
            }
        }
    }
}
