using Epi.Windows;
using Epi.Windows.MakeView.Dialogs;
using ERHMS.Utility;
using System;
using System.Windows.Forms;
using System.Xml;

namespace ERHMS.EpiInfo.Wrappers
{
    public class MakeView
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            Wrapper.MainBase(args);
        }

        public class OpenView : Wrapper
        {
            private static string ProjectPath { get; set; }
            private static string ViewName { get; set; }
            private static MainForm Form { get; set; }

            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => MainInternal(projectPath, viewName));
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
                Application.DoEvents();
                Form.OpenProject(ProjectPath);
                Form.ProjectExplorer.SelectView(ViewName);
            }
        }

        public class InstantiateProjectTemplate : Wrapper
        {
            public static Wrapper Create(string projectPath, string templatePath)
            {
                return Create(() => MainInternal(projectPath, templatePath));
            }

            private static void MainInternal(string projectPath, string templatePath)
            {
                MainForm form = new MainForm(false);
                form.OpenProject(projectPath);
                Template template = new Template(form.Mediator);
                template.InstantiateTemplate(templatePath);
            }
        }

        public class InstantiateViewTemplate : Wrapper
        {
            private static string ProjectPath { get; set; }
            private static string TemplatePath { get; set; }
            private static string ViewNamePrefix { get; set; }
            private static MainForm Form { get; set; }

            public static Wrapper Create(string projectPath, string templatePath, string viewNamePrefix)
            {
                return Create(() => MainInternal(projectPath, templatePath, viewNamePrefix));
            }

            private static void MainInternal(string projectPath, string templatePath, string viewNamePrefix)
            {
                ProjectPath = projectPath;
                TemplatePath = templatePath;
                ViewNamePrefix = viewNamePrefix;
                Form = new MainForm();
                Form.Shown += Form_Shown;
                Application.Run(Form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                Application.DoEvents();
                Project project = new Project(ProjectPath);
                Form.OpenProject(project);
                XmlDocument document = new XmlDocument();
                document.Load(TemplatePath);
                XmlElement element = document.SelectSingleElement("/Template/Project/View");
                string viewName = project.SuggestViewName(ViewNamePrefix + element.GetAttribute("Name"));
                using (CreateViewDialog dialog = new CreateViewDialog(project, viewName))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    if (dialog.ShowDialog(Form) == DialogResult.OK)
                    {
                        element.SetAttribute("Name", dialog.ViewName);
                        string templatePath = IOExtensions.GetTempFileName("ERHMS_{0:N}{1}", TemplateInfo.FileExtension);
                        document.Save(templatePath);
                        Template template = new Template(Form.Mediator);
                        template.InstantiateTemplate(templatePath);
                        int viewId = Form.Mediator.ProjectExplorer.CurrentView.Id;
                        RaiseEvent("ViewCreated", new
                        {
                            ViewId = viewId
                        });
                        Form.TryClose("Form has been created.");
                    }
                    else
                    {
                        Form.TryClose("Form has not been created.", MessageBoxIcon.Warning);
                    }
                }
            }
        }

        public class CreateTemplate : Wrapper
        {
            private static string ProjectPath { get; set; }
            private static string ViewName { get; set; }
            private static MainForm Form { get; set; }

            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => MainInternal(projectPath, viewName));
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
                Application.DoEvents();
                Form.OpenProject(ProjectPath);
                Form.ProjectExplorer.SelectView(ViewName);
                using (CreateTemplateDialog dialog = new CreateTemplateDialog(ViewName))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    if (dialog.ShowDialog(Form) == DialogResult.OK)
                    {
                        Template template = new Template(Form.Mediator);
                        string path = template.CreateTemplate(Form.CurrentView, dialog.TemplateName, dialog.Description);
                        RaiseEvent("TemplateCreated", new
                        {
                            Path = path
                        });
                        Form.ProjectExplorer.UpdateTemplates();
                        Form.TryClose("Template has been created.");
                    }
                    else
                    {
                        Form.TryClose("Template has not been created.", MessageBoxIcon.Warning);
                    }
                }
            }
        }

        public class CreateWebTemplate : Wrapper
        {
            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => MainInternal(projectPath, viewName));
            }

            private static void MainInternal(string projectPath, string viewName)
            {
                MainForm form = new MainForm(false);
                form.OpenProject(projectPath);
                form.ProjectExplorer.SelectView(viewName);
                Template template = new Template(form.Mediator);
                Out.Write(template.CreateWebTemplate());
            }
        }

        public class PublishToMobile : Wrapper
        {
            private static string ProjectPath { get; set; }
            private static string ViewName { get; set; }
            private static MainForm Form { get; set; }

            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => MainInternal(projectPath, viewName));
            }

            private static void MainInternal(string projectPath, string viewName)
            {
                ProjectPath = projectPath;
                ViewName = viewName;
                Form = new MainForm(false);
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
                    Form.OpenProject(ProjectPath);
                    Form.ProjectExplorer.SelectView(ViewName);
                    splash.Close();
                };
                using (CopyToAndroid dialog = new CopyToAndroid(Form.CurrentView, Form.Mediator))
                {
                    dialog.ShowInTaskbar = false;
                    dialog.StartPosition = FormStartPosition.CenterScreen;
                    dialog.ShowDialog(Form);
                }
                Form.Close();
            }
        }
    }
}
