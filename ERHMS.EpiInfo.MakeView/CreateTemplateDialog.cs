using Epi;
using Epi.Windows.Dialogs;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.MakeView
{
    public partial class CreateTemplateDialog : DialogBase
    {
        public Project Project { get; private set; }

        public string TemplateName
        {
            get { return txtTemplateName.Text; }
            set { txtTemplateName.Text = value; }
        }

        public string TemplatePath
        {
            get
            {
                Configuration configuration = Configuration.GetNewInstance();
                return Path.Combine(
                    configuration.Directories.Templates,
                    "Forms",
                    string.Format("{0}{1}", TemplateName, EpiInfo.Template.FileExtension));
            }
        }

        public string Description
        {
            get { return txtDescription.Text; }
            set { txtDescription.Text = value; }
        }

        public CreateTemplateDialog(Project project, string templateName)
        {
            InitializeComponent();
            Project = project;
            TemplateName = templateName;
        }

        private void CreateTemplateDialog_Load(object sender, EventArgs e)
        {
            txtTemplateName.Focus();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            TemplateName = TemplateName.Trim();
            string message;
            if (HasValidTemplateName(out message))
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTemplateName.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private bool HasValidTemplateName(out string message)
        {
            char[] invalidCharacters = Path.GetInvalidFileNameChars();
            if (string.IsNullOrWhiteSpace(TemplateName))
            {
                message = "Please enter a template name.";
                return false;
            }
            else if (TemplateName.Any(character => invalidCharacters.Contains(character)))
            {
                message = string.Format(
                    "Please enter a template name that does not contain any of the following characters:{0}{0}{1}",
                    Environment.NewLine,
                    string.Join(" ", invalidCharacters));
                return false;
            }
            else if (File.Exists(TemplatePath))
            {
                message = "This template name is already in use. Please enter a different template name.";
                return false;
            }
            else
            {
                message = null;
                return true;
            }
        }
    }
}
