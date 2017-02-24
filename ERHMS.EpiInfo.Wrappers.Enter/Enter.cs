using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo.Wrappers
{
    public class Enter : Wrapper
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            MainBase(typeof(Enter), args);
        }

        public static Wrapper Execute()
        {
            return Create(args => Main_Execute(args));
        }
        private static void Main_Execute(string[] args)
        {
            using (MainForm form = new MainForm())
            {
                Application.Run(form);
            }
        }

        public static Wrapper OpenView(View view, object record = null)
        {
            Wrapper wrapper = Create(args => Main_OpenView(args), view.Project.FilePath, view.Name);
            if (record != null)
            {
                foreach (PropertyInfo property in record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    wrapper.WriteLine("{0} = {1}", property.Name, property.GetValue(record, null));
                }
            }
            wrapper.Close();
            return wrapper;
        }
        private static void Main_OpenView(string[] args)
        {
            string projectPath = args[0];
            string viewName = args[1];
            using (MainForm form = new MainForm(projectPath, viewName))
            {
                form.Show();
                form.FireOpenViewEvent(form.View, "+");
                bool refresh = false;
                Regex field = new Regex(@"^(?<name>\S+) = (?<value>.*)$");
                while (true)
                {
                    string line = In.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    refresh = true;
                    Match match = field.Match(line);
                    if (!match.Success)
                    {
                        throw new ArgumentException("Failed to read field.");
                    }
                    form.SetValue(match.Groups["name"].Value, match.Groups["value"].Value);
                }
                if (refresh)
                {
                    form.Refresh();
                }
                Application.Run(form);
            }
        }

        public static Wrapper OpenRecord(View view, int uniqueKey)
        {
            return Create(args => Main_OpenRecord(args), view.Project.FilePath, view.Name, uniqueKey.ToString());
        }
        private static void Main_OpenRecord(string[] args)
        {
            string projectPath = args[0];
            string viewName = args[1];
            string uniqueKey = args[2];
            using (MainForm form = new MainForm(projectPath, viewName))
            {
                form.Show();
                form.FireOpenViewEvent(form.View, uniqueKey);
                Application.Run(form);
            }
        }

        private Enter() { }
    }
}
