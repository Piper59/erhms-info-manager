using Epi.Windows.MakeView.Dialogs;
using ERHMS.EpiInfo.Communication;
using ERHMS.Utility;
using System;
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

        public static Wrapper OpenProject(Project project)
        {
            return Create(args => Main_OpenProject(args), project.FilePath);
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

        public static Wrapper OpenView(View view)
        {
            return Create(args => Main_OpenView(args), view.Project.FilePath, view.Name);
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

        public static Wrapper AddView(Project project, string prefix = null, string tag = null)
        {
            return Create(args => Main_AddView(args), project.FilePath, prefix, tag);
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
            string viewName = ViewExtensions.SanitizeName(prefix);
            using (MainForm form = new MainForm())
            {
                form.OpenProject(project);
                form.Load += (sender, e) =>
                {
                    using (CreateViewDialog dialog = new CreateViewDialog(project, viewName))
                    {
                        dialog.StartPosition = FormStartPosition.CenterParent;
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

        public static Wrapper CreateTemplate(View view)
        {
            return Create(args => Main_CreateTemplate(args), view.Project.FilePath, view.Name);
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
                        dialog.StartPosition = FormStartPosition.CenterParent;
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            Template template = new Template(form.Mediator);
                            template.CreateTemplate(project.Views[viewName], dialog.TemplateName, dialog.Description);
                            IService service = Service.Connect();
                            if (service != null)
                            {
                                service.OnTemplateAdded(dialog.TemplatePath);
                            }
                            string message = "Template has been created. Close Epi Info?";
                            if (MessageBox.Show(message, "Close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                form.Close();
                            }
                        }
                    }
                    form.Close();
                };
                Application.Run(form);
            }
        }

        public static Wrapper CreateWebTemplate(View view)
        {
            return Create(args => Main_CreateWebTemplate(args), view.Project.FilePath, view.Name);
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
                Out.Write(template.CreateWebTemplate());
            }
        }

        public static Wrapper InstantiateTemplate(Project project, TemplateInfo templateInfo, string prefix = null, string tag = null)
        {
            return Create(args => Main_InstantiateTemplate(args), project.FilePath, templateInfo.File.FullName, prefix, tag);
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
            TemplateInfo templateInfo;
            if (!TemplateInfo.TryRead(new FileInfo(templatePath), out templateInfo))
            {
                throw new ArgumentException("Failed to read template.");
            }
            if (templateInfo.Level == TemplateLevel.Project)
            {
                using (MainForm form = new MainForm(false))
                {
                    form.OpenProject(project);
                    Template template = new Template(form.Mediator);
                    template.InstantiateTemplate(templateInfo);
                }
            }
            else if (templateInfo.Level == TemplateLevel.View)
            {
                using (MainForm form = new MainForm())
                {
                    form.OpenProject(project);
                    XmlDocument document = new XmlDocument();
                    document.Load(templatePath);
                    XmlElement viewElement = document.SelectSingleElement("/Template/Project/View");
                    string viewName = ViewExtensions.SanitizeName(string.Format("{0}{1}", prefix, viewElement.GetAttribute("Name")));
                    // TODO: project.SuggestViewName?
                    form.Load += (sender, e) =>
                    {
                        using (CreateViewDialog dialog = new CreateViewDialog(project, viewName))
                        {
                            dialog.StartPosition = FormStartPosition.CenterParent;
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                viewName = dialog.ViewName;
                                viewElement.SetAttribute("Name", viewName);
                                FileInfo templateFile = IOExtensions.GetTemporaryFile("ERHMS_{0:N}{1}", TemplateInfo.FileExtension);
                                document.Save(templateFile.FullName);
                                Template template = new Template(form.Mediator);
                                template.InstantiateTemplate(templateFile.FullName);
                                IService service = Service.Connect();
                                if (service != null)
                                {
                                    service.OnViewAdded(projectPath, dialog.ViewName, tag);
                                }
                                string message = "Form has been created. Close Epi Info?";
                                if (MessageBox.Show(message, "Close?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    form.Close();
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

        public static Wrapper PublishToMobile(View view)
        {
            return Create(args => Main_PublishToMobile(args), view.Project.FilePath, view.Name);
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
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    dialog.ShowDialog();
                }
            }
        }

        private MakeView() { }
    }
}
