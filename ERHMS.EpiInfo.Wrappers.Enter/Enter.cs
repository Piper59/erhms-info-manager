using Epi;
using Epi.Windows;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace ERHMS.EpiInfo.Wrappers
{
    public class Enter
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            Wrapper.MainBase(args);
        }

        public class OpenRecord : Wrapper
        {
            private static string projectPath;
            private static string viewName;
            private static int uniqueKey;
            private static MainForm form;

            public static Wrapper Create(string projectPath, string viewName, int uniqueKey)
            {
                return Create(() => Execute(projectPath, viewName, uniqueKey));
            }

            private static void Execute(string projectPath, string viewName, int uniqueKey)
            {
                OpenRecord.projectPath = projectPath;
                OpenRecord.viewName = viewName;
                OpenRecord.uniqueKey = uniqueKey;
                form = new MainForm();
                form.Shown += Form_Shown;
                form.RecordSaved += Form_RecordSaved;
                Application.Run(form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                using (SplashScreenForm splash = new SplashScreenForm())
                {
                    splash.ShowInTaskbar = false;
                    splash.Show(form);
                    Application.DoEvents();
                    form.OpenRecord(projectPath, viewName, uniqueKey);
                    splash.Close();
                }
            }

            private static void Form_RecordSaved(object sender, SaveRecordEventArgs e)
            {
                RaiseEvent(WrapperEventType.RecordSaved);
            }
        }

        public class OpenNewRecord : Wrapper
        {
            private static string projectPath;
            private static string viewName;
            private static MainForm form;

            public static Wrapper Create(string projectPath, string viewName, object record = null)
            {
                Wrapper wrapper = Create(() => Execute(projectPath, viewName));
                wrapper.Invoked += (sender, e) =>
                {
                    if (record != null)
                    {
                        foreach (PropertyInfo property in record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                        {
                            wrapper.WriteLine(property.Name);
                            wrapper.WriteLine(property.GetValue(record, null));
                        }
                    }
                    wrapper.Close();
                };
                return wrapper;
            }

            private static void Execute(string projectPath, string viewName)
            {
                OpenNewRecord.projectPath = projectPath;
                OpenNewRecord.viewName = viewName;
                form = new MainForm();
                form.Shown += Form_Shown;
                form.RecordSaved += Form_RecordSaved;
                Application.Run(form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                using (SplashScreenForm splash = new SplashScreenForm())
                {
                    splash.ShowInTaskbar = false;
                    splash.Show(form);
                    Application.DoEvents();
                    form.OpenNewRecord(projectPath, viewName);
                    bool refresh = false;
                    while (true)
                    {
                        string fieldName = In.ReadLine();
                        string value = In.ReadLine();
                        if (fieldName == null || value == null)
                        {
                            break;
                        }
                        if (form.TrySetValue(fieldName, value))
                        {
                            refresh = true;
                        }
                    }
                    if (refresh)
                    {
                        form.Refresh();
                    }
                    splash.Close();
                }
            }

            private static void Form_RecordSaved(object sender, SaveRecordEventArgs e)
            {
                RaiseEvent(WrapperEventType.RecordSaved);
            }
        }
    }
}
