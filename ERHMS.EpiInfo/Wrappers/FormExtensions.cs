using ERHMS.Utility;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.Wrappers
{
    public static class FormExtensions
    {
        public static void Initialize(this Form @this)
        {
            @this.Size = new Size(1024, 768);
            @this.WindowState = FormWindowState.Maximized;
        }

        public static bool TryClose(this Form @this, string reason, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            string message = string.Format("{0} Close Epi Info\u2122?", reason);
            DialogResult result = MessageBox.Show(@this, message, "Close?", MessageBoxButtons.YesNo, icon);
            if (result == DialogResult.Yes)
            {
                @this.Close();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Panic(this Form @this, string reason, Exception ex)
        {
            Log.Logger.Fatal("Fatal error", ex);
            string message = string.Format("{0} Epi Info\u2122 must shut down.", reason);
            MessageBox.Show(@this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            @this.Close();
        }
    }
}
