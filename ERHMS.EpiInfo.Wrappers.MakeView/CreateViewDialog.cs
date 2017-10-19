using Epi.Windows.Dialogs;
using System;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.Wrappers
{
    internal partial class CreateViewDialog : DialogBase
    {
        public Project Project { get; private set; }

        public string ViewName
        {
            get { return txtViewName.Text.Trim(); }
            set { txtViewName.Text = value; }
        }

        public CreateViewDialog(Project project, string viewName)
        {
            InitializeComponent();
            Project = project;
            ViewName = viewName;
        }

        private void CreateViewDialog_Load(object sender, EventArgs e)
        {
            txtViewName.Select(txtViewName.Text.Length, 0);
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
                MessageBox.Show(reason.GetErrorMessage(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtViewName.Focus();
            }
        }
    }
}
