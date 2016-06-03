using Epi.Windows.MakeView.Dialogs;
using ERHMS.EpiInfo.Communication;
using ERHMS.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using View = Epi.View;

namespace ERHMS.EpiInfo.MakeView
{
    public class MakeView : Wrapper
    {
        [STAThread]
        internal static void Main(string[] args)
        {
            MainBase(typeof(MakeView), args);
        }

        public static Process Execute()
        {
            return Execute(args => Main_Execute(args));
        }
        private static void Main_Execute(string[] args)
        {
            using (MainForm form = new MainForm())
            {
                Application.Run(form);
            }
        }

        public static Process OpenProject(Project project)
        {
            return Execute(args => Main_OpenProject(args), project.FilePath);
        }
        private static void Main_OpenProject(string[] args)
        {
            string projectPath = args[0];
            using (MainForm form = new MainForm())
            {
                form.OpenProject(projectPath);
                Application.Run(form);
            }
        }

        public static Process OpenView(View view)
        {
            return Execute(args => Main_OpenView(args), view.Project.FilePath, view.Name);
        }
        private static void Main_OpenView(string[] args)
        {
            string projectPath = args[0];
            string viewName = args[1];
            using (MainForm form = new MainForm())
            {
                form.OpenProject(projectPath);
                form.ProjectExplorer.SelectView(viewName);
                Application.Run(form);
            }
        }

        public static Process AddView(Project project, string prefix = null, string tag = null)
        {
            return Execute(args => Main_AddView(args), project.FilePath, prefix, tag);
        }
        private static void Main_AddView(string[] args)
        {
            string projectPath = args[0];
            string prefix = args[1];
            if (prefix != "" && !prefix.EndsWith("_"))
            {
                prefix = string.Format("{0}_", prefix);
            }
            string tag = args[2];
            if (tag == "")
            {
                tag = null;
            }
            Project project = new Project(projectPath);
            string viewName = Project.SanitizeViewName(prefix);
            using (MainForm form = new MainForm())
            {
                form.OpenProject(project);
                form.Load += (sender, e) =>
                {
                    using (CreateViewDialog dialog = new CreateViewDialog(project, viewName))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            viewName = dialog.ViewName;
                            View view = project.CreateView(viewName);
                            view.CreatePage("Page 1", 0);
                            form.ProjectExplorer.AddView(view);
                            IService service = Service.Connect();
                            if (service != null)
                            {
                                service.OnViewAdded(projectPath, viewName, tag);
                            }
                        }
                    }
                };
                Application.Run(form);
            }
        }

        public static Process CreateTemplate(View view)
        {
            return Execute(args => Main_CreateTemplate(args), view.Project.FilePath, view.Name);
        }
        private static void Main_CreateTemplate(string[] args)
        {
            string projectPath = args[0];
            string viewName = args[1];
            using (MainForm form = new MainForm())
            {
                Project project = new Project(projectPath);
                form.OpenProject(projectPath);
                form.ProjectExplorer.SelectView(viewName);
                form.Load += (sender, e) =>
                {
                    using (CreateTemplateDialog dialog = new CreateTemplateDialog(project, viewName))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            Template template = new Template(form.Mediator);
                            template.CreateTemplate(project.Views[viewName], dialog.TemplateName, dialog.Description);
                            IService service = Service.Connect();
                            if (service != null)
                            {
                                service.OnTemplateAdded(dialog.TemplatePath);
                            }
                        }
                    }
                    form.Close();
                };
                Application.Run(form);
            }
        }

        public static string CreateWebTemplate(View view)
        {
            Process process = Execute(args => Main_CreateWebTemplate(args), view.Project.FilePath, view.Name);
            return process.StandardOutput.ReadToEnd();
        }
        private static void Main_CreateWebTemplate(string[] args)
        {
            string projectPath = args[0];
            string viewName = args[1];
            using (MainForm form = new MainForm(false))
            {
                Project project = new Project(projectPath);
                form.OpenProject(project);
                form.ProjectExplorer.SelectView(viewName);
                Template template = new Template(form.Mediator);
                Console.Write(template.CreateWebTemplate());
            }
        }

        public static Process InstantiateTemplate(Project project, EpiInfo.Template template, string prefix = null, string tag = null)
        {
            return Execute(args => Main_InstantiateTemplate(args), project.FilePath, template.File.FullName, prefix, tag);
        }
        private static void Main_InstantiateTemplate(string[] args)
        {
            string projectPath = args[0];
            string templatePath = args[1];
            string prefix = args[2];
            if (prefix != "" && !prefix.EndsWith("_"))
            {
                prefix = string.Format("{0}_", prefix);
            }
            string tag = args[3];
            if (tag == "")
            {
                tag = null;
            }
            Project project = new Project(projectPath);
            EpiInfo.Template template;
            if (!EpiInfo.Template.TryRead(new FileInfo(templatePath), out template))
            {
                throw new ArgumentException("Failed to read template.");
            }
            if (template.Level == TemplateLevel.Project)
            {
                using (MainForm form = new MainForm(false))
                {
                    form.OpenProject(project);
                    Template _template = new Template(form.Mediator);
                    _template.InstantiateTemplate(template);
                }
            }
            else if (template.Level == TemplateLevel.View)
            {
                using (MainForm form = new MainForm())
                {
                    form.OpenProject(project);
                    XmlDocument document = new XmlDocument();
                    document.Load(templatePath);
                    XmlNode viewNode = document.SelectSingleNode("/Template/Project/View");
                    string viewName = Project.SanitizeViewName(string.Format("{0}{1}", prefix, viewNode.Attributes["Name"].Value));
                    form.Load += (sender, e) =>
                    {
                        using (CreateViewDialog dialog = new CreateViewDialog(project, viewName))
                        {
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                viewName = dialog.ViewName;
                                viewNode.Attributes["Name"].Value = viewName;
                                FileInfo templateFile = IOExtensions.GetTemporaryFile(extension: EpiInfo.Template.FileExtension);
                                document.Save(templateFile.FullName);
                                Template _template = new Template(form.Mediator);
                                _template.InstantiateTemplate(templateFile.FullName);
                                IService service = Service.Connect();
                                if (service != null)
                                {
                                    service.OnViewAdded(projectPath, dialog.ViewName, tag);
                                }
                            }
                        }
                    };
                    Application.Run(form);
                }
            }
            else
            {
                throw new ArgumentException("Template level is not project or view.");
            }
        }

        public static Process PublishToMobile(View view)
        {
            return Execute(args => Main_PublishToMobile(args), view.Project.FilePath, view.Name);
        }
        private static void Main_PublishToMobile(string[] args)
        {
            string projectPath = args[0];
            string viewName = args[1];
            using (MainForm form = new MainForm(false))
            {
                Project project = new Project(projectPath);
                form.OpenProject(project);
                form.ProjectExplorer.SelectView(viewName);
                using (CopyToAndroid dialog = new CopyToAndroid(form.CurrentView, form.Mediator))
                {
                    dialog.ShowDialog();
                }
            }
        }
    }
}
