using Epi.Windows.AnalysisDashboard;
using ERHMS.Utility;
using System;
using System.IO;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.Wrappers
{
    public class AnalysisDashboard
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            Wrapper.MainBase(args);
        }

        public class OpenCanvas : Wrapper
        {
            private static string canvasPath;
            private static DashboardMainForm form;

            public static Wrapper Create(string projectPath, int canvasId, string content)
            {
                string canvasPath = IOExtensions.GetTempFileName("ERHMS_{0:N}{1}", Canvas.FileExtension);
                File.WriteAllText(canvasPath, content);
                return Create(() => Execute(projectPath, canvasId, canvasPath));
            }

            private static void Execute(string projectPath, int canvasId, string canvasPath)
            {
                OpenCanvas.canvasPath = canvasPath;
                using (FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(canvasPath), Path.GetFileName(canvasPath)))
                {
                    watcher.NotifyFilter = NotifyFilters.LastWrite;
                    watcher.Changed += Watcher_Changed;
                    watcher.EnableRaisingEvents = true;
                    form = new DashboardMainForm();
                    form.Initialize();
                    form.Shown += Form_Shown;
                    Application.Run(form);
                }
            }

            private static void Watcher_Changed(object sender, FileSystemEventArgs e)
            {
                RaiseEvent(WrapperEventType.CanvasSaved);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                // TODO: Application.DoEvents?
                form.OpenCanvas(canvasPath);
            }
        }
    }
}
