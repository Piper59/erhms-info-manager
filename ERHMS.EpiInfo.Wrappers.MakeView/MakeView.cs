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
            private static string projectPath;
            private static string viewName;
            private static MainForm form;

            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => Execute(projectPath, viewName));
            }

            private static void Execute(string projectPath, string viewName)
            {
                OpenView.projectPath = projectPath;
                OpenView.viewName = viewName;
                form = new MainForm();
                form.Shown += Form_Shown;
                Application.Run(form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                Application.DoEvents();
                form.OpenProject(projectPath);
                form.ProjectExplorer.SelectView(viewName);
            }
        }

        public class InstantiateProjectTemplate : Wrapper
        {
            public static Wrapper Create(string projectPath, string templatePath)
            {
                return Create(() => Execute(projectPath, templatePath));
            }

            private static void Execute(string projectPath, string templatePath)
            {
                MainForm form = new MainForm(false);
                form.OpenProject(projectPath);
                Template template = new Template(form.Mediator);
                template.InstantiateTemplate(templatePath);
            }
        }

        public class InstantiateViewTemplate : Wrapper
        {
            private static string projectPath;
            private static string templatePath;
            private static string namePrefix;
            private static MainForm form;

            public static Wrapper Create(string projectPath, string templatePath, string namePrefix)
            {
                return Create(() => Execute(projectPath, templatePath, namePrefix));
            }

            private static void Execute(string projectPath, string templatePath, string namePrefix)
            {
                InstantiateViewTemplate.projectPath = projectPath;
                InstantiateViewTemplate.templatePath = templatePath;
                InstantiateViewTemplate.namePrefix = namePrefix;
                form = new MainForm();
                form.Shown += Form_Shown;
                Application.Run(form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                Application.DoEvents();
                Project project = new Project(projectPath);
                form.OpenProject(project);
                XmlDocument document = new XmlDocument();
                document.Load(templatePath);
                XmlElement viewElement = document.SelectSingleElement("/Template/Project/View");
                string viewName = project.SuggestViewName(namePrefix + viewElement.GetAttribute("Name"));
                using (CreateViewDialog dialog = new CreateViewDialog(project, viewName))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    if (dialog.ShowDialog(form) == DialogResult.OK)
                    {
                        viewElement.SetAttribute("Name", dialog.ViewName);
                        string tempTemplatePath = IOExtensions.GetTempFileName("ERHMS_{0:N}{1}", TemplateInfo.FileExtension);
                        document.Save(tempTemplatePath);
                        Template template = new Template(form.Mediator);
                        template.InstantiateTemplate(tempTemplatePath);
                        RaiseEvent(WrapperEventType.ViewCreated);
                        form.TryClose("Form has been created.");
                    }
                    else
                    {
                        form.TryClose("Form has not been created.", MessageBoxIcon.Warning);
                    }
                }
            }
        }

        public class CreateTemplate : Wrapper
        {
            private static string projectPath;
            private static string viewName;
            private static MainForm form;

            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => Execute(projectPath, viewName));
            }

            private static void Execute(string projectPath, string viewName)
            {
                CreateTemplate.projectPath = projectPath;
                CreateTemplate.viewName = viewName;
                form = new MainForm();
                form.Shown += Form_Shown;
                Application.Run(form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                Application.DoEvents();
                form.OpenProject(projectPath);
                form.ProjectExplorer.SelectView(viewName);
                using (CreateTemplateDialog dialog = new CreateTemplateDialog(viewName))
                {
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    if (dialog.ShowDialog(form) == DialogResult.OK)
                    {
                        Template template = new Template(form.Mediator);
                        template.CreateTemplate(form.CurrentView, dialog.TemplateName, dialog.Description);
                        RaiseEvent(WrapperEventType.TemplateCreated);
                        form.ProjectExplorer.UpdateTemplates();
                        form.TryClose("Template has been created.");
                    }
                    else
                    {
                        form.TryClose("Template has not been created.", MessageBoxIcon.Warning);
                    }
                }
            }
        }

        public class CreateWebTemplate : Wrapper
        {
            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => Execute(projectPath, viewName));
            }

            private static void Execute(string projectPath, string viewName)
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
            private static string projectPath;
            private static string viewName;
            private static MainForm form;

            public static Wrapper Create(string projectPath, string viewName)
            {
                return Create(() => Execute(projectPath, viewName));
            }

            private static void Execute(string projectPath, string viewName)
            {
                PublishToMobile.projectPath = projectPath;
                PublishToMobile.viewName = viewName;
                form = new MainForm(false);
                form.Shown += Form_Shown;
                Application.Run(form);
            }

            private static void Form_Shown(object sender, EventArgs e)
            {
                using (SplashScreenForm splash = new SplashScreenForm())
                {
                    splash.ShowInTaskbar = false;
                    splash.Show(form);
                    Application.DoEvents();
                    form.OpenProject(projectPath);
                    form.ProjectExplorer.SelectView(viewName);
                    splash.Close();
                };
                using (CopyToAndroid dialog = new CopyToAndroid(form.CurrentView, form.Mediator))
                {
                    dialog.ShowInTaskbar = false;
                    dialog.StartPosition = FormStartPosition.CenterScreen;
                    dialog.ShowDialog(form);
                }
                form.Close();
            }
        }
    }
}
