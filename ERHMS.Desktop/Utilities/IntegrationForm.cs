using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ERHMS.Desktop.Utilities
{
    public partial class IntegrationForm : Form
    {
        public string Body
        {
            get { return BodyLabel.Text; }
            set { BodyLabel.Text = value; }
        }

        public bool Done { get; set; }

        public IntegrationForm()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(
                e.Graphics,
                ClientRectangle,
                Color.Empty, 0, ButtonBorderStyle.None,
                SystemColors.WindowFrame, 1, ButtonBorderStyle.Solid,
                Color.Empty, 0, ButtonBorderStyle.None,
                Color.Empty, 0, ButtonBorderStyle.None);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (Done)
            {
                DialogResult = DialogResult.OK;
                base.OnFormClosing(e);
            }
            else
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine("ERHMS Info Manager is still working.");
                message.AppendLine();
                message.AppendLine("Close anyway?");
                DialogResult dialogResult = MessageBox.Show(
                    this,
                    message.ToString(),
                    Text,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (dialogResult == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    DialogResult = DialogResult.Cancel;
                    base.OnFormClosing(e);
                }
            }
        }
    }
}
