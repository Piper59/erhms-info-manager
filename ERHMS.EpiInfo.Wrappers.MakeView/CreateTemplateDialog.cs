using Epi;
using Epi.Windows.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.Wrappers
{
    internal partial class CreateTemplateDialog : DialogBase
    {
        private static readonly ICollection<char> InvalidChars = Path.GetInvalidFileNameChars().ToList();

        public string TemplateName
        {
            get { return txtTemplateName.Text.Trim(); }
            set { txtTemplateName.Text = value; }
        }

        public string TemplatePath
        {
            get
            {
                Configuration configuration = Configuration.GetNewInstance();
                return Path.Combine(
                    configuration.Directories.Templates,
                    TemplateLevel.View.ToDirectoryName(),
                    TemplateName + TemplateInfo.FileExtension);
            }
        }

        public string Description
        {
            get { return txtDescription.Text.Trim(); }
            set { txtDescription.Text = value; }
        }

        public CreateTemplateDialog(string templateName)
        {
            InitializeComponent();
            TemplateName = templateName;
        }

        private void CreateTemplateDialog_Load(object sender, EventArgs e)
        {
            txtTemplateName.Focus();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
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
            if (TemplateName == "")
            {
                message = "Please enter a template name.";
                return false;
            }
            else if (TemplateName.Any(@char => InvalidChars.Contains(@char)))
            {
                message = string.Format(
                    "Please enter a template name that does not contain any of the following characters:{0}{0}{1}",
                    Environment.NewLine,
                    string.Join(" ", InvalidChars.Where(invalidChar => !char.IsControl(invalidChar))));
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
