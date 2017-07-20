using Epi;
using Epi.Windows.Enter;
using Epi.Windows.Enter.PresentationLogic;
using System;

namespace ERHMS.EpiInfo.Wrappers
{
    internal class MainForm : EnterMainForm
    {
        public GuiMediator Mediator
        {
            get { return GuiMediator.Instance; }
        }

        public MainForm()
        {
            this.Initialize();
        }

        protected override object GetService(Type serviceType)
        {
            if (serviceType == typeof(EnterMainForm))
            {
                return this;
            }
            else
            {
                return base.GetService(serviceType);
            }
        }

        private void OpenRecord(string projectPath, string viewName, string record)
        {
            CurrentProject = new Project(projectPath);
            FireOpenViewEvent(CurrentProject.Views[viewName], record);
        }

        public void OpenRecord(string projectPath, string viewName, int uniqueKey)
        {
            OpenRecord(projectPath, viewName, uniqueKey.ToString());
        }

        public void OpenNewRecord(string projectPath, string viewName)
        {
            OpenRecord(projectPath, viewName, "*");
        }

        public bool TrySetField(string name, string value)
        {
            if (View.Fields.DataFields.Contains(name))
            {
                View.Fields.DataFields[name].CurrentRecordValueString = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public new void Refresh()
        {
            Mediator.Canvas.Render(View.RecStatusField.CurrentRecordValue != 0);
        }
    }
}
