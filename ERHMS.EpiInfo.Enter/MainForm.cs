using Epi;
using Epi.Windows.Enter;
using Epi.Windows.Enter.PresentationLogic;
using ERHMS.EpiInfo.Communication;

namespace ERHMS.EpiInfo.Enter
{
    public class MainForm : EnterMainForm
    {
        public MainForm()
        {
            RecordSaved += (sender, e) =>
            {
                IService service = Service.GetService();
                if (service == null)
                {
                    return;
                }
                service.RefreshRecordData(e.Form.Project.FilePath, e.Form.Name, e.RecordGuid);
            };
        }

        public MainForm(string projectPath, string viewName)
            : this()
        {
            CurrentProject = new Project(projectPath);
            View = CurrentProject.Views[viewName];
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
