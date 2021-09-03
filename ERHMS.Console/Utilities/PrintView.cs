using Epi;
using Epi.Windows.Enter;
using Epi.Windows.Enter.PresentationLogic;
using ERHMS.Console.Infrastructure;
using ERHMS.EpiInfo;
using System;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.Console.Utilities
{
    public class PrintView : Utility
    {
        private static readonly Lazy<EnterMainForm> form = new Lazy<EnterMainForm>(GetForm);
        private static EnterMainForm Form => form.Value;

        private static EnterMainForm GetForm()
        {
            Application.EnableVisualStyles();
            return new EnterMainForm();
        }

        public static void Run(View view, string filePath)
        {
            Form.FireOpenViewEvent(view, "*");
            GuiMediator.Instance.PrintToFile(filePath);
        }

        public string ProjectPath { get; }
        public string ViewName { get; }
        public string FilePath { get; }

        public PrintView(string projectPath, string viewName, string filePath)
        {
            ProjectPath = projectPath;
            ViewName = viewName;
            FilePath = filePath;
        }

        public override void Run()
        {
            Project project = ProjectExtensions.Open(ProjectPath);
            View view = project.Views[ViewName];
            Run(view, FilePath);
        }
    }
}
