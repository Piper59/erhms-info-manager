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
                string message;
                switch (reason)
                {
                    case InvalidViewNameReason.Empty:
                        message = "Please enter a form name.";
                        break;
                    case InvalidViewNameReason.InvalidCharacter:
                        message = "Please enter a form name that contains only letters, numbers, and underscores.";
                        break;
                    case InvalidViewNameReason.InvalidFirstCharacter:
                        message = "Please enter a form name that begins with a letter.";
                        break;
                    case InvalidViewNameReason.Duplicate:
                        message = "This form name is already in use. Please enter a different form name.";
                        break;
                    default:
                        throw new NotSupportedException();
                }
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtViewName.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
