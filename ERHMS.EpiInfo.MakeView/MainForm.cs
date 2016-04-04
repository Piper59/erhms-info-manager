using Epi.Windows.MakeView.Forms;
using Epi.Windows.MakeView.PresentationLogic;
using ERHMS.Utility;
using System;
using System.IO;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.MakeView
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
                if (Mediator.Project == project)
                {
                    return;
                }
                this.Invoke("CloseCurrentProject", Type.EmptyTypes, null);
            }
            this.Invoke("OpenProject", new Type[] { typeof(Epi.Project) }, new object[] { project });
        }

        public void OpenProject(string projectPath)
        {
            Project project = new Project(projectPath);
            OpenProject(project);
        }
    }
}
