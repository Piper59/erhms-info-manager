using System.Windows.Automation;

namespace ERHMS.Test.EpiInfo.Wrappers
{
    public partial class MakeViewTest
    {
        private class MainFormScreen : AutomationElementX
        {
            public readonly AutomationElementX projectTree;

            public MainFormScreen()
                : base(AutomationElement.RootElement.FindFirst(TreeScope.Children, id: "MakeViewMainForm"))
            {
                projectTree = FindFirstX(TreeScope.Descendants, id: "projectTree");
            }

            public AutomationElementX GetCloseDialogScreen()
            {
                return FindFirstX(TreeScope.Children, name: "Close?");
            }

            public CreateTemplateDialogScreen GetCreateTemplateDialogScreen()
            {
                return new CreateTemplateDialogScreen(FindFirst(TreeScope.Children, id: "CreateTemplateDialog"));
            }

            public CreateViewDialogScreen GetCreateViewDialogScreen()
            {
                return new CreateViewDialogScreen(FindFirst(TreeScope.Children, id: "CreateViewDialog"));
            }
        }

        private class CreateTemplateDialogScreen : AutomationElementX
        {
            public readonly AutomationElementX txtTemplateName;
            public readonly AutomationElementX txtDescription;
            public readonly AutomationElementX btnOk;

            public CreateTemplateDialogScreen(AutomationElement element)
                : base(element)
            {
                txtTemplateName = FindFirstX(TreeScope.Descendants, id: "txtTemplateName");
                txtDescription = FindFirstX(TreeScope.Descendants, id: "txtDescription");
                btnOk = FindFirstX(TreeScope.Descendants, id: "btnOk");
            }
        }

        private class CreateViewDialogScreen : AutomationElementX
        {
            public readonly AutomationElementX txtViewName;
            public readonly AutomationElementX btnOk;

            public CreateViewDialogScreen(AutomationElement element)
                : base(element)
            {
                txtViewName = FindFirstX(TreeScope.Descendants, id: "txtViewName");
                btnOk = FindFirstX(TreeScope.Descendants, id: "btnOk");
            }
        }
    }
}
