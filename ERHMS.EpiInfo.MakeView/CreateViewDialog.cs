using Epi.Windows.Dialogs;
using System;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.MakeView
{
    public partial class CreateViewDialog : DialogBase
    {
        public Project Project { get; private set; }

        public string ViewName
        {
            get { return txtViewName.Text; }
            set { txtViewName.Text = value; }
        }

        public CreateViewDialog(Project project, string viewName = "")
        {
            InitializeComponent();
            Project = project;
            ViewName = viewName;
        }

        private void CreateViewDialog_Load(object sender, EventArgs e)
        {
            txtViewName.Select(ViewName.Length, 0);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            InvalidViewNameReason reason;
            if (Project.IsValidViewName(ViewName, out reason))
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(GetErrorMessage(reason), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtViewName.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private string GetErrorMessage(InvalidViewNameReason reason)
        {
            switch (reason)
            {
                case InvalidViewNameReason.Empty:
                    return "Please enter a form name.";
                case InvalidViewNameReason.InvalidCharacter:
                    return "Please enter a form name that contains only letters, numbers, and underscores.";
                case InvalidViewNameReason.InvalidFirstCharacter:
                    return "Please enter a form name that begins with a letter.";
                case InvalidViewNameReason.Duplicate:
                    return "This form name is already in use. Please enter a different form name.";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
