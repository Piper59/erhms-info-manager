using System;
using Epi;
using Epi.Windows.Enter;
using Epi.Windows.Enter.PresentationLogic;
using ERHMS.EpiInfo.Communication;

namespace ERHMS.EpiInfo.Enter
{
    public class MainForm : EnterMainForm
    {
        public MainForm(string tag = null)
        {
            RecordSaved += (sender, e) =>
            {
                IService service = Service.Connect();
                if (service != null)
                {
                    service.OnRecordSaved(e.Form.Project.FilePath, e.Form.Name, e.RecordGuid, tag);
                }
            };
        }

        public MainForm(string projectPath, string viewName, string tag = null)
            : this(tag)
        {
            CurrentProject = new Project(projectPath);
            View = CurrentProject.Views[viewName];
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

        public void SetValue(string fieldName, string value)
        {
            View.Fields.DataFields[fieldName].CurrentRecordValueString = value;
        }

        public new void Refresh()
        {
            GuiMediator.Instance.Canvas.Render(View.RecStatusField.CurrentRecordValue != 0);
        }
    }
}
