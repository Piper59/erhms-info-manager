using Epi.Windows.AnalysisDashboard;
using ERHMS.EpiInfo.Communication;
using ERHMS.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.AnalysisDashboard
{
    public class AnalysisDashboard : Wrapper
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            MainBase(typeof(AnalysisDashboard), args);
        }

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

        public static Process OpenCanvas(Project project, Canvas canvas)
        {
            FileInfo file = IOExtensions.GetTemporaryFile(extension: Canvas.FileExtension);
            File.WriteAllText(file.FullName, canvas.Content);
            return Execute(args => Main_OpenCanvas(args), project.FilePath, canvas.CanvasId.ToString(), file.FullName);
        }
        private static void Main_OpenCanvas(string[] args)
        {
            string projectPath = args[0];
            int canvasId = int.Parse(args[1]);
            string canvasPath = args[2];
            try
            {
                using (DashboardMainForm form = new DashboardMainForm())
                {
                    form.Load += (sender, e) =>
                    {
                        form.OpenCanvas(canvasPath);
                    };
                    Application.Run(form);
                }
            }
            finally
            {
                IService service = Service.Connect();
                if (service != null)
                {
                    service.OnCanvasClosed(projectPath, canvasId, canvasPath);
                }
            }
        }
    }
}
