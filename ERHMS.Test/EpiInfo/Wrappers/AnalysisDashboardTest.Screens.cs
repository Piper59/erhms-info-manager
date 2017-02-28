using System.Windows.Automation;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public partial class AnalysisDashboardTest
    {
        private class MainFormScreen : AutomationElementX
        {
            public readonly AutomationElementX scrollViewer;
            public readonly AutomationElementX standardTextBox;

            public MainFormScreen()
                : base(null)
            {
                AutomationElement scrollViewerElement = null;
                AutomationExtensions.TryWait(() =>
                {
                    Element = AutomationElement.RootElement.FindFirst(TreeScope.Children, id: "DashboardMainForm");
                    scrollViewerElement = FindFirst(TreeScope.Descendants, id: "scrollViewer", immediate: true);
                    return scrollViewerElement != null;
                });
                scrollViewer = new AutomationElementX(scrollViewerElement);
                standardTextBox = FindFirstX(TreeScope.Descendants, id: "standardTextBox");
            }

            public AutomationElementX GetQuestionDialogScreen()
            {
                return FindFirstX(TreeScope.Children, name: "Question");
            }

            public void WaitForReady()
            {
                AutomationElement loadingPanel = FindFirst(TreeScope.Descendants, id: "loadingPanel");
                AutomationExtensions.TryWait(() => loadingPanel.Current.IsOffscreen);
            }
        }
    }
}
