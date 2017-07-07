using Epi.Windows.MakeView.Forms;
using Epi.Windows.MakeView.PresentationLogic;
using ERHMS.Utility;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.Wrappers
{
    internal class MainForm : MakeViewMainForm
    {
        public GuiMediator Mediator
        {
            get { return mediator; }
        }

        public ProjectExplorer ProjectExplorer
        {
            get { return projectExplorer; }
        }

        public MainForm(bool visible = true)
        {
            this.Initialize();
            if (!visible)
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
        }

        protected override object GetService(Type serviceType)
        {
            if (serviceType == typeof(MakeViewMainForm))
            {
                return this;
            }
            else
            {
                return base.GetService(serviceType);
            }
        }

        public void OpenProject(Project project)
        {
            if (ProjectExplorer.IsProjectLoaded)
            {
                new Invoker
                {
                    Object = this,
                    DeclaringType = typeof(MakeViewMainForm),
                    MethodName = "CloseCurrentProject"
                }.Invoke();
            }
            try
            {
                new Invoker
                {
                    Object = this,
                    DeclaringType = typeof(MakeViewMainForm),
                    MethodName = "OpenProject",
                    ArgTypes = new Type[] { typeof(Epi.Project) }
                }.Invoke(project);
            }
            catch (TargetInvocationException ex)
            {
                NullReferenceException innerEx = ex.InnerException as NullReferenceException;
                if (innerEx == null || innerEx.TargetSite.Name != "get_CurrentView")
                {
                    throw;
                }
            }
        }

        public void OpenProject(string path)
        {
            OpenProject(new Project(path));
        }
    }
}
