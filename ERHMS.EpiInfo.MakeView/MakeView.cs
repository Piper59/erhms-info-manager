using Epi.Windows.MakeView.Dialogs;
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

        public static Process AddView(Project project)
        {
            return Execute(args => Main_AddView(args), project.FilePath);
        }

        private static void Main_AddView(string[] args)
        {
            string projectPath = args[0];
            using (MainForm form = new MainForm())
            {
                form.OpenProject(projectPath);
                form.Load += (sender, e) =>
                {
                    form.ProjectExplorer.AddView();
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
                form.Load += (sender, e) =>
                {
                    using (AddTemplateDialog dialog = new AddTemplateDialog(form))
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            Template template = new Template(form.Mediator);
                            template.CreateTemplate(project.Views[viewName], dialog.TemplateName);
                        }
                    }
                };
                Application.Run(form);
            }
        }

        public static Process AddFromTemplate(Project project, EpiInfo.Template template)
        {
            return Execute(args => Main_AddFromTemplate(args), project.FilePath, template.File.FullName);
        }

        private static void Main_AddFromTemplate(string[] args)
        {
            string projectPath = args[0];
            string templatePath = args[1];
            EpiInfo.Template template;
            if (!EpiInfo.Template.TryRead(new FileInfo(templatePath), out template))
            {
                throw new ArgumentException("Failed to read template.");
            }
            Project project = new Project(projectPath);
            if (template.Level == TemplateLevel.Project)
            {
                using (MainForm form = new MainForm(false))
                {
                    form.OpenProject(project);
                    Template _template = new Template(form.Mediator);
                    _template.AddFromTemplate(template);
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
                    string viewName = string.Format("{0}{1}", project.Name, viewNode.Attributes["Name"].Value);
                    string sanitizedViewName = ViewExtensions.SanitizeName(viewName);
                    form.Load += (sender, e) =>
                    {
                        using (CreateViewDialog dialog = new CreateViewDialog(form, project, viewName))
                        {
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                viewNode.Attributes["Name"].Value = dialog.ViewName;
                                FileInfo renamedTemplateFile = IOExtensions.GetTemporaryFile(extension: ".xml");
                                document.Save(renamedTemplateFile.FullName);
                                Template _template = new Template(form.Mediator);
                                _template.AddFromTemplate(renamedTemplateFile.FullName);
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
    }
}
