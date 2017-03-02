using Epi.Windows;
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

            public static Wrapper Create(string projectPath, string content)
            {
                string canvasPath = IOExtensions.GetTempFileName("ERHMS_{0:N}{1}", Canvas.FileExtension);
                File.WriteAllText(canvasPath, content);
                return Create(() => Execute(projectPath, canvasPath));
            }

            private static void Execute(string projectPath, string canvasPath)
            {
                OpenCanvas.canvasPath = canvasPath;
                using (FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(canvasPath), Path.GetFileName(canvasPath)))
                {
                    watcher.NotifyFilter = NotifyFilters.LastWrite;
                    watcher.Changed += Watcher_Changed;
                    watcher.EnableRaisingEvents = true;
                    form = new DashboardMainForm();
                    form.Initialize();
                    form.WindowState = FormWindowState.Minimized;
                    form.ShowInTaskbar = false;
                    form.Shown += Form_Shown;
                    Application.Run(form);
                }
            }

            private static void Watcher_Changed(object sender, FileSystemEventArgs e)
            {
                using (FileStream stream = new FileStream(canvasPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(stream))
                {
                    RaiseEvent(WrapperEventType.CanvasSaved, new
                    {
                        Content = reader.ReadToEnd()
                    });
                }
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                using (SplashScreenForm splash = new SplashScreenForm())
                {
                    splash.ShowInTaskbar = false;
                    splash.Show(form);
                    Application.DoEvents();
                    form.OpenCanvas(canvasPath);
                    splash.Close();
                    form.WindowState = FormWindowState.Maximized;
                    form.ShowInTaskbar = true;
                }
            }
        }
    }
}
