using System;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.Analysis
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run();
        }
    }
}
