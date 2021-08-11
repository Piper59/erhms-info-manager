using ERHMS.Desktop.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace ERHMS.Desktop.Utilities
{
    public partial class UtilityDialog : Form
    {
        public string Lead
        {
            get { return LeadLabel.Text; }
            set { LeadLabel.Text = value; }
        }

        public string Body
        {
            get { return BodyLabel.Text; }
            set { BodyLabel.Text = value; }
        }

        public bool CanClose { get; set; }

        public UtilityDialog()
        {
            InitializeComponent();
            Text = Strings.Title_App;
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
            if (CanClose)
            {
                DialogResult = DialogResult.OK;
                base.OnFormClosing(e);
            }
            else
            {
                DialogResult result = MessageBox.Show(
                    this,
                    Strings.Body_UtilityClosing,
                    Strings.Title_App,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (result == DialogResult.No)
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
