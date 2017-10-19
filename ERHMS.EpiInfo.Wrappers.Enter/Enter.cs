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
            private static string ProjectPath { get; set; }
            private static string ViewName { get; set; }
            private static int UniqueKey { get; set; }
            private static MainForm Form { get; set; }

            public static Wrapper Create(string projectPath, string viewName, int uniqueKey)
            {
                return Create(() => MainInternal(projectPath, viewName, uniqueKey));
            }

            private static void MainInternal(string projectPath, string viewName, int uniqueKey)
            {
                ProjectPath = projectPath;
                ViewName = viewName;
                UniqueKey = uniqueKey;
                Form = new MainForm();
                Form.Shown += Form_Shown;
                Application.Run(Form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                using (SplashScreenForm splash = new SplashScreenForm())
                {
                    splash.ShowInTaskbar = false;
                    splash.Show(Form);
                    Application.DoEvents();
                    Form.OpenRecord(ProjectPath, ViewName, UniqueKey);
                    splash.Close();
                }
            }
        }

        public class OpenNewRecord : Wrapper
        {
            private static string ProjectPath { get; set; }
            private static string ViewName { get; set; }
            private static MainForm Form { get; set; }

            public static Wrapper Create(string projectPath, string viewName, object record = null)
            {
                Wrapper wrapper = Create(() => MainInternal(projectPath, viewName));
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

            private static void MainInternal(string projectPath, string viewName)
            {
                ProjectPath = projectPath;
                ViewName = viewName;
                Form = new MainForm();
                Form.Shown += Form_Shown;
                Application.Run(Form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                using (SplashScreenForm splash = new SplashScreenForm())
                {
                    splash.ShowInTaskbar = false;
                    splash.Show(Form);
                    Application.DoEvents();
                    Form.OpenNewRecord(ProjectPath, ViewName);
                    bool refresh = false;
                    while (true)
                    {
                        string name = In.ReadLine();
                        if (name == null)
                        {
                            break;
                        }
                        string value = In.ReadLine();
                        if (value == null)
                        {
                            break;
                        }
                        if (Form.TrySetField(name, value))
                        {
                            refresh = true;
                        }
                    }
                    if (refresh)
                    {
                        Form.Refresh();
                    }
                    splash.Close();
                }
            }
        }
    }
}
