using Epi.Windows.Dialogs;
using ERHMS.Utility;
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
                case InvalidViewNameReason.InvalidChar:
                    return "Please enter a form name that contains only letters, numbers, and underscores.";
                case InvalidViewNameReason.InvalidBeginning:
                    return "Please enter a form name that begins with a letter.";
                case InvalidViewNameReason.TooLong:
                    return "Please enter a form name that is no longer than 64 characters.";
                case InvalidViewNameReason.ViewExists:
                    return "This form name is already in use. Please enter a different form name.";
                case InvalidViewNameReason.TableExists:
                    return "A table with this name already exists in the database. Please enter a different form name.";
                default:
                    throw new InvalidEnumValueException(reason);
            }
        }
    }
}
