using Epi.Windows.AnalysisDashboard;
using System.Diagnostics;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.AnalysisDashboard
{
    public class AnalysisDashboard : Wrapper
    {
        public static Process Execute()
        {
            return Execute(args => Main_Execute(args));
        }

        private static void Main_Execute(string[] args)
        {
            using (DashboardMainForm form = new DashboardMainForm())
            {
                Application.Run(form);
            }
        }

        public static Process OpenCanvas(Canvas canvas)
        {
            return Execute(args => Main_OpenCanvas(args), canvas.File.FullName);
        }

        private static void Main_OpenCanvas(string[] args)
        {
            string canvasPath = args[0];
            using (DashboardMainForm form = new DashboardMainForm())
            {
                form.Load += (sender, e) =>
                {
                    form.OpenCanvas(canvasPath);
                };
                Application.Run(form);
            }
        }
    }
}
