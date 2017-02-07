using Epi.Windows.AnalysisDashboard;
using ERHMS.EpiInfo.Communication;
using ERHMS.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.AnalysisDashboard
{
    public static class AnalysisDashboard
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            WrapperBase.MainBase(typeof(AnalysisDashboard), args);
        }

        public static Process Execute()
        {
            return WrapperBase.Execute(args => Main_Execute(args));
        }
        private static void Main_Execute(string[] args)
        {
            using (DashboardMainForm form = new DashboardMainForm())
            {
                Application.Run(form);
            }
        }

        public static Process OpenCanvas(Project project, Canvas canvas, string tag = null)
        {
            FileInfo file = IOExtensions.GetTemporaryFile("ERHMS_{0:N}{1}", Canvas.FileExtension);
            File.WriteAllText(file.FullName, canvas.Content);
            return WrapperBase.Execute(args => Main_OpenCanvas(args), project.FilePath, canvas.CanvasId.ToString(), file.FullName, tag);
        }
        private static void Main_OpenCanvas(string[] args)
        {
            string projectPath = args[0];
            int canvasId = int.Parse(args[1]);
            string canvasPath = args[2];
            string tag = args[3];
            if (tag == "")
            {
                tag = null;
            }
            FileInfo file = new FileInfo(canvasPath);
            using (FileSystemWatcher watcher = new FileSystemWatcher(file.DirectoryName, file.Name))
            using (DashboardMainForm form = new DashboardMainForm())
            {
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Changed += (sender, e) =>
                {
                    IService service = Service.Connect();
                    if (service != null)
                    {
                        service.OnCanvasSaved(projectPath, canvasId, canvasPath, tag);
                    }
                };
                watcher.EnableRaisingEvents = true;
                form.Load += (sender, e) =>
                {
                    form.OpenCanvas(canvasPath);
                };
                Application.Run(form);
            }
        }
    }
}
